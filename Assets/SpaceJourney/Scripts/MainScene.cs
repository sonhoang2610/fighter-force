using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EasyMobile;
using System;
using UnityEngine.Networking;

namespace EazyEngine.Space.UI
{

    public class MainScene : Singleton<MainScene>,IBackBehavior
    {
    
        public BoxInfoPlane boxInfo;
        public UIElement boxSetting;
        public ItemStorageRequire requireUnlock;
        public UIButton btnUnlock, btnFreePlay;
        public UIButton btnFight;
        public GameObject boxRank;
        public UILabel nameUser,idUser;
        public GameObject block;
        protected List<BoxBasePlane> selectedBoxPlane = new List<BoxBasePlane>();
        public EazyGroupTabNGUI chooseHardMode;
        public UILabel desItemSp;
        public LayerPrepare layerPrepare;
        public AudioClip fightSfx;
        protected PlaneInfoConfig selectedPlane;

        protected List<string> stateGames = new List<string>();
        protected override void Awake()
        {
            base.Awake();
            Time.timeScale = 1;
            GroupManager.clearCache();
            if (layerPrepare)
            {
                layerPrepare.showInfo(0,0);
            }
            stateGames.Add("Main");
            if ( GameManager.Instance.Database.lastOnline.Date != System.DateTime.Now.Date && GameManager.Instance.wincount == 1)
            {
                GameManager.Instance.Database.lastOnline = System.DateTime.Now;
                if (StoreReview.CanRequestRating())
                {
                    StoreReview.RequestRating();
                }               
            }
            bool isConnected = false;
              var pDateTime = TimeExtension.GetNetTime(ref isConnected);
                if (isConnected && GameManager.Instance.dailyGiftModule.lastDate != pDateTime.DayOfYear && GameManager.Instance.dailyGiftModule.currentDay < GameDatabase.Instance.databaseDailyGift.item.Count)
                {
                    int pStepGame = PlayerPrefs.GetInt("firstGame", 0);
                    if (pStepGame <2) return;
                    MidLayer.Instance.boxDailyGift.GetComponent<UIElement>().show();
                }
      
        }

   
        // Subscribe to events in the OnEnable method of a MonoBehavior script
        void OnEnable()
        {
            GameServices.UserLoginSucceeded += OnUserLoginSucceeded;
            GameServices.UserLoginFailed += OnUserLoginFailed;
        }

        // Unsubscribe
        void OnDisable()
        {
            GameServices.UserLoginSucceeded -= OnUserLoginSucceeded;
            GameServices.UserLoginFailed -= OnUserLoginFailed;
        }

        
        IEnumerator checkInternetConnection(Action<bool> action)
        {
            UnityWebRequest www = new UnityWebRequest("http://google.com");
            yield return www;
            action(www.error == null);
        }
        public void clearBox()
        {
          
            for (int i = 0; i < selectedBoxPlane.Count; ++i)
            {
                selectedBoxPlane[i].selected(false);
            }
	        selectedBoxPlane.Clear();
        }

        public void clearBoxIfHasItemInBox(BoxBasePlane pPlane)
        {
            if (pPlane.DataSource.Count > 0)
            {
       
                for (int i = 0; i < selectedBoxPlane.Count; ++i)
                {
                    selectedBoxPlane[i].selected(false);
                }
                selectedBoxPlane.Clear();
            }
        }

        public void addSelectedBoxPlane(BoxBasePlane pPlane)
        {
            if (selectedBoxPlane.Contains(pPlane)) return;
            if (pPlane.DataSource.Count <= 0) return;
                selectedBoxPlane.Add(pPlane);
            pPlane.selected(true);
            pPlane.updatePage();
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
            for(int i = selectedBoxPlane.Count -1; i >= 0; --i)
            {
                selectedBoxPlane[i].selected(false);
                selectedBoxPlane.RemoveAt(i);
            }
        }

        public void nextPage()
        {
            for(int i = 0; i < selectedBoxPlane.Count; ++i)
            {
                selectedBoxPlane[i].nextPage();
            }
        }

        public void previousPage()
        {
            for (int i = 0; i < selectedBoxPlane.Count; ++i)
            {
                selectedBoxPlane[i].previousPage();
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

        public void showBoxOnline()
        {
            MidLayer.Instance.BoxGiftOnline.show();
        }
        public void showBoxRankServer()
        {
            if (GameServices.IsInitialized())
            {
                GameServices.ShowLeaderboardUI();
            }
        }

        public void showBoxShop()
        {
            MidLayer.Instance.boxShop.show();
        }
        public void setDataMainPlane(PlaneInfoConfig pInfo)
        {
            selectedPlane = pInfo;
            boxInfo.Data = pInfo;
            btnFreePlay.gameObject.SetActive(!(pInfo.CurrentLevel > 0));
            boxRank.gameObject.SetActive((pInfo.CurrentLevel > 0));
            boxRank.GetComponentInChildren<EazyFrameCache>().setFrameIndex(pInfo.Rank);
            if (pInfo.CurrentLevel == 0)
            {
                requireUnlock.gameObject.SetActive(true);
                if (pInfo.Info.conditionUnlock.craftItem)
                {
                    requireUnlock.Data = pInfo.Info.conditionUnlock;
                    btnUnlock.isEnabled = requireUnlock.isEnough;
                }
            }
            else
            {
                requireUnlock.gameObject.SetActive(false);
            }
            if(pInfo.GetType() != typeof(EazyEngine.Space.SupportPlaneInfoConfig))
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
            SoundManager.Instance.PlaySound(fightSfx, Vector3.zero);
            GameManager.Instance.LoadLevel(GameManager.Instance.ChoosedLevel);
                
        }

        public void freePlay()
        {
            GameManager.Instance.isFree = true;
            GameManager.Instance.ConfigLevel = new LevelConfig();
            GameManager.Instance.ChoosedLevel = -1;
            GameManager.Instance.LoadLevel(GameManager.Instance.ChoosedLevel);
        }

        public void choosedMap()
        {          
            stateGames.Add("ChooseMap");
            EzEventManager.TriggerEvent(new UIMessEvent("ChooseMap"));
        }
        public void preparePlay()
        {
 
            StartCoroutine(delayAction(0.25f, delegate
            {
                chooseHardMode.changeTab(GameManager.Instance.ChoosedLevel == GameManager.Instance.Database.lastPlayStage.x ?  GameManager.Instance.Database.lastPlayStage.y : 0);
            }));
       
            GameManager.Instance.ConfigLevel = new LevelConfig();
            stateGames.Add("Play");
            EzEventManager.TriggerEvent(new UIMessEvent("Play"));
        }

        public void upgrade()
        {           
            stateGames.Add("Upgrade");
            EzEventManager.TriggerEvent(new UIMessEvent("Upgrade"));
        }


        public void chooseUseItem(object pObject)
        {
            if (pObject == null) return;
            var pItem = (BaseItemGameInstanced)pObject;
            desItemSp.text = pItem.item.descriptionItem.Value;
            if (GameManager.Instance.ConfigLevel.itemUsed.Contains( (ItemGame) pItem.item))
            {
                desItemSp.gameObject.SetActive(false);
                GameManager.Instance.ConfigLevel.itemUsed.Remove((ItemGame)pItem.item);
            }
            else
            {
                desItemSp.gameObject.SetActive(true);
                GameManager.Instance.ConfigLevel.itemUsed.Add((ItemGame)pItem.item);
            }
        }
        // Start is called before the first frame update
        void Start()
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
            if (GameManager.Instance.scehduleUI == ScheduleUIMain.GAME_IMEDIATELY)
            {
                choosedMap();
                StartCoroutine(delayAction(0.25f, preparePlay));
            }else if(GameManager.Instance.scehduleUI == ScheduleUIMain.UPGRADE)
            {
                upgrade();
            }

            int pFirstGame = PlayerPrefs.GetInt("firstGame", 0);
            if (pFirstGame == 0)
            {
                GameManager.Instance.Database.firstOnline = System.DateTime.Now;
                GameManager.Instance.SaveGame();
                EzEventManager.TriggerEvent(new GuideEvent("FirstGame", delegate
                {
                    PlayerPrefs.SetInt("firstGame", 1); 
                    MainScene.Instance.freePlay();
                }));
            }else if (pFirstGame == 1)
            {
                EzEventManager.TriggerEvent(new GuideEvent("SecondGame" + ((GameManager.Instance.lastResultWin == 1)  ? "Win" :  "Lose"),
                    delegate
                    {
              
                        PlayerPrefs.SetInt("firstGame", 2); 
                        MainScene.Instance.upgrade();
              
                    },true));
              
            }
        }


        IEnumerator delayAction(float pDelay ,System.Action action)
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
    Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");
#endif
            }
        }

        void OnUserLoginSucceeded()
        {
            if (GameServices.LocalUser != null)
            {
                nameUser.text = GameServices.LocalUser.userName.Length< 10 ? GameServices.LocalUser.userName :  (GameServices.LocalUser.userName.Substring(0,7) + "...");
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
            if(stateGames.Count > 1)
            {
                stateGames.RemoveAt(stateGames.Count - 1);
                EzEventManager.TriggerEvent(new UIMessEvent(stateGames[stateGames.Count-1]));
                return true;
            }
            return false;
        }

        public int getLevel()
        {
            return 0;
        }
    }
}
