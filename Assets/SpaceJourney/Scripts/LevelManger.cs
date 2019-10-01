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
            if(eventType.CurrentHealth <= 0 && eventType.Instigator == CurrentPlayer.gameObject)
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
                var pConfig = GameDatabase.Instance.dropMonyeconfig[GameManager.Instance.ChoosedLevel - 1][GameManager.Instance.ChoosedHard];
                int pRandomTotal = UnityEngine.Random.Range((int)pConfig.totalMoney.x,(int) pConfig.totalMoney.y);
                int pRandomTotalCoin = UnityEngine.Random.Range((int)pConfig.totalCoin.x, (int)pConfig.totalCoin.y);
                return pRandomTotal / pRandomTotalCoin; 
            }
        }
        protected float cacheVolum;
        public void turnOnVolume()
        {
            AudioListener.volume = cacheVolum;
        }
        protected override void Awake()
        {
            base.Awake();
            cacheVolum = AudioListener.volume;
            AudioListener.volume = 0;
            Invoke("turnOnVolume", 1);      
            GameManager.Instance.scehduleUI = ScheduleUIMain.NONE;
            GUIManager.Instance.enableEnergy(false);
            GUIManager.Instance.setBarBooster(0);
            var psate = LoadAssets.loadAsset<GameObject>("States"+ GameManager.Instance.ChoosedLevel + "_" + GameManager.Instance.ChoosedHard, "Variants/States/");
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
            int pSelectedPlane = 0;
            for(int i = 0; i < GameManager.Instance.Database.planes.Count; ++i)
            {
                if(GameManager.Instance.Database.planes[i].info.itemID == GameManager.Instance.Database.selectedMainPlane)
                {
                    pSelectedPlane = i;
                    break;
                }
            }
            players = new Character[1];
            players[0] = Instantiate<Character>(GameManager.Instance.Database.planes[pSelectedPlane].Info.modelPlane);
            players[0].setData(GameManager.Instance.Database.planes[pSelectedPlane]);
            GUIManager.Instance.setIconPlane(GameManager.Instance.Database.planes[pSelectedPlane].info.iconGame);
            List<SkillInputData> skills = new List<SkillInputData>();
            skills.AddRange(convert(players[0]._info.Info.skills.ToArray(), players[0]));
            int pSelectedspPlane = 0;
            for (int i = 0; i < GameManager.Instance.Database.spPlanes.Count; ++i)
            {
                if (GameManager.Instance.Database.spPlanes[i].info.itemID == GameManager.Instance.Database.selectedSupportPlane1)
                {
                    pSelectedspPlane = i;
                    break;
                }
            }
            int pSelectedSPPlane1 = pSelectedspPlane;
            if (pSelectedSPPlane1 >= 0)
            {
                var spPlane1 = Instantiate(GameManager.Instance.Database.spPlanes[pSelectedSPPlane1].Info.modelPlane);
                spPlane1.setData(GameManager.Instance.Database.spPlanes[pSelectedSPPlane1]);
                spPlane1.GetComponent<FollowerMainPlayer>().OffsetSupportPlane = players[0].transform.Find("slot1").transform.localPosition;
                players[0].addChild(spPlane1);
                skills.AddRange(convert(spPlane1._info.Info.skills.ToArray(), spPlane1));
            }
            int pSelectedSPPlane2 = pSelectedspPlane;
            if (pSelectedSPPlane2 >= 0)
            {
                var spPlane2 = Instantiate(GameManager.Instance.Database.spPlanes[pSelectedSPPlane2].Info.modelPlane);
                spPlane2.setData(GameManager.Instance.Database.spPlanes[pSelectedSPPlane2]);
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
                GUIManager.Instance.setBarBooster((float)((int)eventType._objects[1]) / 5);
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
