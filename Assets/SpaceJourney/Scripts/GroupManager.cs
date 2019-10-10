using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.DemiLib;
using DG.Tweening.Core;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using EazyEngine.Tools.Space;
using System.Linq;
using EazyEngine.Timer;
using Spine.Unity;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class DictionnaryGroupManager
    {
        [TableColumnWidth(200)]
        public MovingLeader leader;
        [TableColumnWidth(200)]
        public GroupElement[] elements;
        public DictionnaryGroupManager(MovingLeader pLeader, GroupElement[] pElements)
        {
            leader = pLeader;
            elements = pElements;
        }
    }
    [System.Serializable]
    public class GroupManager : TimeControlBehavior, EzEventListener<MessageGamePlayEvent>
    {
        public bool isManual = false;

        protected List<GroupElement> elements = new List<GroupElement>();
        [ShowIf("isValidateShow")]
        [TableList]
        public List<DictionnaryGroupManager> leaderGroup = new List<DictionnaryGroupManager>();
        [HideInEditorMode]
        public LevelState _parentState;
        protected int currentStep = 0;
        [HideInInspector]
        public int setUpIndex = 0;

        protected bool completeAndWating = false;
        protected int countDetach  = 0;
        public LevelState ParentState
        {
            set
            {
                _parentState = value;
                indexQueue = 0;
                initMove(initState());          
                initLoot();
            }
            get
            {
                return _parentState;
            }
        }
        private void Awake()
        {
            time._groupName = TimeKeeper.Instance.getTimeLineIndex("Enemies");
        }
        public bool isValidateShow()
        {
            return isManual || Application.isPlaying;
        }
        int indexQueue = 0;
        public GameObject randomInFlagEnemy(GameObject[] pEnemyType, TypeSpawn typeSpawn,bool isFirst)
        {
            if (isFirst && typeSpawn == TypeSpawn.RandomForAll)
            {
                indexQueue = UnityEngine.Random.Range(0, pEnemyType.Length);
            }
            var myEnum = pEnemyType[typeSpawn == TypeSpawn.Random ? UnityEngine.Random.Range(0, pEnemyType.Length) : indexQueue];
            if (typeSpawn == TypeSpawn.Queue)
            {
                indexQueue++;
            }
            return myEnum;
        }
        
        public static List<MovingLeader> leaders = new List<MovingLeader>();
        public void initMove(GameObject[][] pEnemiesLoaded)
        {
            LevelState pState = _parentState;
            int pCounter = 0;
            //Vector2[][] arrayPos = JourneySpaceUlities.getPointFormat(pState.formatInfo, pState.formatInfo.quantity);
            //float pDegree = VectorExtension.FindDegree(pState.formatInfo.directionStart);
            //arrayPos = arrayPos.convertAfterRotation(Vector2.zero, pDegree);
            for (int j = 0; j < pEnemiesLoaded.Length; ++j)
            {
                
                MovingLeader pLeader =  leaders.Find(x => !x.gameObject.activeSelf);
                GameObject pMainLeaderObject = null;
                if (!pState.isManual)
                {
                    if (!pLeader)
                    {
                        pMainLeaderObject = new GameObject();
                        var pTime = pMainLeaderObject.AddComponent<TimeControllerElement>();
                        pTime._groupName = TimeKeeper.Instance.getTimeLineIndex("Enemies") + 1;
                        pLeader = pMainLeaderObject.AddComponent<MovingLeader>();
                        leaders.Add(pLeader);
                    }
                    else
                    {
                        pMainLeaderObject = pLeader.gameObject;
                        pMainLeaderObject.gameObject.SetActive(true);
                    }
             
                }
                else
                {
                    pMainLeaderObject = leaderGroup[j].leader.gameObject;
                    pLeader = pMainLeaderObject.GetComponent<MovingLeader>();
                }
                pMainLeaderObject.transform.parent = transform;
                pMainLeaderObject.transform.RotationDirect2D(pState.formatInfo.directionStart, TranformExtension.FacingDirection.DOWN);
                pLeader._manager = this;
                pLeader.transform.localPosition = pState.formatInfo.startSpawnPos;
                //pLeader.setInfoMove = pState;
                GroupElement[] pElements = new GroupElement[pEnemiesLoaded[j].Length];
                for (int i = 0; i < pEnemiesLoaded[j].Length; ++i)
                {
                    pElements[i] = pEnemiesLoaded[j][i].GetComponent<GroupElement>();
                    pElements[i]._parentGroup = this;
                    pElements[i].setInfoSort(pCounter);
                    pElements[i].transform.localPosition = (Vector2)pElements[i].transform.localPosition + pState.formatInfo.startSpawnPos;
                    var pChar = pElements[i].GetComponent<Character>();
                    if (pChar)
                    {
                        pChar.changeState(StateCharacter.Birth);
                        if (pChar.modelObject)
                        {
                            var pSke = pChar.modelObject.GetComponent<MeshRenderer>();
                            if (pSke && pSke.sortingOrder == 0)
                            {
                                pSke.sortingOrder = 1;
                            }
                        }
                   
                    }

                    pElements[i].gameObject.SetActive(true);
                    pElements[i].setAnchor(pLeader);
                    pElements[i].sortImediately();
                    pCounter++;
                }
                pLeader.elements.AddRange (pElements);
                elements.AddRange(pElements);
                pLeader._parentState = _parentState;
                pLeader.setInfoMove(pState.moveInfos[0], j);
                if (!pState.isManual)
                {
                    leaderGroup.Add(new DictionnaryGroupManager(pLeader, pElements));
                }
            }
        }
        public void initLoot()
        {
            for (int i = 0; i < _parentState.lootItems.Length; ++i)
            {
                if (_parentState.lootItems[i].baseLoot == AIBaseType.Group)
                {
                    float percentRandom = UnityEngine.Random.Range(0.0f, 1.0f);
                    if (percentRandom < _parentState.lootItems[i].percent)
                    {
                        int pCountItem = UnityEngine.Random.Range((int)_parentState.lootItems[i].dropCountRange.x, (int)_parentState.lootItems[i].dropCountRange.y);
                        for (int j = 0; j < pCountItem; ++j)
                        {
                            int pRandomElement = UnityEngine.Random.Range(0, elements.Count);
                            int pRandomItem = UnityEngine.Random.Range(0, _parentState.lootItems[i].items.Length);
                            elements[pRandomElement].GetComponent<DopItem>().itemDropOnDeath.Add(_parentState.lootItems[i].items[pRandomItem]);
                        }
                    }
                }
            }
            for (int i  =0; i < elements.Count; ++i)
            {
                for (int j = 0; j < _parentState.lootItems.Length; ++j)
                {
                    if (_parentState.lootItems[j].baseLoot == AIBaseType.SelfEmiter)
                    {
                        float percentRandom = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (percentRandom < _parentState.lootItems[j].percent)
                        {
                            int pCountItem = UnityEngine.Random.Range((int)_parentState.lootItems[j].dropCountRange.x, (int)_parentState.lootItems[j].dropCountRange.y);
                            for (int g = 0; g < pCountItem; ++g)
                            {
                                int pRandomElement = UnityEngine.Random.Range(0, elements.Count);
                                int pRandomItem = UnityEngine.Random.Range(0, _parentState.lootItems[j].items.Length);
                                elements[i].GetComponent<DopItem>().itemDropOnDeath.Add(_parentState.lootItems[j].items[pRandomItem]);
                            }
                        }
                    }
                }
            }
        }
        public GameObject[][] initState()
        {
            List<GameObject> pElements = new List<GameObject>();
            var machine = gameObject.AddComponent<AIMachine>();
            LevelState pState = _parentState;
            Vector2[][] arrayPos = JourneySpaceUlities.getPointFormat(pState.formatInfo, pState.formatInfo.quantity);
            arrayPos = arrayPos.convertAfterRotation(Vector2.zero, 0);
            if (!pState.isManual)
            {
                TypeSpawn pTypeGetEnemy = pState.formatInfo.typeSpawn;
              
                int pQuantity = pState.formatInfo.quantity;

                for (int i = 0; i < pQuantity; ++i)
                {
                    var _typeSpawn = randomInFlagEnemy(pState.formatInfo.prefabEnemies, pTypeGetEnemy,i== 0);

                    var pObjectEnemy = EnemyEnviroment.Instance.getEnemyFromPool(_typeSpawn);
                    pObjectEnemy.transform.position = new Vector3(999, 999, 0);
                    LevelManger.Instance.addBornEnemy(pObjectEnemy);
                
                    pObjectEnemy.GetComponent<Health>().Revive();
                    // pObjectEnemy.transform.localPosition = arrayPos.getPointFromIndex(i) + pState.formatInfo.startSpawnPos;
                    pObjectEnemy.transform.RotationDirect2D(pState.formatInfo.directionStart, TranformExtension.FacingDirection.UP);
                    pElements.Add(pObjectEnemy);
                    machine._elements.Add(pObjectEnemy.GetComponent<AIElement>());
                    pObjectEnemy.GetComponent<AIElement>().ParentMachine = machine;

                }
                machine.setInfoAIMachine(pState.attackInfo);
                machine.ParentState = pState;

                return pElements.ToArray().changeFormatArray(arrayPos,pState.isManual);
            }
            else
            {
                List<GameObject[]> pElementBigs = new List<GameObject[]>();
                int pQuantity = 0;
                for(int i = 0; i < leaderGroup.Count; ++i)
                {
               
                    List<GameObject> pSubElements = new List<GameObject>();
                    for (int j  = 0; j < leaderGroup[i].elements.Length; ++j)
                    {
                        var pObjectEnemy = leaderGroup[i].elements[j];
                        machine._elements.Add(pObjectEnemy.GetComponent<AIElement>());
                        pObjectEnemy.GetComponent<AIElement>().ParentMachine = machine;
                        LevelManger.Instance.addBornEnemy(pObjectEnemy.gameObject);
                        pSubElements.Add(pObjectEnemy.gameObject);
                        pQuantity++;
                    }
                    pElementBigs.Add(pSubElements.ToArray());
                }
                pState.formatInfo.quantity = pQuantity;
                machine.setInfoAIMachine(pState.attackInfo);
                machine.ParentState = pState;
                return pElementBigs.ToArray();
            }
  
        }

        

        public void detachElement(GroupElement pElement)
        {
            countDetach++;
            if (completeAndWating)
            {
                if(countDetach >= elements.Count)
                {
                    _parentState.onCompleteState.Invoke();
                    Destroy(gameObject);
                }
                return;
            }
            if (_parentState != null)
            {
                _parentState.TotalComplete++;
            }
            elements.Remove(pElement);
            pElement.objectAnchor.elements.Remove(pElement);
            _parentState.totalDeath++;
            var pLeader = pElement.objectAnchor;
            for (int i = 0; i < pLeader._moveInfo.conditionComplete.Length; ++i)
            {
                if (pLeader._moveInfo.conditionComplete[i].condition == ConditionEndMoveType.DestroyAll )
                {
                    if(_parentState.totalDeath >= _parentState.formatInfo.quantity)
                    {
                        pLeader.nextStep();
                    }
                }else if(pLeader._moveInfo.conditionComplete[i].condition == ConditionEndMoveType.DestroyQuantity)
                {
                    if (_parentState.totalDeath >= pLeader._moveInfo.conditionComplete[i].destroyQuantity)
                    {
                        pLeader.nextStep();
                    }
                }
            }
        }
        public void OnEzEvent(MessageGamePlayEvent eventType)
        {
           for(int i = 0; i < leaderGroup.Count; ++i)
            {
                leaderGroup[i].leader.OnEzEvent(eventType);
            }
        }
        private void OnEnable()
        {
            countDetach = 0;
            EzEventManager.AddListener(this);
            completeAndWating = false;
        }
        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
            foreach (var pElement in leaderGroup)
            {
                pElement.leader.gameObject.SetActive(false);
                pElement.leader.transform.parent = null;
            }
        }
        public void onComplete(MovingLeader pLeader)
        {
            int pCount = 0;
            for (int i = 0; i < pLeader.elements.Count; ++i)
            {
                    pCount++;
            }
            if (_parentState._completeAction == CompleteAction.DestroyLeft)
            {
                for (int i = 0; i < pLeader.elements.Count; ++i)
                {
                    pLeader.elements[i].gameObject.SetActive(false);
                   var pChar = pLeader.elements[i].GetComponent<Character>();
                    if (pChar) {
                        pChar.changeState(StateCharacter.Death);
                    }
                }
                pLeader.gameObject.SetActive(false);
                pLeader.transform.parent = null;
            }

            _parentState.TotalComplete += pCount;
            if (_parentState.IsComplete)
            {
                if (isManual)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    if (_parentState._completeAction == CompleteAction.DestroyLeft)
                    {
                        _parentState.onCompleteState.Invoke();
                        Destroy(gameObject);
                    
                    }
                    else {
                        completeAndWating = true;
                    }
                }
            }
        }
    }
}
