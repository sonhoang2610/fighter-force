using EazyEngine.Space.UI;
using EazyEngine.Tools;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace EazyEngine.Space
{
    [System.Serializable]
    public class MissionModule
    {
        public string IDMission;
   
        [OdinSerialize, System.NonSerialized]
        public FlowCanvas.FlowScript condition;
        [OdinSerialize, System.NonSerialized]
        public int weight;
        [OdinSerialize,System.NonSerialized]
        public Dictionary<string, object> VariableDict = new Dictionary<string, object>();
        [ListDrawerSettings(AddCopiesLastElement =true)]
        public MissionItemInstanced[] missions;
    }

    public struct MissionEvent
    {
        public string missionID;

        public MissionEvent(string pID)
        {
            missionID = pID;
        }
    }



    [System.Serializable]
    public class MissionDailyModule
    {
        public int dateOfYear;
        public MissionModule missionProcessing;
    }
    [System.Serializable]
    public class MissionContainerInfo
    {
        public MissionDailyModule dailyMission;
        public MissionModule mainMission;
    }
    public class MissionContainer : SingletonOdin<MissionContainer>
    {

        [System.NonSerialized]
        public MissionDatabase database;
        private void Start()
        {
            database = MissionDatabase.Instance;
        }
        int compareMission(MissionModule a, MissionModule b)
        {
            return b.weight.CompareTo(a.weight);
        }
        public void LoadState()
        {
            database = MissionDatabase.Instance;
            Info = GameManager.Instance.Database.missionContainerInfo;
            bool dirty = false;
            if (Info == null)
            {
                Info = new MissionContainerInfo();
                dirty = true;
            }
            if (Info.dailyMission ==null  || Info.dailyMission.dateOfYear != System.DateTime.Now.DayOfYear)
            {

                Info.dailyMission = new MissionDailyModule()
                {
                    dateOfYear = System.DateTime.Now.DayOfYear
                };
                dirty = true;
            }
            MissionModule currentMissionModule = Info.dailyMission.missionProcessing;
            MissionModule defaultMissionModule = null;
            if (currentMissionModule != null)
            {
                defaultMissionModule = System.Array.Find(database.dailyMissions, x => x.IDMission == currentMissionModule.IDMission);
                if (defaultMissionModule == null)
                {
                    currentMissionModule = null;
                }
            }
            if (currentMissionModule == null)
            {
                System.Array.Sort(database.dailyMissions, compareMission);
    
                for (int i = 0; i < database.dailyMissions.Length; ++i)
                {

                    var pOwnerMission = Instantiate(transform.Find("DefaultDailyMissionChecker"), transform);
                    pOwnerMission.name = "Checker";
                    var blackBoard = pOwnerMission.GetComponent<Blackboard>();
                    if (database.dailyMissions[i].VariableDict != null)
                    {
                        for (int j = 0; j < database.dailyMissions[i].VariableDict.Count; ++j)
                        {
                            var pDict = database.dailyMissions[i].VariableDict;
                            blackBoard.AddVariable(pDict.Keys.ElementAt(j), pDict[pDict.Keys.ElementAt(j)]);
                        }
                    }
                    var pNode = new EazyNode()
                    {
                        flow = database.dailyMissions[i].condition,
                        owner = pOwnerMission.GetComponent<GraphOwner>()
                    };
                    successCondition = false;
                    pNode.runGraph(onFinishNode,"flow");
                    if (successCondition)
                    {
                        currentMissionModule = database.dailyMissions[i].CloneData();
                        break;
                    }
                    else
                    {
                        Destroy(pOwnerMission.gameObject);
                    }

                }
            }
            defaultMissionModule = System.Array.Find(database.dailyMissions, x => x.IDMission == currentMissionModule.IDMission);
            bool pMergeDirty = mergeMission(ref currentMissionModule, defaultMissionModule);
            dirty = dirty ? dirty : pMergeDirty;
            Info.dailyMission.missionProcessing = currentMissionModule;
            currentMissionModule = Info.mainMission;
            defaultMissionModule = database.achievements;
            if (currentMissionModule == null || currentMissionModule.missions == null)
            {
                currentMissionModule = defaultMissionModule.CloneData();
            }

            pMergeDirty = mergeMission(ref currentMissionModule, defaultMissionModule);
            dirty = dirty ? dirty : pMergeDirty;
            Info.mainMission = currentMissionModule;
            if (dirty)
            {
                GameManager.Instance.SaveGame();
            }

        }

        public bool mergeMission(ref MissionModule currentMissionModule, MissionModule defaultMissionModule)
        {
            bool dirty = false;
       
            for (int i = 0; i < defaultMissionModule.missions.Length; ++i)
            {
                var pDefaultMission = defaultMissionModule.missions[i];
                var pMission = System.Array.Find(currentMissionModule.missions, x => x.mission.ItemID == defaultMissionModule.missions[i].mission.ItemID);
                if (pMission == null)
                {
                    System.Array.Resize(ref currentMissionModule.missions, currentMissionModule.missions.Length + 1);
                    pMission = new MissionItemInstanced()
                    {
                        mission = pDefaultMission.mission,
                        Process = 0,
                        rewards = pDefaultMission.rewards
                    };
                    currentMissionModule.missions[currentMissionModule.missions.Length - 1] = pMission;
                    dirty = true;
                }
                else
                {
                    pMission.rewards = pDefaultMission.rewards;
                    pMission.limitLevel = pDefaultMission.limitLevel;
                }
                pMission.VariableDict = pDefaultMission.VariableDict;
            }
            List<MissionItemInstanced> pTempMission = new List<MissionItemInstanced>(currentMissionModule.missions);
            for (int i = pTempMission.Count - 1; i >= 0; --i)
            {
                var pMission = System.Array.Find(defaultMissionModule.missions, x => x.mission.itemID == pTempMission[i].mission.ItemID);
                if (pMission == null)
                {
                    pTempMission.RemoveAt(i);
                    dirty = true;
                }
            }
            currentMissionModule.missions = pTempMission.ToArray();
            for (int i = currentMissionModule.missions.Length - 1; i >= 0; --i)
            {
                excuteMission(currentMissionModule.missions[i], currentMissionModule.IDMission);
            }
            return dirty;
        }
        protected void excuteMission(MissionItemInstanced pMission,string pIdModule)
        {
            var pOwnerMission = transform.Find(pMission.mission.ItemID + pIdModule);
            pMission.ModuleID = pMission.mission.ItemID + pIdModule;
            // var pVaraiables = pMission.mission.VariableDict == null ? pMission.mission.VariableDict : pMission.mission.VariableDict.MergeLeft(pMission.VariableDict);
            var pVaraiables = pMission.mission.VariableDict.MergeLeft(pMission.VariableDict);
            if(!pMission.mission.VariableDictInstanced.ContainsKey(pMission.mission.ItemID + pIdModule))
            {
                pMission.mission.VariableDictInstanced.Add(pMission.mission.ItemID + pIdModule, pVaraiables);
            }
            pMission.mission.VariableDictInstanced[pMission.mission.ItemID + pIdModule] = pVaraiables;
            if (pOwnerMission == null)
            {
                pOwnerMission = Instantiate(transform.Find("DefaultMission"),transform);
                pOwnerMission.name = pMission.mission.ItemID + pIdModule;
            }
            var blackBoard = pOwnerMission.GetComponent<IBlackboard>();
            if (pMission.mission.checkComplete)
            {
                blackBoard.AddVariable("flow", pMission.mission.checkComplete);
            }
            blackBoard.AddVariable("MainMission", pMission);
            blackBoard.AddVariable("MissionID", pMission.mission.ItemID);
            blackBoard.AddVariable("ListenID", pMission.mission.listenID);
            if (pVaraiables != null)
            {
                for (int i = 0; i < pVaraiables.Count; ++i)
                {
                    var pDict = pVaraiables;
                    blackBoard.AddVariable(pDict.Keys.ElementAt(i), pDict[pDict.Keys.ElementAt(i)]);
                }
            }
            pMission.BlackBoard = blackBoard;
            pMission.extraInfo();
            StartCoroutine(delayAction(0.1f, delegate
            {
                Graph.SendGlobalEvent(new ParadoxNotion.EventData<string>("MissionDirty", pMission.mission.ItemID), this);
            }));
       
        }
        [ContextMenu("test menu")]
        public void testSend()
        {
            Graph.SendGlobalEvent(new ParadoxNotion.EventData<string>("MissionDirty", "PassLevels"), this);
        }
        IEnumerator delayAction(float pDelay, System.Action action)
        {
            yield return new WaitForSeconds(pDelay);
            action();
        }
        protected bool successCondition;

        [ShowInInspector]
        public MissionContainerInfo Info {
            get
            {
                return GameManager.Instance.Database.missionContainerInfo;
            }
            set
            {
                GameManager.Instance.Database.missionContainerInfo = value;
            }
        }

        public void onFinishNode(EazyNode pNode)
        {
            successCondition = pNode.isSuccess;
        }
    }
}
