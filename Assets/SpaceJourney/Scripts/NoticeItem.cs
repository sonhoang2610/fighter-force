using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.Events;

namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public class EventTime : UnityEvent<TimeCountDown>
    {

    }
    public class NoticeItem : MonoBehaviour, EzEventListener<GameDatabaseInventoryEvent>, EzEventListener<EventTimer>
    {
        public string[] itemIDs;
        public string[] timerKey;
        public UnityEvent onNotice;
        public UnityEvent onNoneNotice;
        public EventTime onTimerCounting;
        public UnityEvent onNonTimer;

        protected TimeCountDown timerPending;
        protected int timeCounting = 0;
        protected bool isNotice = false;

        private IEnumerator Awake()
        {
            yield return new WaitForSeconds(5);
        }
        public void OnEzEvent(GameDatabaseInventoryEvent eventType)
        {
            reload();
        }
        public void reload()
        {
        
            bool notice = false;
            foreach (var pItem in itemIDs)
            {
                var pItemExist = GameManager.Instance.Database.getComonItem(pItem);
                if (pItemExist.quantity >= 1)
                {
                    notice = true;
                    onNotice.Invoke();
                }
            }
            timerPending = null;
            isNotice = notice;
            if (!notice)
            {
                checkTimer();
            }
            if (timerPending == null)
            {
                onNonTimer.Invoke();
            }
            if(timeCounting < timerKey.Length)
            {
                isNotice = true;
            }
            if (isNotice)
            {
                onNotice.Invoke();
            }
            else
            {
                onNoneNotice.Invoke();
            }
        }
        private void OnEnable()
        {
            EzEventManager.AddListener<GameDatabaseInventoryEvent>(this);
            EzEventManager.AddListener<EventTimer>(this);
            Invoke(nameof(reload), 0.2f);
        }


        public void checkTimer()
        {
            double pMinTime = 99999;
            TimeCountDown pTimeCount = null;
            timeCounting = 0;
            foreach (var pKey in timerKey)
            {
                var pCount = GameManager.Instance.Database.timers.Find(x => x.key == pKey);
                if (pCount != null)
                {
                    var pTime = System.DateTime.Now - pCount.lastimeWheelFree;
                    if ((pCount.length- pTime.TotalSeconds) < pMinTime)
                    {
                        pMinTime = pCount.length - pTime.TotalSeconds;
                        pTimeCount = pCount;
                    }
             
                    timeCounting++;
                }
            }
            if (pTimeCount != null)
            {
                onTimerCounting.Invoke(pTimeCount);
            }

            timerPending = pTimeCount;
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener<GameDatabaseInventoryEvent>(this);
            EzEventManager.RemoveListener<EventTimer>(this);
        }
        
        public void OnEzEvent(EventTimer eventType)
        {
            if (eventType.state == TimerState.Start)
            {
                foreach (var time in timerKey)
                {
                    if (time == eventType.key)
                    {
                        reload();
                    }
                }
            }
            if (eventType.state == TimerState.Complete && timerPending != null && timerPending.key == eventType.key)
            {
                isNotice = true;
                timerPending = null;
                onNotice.Invoke();
            }
        }
    }
}

