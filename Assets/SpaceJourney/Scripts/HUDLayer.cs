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
        public UIElement boxDialog,boxReborn,boxRate,boxSetting;
        [HideInInspector]
        public List<IBackBehavior> _onBacks = new List<IBackBehavior>();
        public void rebornCrystal()
        {
            var pItem = GameManager.Instance.Database.getComonItem("Crystal");
            if (pItem.Quantity >= 50)
            {
                TimeKeeper.Instance.getTimer("Global").TimScale = 1;
                pItem.Quantity -= 50;
                reviePlayer();
            }
        }
        public void rebornWatchAds()
        {
            TimeKeeper.Instance.getTimer("Global").TimScale = 1;
	     
	        boxReborn.close();
	        GameManager.Instance.showRewardAds(boxReborn.GetComponent<BoxReborn>().itemExchange,delegate(bool pBool){
		        reviePlayer();
	        });
            //GameManager.Instance.
        }

       public void reviePlayer()
        {

            HUDLayer.Instance.boxReborn.close();

            LevelManger.Instance.CurrentPlayer.GetComponent<Health>().Revive();
            LevelManger.Instance.CurrentPlayer.GetComponent<CharacterHandleWeapon>().ShootStart();
            LevelManger.Instance.CurrentPlayer.GetComponent<CharacterHandleWeapon>().booster("booster3");
            GUIManager.Instance.setHealthMainPlane(LevelManger.Instance.CurrentPlayer.GetComponent<Health>().currentHealth, LevelManger.Instance.CurrentPlayer.GetComponent<Health>().MaxiumHealth);
        }

        public void checkCrystal(UIButton btn)
        {
            var pItem = GameManager.Instance.Database.getComonItem("Crystal");
            if (pItem.Quantity < 50)
            {
                btn.isEnabled = false;
            }
            else
            {
                btn.isEnabled = true;
            }
        }
        public void skipReborn()
        {
            boxReborn.close();
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

        public void showDialog(string pTitle,string pContent, ButtonInfo pAction1 =  null, ButtonInfo pAction2 = null)
        {
            ((BoxDialog)boxDialog).Title = pTitle;
            ((BoxDialog)boxDialog).Content = pContent;
            ((BoxDialog)boxDialog).setButton1Info(pAction1);
            ((BoxDialog)boxDialog).setButton2Info(pAction2);
            boxDialog.show();
        }

        public void showDialogTag(string pTagTitle, string pTagContent, ButtonInfo pAction1 = null, ButtonInfo pAction2 = null)
        {
            showDialog(I2.Loc.LocalizationManager.GetTranslation(pTagTitle), I2.Loc.LocalizationManager.GetTranslation(pTagContent), pAction1.convertTag(), pAction2.convertTag());
        }


        public void showDialogNotEnoughMoney(string pItemNameNotEnough, UnityAction pAction1 = null, UnityAction pAction2 = null)
        {
            showDialog(I2.Loc.LocalizationManager.GetTranslation("ui/notice"),string.Format( I2.Loc.LocalizationManager.GetTranslation("text/not_enough_money"), pItemNameNotEnough),  new ButtonInfo() {str = "ui/yes",isTag = true,action = pAction1 }, new ButtonInfo() { str = "ui/no", isTag = true, action = pAction2 });
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
