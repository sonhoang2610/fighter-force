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

    public class LevelConfig
    {
        public List<ItemGame> itemUsed = new List<ItemGame>();
    }
    public class LevelManger : Singleton<LevelManger>, EzEventListener<DamageTakenEvent>,EzEventListener<PickEvent>,EzEventListener<MessageGamePlayEvent>
    {
        public GameObject startPoint;
        public GameObject endPoint;
        public float delayStartGame = 2;
        [HideInEditorMode]
        public bool isPlaying = false;
        [HideInEditorMode]
        public bool isMatching = false;
        [HideInInspector]
        public int currentPlayerIndex = 0;
        [InlineEditor]
        public Camera mainPlayCamera;
        public Character[] players;
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
                _infoLevel.score += eventType.AffectedCharacter.mainInfo.score;
            }
            if(eventType.CurrentHealth <= 0 && LevelManger.Instance.BornEnemy.Contains(eventType.AffectedCharacter.gameObject))
            {
                LevelManger.Instance.BornEnemy.Remove(eventType.AffectedCharacter.gameObject);
                LevelManger.Instance._infoLevel.enemyKill++;
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
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener<DamageTakenEvent>(this);
            EzEventManager.RemoveListener<PickEvent>(this);
            EzEventManager.RemoveListener<MessageGamePlayEvent>(this);
            ObjectPooler.poolManager.Clear();
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
            AudioListener.volume = SoundManager.Instance.SfxOn ? 1: 0;
      
            SoundManager.Instance.PlayBackgroundMusic(GameManager.Instance.backgroundStage[GameManager.Instance.ChoosedHard]);
        }
        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < 24; i++)
            {
                var pMainLeaderObject = new GameObject();
                var pTime = pMainLeaderObject.AddComponent<TimeControllerElement>();
                pTime._groupName = TimeKeeper.Instance.getTimeLineIndex("Enemies") + 1;
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
                GroupManager.managers.Add(pManager);
                pMainLeaderObject.SetActive(false);
                pMainLeaderObject.name = "manager";
            }
            cacheVolum = AudioListener.volume;
            AudioListener.volume = 0;
            GameManager.Instance.backgroundStage[GameManager.Instance.ChoosedHard].gameObject.SetActive(true);
            GameManager.Instance.bossStage[GameManager.Instance.ChoosedHard].gameObject.SetActive(true);
            Invoke("turnOnVolume", 2);      
            GameManager.Instance.scehduleUI = ScheduleUIMain.NONE;
            GUIManager.Instance.enableEnergy(false);
            GUIManager.Instance.setBarBooster(0);
            string stringState = GameManager.Instance.isFree ? "Statesfree" : "States" + GameManager.Instance.ChoosedLevel + "_" + GameManager.Instance.ChoosedHard ;
            var psate = LoadAssets.loadAsset<GameObject>(stringState, "Variants/States/");
            Instantiate(psate);
            _infoLevel = GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard).infos.CloneData();
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
                    if (pItem[i].itemID == pItems[j].item.itemID)
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
            int pSelectedPlane = -1;
            for(int i = 0; i < GameManager.Instance.Database.planes.Count; ++i)
            {
                if(GameManager.Instance.Database.planes[i].info.itemID == ((!GameManager.Instance.isFree || string.IsNullOrEmpty(GameManager.Instance.freePlaneChoose) ? GameManager.Instance.Database.SelectedMainPlane : GameManager.Instance.freePlaneChoose)))
                {
                    pSelectedPlane = i;
                    break;
                }
            }
            players = new Character[1];
            players[0] = Instantiate<Character>(GameManager.Instance.Database.planes[pSelectedPlane].Info.modelPlane);
            var pDataPlane = pSelectedPlane >=0 ? GameManager.Instance.Database.planes[pSelectedPlane] : null;
            if (pDataPlane == null || pDataPlane.CurrentLevel == 0) {
                var pAllItem = GameDatabase.Instance.getAllItem(CategoryItem.PLANE);
                foreach(var pItemPlane in pAllItem)
                {
                   if( pItemPlane.itemID == GameManager.Instance.freePlaneChoose)
                    {
                        pDataPlane = PlaneInfoConfig.CloneDefault((PlaneInfo) pItemPlane);
                    }
                }
            }
            players[0].setData(pDataPlane);
            GUIManager.Instance.setIconPlane(pDataPlane.info.iconGame);
            List<SkillInputData> skills = new List<SkillInputData>();
            skills.AddRange(convert(players[0]._info.Info.skills.ToArray(), players[0]));
            int pSelectedspPlane =-1;
            for (int i = 0; i < GameManager.Instance.Database.spPlanes.Count; ++i)
            {
                if (GameManager.Instance.Database.spPlanes[i].info.itemID == ((!GameManager.Instance.isFree || string.IsNullOrEmpty(GameManager.Instance.freeSpPlaneChoose) ? GameManager.Instance.Database.SelectedSupportPlane1 : GameManager.Instance.freeSpPlaneChoose)))
                {
                    pSelectedspPlane = i;
                    break;
                }
            }
            int pSelectedSPPlane1 = pSelectedspPlane;
            var pDataSpPlane = pSelectedSPPlane1 >= 0 ? GameManager.Instance.Database.spPlanes[pSelectedSPPlane1] : null;
            if (pDataSpPlane == null || pDataSpPlane.CurrentLevel == 0)
            {
                var pAllItem = GameDatabase.Instance.getAllItem(CategoryItem.SP_PLANE);
                foreach (var pItemPlane in pAllItem)
                {
                    if (pItemPlane.itemID == GameManager.Instance.freeSpPlaneChoose)
                    {
                        pDataSpPlane = SupportPlaneInfoConfig.CloneDefaultSp((PlaneInfo)pItemPlane);
                    }
                }
            }
            if (pDataSpPlane != null)
            {
              
                var spPlane1 = Instantiate(pDataSpPlane.Info.modelPlane);
                spPlane1.setData(pDataSpPlane);
                spPlane1.GetComponent<FollowerMainPlayer>().OffsetSupportPlane = players[0].transform.Find("slot1").transform.localPosition;
                players[0].addChild(spPlane1);
                skills.AddRange(convert(spPlane1._info.Info.skills.ToArray(), spPlane1));
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
            currentDelayStartGame = delayStartGame;
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
            }
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

        private void Start()
        {
            if (PlayerEnviroment.eviromentInstant != null)
            {
                PlayerEnviroment.clear();
            }
        }

        private void OnDestroy()
        {
            if (PlayerEnviroment.eviromentInstant != null)
            {
                PlayerEnviroment.eviromentInstant.Clear();
            }
        }
        protected bool startTime = false;
        private float currentTime = 0;
        private void Update()
        {
 
            if (startTime)
            {
                currentTime += Time.deltaTime;
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
                            EzEventManager.TriggerEvent(new InputButtonTrigger(_infoLevel.InputConfig.itemUsed[i].itemID, _infoLevel.InputConfig.itemUsed[i].categoryItem));
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
            if (!GameManager.Instance.objectExcludes.Contains(pChar.originalPreb))
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
        public bool IsMatching { get => isMatching; set => isMatching = value; }
    }
}
