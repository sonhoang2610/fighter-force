using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{


    public class GiftOnlineIcon : Singleton<GiftOnlineIcon>,EzEventListener<EventTimer>
    {
        public UILabel labelTimer;
        public GameObject boxTimer;
        public GameObject dot;

        protected ItemGiftOnlineInfo item;

        public void claim()
        {
            var pCounting = GameManager.Instance.Database.timers.Find(x => x.key == "GiftOnline");
            if(pCounting != null)
            {
                GameManager.Instance.Database.timers.Remove(pCounting);
                GameManager.Instance.removeTimer(pCounting);
            }
            else
            {
                Start();
            }
        }
        private void OnDestroy()
        {
            var pCounting = GameManager.Instance.Database.timers.Find(x => x.key == "GiftOnline");
            if (pCounting != null)
            {
                GameManager.Instance.Database.timers.Remove(pCounting);
                GameManager.Instance.removeTimer(pCounting);
            }
        }

        public void OnEzEvent(EventTimer eventType)
        {
            if(eventType.key == "GiftOnline")
            {
                if(eventType.state == TimerState.Complete)
                {
                    dot.gameObject.SetActive(true);
                    boxTimer.gameObject.SetActive(false);
                    labelTimer.gameObject.SetActive(false);
                }
            }
        }

        private void OnEnable()
        {
            EzEventManager.AddListener(this);
        }
        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }


        // Start is called before the first frame update
        void Start()
        {
            var pDatabase = GameDatabase.Instance.databaseGiftOnline;
            if (GameManager.Instance.giftOnlineModule.calimedIndex < pDatabase.item.Length - 1)
            {
                item = pDatabase.item[GameManager.Instance.giftOnlineModule.calimedIndex + 1];
                if(GameManager.Instance.giftOnlineModule.onlineTime > item.time)
                {
                    dot.gameObject.SetActive(true);
                    labelTimer.gameObject.SetActive(false);
                    boxTimer.gameObject.SetActive(false);
                }
                else
                {
                    dot.gameObject.SetActive(false);
                    labelTimer.gameObject.SetActive(true);
                    boxTimer.gameObject.SetActive(true);
                    var pCounting = GameManager.Instance.Database.timers.Find(x => x.key == "GiftOnline");
                    if(pCounting== null)
                    {
                        pCounting = new TimeCountDown()
                        {
                            key = "GiftOnline",
                            length = item.time - GameManager.Instance.giftOnlineModule.onlineTime,
                            lastimeWheelFree = System.DateTime.Now
                        };
                        GameManager.Instance.addTimer(pCounting);
                    }
                    if (!pCounting.LabelTimer.Contains(labelTimer))
                    {
                        pCounting.LabelTimer.Add(labelTimer);
                    }
                }
            }
            else 
            {
                dot.gameObject.SetActive(false);
                labelTimer.gameObject.SetActive(false);
                boxTimer.gameObject.SetActive(false);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
