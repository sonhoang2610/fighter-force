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
	     
	        BoxReborn.close();
	        GameManager.Instance.showRewardAds(BoxReborn.GetComponent<BoxReborn>().itemExchange,delegate(bool pBool){
		        reviePlayer();
	        });
            //GameManager.Instance.
        }

       public void reviePlayer()
        {

            HUDLayer.Instance.BoxReborn.close();

            LevelManger.Instance.CurrentPlayer.GetComponent<Health>().Revive();
            LevelManger.Instance.CurrentPlayer.GetComponent<CharacterHandleWeapon>().ShootStart();
            LevelManger.Instance.CurrentPlayer.GetComponent<Character>().machine.SetTrigger("Start");
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
            BoxReborn.close();
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

        public void showDialog(string pTitle,string pContent, ButtonInfo pAction1 =  null, ButtonInfo pAction2 = null)
        {
            ((BoxDialog)BoxDialog).Title = pTitle;
            ((BoxDialog)BoxDialog).Content = pContent;
            ((BoxDialog)BoxDialog).setButton1Info(pAction1);
            ((BoxDialog)BoxDialog).setButton2Info(pAction2);
            BoxDialog.show();
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
