using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EasyMobile;
using System;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms;

namespace EazyEngine.Space.UI
{

    public class MainScene : Singleton<MainScene>,IBackBehavior
    {
    
        public BoxInfoPlane boxInfo;
        public UIElement boxSetting;
        public BoxDailyGift boxdailyGift;
        public ItemStorageRequire requireUnlock;
        public UIButton btnUnlock, btnFreePlay;
        public GameObject boxRank;
        public UIButton btnFight;
        public UILabel nameUser,idUser;
        public GameObject block;
       protected List<BoxBasePlane> selectedBoxPlane = new List<BoxBasePlane>();
        public EazyGroupTabNGUI chooseHardMode;
        protected PlaneInfoConfig selectedPlane;

        protected List<string> stateGames = new List<string>();
        protected override void Awake()
        {
            base.Awake();
            stateGames.Add("Main");
            if ( GameManager.Instance.Database.lastOnline.Date != System.DateTime.Now.Date && GameManager.Instance.wincount == 1)
            {
                GameManager.Instance.Database.lastOnline = System.DateTime.Now;
                if (StoreReview.CanRequestRating())
                {
                    HUDLayer.Instance.BoxRate.gameObject.SetActive(true);
                }               
            }
            bool isConnected = false;
            var pDateTime = TimeExtension.GetNetTime(ref isConnected);
                if (isConnected && GameManager.Instance.dailyGiftModule.lastDate != pDateTime.DayOfYear && GameManager.Instance.dailyGiftModule.currentDay < GameDatabase.Instance.databaseDailyGift.item.Count)
                {
                    boxdailyGift.GetComponent<UIElement>().show();
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
            if (www.error != null)
            {
                action(false);
            }
            else
            {
                action(true);
            }
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
            boxSetting.show();
        }
        public void setDataMainPlane(PlaneInfoConfig pInfo)
        {
            selectedPlane = pInfo;
            boxInfo.Data = pInfo;
            btnUnlock.gameObject.SetActive(!(pInfo.CurrentLevel > 0));
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
        
            GameManager.Instance.LoadLevel(GameManager.Instance.ChoosedLevel);
                
        }

        public void freePlay()
        {
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
                //for(int i = 0; i <3; ++i)
                //{
                //   var pLevel = GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, i);
                //    if(pLevel.isLocked)
                //    {
                //        chooseHardMode.GroupTab[i].GetComponent<UIButton>().isEnabled = false;
                //    }
                //    else
                //    {
                //        chooseHardMode.GroupTab[i].GetComponent<UIButton>().isEnabled = true;
                //    }
                //}
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
            if (GameManager.Instance.ConfigLevel.itemUsed.Contains( (ItemGame) pItem.item))
            {
                GameManager.Instance.ConfigLevel.itemUsed.Remove((ItemGame)pItem.item);
            }
            else
            {
                GameManager.Instance.ConfigLevel.itemUsed.Add((ItemGame)pItem.item);
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            if (GameServices.LocalUser != null)
            {
                nameUser.text = GameServices.LocalUser.userName;
                idUser.text = GameServices.LocalUser.id;
            }
            else
            {
                nameUser.text = "GUEST";
                idUser.text = "UNKNOW";
            }
            if (GameManager.Instance.planNextLevel)
            {
                choosedMap();
                StartCoroutine(delayAction(0.25f, preparePlay));
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
                nameUser.text = GameServices.LocalUser.userName;
                idUser.text = GameServices.LocalUser.id;
            }
            else
            {
                nameUser.text = "GUEST";
                idUser.text = "UNKNOW";
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
