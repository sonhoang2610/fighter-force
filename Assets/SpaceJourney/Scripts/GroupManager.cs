using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using EazyEngine.Tools.Space;
using EazyEngine.Space.UI;
using EazyEngine.Timer;
using ParadoxNotion.Services;
using System.Linq;

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
        protected AIMachine mAi;
        int indexQueue = 0;
        public LevelState ParentState
        {
            set
            {
                _parentState = value;
                indexQueue = 0;
                countDetach = 0;
                currentStep = 0;
                elements.Clear();
                leaderGroup.Clear();
                initMove(initState());          
                initLoot();
            }
            get => _parentState;
        }
        private void Awake()
        {
            time._groupName = TimeKeeper.Instance.getTimeLineIndex("Enemies");
            mAi = GetComponent<AIMachine>();

        }
        public bool isValidateShow()
        {
            return isManual || Application.isPlaying;
        }
   
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
        public static List<GroupManager> managers = new List<GroupManager>();
        public void initMove(GameObject[][] pEnemiesLoaded)
        {
            var pState = _parentState;
            var pCounter = 0;
            for (var j = 0; j < pEnemiesLoaded.Length; ++j)
            {
                
                var pLeader =  leaders.FindAndClean(x => !x.gameObject.activeSelf,x => x.IsDestroyed());
                GameObject pMainLeaderObject = null;
                if (!pState.isManual)
                {
                    if (!pLeader)
                    {
                        //pMainLeaderObject = new GameObject();
                        //var pTime = pMainLeaderObject.AddComponent<TimeControllerElement>();
                        //pTime._groupName = TimeKeeper.Instance.getTimeLineIndex("Enemies") + 1;
                        //pLeader = pMainLeaderObject.AddComponent<MovingLeader>();
                        pLeader = Instantiate<MovingLeader>(GameManager.Instance.leaderTemplate);
                        pMainLeaderObject = pLeader.gameObject;
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

                pMainLeaderObject.name = "leader";
                pMainLeaderObject.transform.parent = transform;
                pMainLeaderObject.transform.RotationDirect2D(pState.formatInfo.directionStart, TranformExtension.FacingDirection.DOWN);
                pLeader._manager = this;
                pLeader.transform.localPosition = pState.formatInfo.startSpawnPos;
                for (int i = 0; i < pLeader.elements.Count; ++i)
                {
                        if (pLeader.elements[i].objectAnchor != null && pLeader.elements[i].objectAnchor.gameObject == pLeader.gameObject)
                    {
                        pLeader.elements[i].objectAnchor = null;
                    }
                }
                pLeader.elements.Clear();
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
                    var percentRandom = UnityEngine.Random.Range(0.0f, 1.0f);
                    if (percentRandom < _parentState.lootItems[i].percent)
                    {
                        var pCountItem = UnityEngine.Random.Range((int)_parentState.lootItems[i].dropCountRange.x, (int)_parentState.lootItems[i].dropCountRange.y);
                        for (var j = 0; j < pCountItem; ++j)
                        {
                            var pRandomElement = UnityEngine.Random.Range(0, elements.Count);
                            var pRandomItem = UnityEngine.Random.Range(0, _parentState.lootItems[i].items.Length);
                            elements[pRandomElement].GetComponent<DopItem>().itemDropOnDeath.Add(_parentState.lootItems[i].items[pRandomItem]);
                        }
                    }
                }
            }
            for (var i  =0; i < elements.Count; ++i)
            {
                for (var j = 0; j < _parentState.lootItems.Length; ++j)
                {
                    if (_parentState.lootItems[j].baseLoot == AIBaseType.SelfEmiter)
                    {
                        var percentRandom = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (percentRandom < _parentState.lootItems[j].percent)
                        {
                            var pCountItem = UnityEngine.Random.Range((int)_parentState.lootItems[j].dropCountRange.x, (int)_parentState.lootItems[j].dropCountRange.y);
                            for (var g = 0; g < pCountItem; ++g)
                            {
                                var pRandomItem = UnityEngine.Random.Range(0, _parentState.lootItems[j].items.Length);
                                elements[i].GetComponent<DopItem>().itemDropOnDeath.Add(_parentState.lootItems[j].items[pRandomItem]);
                            }
                        }
                    }
                }
            }
        }
        public GameObject[][] initState()
        {
            var pElements = new List<GameObject>();
            var machine = mAi;
            if (!machine)
            {
                machine = gameObject.AddComponent<AIMachine>();
                mAi = machine;
            }
            var pState = _parentState;
            var arrayPos = JourneySpaceUlities.getPointFormat(pState.formatInfo, pState.formatInfo.quantity);
            bool pMultipeLeader = false;
            for(int i = 0; i < pState.moveInfos.Length; ++i)
            {
                if(pState.moveInfos[0].RowDelay != 0)
                {
                    pMultipeLeader = true;
                }
            }
            if (!pMultipeLeader)
            {
                List<Vector2> pTempPos = new List<Vector2>();
                for(int i = 0; i < arrayPos.Length; ++i)
                {
                    pTempPos.AddRange(arrayPos[i]);
                }
                arrayPos = new Vector2[][]
                {
                    pTempPos.ToArray()
                };
            }
            List<Vector2> pRecalculate = new List<Vector2>();
            arrayPos = arrayPos.convertAfterRotation(Vector2.zero, 0);
            if (!pState.isManual)
            {
                var pTypeGetEnemy = pState.formatInfo.typeSpawn;
              
                var pQuantity = pState.formatInfo.quantity;

                for (var i = 0; i < pQuantity; ++i)
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
                var pElementBigs = new List<GameObject[]>();
                var pQuantity = 0;
                for(var i = 0; i < leaderGroup.Count; ++i)
                {
               
                    var pSubElements = new List<GameObject>();
                    for (var j  = 0; j < leaderGroup[i].elements.Length; ++j)
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
                    detachAllChild();
                    gameObject.SetActive(false);
                }
                return;
            }
            if (_parentState != null)
            {
                _parentState.TotalComplete++;
            }
            elements.Remove(pElement);
            pElement.objectAnchor.elements.Remove(pElement);
            if (_parentState != null)
            {
                _parentState.totalDeath++;
                var pLeader = pElement.objectAnchor;
                for (int i = 0; i < pLeader._moveInfo.conditionComplete.Length; ++i)
                {
                    if (pLeader._moveInfo.conditionComplete[i].condition == ConditionEndMoveType.DestroyAll)
                    {
                        if (_parentState.totalDeath >= _parentState.formatInfo.quantity)
                        {
                            pLeader.nextStep();
                        }
                    }
                    else if (pLeader._moveInfo.conditionComplete[i].condition == ConditionEndMoveType.DestroyQuantity)
                    {
                        if (_parentState.totalDeath >= pLeader._moveInfo.conditionComplete[i].destroyQuantity)
                        {
                            pLeader.nextStep();
                        }
                    }
                }
            }
        }
        public void OnEzEvent(MessageGamePlayEvent eventType)
        {
           for(int i = leaderGroup.Count - 1; i >= 0; --i)
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
            if (!GameManager.Instance.inGame) return;
            EzEventManager.RemoveListener(this);
        
        }

        public void detachAllChild()
        {
            foreach (var pElement in leaderGroup)
            {
                if (pElement.leader.gameObject.activeSelf)
                {
                    pElement.leader.transform.parent = null;
                }
                pElement.leader.gameObject.SetActive(false);
           
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
                        detachAllChild();
                        gameObject.SetActive(false);
                    
                    }
                    else {
                        completeAndWating = true;
                    }
                }
            }
        }

        public static void clearCache()
        {
            leaders.Clear();
            managers.Clear();
            GameManager.Instance.pendingObjects.Clear();
            PlayerEnviroment.eviromentInstant.Clear();
            SoundManager.PoolInGameAudios.Clear();
            SoundManager.Instance.StopAllCoroutines();
            if (!SceneManager.Instance.currentScene.Contains("Zone"))
            {
                TopLayer.Instance.inGame(false);
            }
           if(MonoManager.current)
            {
                MonoManager.current.StopAllCoroutines();
            }
           for(int i = 0; i < ObjectPooler.poolManager.Count; ++i)
            {
                if (!ObjectPooler.poolManager.ElementAt(i).Key.IsDestroyed())
                {
                    foreach(var pObject in ObjectPooler.poolManager.ElementAt(i).Value.poolObjects)
                    {
                        if (!pObject.IsDestroyed())
                        {
                            pObject.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}
