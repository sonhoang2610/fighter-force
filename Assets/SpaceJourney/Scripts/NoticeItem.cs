using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.Events;

namespace EazyEngine.Space.UI
{
    public class NoticeItem : MonoBehaviour, EzEventListener<GameDatabaseInventoryEvent>
    {
        public string[] itemIDs;
        public int minQuantity = 1;
        public UnityEvent onNotice;
        public UnityEvent onNoneNotice;

        public void OnEzEvent(GameDatabaseInventoryEvent eventType)
        {
            bool notice = false;
            bool handle = false;
            foreach(var pItem in itemIDs)
            {
                if (eventType.item != null && eventType.item.item != null && eventType.item.item.itemID == pItem)
                {
                    handle = true;
                    if (eventType.item.quantity >= minQuantity)
                    {
                        notice = true;
                        onNotice?.Invoke();
                    }
                    else
                    {
                        onNoneNotice?.Invoke();
                    }
                }
            }
            if (!notice && handle)
            {
                onNoneNotice?.Invoke();
            }
          
        }

        private void OnEnable()
        {
            EzEventManager.AddListener(this);
            bool notice = false;
            foreach (var pItem in itemIDs)
            {
                var pItemExist = GameManager.Instance.Database.getComonItem(pItem);
                if (pItemExist.quantity >= minQuantity)
                {
                    notice = true;
                    onNotice?.Invoke();
                }
            }
            if (!notice)
            {
                onNoneNotice?.Invoke();
            }
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
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

