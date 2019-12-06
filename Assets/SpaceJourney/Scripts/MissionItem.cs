using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    using NodeCanvas.Framework;
    using Sirenix.OdinInspector;
    using Sirenix.Serialization;
    using System;
    using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
    public class MissionItemCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/MissionItem")]
        public static void CreateMyAsset()
        {
            MissionItem asset = ScriptableObject.CreateInstance<MissionItem>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/MissionItem.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif

    [System.Serializable]
    public class MissionItem : BaseItemGame
    {
        public string listenID;
        public FlowCanvas.FlowScript checkComplete;
        [OdinSerialize]
        public Dictionary<string, object> VariableDict = new Dictionary<string, object>();
        [System.NonSerialized]
        public Dictionary<string, Dictionary<string, object>> VariableDictInstanced = new Dictionary<string, Dictionary<string, object>>();
        [System.NonSerialized]
        private string instancedModuleId;
        public override string Desc
        {
            get
            {
                var pStr = base.Desc;
                var pStrs = pStr.Split(' ');
                var pDict = VariableDictInstanced.ElementAt(0).Value;
                if (!string.IsNullOrEmpty(instancedModuleId))
                {
                    pDict = VariableDictInstanced[instancedModuleId];
                }
                foreach (var pString in pStrs)
                {
                    if (pString.StartsWith("$"))
                    {
                        var pVar = pString.Remove(0, 1);
                        if (pDict.ContainsKey(pVar) && pDict[pVar] != null)
                        {
                            pStr = pStr.Replace(pString, pDict[pVar].ToString());
                        }
                    }
                }

                return pStr;
            }
        }

        public string InstancedModuleId { get => instancedModuleId; set => instancedModuleId = value; }
    }

    [System.Serializable]
    public class MissionItemInstanced :IObservable
    {
        [System.NonSerialized]
        private IBlackboard blackBoard;
        [OdinSerialize, System.NonSerialized]
        public FlowCanvas.FlowScript condition;
        [OdinSerialize,System.NonSerialized]
        public Dictionary<string, object> VariableDict = new Dictionary<string, object>();
        public MissionItem mission;
        public float process = 0;
        public bool claimed = false;
        [OdinSerialize, System.NonSerialized]
        public int limitLevel = 1;
        [HideInEditorMode]
        public int currentLevel = 0;
        public RewardInfo[] rewards;
        [System.NonSerialized]
        private string moduleID;
        public void extraInfo()
        {
            if(currentLevel == 0)
            {
                currentLevel = 1;
            }
            if ((currentLevel -1) < limitLevel)
            {
                claimed = false;
            }
            for (int i = 0; i < mission. VariableDictInstanced.Count; ++i)
            {
                if (mission.VariableDictInstanced.Values.ElementAt(i) == null) continue;
                var pDict = mission.VariableDictInstanced.Values.ElementAt(i);
                for (int j = 0; j < pDict.Count; ++j)
                {
                    if (typeof(ILevelSetter).IsAssignableFrom(pDict.Values.ElementAt(j).GetType()))
                    {
                        ((ILevelSetter)pDict.Values.ElementAt(j)).setLevel(currentLevel);
                    }
                }
            }       
            EzEventManager.TriggerEvent(new MessageGamePlayEvent("MissionDirty"));
        }

        public string Destiny { get { return BlackBoard.GetVariable("Goal").value.ToString(); } }
        public float DestinyFloat {
            get {
                float pResult = 1;
                var pVar = BlackBoard.GetVariable("Goal");
                if (pVar != null)
                {
                    float.TryParse(pVar.value.ToString(), out pResult);
                }
                return pResult;
            }
        }
        public float Process { get => process; set { process = value; OnChangeObject(); } }
        public bool Claimed { get => claimed; set { claimed = value; OnChangeObject(); } }
        [ShowInInspector]
        public IBlackboard BlackBoard { get => blackBoard; set => blackBoard = value; }
        public string ModuleID { get => moduleID; set => moduleID = value; }

        [field: NonSerialized]
        public event OnChange OnChange;

        public void OnChangeObject()
        {
            OnChange?.Invoke();
        }
    }
    [System.Serializable]
    public class RewardInfo :ILevelSetter
    {
        public BaseItemGame item;
        public int quantity;
        [System.NonSerialized,OdinSerialize]
        public UnitDefineLevel quantiyLevel;

        public int Quantity { get
            {
                if(quantiyLevel != null)
                {
                    return quantiyLevel;
                }
                return quantity;
            }
            set => quantity = value; }

        public void setLevel(int pLevel)
        {
            if (quantiyLevel != null)
            {
                quantiyLevel.setLevel(pLevel);
            }
        }
    }
}