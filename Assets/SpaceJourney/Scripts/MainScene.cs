using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EasyMobile;
using System;
using UnityEngine.Networking;
using ParadoxNotion.Services;
using Firebase.Analytics;
using LitJson;

namespace EazyEngine.Space.UI
{

    public class MainScene : Singleton<MainScene>, IBackBehavior,EzEventListener<UIMessEvent>,EzEventListener<TriggerLoadAsset>
    {

        public BoxInfoPlane boxInfo;
        public UIElement boxSetting;
        public ItemStorageRequire requireUnlock;
        public UIButton btnUnlock, btnFreePlay;
        public UIButton btnFight;
        public GameObject boxRank;
        public GameObject boxLevel;
        public UILabel nameUser, idUser;
        public GameObject layerMain, layerChooseMap;
        protected List<BoxBasePlane> selectedBoxPlane = new List<BoxBasePlane>();
        
        public LayerPrepare layerPrepare;
        protected PlaneInfoConfig selectedPlane;

        protected List<string> stateGames = new List<string>();
       
        protected override void Awake()
        {
            base.Awake();
           

            downResolution();
            if (layerPrepare)
            {
                layerPrepare.showInfo(0, 0);
            }
            stateGames.Add("Main");
            if ( GameManager.Instance.Wincount == 3)
            {
                GameManager.Instance.Database.lastOnline = System.DateTime.Now;
                if (StoreReview.CanRequestRating() )
                {
                    StoreReview.RequestRating();
                }
            }
            //StartCoroutine(TimeExtension.GetNetTime((time, error) => {
            //    if (string.IsNullOrEmpty(error))
            //    {
            //        var pJson = JsonMapper.ToObject(time);
            //        DateTime pDateTime = TimeExtension.UnixTimeStampToDateTime(double.Parse(pJson["time"].ToString())).ToLocalTime();
            //        if (GameManager.Instance.dailyGiftModule.lastDate != pDateTime.DayOfYear && GameManager.Instance.dailyGiftModule.currentDay < GameDatabase.Instance.databaseDailyGift.item.Count)
            //        {
            //            int pStepGame = PlayerPrefs.GetInt("firstGame", 0);
            //            int pFirstBox = PlayerPrefs.GetInt("FirstBoxReward", 0);
            //            if (pStepGame < 2) return;
            //            if (pFirstBox == 1) return;

            //            MidLayer.Instance.boxDailyGift.FirstTime = true;
            //            MidLayer.Instance.boxDailyGift.Time = pDateTime;
            //            MidLayer.Instance.boxDailyGift.IsGetTime = true;
            //            MidLayer.Instance.boxDailyGift.GetComponent<UIElement>().show();
            //        }
            //    }
            //}));
            StartCoroutine(moduleUpdate());
        }
        public IEnumerator moduleUpdate()
        {
            if (AssetLoaderManager.Instance.getPercentJob("Main") >= 1)
            {
                yield return null;
            }
            layerChooseMap.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            layerMain.gameObject.SetActive(true);
           
        }



        // Subscribe to events in the OnEnable method of a MonoBehavior script
        void OnEnable()
        {
            GameServices.UserLoginSucceeded += OnUserLoginSucceeded;
            GameServices.UserLoginFailed += OnUserLoginFailed;
            EzEventManager.AddListener<UIMessEvent>(this);
            EzEventManager.AddListener<TriggerLoadAsset>(this);
        }

        // Unsubscribe
        void OnDisable()
        {
            GameServices.UserLoginSucceeded -= OnUserLoginSucceeded;
            GameServices.UserLoginFailed -= OnUserLoginFailed;
            EzEventManager.RemoveListener<UIMessEvent>(this);
            EzEventManager.RemoveListener<TriggerLoadAsset>(this);
        }

        //public void checkUpgradeFirstSuccess()
        //{
        //    int pFirstGame = PlayerPrefs.GetInt("firstGame", 0);
        //    if (pFirstGame == 6)
        //    {
        //        EzEventManager.TriggerEvent(new GuideEvent("FirstUpgradeSuccess1",
        //            delegate
        //            {

        //                PlayerPrefs.SetInt("firstGame", 7);
        //                MainScene.Instance.upgrade();

        //            }, true));

        //    }
        //}
        public void activeRuntime(GameObject pObject)
        {
            if (!RuntimeManager.IsInitialized())
            {
                RuntimeManager.Init();
            }
            pObject.gameObject.SetActive(false);

        }
        public void activeInapp(GameObject pObject)
        {
            bool isInitialized = InAppPurchasing.IsInitialized();
            if (!isInitialized)
            {
                InAppPurchasing.InitializePurchasing();
            }
            pObject.gameObject.SetActive(false);
        }

        public void activeNotifi(GameObject pObject)
        {
            Notifications.Init();
            pObject.gameObject.SetActive(false);
        }

        public void deactiveGameAndMono()
        {
            GameManager.Instance.gameObject.SetActive(false);
            MonoManager.current.gameObject.SetActive(false);
        }
        public void deactiveAll()
        {
            var pObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach(var pObject in pObjects)
            {
                if (pObject.activeInHierarchy && pObject.transform.parent == null)
                {
                    pObject.gameObject.SetActive(false);
                }
            }
            Resources.UnloadUnusedAssets();


            //GameManager.Instance.gameObject.SetActive(true);
            // MonoManager.current.gameObject.SetActive(true);
        }

        public void clean()
        {
            var pObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (var pObject in pObjects)
            {
                if (!pObject.IsDestroyed() && pObject.activeInHierarchy && pObject.transform.parent == null)
                {
                    Destroy(pObject);
                }
            }
            Resources.UnloadUnusedAssets();
        }
        static bool setresolotuon = false;
        public void downResolution()
        {
            if (setresolotuon) return;
            setresolotuon = true;
            //Vector2 pCurrenResolution = FindObjectOfType<UIRoot>().GetComponent<UIPanel>().GetViewSize();
            //if (Screen.height > 1920 || Screen.width > 1920)
            //{
            //    if (pCurrenResolution.x > pCurrenResolution.y)
            //    {
            //        float ratio = (float)pCurrenResolution.x / (float)pCurrenResolution.y;
            //        Screen.SetResolution(1080, (int)(1080.0f * ratio), true);
            //    }
            //    else
            //    {
            //        float ratio = (float)pCurrenResolution.x / (float)pCurrenResolution.y;
            //        Screen.SetResolution((int)(1920.0f * ratio), 1920, true);
            //    }
            //}
            //else
            //{
            //    if (pCurrenResolution.x > pCurrenResolution.y)
            //    {
            //        float ratio = (float)pCurrenResolution.x / (float)pCurrenResolution.y;
            //        Screen.SetResolution(720, (int)(720.0f * ratio), true);
            //    }
            //    else
            //    {
            //        float ratio = (float)pCurrenResolution.x / (float)pCurrenResolution.y;
            //        Screen.SetResolution((int)(1280.0f * ratio), 1280, true);
            //    }
            //}

        }

        public void downFps()
        {
            Application.targetFrameRate = 60;
        }
        IEnumerator checkInternetConnection(Action<bool> action)
        {
            UnityWebRequest www = new UnityWebRequest("http://google.com");
            yield return www;
            action(www.error == null);
            www.Dispose();
        }
        public void clearBox()
        {
            //for (int i = 0; i < selectedBoxPlane.Count; ++i)
            //{
            //    selectedBoxPlane[i].selected(false);
            //}
            //selectedBoxPlane.Clear();
        }

        public void clearBoxIfHasItemInBox(BoxBasePlane pPlane)
        {
         
            //if (pPlane.DataSource.Count > 0)
            //{

            //    for (int i = 0; i < selectedBoxPlane.Count; ++i)
            //    {
            //        selectedBoxPlane[i].selected(false);
            //    }
            //    selectedBoxPlane.Clear();
            //}
        }

        public void addSelectedBoxPlane(BoxBasePlane pPlane)
        {
        
            if (selectedBoxPlane.Contains(pPlane))
            {
                if(stateGames[stateGames.Count-1] == "Upgrade")
                {
                    return;
                }
                if (pPlane.GetType() == typeof(BoxPlaneMain))
                {
                    upgrade();
                }
                else
                {
                    upgradeSp();
                }
                return;
            }
            if (pPlane.DataSource.Count <= 0) return;
            selectedBoxPlane.Add(pPlane);
            pPlane.selected(true);
            pPlane.updatePage();
        }
        [ContextMenu("pha game")]
        public void phagame()
        {
            GameObject pObject = new GameObject();
             var pMAnager =  pObject.AddComponent<GameManager>();
            pMAnager.SaveGame();
        }

        public void removeSelectedPlane(BoxBasePlane pPlane)
        {
            if (selectedBoxPlane.Contains(pPlane))
            {
                pPlane.selected(false);
                selectedBoxPlane.Remove(pPlane);
            }
        }

        public void clearAllSelected()
        {
            for (int i = selectedBoxPlane.Count - 1; i >= 0; --i)
            {
                selectedBoxPlane[i].selected(false);
                selectedBoxPlane.RemoveAt(i);
            }
        }

        public void nextPage()
        {
            for (int i = 0; i < selectedBoxPlane.Count; ++i)
            {
                if (selectedBoxPlane[i].GetType() == typeof(BoxPlaneMain))
                {
                    selectedBoxPlane[i].nextPage();
                }
            }
        }

        public void previousPage()
        {
            for (int i = 0; i < selectedBoxPlane.Count; ++i)
            {
                if (selectedBoxPlane[i].GetType() == typeof(BoxPlaneMain))
                {
                    selectedBoxPlane[i].previousPage();
                }
            }
        }
        public void nextPageSp()
        {
            for (int i = 0; i < selectedBoxPlane.Count; ++i)
            {
                if (selectedBoxPlane[i].GetType() == typeof(BoxSpPlaneMain))
                {
                    selectedBoxPlane[i].nextPage();
                }
            }
        }

        public void previousPageSp()
        {
            for (int i = 0; i < selectedBoxPlane.Count; ++i)
            {
                if (selectedBoxPlane[i].GetType() == typeof(BoxSpPlaneMain))
                {
                    selectedBoxPlane[i].previousPage();
                }
            }
        }

        public void chooseIndexMainPlane(int index)
        {

        }
        public void chooseIndexSpPlane(int index)
        {
            // GameManager.Instance.Database.selectedSupportPlane1 = index;
            // GameManager.Instance.Database.selectedSupportPlane2 = index;
        }

        public void showBoxSetting()
        {
            TopLayer.Instance.boxsetting.show();
        }

        public void showBoxLucky()
        {
            MidLayer.Instance.boxLucky.show();
        }

        public void showBoxDaily()
        {
            MidLayer.Instance.showBoxDailyGift();
        }
        public void showBoxMission()
        {
            MidLayer.Instance.showBoxMission();
        }
        public void showBoxOnline()
        {
            MidLayer.Instance.BoxGiftOnline.show();
        }
        public void showBoxRankServer()
        {
            Debug.Log("leader board");
            if (GameServices.IsInitialized())
            {
                GameServices.ShowLeaderboardUI();
            }
        }

        public void showBoxShop()
        {
        
            EazyAnalyticTool.LogEvent("PressShop");
            MidLayer.Instance.boxShop.show();
        }
        public void setDataMainPlane(PlaneInfoConfig pInfo)
        {
            if (pInfo.GetType() == typeof(SupportPlaneInfoConfig)) return;
            selectedPlane = pInfo;
            if (boxInfo)
            {
                boxInfo.Data = pInfo;
            }
            btnFreePlay.gameObject.SetActive(!(pInfo.CurrentLevel > 0));
            boxRank.gameObject.SetActive((pInfo.CurrentLevel > 0));
            boxRank.GetComponentInChildren<EazyFrameCache>().setFrameIndex(pInfo.info.RankPlane);
            if (pInfo.CurrentLevel == 0 && pInfo.Info.conditionUnlock.quantityRequire > 0)
            {
                requireUnlock.gameObject.SetActive(true);
                boxLevel.gameObject.SetActive(false);
                if (pInfo.Info.conditionUnlock.craftItem)
                {
                    requireUnlock.Data = pInfo.Info.conditionUnlock;
                    btnUnlock.isEnabled = requireUnlock.isEnough;
                }
            }
            else
            {
                if(pInfo.CurrentLevel > 0)
                {
                    boxLevel.transform.Find("level").GetComponent<UILabel>().text = pInfo.CurrentLevel.ToString();
                    boxLevel.gameObject.SetActive(true);
                }
                requireUnlock.gameObject.SetActive(false);
            }
            if (pInfo.GetType() != typeof(EazyEngine.Space.SupportPlaneInfoConfig))
            {
                btnFight.isEnabled = pInfo.CurrentLevel > 0;
            }
        }

        public void UnlockPlane()
        {
            selectedPlane.CurrentLevel = 1;
            GameManager.Instance.Database.planes.Add(selectedPlane);

            for (int i = 0; i < selectedBoxPlane.Count; ++i)
            {
                selectedBoxPlane[i].updatePage();
                selectedBoxPlane[i].reloadData();
            }
            var pItem = GameManager.Instance.Database.getComonItem(selectedPlane.info.conditionUnlock.craftItem);
            pItem.Quantity -= selectedPlane.info.conditionUnlock.quantityRequire;
            GameManager.Instance.SaveGame();
        }

        public void LoadLevel(int pLevel)
        {
            GameManager.Instance.LoadLevel(pLevel);
        }

        public void test()
        {
            GameManager.Instance.isFree = false;
            GameManager.Instance.LoadLevel(GameManager.Instance.ChoosedLevel);

        }
        private void OnDestroy()
        {
            string[] pJobDestroy = new string[] { "Main/Upgrade", "Main/ChooseMap", "Main/MainScene" };
            for(int i = 0; i < pJobDestroy.Length; ++i)
            {
                AssetLoaderManager.Instance.destroyJob.Add(pJobDestroy[i]);
            }
            AssetLoaderManager.Instance.clearJob();
        }
        public void freePlay(UIButton btnDisable)
        {
            GameManager.Instance.isGuide = false;
            GameManager.Instance.isFree = true;
            GameManager.Instance.ConfigLevel = new LevelConfig();
            GameManager.Instance.ChoosedLevel = -1;
            GameManager.Instance.LoadLevel(GameManager.Instance.ChoosedLevel);
            btnDisable.isEnabled = false;


        }
        public void freePlayGuide()
        {
            GameManager.Instance.isGuide = true;
            GameManager.Instance.isFree = true;
            GameManager.Instance.ConfigLevel = new LevelConfig();
            GameManager.Instance.ChoosedLevel = -1;
            GameManager.Instance.LoadLevel(GameManager.Instance.ChoosedLevel);

        }

        public void choosedMap()
        {
            stateGames.Add("ChooseMap");
            EzEventManager.TriggerEvent(new UIMessEvent("ChooseMap"));
            EazyAnalyticTool.LogEvent("FightButton");
        }
        public void preparePlay()
        {

            //StartCoroutine(delayAction(0.25f, delegate
            //{
            //    chooseHardMode.changeTab(GameManager.Instance.ChoosedLevel == GameManager.Instance.Database.lastPlayStage.x ? GameManager.Instance.Database.lastPlayStage.y : 0);
            //}));

            GameManager.Instance.ConfigLevel = new LevelConfig();
            MidLayer.Instance.boxPrepare.show();
            //stateGames.Add("Play");
            //EzEventManager.TriggerEvent(new UIMessEvent("Play"));
        }

        public void upgrade()
        {
            stateGames.Add("Upgrade");
            EzEventManager.TriggerEvent(new UIMessEvent("Upgrade"));
        }
        public void upgradeSp()
        {
            stateGames.Add("Upgrade");
            EzEventManager.TriggerEvent(new UIMessEvent("Upgrade"));
            Invoke(nameof(changeTabSp), 0.1f);
        }

        public void changeTabSp()
        {
            EzEventManager.TriggerEvent(new UIMessEvent("ChangeTabSp"));
        }

        public IEnumerator setUpNotify()
        {
            Debug.Log("before notify");
            var pContainer = DatabaseNotifyReward.Instance.container;
            List<ItemNotifyReward> pNotifies = new List<ItemNotifyReward>();
            pNotifies.AddRange(pContainer);
            pNotifies.Sort((x1, x2) => x2.TimeAFK.CompareTo(x1.TimeAFK));
            for (int i = 0; i < pNotifies.Count; ++i)
            {
                var pAFKTime = (System.DateTime.Now - GameManager.Instance.Database.LastInGameTime).TotalSeconds;
                if (pAFKTime >= pNotifies[i].TimeAFK)
                {
                    GameManager.Instance.Database.lastInGameTime = System.DateTime.Now;
                    var pItems = pNotifies[i].ExtractHere(true);
                    for (int j = 0; j < pItems.Length; ++j)
                    {
                        var pStorage = GameManager.Instance.Database.getComonItem(pItems[j].item);
                        pStorage.Quantity += pItems[j].Quantity;
                    }
                    TopLayer.Instance.boxReward.show();

                    EzEventManager.TriggerEvent(new RewardEvent() { item = new BaseItemGameInstanced() { item = pNotifies[i], quantity = 1 } });
                    GameManager.Instance.SaveGame();
                    Debug.Log("break notify");
                    break;
                }

            }
            if (GameManager.Instance.Database.IdNotifySchedule.Count > 0)
            {
                foreach (var pNoti in GameManager.Instance.Database.IdNotifySchedule)
                {
                    Notifications.CancelPendingLocalNotification(pNoti);
                }
                GameManager.Instance.Database.IdNotifySchedule.Clear();
            }
            Debug.Log("end notify");
            yield return null; 
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("before start");
            if (GameManager.Instance.Database.firstTimeGame == 0)
            {
                GameManager.Instance.Database.lastInGameTime = System.DateTime.Now;
            }
            GameManager.Instance.Database.firstTimeGame++;


            StartCoroutine(setUpNotify());
   
            Time.timeScale = 1;
            GroupManager.clearCache();
            if (GameServices.LocalUser != null)
            {
                nameUser.text = GameServices.LocalUser.userName.Length < 10 ? GameServices.LocalUser.userName : (GameServices.LocalUser.userName.Substring(0, 7) + "...");
                idUser.text = GameServices.LocalUser.id.Length < 10 ? GameServices.LocalUser.id : (GameServices.LocalUser.id.Substring(0, 7) + "...");
            }
            else
            {
                nameUser.text = "GUEST";
                idUser.text = "UNKNOWN";
            }
            //if (GameManager.Instance.scehduleUI == ScheduleUIMain.GAME_IMEDIATELY)
            //{
            //    choosedMap();
            //    StartCoroutine(delayAction(0.25f, preparePlay));
            //}
            /*else*/ 
            var pJobMain = AssetLoaderManager.Instance.getPercentJob("Main");
            if (pJobMain >= 1)
            {
                if (GameManager.Instance.scehduleUI == ScheduleUIMain.UPGRADE)
                {
                    upgrade();
                }
                checkGuide = true;
                int pFirstBox = PlayerPrefs.GetInt("FirstBoxReward", 0);
                int pFirstGame = PlayerPrefs.GetInt("firstGame", 0);
                int pFirstOpenGoogle = PlayerPrefs.GetInt("FirstOpenGoogle", 0);
                if (pFirstGame != 0 && (SceneManager.Instance.previousScene.Contains("Home") || pFirstOpenGoogle == 0))
                {
#if !UNITY_STANDALONE
                    GameServices.ManagedInit();
#endif
                    PlayerPrefs.SetInt("FirstOpenGoogle", 1);
                }
                if (pFirstGame == 0)
                {
                    SceneManager.Instance.markDirtyBloomMK();
                    GameManager.Instance.Database.firstOnline = System.DateTime.Now;
                    GameManager.Instance.SaveGame();
                    EzEventManager.TriggerEvent(new GuideEvent("FirstGame", delegate
                    {
                        SceneManager.Instance.removeDirtyBloomMK();
                        PlayerPrefs.SetInt("firstGame", 9999);
                        MainScene.Instance.freePlayGuide();
                    }));

                }
                else if (pFirstGame == 1)
                {
                    EzEventManager.TriggerEvent(new GuideEvent("SecondGame" + ((GameManager.Instance.lastResultWin == 1) ? "Win" : "Lose"),
                        delegate
                        {

                            PlayerPrefs.SetInt("firstGame", 2);
                            MainScene.Instance.upgrade();

                        }, true));

                }
                else if (pFirstBox == 1)
                {
                    EzEventManager.TriggerEvent(new GuideEvent("FirstRewardBox1", delegate
                    {

                        PlayerPrefs.SetInt("FirstBoxReward", 2);
                    }, false));
                }
            }
            TopLayer.Instance.block.gameObject.SetActive(false);
            GameManager.Instance.Database.logData();
        }



        IEnumerator delayAction(float pDelay, System.Action action)
        {
            yield return new WaitForSeconds(pDelay);
            action();
        }

        public void loginGameService()
        {
            if (!GameServices.IsInitialized())
            {
#if UNITY_ANDROID
                GameServices.Init();    // start a new initialization process
#elif UNITY_IOS
                GameServices.Init();
#endif
            }
        }

        void OnUserLoginSucceeded()
        {
            if (GameServices.LocalUser != null)
            {
                nameUser.text = GameServices.LocalUser.userName.Length < 10 ? GameServices.LocalUser.userName : (GameServices.LocalUser.userName.Substring(0, 7) + "...");
                idUser.text = GameServices.LocalUser.id.Length < 10 ? GameServices.LocalUser.id : (GameServices.LocalUser.id.Substring(0, 7) + "...");
            }
            else
            {
                nameUser.text = "GUEST";
                idUser.text = "UNKNOWN";
            }
        }

        void OnUserLoginFailed()
        {
            Debug.Log("User login failed.");
        }
        // Update is called once per frame
        void Update()
        {

        }

        public void btnBack()
        {
            onBack();
        }

        public bool onBack()
        {
            if (stateGames.Count > 1)
            {
                stateGames.RemoveAt(stateGames.Count - 1);
                EzEventManager.TriggerEvent(new UIMessEvent(stateGames[stateGames.Count - 1]));
                return true;
            }
            return false;
        }

        public int getLevel()
        {
            return 0;
        }

        public void OnEzEvent(UIMessEvent eventType)
        {
            if(eventType.Event == "GameServiceLogOut")
            {
                nameUser.text = "UNKNOWN";
                idUser.text = "";
            }
        }
        bool checkGuide = false;
        public void OnEzEvent(TriggerLoadAsset eventType)
        {
            if(eventType.name == "Main/ChooseMap/Init")
            {
                if(AssetLoaderManager.Instance.getJob("Main/ChooseMap").CurrentPercent >= 1)
                {
                    layerChooseMap.gameObject.SetActive(false);
                }
            }
            var pJobMain = AssetLoaderManager.Instance.getPercentJob("Main");
            if (pJobMain >= 1 && !checkGuide)
            {
                if (GameManager.Instance.scehduleUI == ScheduleUIMain.UPGRADE)
                {
                    upgrade();
                }
                int pFirstBox = PlayerPrefs.GetInt("FirstBoxReward", 0);
                int pFirstGame = PlayerPrefs.GetInt("firstGame", 0);
                int pFirstOpenGoogle = PlayerPrefs.GetInt("FirstOpenGoogle", 0);
                if (pFirstGame != 0 && (SceneManager.Instance.previousScene.Contains("Home") || pFirstOpenGoogle == 0))
                {
#if !UNITY_STANDALONE
                    GameServices.ManagedInit();
#endif
                    PlayerPrefs.SetInt("FirstOpenGoogle", 1);
                }
                if (pFirstGame == 0)
                {
                    SceneManager.Instance.markDirtyBloomMK();
                    GameManager.Instance.Database.firstOnline = System.DateTime.Now;
                    GameManager.Instance.SaveGame();
                    EzEventManager.TriggerEvent(new GuideEvent("FirstGame", delegate
                    {
                        SceneManager.Instance.removeDirtyBloomMK();
                        PlayerPrefs.SetInt("firstGame", 9999);
                        MainScene.Instance.freePlayGuide();
                    }));

                }
                else if (pFirstGame == 1)
                {
                    EzEventManager.TriggerEvent(new GuideEvent("SecondGame" + ((GameManager.Instance.lastResultWin == 1) ? "Win" : "Lose"),
                        delegate
                        {

                            PlayerPrefs.SetInt("firstGame", 2);
                            MainScene.Instance.upgrade();

                        }, true));

                }
                else if (pFirstBox == 1)
                {
                    EzEventManager.TriggerEvent(new GuideEvent("FirstRewardBox1", delegate
                    {

                        PlayerPrefs.SetInt("FirstBoxReward", 2);
                    }, false));
                }
            }
        }
    }
}
