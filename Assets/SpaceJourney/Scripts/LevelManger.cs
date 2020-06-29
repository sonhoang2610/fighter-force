using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using Unity.Collections;
using DG.Tweening;
using DG;
using System;
using NodeCanvas.Framework;
using EazyEngine.Timer;
using Spine.Unity;
using System.Linq;
using EazyEngine.Space.UI;
using UnityEngine.Animations;
using LitJson;

namespace EazyEngine.Space
{
    public class ConnectLayerTrigger
    {
        public LayerMask layer;
        public GameObject ignoreObject;
    }
    [System.Serializable]
    public class LevelContainer
    {
        public List<LevelInfoInstance> levels = new List<LevelInfoInstance>();

        public LevelInfoInstance getLevelInfo(int pLevel,int pHard)
        {
            for(int i = 0; i < levels.Count; ++i)
            {
                if(levels[i].level == pLevel && levels[i].hard == pHard)
                {
                    return levels[i];
                }
            }
            LevelInfoInstance pLevelInfo = new LevelInfoInstance() { level = pLevel, hard = pHard };
            levels.Add(pLevelInfo);
            return pLevelInfo;
        }
    }
    [System.Serializable]
    public class LevelInfoInstance
    {
        public int level = 0;
        public int hard = 0;
        public bool isLocked = true;
        public bool isPassed = false; 
        public LevelInfo infos = new LevelInfo();
    }
    [System.Serializable]
    public class LevelInfo
    {
        public List<MissionItemInstanced> missions = new List<MissionItemInstanced>();
        public int score=0;
        [System.NonSerialized]
        public int goldTaken = 0;
        [System.NonSerialized]
        public int enemyKill = 0;
        [System.NonSerialized]
        private LevelConfig inputConfig;

        public List<MissionItemInstanced> Missions { get => missions; set => missions = value; }
        public LevelConfig InputConfig { get {
                if (inputConfig == null)
                {
                    inputConfig = new LevelConfig();
                }
                return inputConfig;
            } set => inputConfig = value; }
    }

    public enum LayerCharType
    {
        Enemy,
        Player
    }
    public class PlaneInfoToCoppy
    {
        public GameObject model;
        public Vector3 attachMentPos;
        public PlaneInfo info;
    }


    public class LevelConfig
    {
        public List<ItemGame> itemUsed = new List<ItemGame>();
    }
    [System.Serializable]
    public class HistoryMatchInfo
    {
        public string matchID;
        public List<HistoryDetailLifeInfo> timeLifes = new List<HistoryDetailLifeInfo>();
        public string resultGame;
    }
    [System.Serializable]
    public class DetailItemUsedInfo
    {
        public string itemID;
        public int time;
    }
    [System.Serializable]
    public class StartGameInfo 
    {
        public int level;
        public int mode;
        public string matchID;
        public string selectedMainPlane;
        public int levelMain;
        public int[] levelSkillMain;
        public string selectedSupport;
        public int levelMainSp;
        public int[] levelSkillSp;
        public List<string> usedItem = new List<string>();
        public bool IsFreePlay;
    }
    [System.Serializable]
    public class HistoryDetailLifeInfo
    {
        public int timeStart = 0;
        public int startHeath;
        public InstigatorInfo[] instigatorInfos;
        public List<DetailItemUsedInfo> usedItems = new List<DetailItemUsedInfo>();
        public List<DetailItemUsedInfo> eatedItems = new List<DetailItemUsedInfo>();
        public List<DetailItemUsedInfo> skillUsed = new List<DetailItemUsedInfo>();
        public List<DetailItemUsedInfo> boosterChange = new List<DetailItemUsedInfo>();
        public List<DropItemInfo> dropItem = new List<DropItemInfo>(); 
        public string reborn = "None";
        public bool IsDeath = false;
        public int timeEnd = 0; 
    }
    public class LevelManger : Singleton<LevelManger>, EzEventListener<DamageTakenEvent>,EzEventListener<PickEvent>,EzEventListener<MessageGamePlayEvent>
    {
        public GameObject startPoint;
        public GameObject endPoint;
        public float delayStartGame = 3;
        [HideInEditorMode]
        public bool isPlaying = false;
        [HideInEditorMode]
        public bool isMatching = false;
        [HideInInspector]
        public int currentPlayerIndex = 0;
        [InlineEditor]
        public Camera mainPlayCamera;
        public Character[] players;
        [System.NonSerialized]
        public HistoryMatchInfo historyMatch;
        [System.NonSerialized]
        public StartGameInfo startMatchInfo;
        private List<PlaneInfoToCoppy> cachePlanePreload = new List<PlaneInfoToCoppy>();
        public List<PlaneInfoToCoppy> CachePlanePreload { get => cachePlanePreload; set => cachePlanePreload = value; }
        public Character CurrentPlayer
        {
            get
            {
                return (players != null && currentPlayerIndex < players.Length) ? players[currentPlayerIndex] : null;
            }
        }
        [HideInEditorMode]
        [System.NonSerialized]
        [ShowInInspector]
        public LevelInfo _infoLevel;

        [HideInEditorMode]
        public List<Character> _charList = new List<Character>();
        [System.NonSerialized]
        public List<GameObject> pullObject = new List<GameObject>();
        public List<ConnectLayerTrigger> ignoreObjects = new List<ConnectLayerTrigger>();
        public ConnectLayerTrigger checkAssignedObject(GameObject pObject)
        {
            for(int i = ignoreObjects.Count-1; i >= 0; --i)
            {
                if(ignoreObjects[i].ignoreObject == pObject)
                {
                    return ignoreObjects[i];
                }
            }
            return null;
        }

        public void removeIgnoreObject(GameObject pObject)
        {
            for (int i = ignoreObjects.Count - 1; i >= 0; --i)
            {
                if (ignoreObjects[i].ignoreObject == pObject)
                {
                    ignoreObjects.RemoveAt(i);
                }
            }
        }

        public void addIgnoreObjectWithLayer(GameObject pObject,int mask)
        {
             var pAssigned = checkAssignedObject(pObject);
            if (pAssigned == null)
            {
                pAssigned = new ConnectLayerTrigger() { ignoreObject = pObject };
                ignoreObjects.Add(pAssigned);
            }
            pAssigned.layer |= (1 << mask);
        }
        public bool isPause
        {
            get; set;
        }
        protected float currentDelayStartGame = 0;
        public bool isMovingMap
        {
            get; set;
        }
        public bool isFireAble
        {
            get; set;
        }

        public void OnEzEvent(DamageTakenEvent eventType)
        {
            if (eventType.AffectedCharacter == null) return;
            if(eventType.CurrentHealth <= 0 && eventType.Instigator == CurrentPlayer.gameObject && eventType.AffectedCharacter.mainInfo != null)
            {
                if (eventType.AffectedCharacter.mainInfo.isBoss)
                {
                    CurrentPlayer._health.Invulnerable = true;
                }
                _infoLevel.score += eventType.AffectedCharacter.mainInfo.score;
            }
            if(eventType.CurrentHealth <= 0 && LevelManger.Instance.BornEnemy.Contains(eventType.AffectedCharacter.gameObject))
            {
                LevelManger.Instance.BornEnemy.Remove(eventType.AffectedCharacter.gameObject);
                LevelManger.Instance._infoLevel.enemyKill++;
                var pChar = eventType.AffectedCharacter.gameObject.GetComponent<Character>();
                if (pChar)
                {
                    GameManager.Instance.Database.collectionDailyInfo.addQuantityDestroyEnemy(1, pChar.EnemyType);
                    GameManager.Instance.Database.collectionInfo.addQuantityDestroyEnemy(1, pChar.EnemyType);
                }
            }

            if(eventType.Instigator  && eventType.AffectedCharacter == CurrentPlayer)
            {
                if( eventType.Instigator.gameObject != eventType.AffectedCharacter.gameObject)
                {
                    GUIManager.Instance.showDangerEffect();
                    if (PlayerPrefs.GetInt("Vibrate", 1) == 1)
                    {
#if UNITY_MOBILE
                        Handheld.Vibrate();
#endif
                    }
                }
                else if(eventType.CurrentHealth > eventType.PreviousHealth)
                {
                    GUIManager.Instance.showHealEffect();
                }
         
                int pMaxHealth = eventType.AffectedCharacter._health.MaxiumHealth;
                GUIManager.Instance.setHealthMainPlane((int)eventType.CurrentHealth, pMaxHealth);
                if(eventType.CurrentHealth <= 0)
                {
                    EzEventManager.TriggerEvent(new MessageGamePlayEvent("LoseGame"));
                }
            }
        }

        private void OnEnable()
        {
            isFireAble = true;
            isPause = false;
            isMovingMap = true;
            EzEventManager.AddListener<DamageTakenEvent>(this);
            EzEventManager.AddListener<PickEvent>(this);
            EzEventManager.AddListener<MessageGamePlayEvent>(this);
            TopLayer.Instance.gameObject.SetActive(!IsMatching);
        }

        private void OnDisable()
        {
            if (TopLayer.Instance.IsDestroyed()) return;
            if ( !TopLayer.Instance.gameObject.activeSelf)
            {
                TopLayer.Instance.gameObject.SetActive(true);
            }
        
            EzEventManager.RemoveListener<DamageTakenEvent>(this);
            EzEventManager.RemoveListener<PickEvent>(this);
            EzEventManager.RemoveListener<MessageGamePlayEvent>(this);
         //   ObjectPooler.poolManager.Clear();
            for(int i  = ObjectPooler.poolManager.Count -1; i >= 0 ; --i)
            {
                if (!ObjectPooler.poolManager.ElementAt(i).Value.dontDestroyOnload)
                {
                    ObjectPooler.poolManager.Remove(ObjectPooler.poolManager.ElementAt(i).Key);
                }
            }
        }

        public float CurrentRateCoin
        {
            get
            {
                int pSelectDrop = GameManager.Instance.ChoosedLevel - 1;
                if (GameManager.Instance.isFree)
                {
                    pSelectDrop = 0;
                }
                var pConfig = GameDatabase.Instance.dropMonyeconfig[pSelectDrop][GameManager.Instance.ChoosedHard];
                int pRandomTotal = UnityEngine.Random.Range((int)pConfig.totalMoney.x,(int) pConfig.totalMoney.y);
                int pRandomTotalCoin = UnityEngine.Random.Range((int)pConfig.totalCoin.x, (int)pConfig.totalCoin.y);
                return pRandomTotal / pRandomTotalCoin; 
            }
        }
        protected float cacheVolum;
        public void turnOnVolume()
        {
            if (SoundManager.Instance.SfxOn)
            {
                DOTween.To(() => AudioListener.volume, x => AudioListener.volume = x, 1, 0.5f);
            }
        
           // SoundManager.Instance.PlayBackgroundMusic(GameManager.Instance.backgroundStage[GameManager.Instance.ChoosedHard]);
        }
        protected bool isInitDone = false;
        IEnumerator preloadAwake()
        {
            historyMatch = new HistoryMatchInfo();
            SceneManager.Instance.addloading(1);
            GameManager.Instance.scehduleUI = ScheduleUIMain.NONE;
            GUIManager.Instance.enableEnergy(false);
            GUIManager.Instance.setBarBooster(0);
            string stringState = GameManager.Instance.isFree ? "Statesfree" : "States" + GameManager.Instance.ChoosedLevel + "_" + GameManager.Instance.ChoosedHard;
            var psate = SceneManager.Instance.cacheStatePreload;
            Instantiate(psate);
            var pInfoOriginal = GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard).infos;

            if (pInfoOriginal.Missions.Count == 0)
            {
                var pMissionDefaults = GameDatabase.Instance.getMissionForLevel(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard);
                for (int i = 0; i < pMissionDefaults.Length; ++i)
                {
                    pInfoOriginal.Missions.Add(new MissionItemInstanced() { mission = pMissionDefaults[i].mission, Process = 0, rewards = pMissionDefaults[i].rewards });
                }
            }
            _infoLevel = pInfoOriginal.CloneData();
            List<ItemGame> pItem = new List<ItemGame>();
            List<ItemGameInstanced> pItems = new List<ItemGameInstanced>();

            for (int i = 0; i < _infoLevel.InputConfig.itemUsed.Count; ++i)
            {
                if (_infoLevel.InputConfig.itemUsed[i].isActive)
                {
                    pItem.Add(_infoLevel.InputConfig.itemUsed[i]);
                }
            }
            for (int i = 0; i < pItem.Count; ++i)
            {
                bool exist = false;
                for (int j = 0; j < pItems.Count; ++j)
                {
                    if (pItem[i].ItemID == pItems[j].item.ItemID)
                    {
                        pItems[j].quantity++;
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    pItems.Add(new ItemGameInstanced() { item = pItem[i], quantity = 1 });
                }
            }
            BoxItemInGame.Instance.DataSource = pItems.ToObservableList();
            int pStepGame = PlayerPrefs.GetInt("firstGame", 0);
            if (GameManager.Instance.isFree && GameManager.Instance.isGuide)
            {
                EazyAnalyticTool.LogEvent("FreeFirst");
                GameManager.Instance.freePlaneChoose = "MainPlane5";
                GameManager.Instance.freeSpPlaneChoose = "SpPlane4";
            }
            var pHistory = new HistoryDetailLifeInfo()
            {
                timeStart = (int)CurrentTime.TotalSeconds
            };
            historyMatch.timeLifes.Add(pHistory);
            int pSelectedPlane = -1;
            for (int i = 0; i < GameManager.Instance.Database.planes.Count; ++i)
            {
                if (GameManager.Instance.Database.planes[i].info.ItemID == ((!GameManager.Instance.isFree || string.IsNullOrEmpty(GameManager.Instance.freePlaneChoose) ? GameManager.Instance.Database.SelectedMainPlane : GameManager.Instance.freePlaneChoose)))
                {
                    pSelectedPlane = i;
                    break;
                }
            }
            int pSelectedspPlane = -1;
            for (int i = 0; i < GameManager.Instance.Database.spPlanes.Count; ++i)
            {
                if (GameManager.Instance.Database.spPlanes[i].info.ItemID == ((!GameManager.Instance.isFree || string.IsNullOrEmpty(GameManager.Instance.freeSpPlaneChoose) ? GameManager.Instance.Database.SelectedSupportPlane1 : GameManager.Instance.freeSpPlaneChoose)))
                {
                    pSelectedspPlane = i;
                    break;
                }
            }
            List<int> listIndex = new List<int>();
            for(int i = 0; i < GameManager.Instance.Database.planes.Count; ++i)
            {
                listIndex.Add(i);
            }
            Debug.Log(pSelectedPlane + "Plane");
            listIndex.Sort((x1, x2) =>
            {
                if(x1 != x2)
                {
                    if(x1 == pSelectedPlane)
                    {
                        return -1;
                    }
                    else if (x2 == pSelectedPlane)
                    {
                        return 1;
                    }
                }
                return x1.CompareTo(x2);
            });
            players = new Character[GameManager.Instance.Database.planes.Count];
            for (int i = 0; i < 1; ++i)
            {
                int pIndex = i;
                var pAsync = GameManager.Instance.Database.planes[listIndex[i]].Info.modelPlaneRef.loadAssetAsync<Character>();
                pAsync.completed += delegate (AsyncOperation a)
                {
                    players[pIndex] = Instantiate<Character>((Character)((ResourceRequest)a).asset);
                };
                while (players[pIndex] == null)
                {
                    yield return new WaitForEndOfFrame();
                }
                players[pIndex].GetComponent<Blackboard>().SetValue("Main", players[pIndex].gameObject);
                var pDataPlane = listIndex[i] >= 0 ? GameManager.Instance.Database.planes[listIndex[i]] : null;
                var pAllItemMainPlane = GameDatabase.Instance.getAllItem(CategoryItem.PLANE);
                if (pDataPlane == null || pDataPlane.CurrentLevel == 0)
                {
                    foreach (var pItemPlane in pAllItemMainPlane)
                    {
                        if (pItemPlane.ItemID == GameManager.Instance.freePlaneChoose)
                        {
                            pDataPlane = PlaneInfoConfig.CloneDefault((PlaneInfo)pItemPlane, 20);
                        }
                    }
                }
                
                players[pIndex].setData(pDataPlane);
                GUIManager.Instance.setIconPlane(pDataPlane.info.iconGame);
                if(pIndex != 0)
                {
                    var pContraint = players[pIndex].gameObject.AddComponent<PositionConstraint>();
                    pContraint.AddSource(new ConstraintSource() { sourceTransform = players[0].transform, weight = 1 });
                    pContraint.translationOffset = Vector3.zero;
                    pContraint.constraintActive = true;
                    players[pIndex].transform.GetComponentInChildren<SkeletonMecanim>().Skeleton.A = 0;
                    players[0].GetComponent<CharacterHandleWeapon>().anotherPlane.Add(players[pIndex].GetComponent<CharacterHandleWeapon>());
                    players[pIndex].GetComponent<CharacterHandleWeapon>().disableShoot = true;
                    players[pIndex].GetComponent<Collider2D>().enabled = false;
                }
            }
            players[0].GetComponent<Health>().recordDamage = true;
            List<SkillInputData> skills = new List<SkillInputData>();
            skills.AddRange(convert(players[0]._info.Info.skills.ToArray(), players[0]));
            startMatchInfo = new StartGameInfo();
            startMatchInfo.levelSkillMain = new int[skills.Count];
            for(int i = 0; i < startMatchInfo.levelSkillMain.Length; ++i)
            {
                startMatchInfo.levelSkillMain[i] = skills[i]._info.CurrentLevelSkill;
            }
            int pSelectedSPPlane1 = pSelectedspPlane;
            var pDataSpPlane = pSelectedSPPlane1 >= 0 ? GameManager.Instance.Database.spPlanes[pSelectedSPPlane1] : null;
            if (pDataSpPlane == null || pDataSpPlane.CurrentLevel == 0)
            {
                var pAllItem = GameDatabase.Instance.getAllItem(CategoryItem.SP_PLANE);
                foreach (var pItemPlane in pAllItem)
                {
                    if (pItemPlane.ItemID == GameManager.Instance.freeSpPlaneChoose)
                    {
                        pDataSpPlane = SupportPlaneInfoConfig.CloneDefaultSp((PlaneInfo)pItemPlane);
                    }
                }
            }
           var pAsyncSpPlane = pDataSpPlane.Info.modelPlaneRef.loadAssetAsync<Character>();
            pAsyncSpPlane.completed += delegate (AsyncOperation a)
            {
                pDataSpPlane.Info.modelPlane = ((Character)((ResourceRequest)a).asset);
            };
                while (pDataSpPlane.Info.modelPlane == null)
            {
                yield return new WaitForEndOfFrame();
            }
            if (pDataSpPlane != null)
            {

                var spPlane1 = Instantiate(pDataSpPlane.Info.modelPlane);
                spPlane1.setData(pDataSpPlane);
                spPlane1.GetComponent<FollowerMainPlayer>().OffsetSupportPlane = players[0].transform.Find("slot1").transform.localPosition;
                players[0].addChild(spPlane1);
                var pSkillSp = convert(spPlane1._info.Info.skills.ToArray(), spPlane1);
                skills.AddRange(pSkillSp);
                startMatchInfo.levelSkillSp = new int[pSkillSp.Length];
                for (int i = 0; i < startMatchInfo.levelSkillSp.Length; ++i)
                {
                    startMatchInfo.levelSkillSp[i] = pSkillSp[i]._info.CurrentLevelSkill;
                }
            }
            if (pDataSpPlane != null)
            {
                var spPlane2 = Instantiate(pDataSpPlane.Info.modelPlane);
                spPlane2.setData(pDataSpPlane);
                spPlane2.GetComponent<FollowerMainPlayer>().OffsetSupportPlane = players[0].transform.Find("slot2").transform.localPosition;
                spPlane2.GetComponent<CharacterHandleWeapon>().DatabaseWeapon[0].weapons[0].AttachmentWeapon.transform.localScale = new Vector3(-1, 1, 1);
                players[0].addChild(spPlane2);
                skills.AddRange(convert(spPlane2._info.Info.skills.ToArray(), spPlane2));
            }
            SkillContainer.Instance.DataSource = skills.ToObservableList();
            NodeCanvas.Framework.GlobalBlackboard.Find("Global").SetValue("Main", players[0]);
            GetComponent<IBlackboard>().SetValue("Main", players[0]);

            players[0].transform.position = startPoint.transform.position;
            Sequence pSeq = DOTween.Sequence();
            pSeq.AppendInterval(0.5f);
            pSeq.Append(players[0].transform.DOMove(endPoint.transform.position, 1).SetEase(Ease.OutExpo));
            pSeq.AppendCallback(delegate {

                players[0].machine.SetTrigger("Start");
            });
            pSeq.Play();
            if (GameManager.Instance.isFree)
            {
                GameManager.Instance.Database.SelectedMainPlane = GameManager.Instance.Database.CacheSelectedMainPlane;
                GameManager.Instance.Database.SelectedSupportPlane1 = GameManager.Instance.Database.CacheSelectedSpPlane;
                GameManager.Instance.SaveGame();
            }
            isInitDone = true;
            GUIManager.Instance.initLevelDone();
            SceneManager.Instance.loadingDirty(StateLoadingGame.PoolFirst);
            pHistory.startHeath = CurrentPlayer._health.CurrentHealth;

            startMatchInfo.selectedMainPlane = GameManager.Instance.Database.SelectedMainPlane;
            startMatchInfo.levelMain = GameManager.Instance.Database.getPlane(GameManager.Instance.Database.SelectedMainPlane).CurrentLevel;
            startMatchInfo.selectedSupport = GameManager.Instance.Database.SelectedSupportPlane1;
            startMatchInfo.levelMainSp = GameManager.Instance.Database.getSPPlane(GameManager.Instance.Database.SelectedSupportPlane1).CurrentLevel;
            startMatchInfo.IsFreePlay = GameManager.Instance.isFree;
            for(int i = 0; i < _infoLevel.InputConfig.itemUsed.Count; ++i)
            {
                startMatchInfo.usedItem.Add(_infoLevel.InputConfig.itemUsed[i].ItemID);
            }
            startMatchInfo.level = GameManager.Instance.ChoosedLevel;
            startMatchInfo.mode = GameManager.Instance.ChoosedHard;
            EazyAnalyticTool.LogEventQueue("StartGame",delegate(bool pResult,string pData) {
                if (pResult)
                {
                    var pJson = JsonMapper.ToObject(pData); 
                   startMatchInfo.matchID = pJson["match_id"].ToJson();
                }
            },5, "Data",JsonUtility.ToJson(startMatchInfo));
     
        }
        protected override void Awake()
        {
            HUDLayer.Instance.BoxReborn.GetComponent<BoxReborn>().resetFreeTurn();
            SoundManager.Instance.PlayMusic(GameManager.Instance.ChoosedHard ==0 ? AudioGroupConstrant.MusicRegion.MusicGameEasy : (GameManager.Instance.ChoosedHard == 1 ? AudioGroupConstrant.MusicRegion.MusicGameHard : AudioGroupConstrant.MusicRegion.MusicGameSuperHard),true,SoundManager.Instance.gameObject,"",1);
            if (SceneManager.Instance.currentScene.Contains("Main"))
            {
                Destroy(gameObject);
                return;
            }
            base.Awake();
      
            TimeKeeper.Instance.getTimer("Map").TimScale = 1 ;
            for (int i = 0; i < 24; i++)
            {
                var pMainLeaderObject = new GameObject();
                var pTime = pMainLeaderObject.AddComponent<TimeControllerElement>();
                pTime._groupName = TimeKeeper.Instance.getTimeLineIndex("Enemies") ;
                pMainLeaderObject.AddComponent<RootMotionController>();
                var pLeader = pMainLeaderObject.AddComponent<MovingLeader>();
                GroupManager.leaders.Add(pLeader);
                 pMainLeaderObject.SetActive(false);
                 pMainLeaderObject.name = "leader";
            }
            for (int i = 0; i < 5; i++)
            {
                var pMainLeaderObject = new GameObject();
                var pTime = pMainLeaderObject.AddComponent<TimeControllerElement>();
                pTime._groupName = TimeKeeper.Instance.getTimeLineIndex("Enemies") + 1;
                var pManager = pMainLeaderObject.AddComponent<GroupManager>();
                pMainLeaderObject.AddComponent<AIMachine>();
                GroupManager.managers.Add(pManager);
                pMainLeaderObject.SetActive(false);
                pMainLeaderObject.name = "manager";
            }
            //  cacheVolum = AudioListener.volume;
            //AudioListener.volume = 0;
            //  GameManager.Instance.backgroundStage[GameManager.Instance.ChoosedHard].gameObject.SetActive(true);


            // Invoke("turnOnVolume", 2);      
            StartCoroutine(preloadAwake());
          
           
        }

        public void OnEzEvent(PickEvent eventType)
        {
            if(eventType._owner == CurrentPlayer.gameObject)
            {
                if (eventType._nameItem == "Coin")
                {
                    _infoLevel.goldTaken +=(int)CurrentRateCoin * (int)eventType._owner.GetComponent<Character>().getFactorWithItem("Coin");
                    GUIManager.Instance.setGoldScore(_infoLevel.goldTaken);
                }
                var  pCurrentLife = historyMatch.timeLifes[historyMatch.timeLifes.Count - 1];
                if (eventType._nameItem != "Coin")
                {
                    pCurrentLife.eatedItems.Add(new DetailItemUsedInfo() { itemID = eventType._nameItem, time = (int)CurrentTime.TotalSeconds });
                }
            }
        }
        public void StartGame()
        {
            currentDelayStartGame = delayStartGame;
            StartCoroutine(delayAction(delayStartGame, delegate
            {
                Physics2D.autoSimulation = true;
            }));
        }
        public SkillInputData[] convert(SkillInfoInstance[] pInfos ,Character pTarget)
        {
            List<SkillInputData> pDatas = new List<SkillInputData>();
            for(int i = 0; i <pInfos.Length; ++i)
            {
                pDatas.Add( new SkillInputData(pInfos[i], pTarget));
            }
            return pDatas.ToArray();
        }

        private IEnumerator delayAction(float pSec, System.Action pAction)
        {
            yield return new WaitForSeconds(pSec);
            pAction.Invoke();
        }

        private void Start()
        {
     
           
            if (PlayerEnviroment.eviromentInstant != null)
            {
                PlayerEnviroment.clear();
            }
        }

        private void OnDestroy()
        {
            if (SoundManager.Instance.IsDestroyed() || GameManager.Instance.IsDestroyed()) return;
            SoundManager.Instance.StopMusicGroupName(GameManager.Instance.ChoosedHard == 0 ? AudioGroupConstrant.MusicRegion.MusicGameEasy : (GameManager.Instance.ChoosedHard == 1 ? AudioGroupConstrant.MusicRegion.MusicGameHard : AudioGroupConstrant.MusicRegion.MusicGameSuperHard), null, 1);
            if (PlayerEnviroment.eviromentInstant != null)
            {
                PlayerEnviroment.eviromentInstant.Clear();
            }
        }
        protected bool startTime = false;
        private float currentTime = 0;
        private void Update()
        {
            if (!isInitDone) return;
            if (startTime)
            {
                if(!isPause && IsMatching)
                {
                    currentTime += Time.deltaTime;
                }
            }
            if(currentDelayStartGame > 0)
            {
                currentDelayStartGame -= Time.deltaTime;
                if(currentDelayStartGame <= 0 )
                {
                    startTime = true;
                    IsPlaying = true;
                    IsMatching = true;
                    EzEventManager.TriggerEvent(new MessageGamePlayEvent("StartGame"));
                    for (int i = 0; i < _infoLevel.InputConfig.itemUsed.Count; ++i)
                    {
                        if (!_infoLevel.InputConfig.itemUsed[i].isActive)
                        {
                            var pItemID = _infoLevel.InputConfig.itemUsed[i].ItemID;
                            var pCate = _infoLevel.InputConfig.itemUsed[i].categoryItem;
                            StartCoroutine(delayAction(i * 0.1f, delegate
                            {                   
                                EzEventManager.TriggerEvent(new InputButtonTrigger(pItemID, pCate));
                            }));
          
                        }
                    }
                }
            
            }
      
        }

        public void OnEzEvent(MessageGamePlayEvent eventType)
        {
            if(eventType._message == "WinGame")
            {
                GameManager.Instance.Database.getComonItem("Energy").Quantity++;
                GetComponent<IBlackboard>().SetValue("isWin", true);
                startTime = false;
                IsMatching = false;
                Time.timeScale = 1;
                GUIManager.Instance.stopDrag();
            }
            else if (eventType._message == "LoseGame")
            {
                IsMatching = false;
                GetComponent<IBlackboard>().SetValue("isWin", false);
                startTime = false;
            }
            else if (eventType._message.StartsWith( "Result"))
            {
                TimeKeeper.Instance.getTimer("Global").TimScale = 0;
            }else if (eventType._message.StartsWith("ChangeBooster") && (GameObject)eventType._objects[0] == CurrentPlayer.gameObject)
            {
                GUIManager.Instance.setBarBooster((int)eventType._objects[1]);
                TimeKeeper.Instance.getTimer("Map").TimScale = 1 + (int) eventType._objects[1] * 0.1f;
            }
        }

        List<GameObject> bornEnemy = new List<GameObject>();

        public void addBornEnemy(GameObject pObject)
        {
            var pChar = pObject.GetComponent<Character>();
            if (!pChar) return;
            string pKey = "";
            if (pChar.originalPreb.tryGetRuntimeKey(out pKey) && !pObject.name.Contains("stone"))
            {
                BornEnemy.Add(pObject);
            }
        }
        [ShowInInspector]
        [HideInEditorMode ]
        public List<GameObject> BornEnemy
        {
            get
            {
                return bornEnemy;
            }
            set
            {
                bornEnemy = value;
            }
        }

        public TimeSpan CurrentTime {
            get
            {
                return TimeSpan.FromSeconds(currentTime);
            }
        }

        

        public bool IsPlaying { get => isPlaying; set => isPlaying = value; }
        public bool IsMatching { get => isMatching; set {
                isMatching = value;
                if (!TopLayer.Instance.IsDestroyed())
                {
                    TopLayer.Instance.gameObject.SetActive(!IsMatching);
                }
                
                if (value)
                {
                    if (!SoundManager.Instance.states.Contains("Matching"))
                    {
                        SoundManager.Instance.states.Add("Matching");
                    }
                }
                else
                {
                    SoundManager.Instance.states.Remove("Matching");
                }
            }
        }
  
    }
}
