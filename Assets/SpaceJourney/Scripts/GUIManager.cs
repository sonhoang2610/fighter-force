using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EazyEngine.Tools;
using EazyEngine.Space.UI;

namespace EazyEngine.Space
{
    public class GUIManager : Singleton<GUIManager>, EzEventListener<UIMessEvent>, EzEventListener<MessageGamePlayEvent>
    {
        [SerializeField]
        private Camera camGUI;
        [SerializeField]
        private UIElement boxsetting;
        [SerializeField]
        protected UI2DSprite dangerImage;
        [SerializeField]
        protected UI2DSprite healImage;
        [SerializeField]
        protected UI2DSprite hpBarImage;
        [SerializeField]
        protected UI2DSprite iconPlane;
        [SerializeField]
        protected UILabel goldScore;
        [SerializeField]
        protected GameObject containerHandle;
        [SerializeField]
        protected GameObject posLeft, posRight, control1;
        public BoxResult boxResult;

        public Camera CamGUI { get => camGUI; set => camGUI = value; }

        public void stopDrag()
        {
            if (PlayerPrefs.GetInt("Control", 1) == 1)
            {
                control1.gameObject.SetActive(false);
            }
        }
        public void pauseGame()
        {
            if (LevelManger.InstanceRaw == null) return;
            Time.timeScale = 0;

            LevelManger.Instance.IsMatching = false;
            boxsetting.show();
        }

        public void resumeGame()
        {
            if (LevelManger.InstanceRaw == null) return;
            Time.timeScale = 1;

            LevelManger.Instance.IsMatching = true;
            boxsetting.close();
        }
        public void setIconPlane(Sprite pSprite)
        {
            iconPlane.sprite2D = pSprite;
        }
        private void OnEnable()
        {

            EzEventManager.AddListener<UIMessEvent>(this);
            EzEventManager.AddListener<MessageGamePlayEvent>(this);
        }


        private void OnDisable()
        {
            EzEventManager.RemoveListener<UIMessEvent>(this);
            EzEventManager.RemoveListener<MessageGamePlayEvent>(this);
        }
        public void setGoldScore(int pGold)
        {
            goldScore.text = pGold.ToString();
        }

        public void showDangerEffect()
        {
            if (!dangerImage) return;
            dangerImage.gameObject.SetActive(false);
            dangerImage.gameObject.SetActive(true);
        }

        public void showHealEffect()
        {
            if (!healImage) return;
            healImage.gameObject.SetActive(false);
            healImage.gameObject.SetActive(true);
        }
        public void showResult(bool pBool)
        {
            boxResult.showResult(pBool);
        }
        public void setHealthMainPlane(int pCurrent, int pMax)
        {
            hpBarImage.fillAmount = ((float)pCurrent / (float)pMax);
        }


        // Start is called before the first frame update
        void Start()
        {
            if (PlayerPrefs.GetInt("Handle", 0) == 0)
            {
                containerHandle.transform.position = posLeft.transform.position;
            }
            else
            {
                containerHandle.transform.position = posRight.transform.position;
            }

            if (PlayerPrefs.GetInt("Control", 1) == 0)
            {
                control1.gameObject.SetActive(false);
                LevelManger.Instance.CurrentPlayer.GetComponent<DragObject>().enabled = true;
            }
            else
            {
                control1.gameObject.SetActive(true);
                LevelManger.Instance.CurrentPlayer.GetComponent<DragObject>().enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnEzEvent(UIMessEvent eventType)
        {
            if (eventType.Event.StartsWith("ChangeSide"))
            {
                if (eventType.Event.Contains("Left"))
                {
                    containerHandle.transform.position = posLeft.transform.position;
                }
                else
                {
                    containerHandle.transform.position = posRight.transform.position;
                }
            }
            if (eventType.Event.StartsWith("Control"))
            {
                if (eventType.Event.Contains("1"))
                {
                    control1.gameObject.SetActive(true);
                    LevelManger.Instance.CurrentPlayer.GetComponent<DragObject>().enabled = false;
                }
                else
                {
                    control1.gameObject.SetActive(false);
                    LevelManger.Instance.CurrentPlayer.GetComponent<DragObject>().enabled = true;
                }
            }
        }

        public void OnEzEvent(MessageGamePlayEvent eventType)
        {
            if (eventType._message == "Warning")
            {
                transform.Find("dangerboss").gameObject.SetActive(true);
            }
        }
    }
}