using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Timer;

namespace EazyEngine.Space.UI
{
    public class BoxSetting : BaseBox<ItemLanguage,string>
    {
        public CustomCenterChild center;
        public ToogleSlider slideSound, slideVibrate;
        public EazyGroupTabNGUI groupSide,groupControl;

        protected bool init = false;
        public void centerLanguage(GameObject pObject)
        {
            if (init)
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    if (items[i].gameObject == pObject)
                    {
                        EzEventManager.TriggerEvent(new UIMessEvent("ChangeLanguage" + items[i].Data));
                        I2.Loc.LocalizationManager.CurrentLanguage = items[i].Data;
                    }
                }
            }
        }
        public void pause(bool pBool)
        {
            if (LevelManger.InstanceRaw == null) return;
            LevelManger.Instance.IsMatching = !pBool;
            TimeKeeper.Instance.getTimer("Global").TimScale = pBool ? 0 : 1;
        }

        public void replay()
        {
            if (GameManager.Instance.isFree)
            {
                Home();
                return;
            }
            GameManager.Instance.scehduleUI = ScheduleUIMain.REPLAY;
            MidLayer.Instance.boxPrepare.show();
            //GameManager.Instance.scehduleUI = ScheduleUIMain.GAME_IMEDIATELY;
            //PlayerEnviroment.clear();
            //GroupManager.clearCache();
            //Time.timeScale = 1;
            //LevelManger.InstanceRaw = null;
            //SceneManager.Instance.loadScene("Main");
        }

        public void Home()
        {
            HUDLayer.Instance.showDialogTag("ui/notice", "ui/alert_leave", new ButtonInfo() { str = "ui/yes", isTag = true, action = delegate {
                PlayerEnviroment.clear();
                GroupManager.clearCache();
                Time.timeScale = 1;
                GameManager.Instance.inGame = false;
                LevelManger.InstanceRaw = null;
                SceneManager.Instance.loadScene("Main");
                HUDLayer.Instance.BoxDialog.close();
            } }, new ButtonInfo() { str = "ui/no", isTag = true, action = null });
          
        }
        public void turnSound(bool pBool)
        {
            if (init)
            {
                SoundManager.Instance.SfxOn = pBool;
            }
        }

        public void turnSound()
        {
            turnSound(!SoundManager.Instance.SfxOn);
        }

        private void OnEnable()
        {
            GameManager.Instance.showBannerAds(true);
        }
        private void OnDisable()
        {
            if (GameManager.Instance.IsDestroyed()) return;
            GameManager.Instance.showBannerAds(false);
        }
        public void turnVibrate(bool pBool)
        {
            //SoundManager.Instance.vib = pBool;
            if (init)
            {
                PlayerPrefs.SetInt("Vibrate", pBool ? 1 : 0);
            }
        }
        public void turnVibrate()
        {
            turnVibrate(PlayerPrefs.GetInt("Vibrate",1) == 1);
        }
        public void chooseSide(int index)
        {
            if (init)
            {
                EzEventManager.TriggerEvent(new UIMessEvent("ChangeSide" + (index == 0 ? "Left" : "Right")));        
                PlayerPrefs.SetInt("Handle", index);
            }
        }

        public void chooseControl(int index)
        {
            if (init)
            {
                EzEventManager.TriggerEvent(new UIMessEvent("Control" + index));
                PlayerPrefs.SetInt("Control", index);
            }
        }
         public void reload()
        {
            if (slideSound.Value != SoundManager.Instance.SfxOn) { slideSound.turnValue(); }
            groupControl.changeTab(PlayerPrefs.GetInt("Control", 1));
            groupSide.changeTab(PlayerPrefs.GetInt("Handle",0));
            if (slideVibrate.Value != (PlayerPrefs.GetInt("Vibrate", 1) == 1)) { slideVibrate.turnValue(); }
            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i].Data == I2.Loc.LocalizationManager.CurrentLanguage)
                {
                    center.setCurrentPage(i);
                }
            }
            init = true;
         }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
