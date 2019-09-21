using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;


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
            Time.timeScale = pBool ? 0 : 1;
        }

        public void replay()
        {
            GameManager.Instance.planNextLevel = true;
            PlayerEnviroment.clear();
            Time.timeScale = 1;
            LevelManger.InstanceRaw = null;
            SceneManager.Instance.loadScene("Main");
        }

        public void Home()
        {
            PlayerEnviroment.clear();
            Time.timeScale = 1;
            LevelManger.InstanceRaw = null;
            SceneManager.Instance.loadScene("Main");
        }
        public void turnSound(bool pBool)
        {
            if (init)
            {
                SoundManager.Instance.SfxOn = pBool;
            }
        }


        private void OnEnable()
        {
            GameManager.Instance.showBannerAds(true);
        }
        private void OnDisable()
        {
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
