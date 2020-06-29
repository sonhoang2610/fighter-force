using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.Events;
using EazyEngine.Timer;
using NodeCanvas.Framework;
using EasyMobile;

namespace EazyEngine.Space.UI {

    public class ButtonInfo
    {
        public string str;
        public UnityAction action;
        public bool  isTag = false;

        public string Value
        {
            get
            {
                return isTag ? I2.Loc.LocalizationManager.GetTranslation(str) : str;
            }
        }
        public ButtonInfo convertTag()
        {
            isTag = true;
            return this;
        }
    }
    public class HUDLayer : PersistentSingleton<HUDLayer>
    {
        public string boxDialogName, boxRebornName, boxRateName;
        private UIElement boxRate;

        [HideInInspector]
        public List<IBackBehavior> _onBacks = new List<IBackBehavior>();
        private BoxDialog boxDialog;
        private UIElement boxReborn;

        public BoxDialog BoxDialog { get => boxDialog; set => boxDialog = value; }
        public UIElement BoxReborn { get => boxReborn; set => boxReborn = value; }
        public UIElement BoxRate { get => boxRate; set => boxRate = value; }

        public System.Action _onSkipReborn;

        public void rebornCrystal()
        {
            EazyAnalyticTool.LogEvent("Reborn", "Level", GameManager.Instance.ChoosedLevel.ToString(), "Mode", GameManager.Instance.ChoosedHard.ToString(), "Type", "Normal","Count",boxReborn.GetComponent<BoxReborn>().CurrentReborn.ToString(),"matchID",LevelManger.Instance.startMatchInfo.matchID);
            var pItem = GameManager.Instance.Database.getComonItem("Crystal");
            LevelManger.Instance.IsMatching = true;
            if (pItem.Quantity >= BoxReborn.GetComponent<BoxReborn>().getPriceRebornCrystal())
            {
                TimeKeeper.Instance.getTimer("Global").TimScale = 1;
                pItem.Quantity -= BoxReborn.GetComponent<BoxReborn>().getPriceRebornCrystal();
                GameManager.Instance.showBannerAds(false);
                reviePlayer(1,true);
                BoxReborn.GetComponent<BoxReborn>().CurrentReborn++;
            }
            else
            {
                BoxReborn.gameObject.SetActive(false);
                ShopManager.Instance.showBoxShop("Crystal");
                ShopManager.Instance.GetComponent<UIElement>().onDisableEvent.RemoveListener(onSkipShop);
                ShopManager.Instance.GetComponent<UIElement>().onDisableEvent.AddListener(onSkipShop);
            }
        }

        private void OnDisable()
        {
         
        }

        public void onSkipShop()
        {
            if (LevelManger.InstanceRaw && LevelManger.Instance.IsMatching)
            {
                BoxReborn.show();
            }
        }
        public void rebornWatchAds()
        {
         
            GameManager.Instance.showRewardAds(BoxReborn.GetComponent<BoxReborn>().itemExchange,delegate(ResultStatusAds pBool){
                BoxReborn.close();
          
                if (pBool == ResultStatusAds.Success)
                {
                    EazyAnalyticTool.LogEvent("Reborn", "Level", GameManager.Instance.ChoosedLevel.ToString(), "Mode", GameManager.Instance.ChoosedHard.ToString(), "Type", "ADS");
                    LevelManger.Instance.IsMatching = true;
                    TimeKeeper.Instance.getTimer("Global").TimScale = 1;
                    GameManager.Instance.showBannerAds(false);
                    reviePlayer(1,false);
                    BoxReborn.GetComponent<BoxReborn>().watchReborn();
                }
                else if(pBool == ResultStatusAds.Failed)
                {
                    skipReborn();
                }
                else
                {
                    BoxReborn.show();
                }
       
	        },PositionADS.Reborn);
            //GameManager.Instance.
        }

       public void reviePlayer(float pHealthFactor = 1,bool booster = true)
        {
            var pOldLife = LevelManger.Instance.historyMatch.timeLifes[LevelManger.Instance.historyMatch.timeLifes.Count - 1];
            pOldLife.reborn = booster ? "Crystal" : "ADS";
            pOldLife.timeEnd = (int)LevelManger.Instance.CurrentTime.TotalSeconds;
            LevelManger.Instance.historyMatch.timeLifes.Add(new HistoryDetailLifeInfo()
            {
                timeStart = (int)LevelManger.Instance.CurrentTime.TotalSeconds,
                startHeath = (int)(LevelManger.Instance.CurrentPlayer.GetComponent<Health>().InitialHealth * pHealthFactor)
            });
            HUDLayer.Instance.BoxReborn.close();

            LevelManger.Instance.CurrentPlayer.GetComponent<Health>().Revive();
            LevelManger.Instance.CurrentPlayer.GetComponent<Health>().currentHealth = (int)(LevelManger.Instance.CurrentPlayer.GetComponent<Health>().currentHealth * pHealthFactor);
            LevelManger.Instance.CurrentPlayer.GetComponent<CharacterHandleWeapon>().ShootStart();
            LevelManger.Instance.CurrentPlayer.GetComponent<Character>().machine.SetTrigger("Start");
            if (booster)
            {
                LevelManger.Instance.CurrentPlayer.GetComponent<CharacterHandleWeapon>().booster("Booster3");
            }
            else
            {
                LevelManger.Instance.CurrentPlayer.GetComponent<CharacterHandleWeapon>().booster("Booster2");
            }
            GUIManager.Instance.setHealthMainPlane(LevelManger.Instance.CurrentPlayer.GetComponent<Health>().currentHealth, LevelManger.Instance.CurrentPlayer.GetComponent<Health>().MaxiumHealth);
        }

        public void checkCrystal(UIButton btn)
        {
            var pItem = GameManager.Instance.Database.getComonItem("Crystal");
            btn.GetComponentInChildren<UILabel>().text = BoxReborn.GetComponent<BoxReborn>().getPriceRebornCrystal().ToString();
            if (pItem.Quantity < BoxReborn.GetComponent<BoxReborn>().getPriceRebornCrystal())
            {
              //  btn.isEnabled = false;
            }
            else
            {
              //  btn.isEnabled = true;
            }
        }
        public void skipReborn()
        {
            BoxReborn.close();
            _onSkipReborn?.Invoke();
            GUIManager.Instance.boxResult.showResult(false);
        }
        public int compareBack(IBackBehavior pBack1, IBackBehavior pBack2)
        {
            return pBack1.getLevel().CompareTo(pBack2.getLevel());
        }
        public void addListenerBack(IBackBehavior pBack)
        {
            _onBacks.Add(pBack);
            _onBacks.Sort(compareBack);
        }

        public void removeLisnterBack(IBackBehavior pBack)
        {
            _onBacks.Remove(pBack);
            _onBacks.Sort(compareBack);
        }
        protected override void Awake()
        {
            base.Awake();
            BoxDialog = transform.Find(boxDialogName).GetComponent<BoxDialog>();
            BoxReborn = transform.Find(boxRebornName).GetComponent<UIElement>();
            BoxRate = transform.Find(boxRateName).GetComponent<UIElement>();
        }
        public void onBack()
        {
            for (int i = _onBacks.Count - 1; i >= 0; --i)
            {
                if (_onBacks[i].onBack())
                {
                    return;
                }
            }
        }

        public void showDialog(string pTitle, string pContent, ButtonInfo pAction1 = null, ButtonInfo pAction2 = null, bool showButton = true,bool showCLose = true)
        {
            ((BoxDialog)BoxDialog).Title = pTitle;
            ((BoxDialog)BoxDialog).Content = pContent;
            ((BoxDialog)BoxDialog).setButton1Info(pAction1);
            ((BoxDialog)BoxDialog).setButton2Info(pAction2);
            ((BoxDialog)BoxDialog).disableButton(!showButton);
            ((BoxDialog)BoxDialog).enableButtonClose(showCLose);
            BoxDialog.show();
        }

        public void showDialogTag(string pTagTitle, string pTagContent, ButtonInfo pAction1 = null, ButtonInfo pAction2 = null, bool showCLose = true)
        {
            showDialog(I2.Loc.LocalizationManager.GetTranslation(pTagTitle), I2.Loc.LocalizationManager.GetTranslation(pTagContent), pAction1.convertTag(), pAction2.convertTag(),showCLose);
        }

     
        public void showDialogNotEnoughMoney(string pItemNameNotEnough, UnityAction pAction1 = null, UnityAction pAction2 = null, bool showCLose = true)
        {
            showDialog(I2.Loc.LocalizationManager.GetTranslation("ui/notice"),string.Format( I2.Loc.LocalizationManager.GetTranslation("text/not_enough_money"), pItemNameNotEnough),  new ButtonInfo() {str = "ui/yes",isTag = true,action = pAction1 }, new ButtonInfo() { str = "ui/no", isTag = true, action = pAction2 }, true,showCLose);
        }
        public void showDialogNotEnoughCantBuy(string pItemNameNotEnough, UnityAction pAction1 = null, UnityAction pAction2 = null, bool showCLose = true)
        {
            showDialog(I2.Loc.LocalizationManager.GetTranslation("ui/notice"), string.Format(I2.Loc.LocalizationManager.GetTranslation("text/not_enough_cant_buy"), pItemNameNotEnough), new ButtonInfo() { str = "ui/yes", isTag = true, action = pAction1 }, new ButtonInfo() { str = "ui/no", isTag = true, action = pAction2 }, false,showCLose);
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void rating()
        {
            StoreReview.RequestRating();
        }
    }
}
