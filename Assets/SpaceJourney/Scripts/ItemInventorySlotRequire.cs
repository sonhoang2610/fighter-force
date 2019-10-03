using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public class ItemRequireInfo : BaseItemGameInstanced, IConvertBaseItemGameInstanced<ItemRequireInfo>
    {
        public int quantityRequire;

        public ItemRequireInfo convertFromBaseItemGameInstanced(BaseItemGameInstanced pObject)
        {
            if (pObject.item.limitModule != null)
            {
                GameManager.Instance.Database.reupdateTimerCount();
            }
            return new ItemRequireInfo() { quantity = pObject.quantity, item = pObject.item, quantityRequire = pObject.item.limitModule != null ? pObject.item.limitModule.limitInInventory : 0 };
        }
        //public static implicit operator ItemRequireInfo(BaseItemGameInstanced obj2)
        //{
        //    return new ItemRequireInfo() { quantity = obj2.quantity, item = obj2.item, quantityRequire = obj2.item.limitInInventory };
        //}
    }
    [System.Serializable]
    public class ItemRequireInfoUnityEvent : UnityEngine.Events.UnityEvent<ItemRequireInfo>
    {

    }
    public class ItemInventorySlotRequire : ItemInventorySlotGeneric<ItemRequireInfo>
    {
        public ItemRequireInfoUnityEvent onFillData;
        public UILabel labelTimer;
        protected bool isAddTimer = false;
        bool addLabel;
        public override void reloadData()
        {
            Data = new ItemRequireInfo().convertFromBaseItemGameInstanced(GameManager.Instance.Database.getComonItem(Data.item));
        }
        protected override void Start()
        {
            base.Start();
            isAddTimer = false;
            if (labelTimer)
                labelTimer.gameObject.SetActive(false);
            if (Data != null && Data.item != null && Data.item.limitModule != null)
            {
                var pTime = GameManager.Instance.Database.getTimerCountdownRestoreModule(Data.item);
                if (pTime != null)
                {
                    if (labelTimer && !pTime.LabelTimer.Contains(labelTimer))
                    {
                        labelTimer.gameObject.SetActive(true);
                        pTime.LabelTimer.Add(labelTimer);
                        addLabel = true;
                    }
                    isAddTimer = GameManager.Instance.addTimer(pTime);
                }
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (isInit)
            {
             
                reloadData();
                isAddTimer = false;
                if (labelTimer)
                     labelTimer.gameObject.SetActive(false);
                if (Data != null && Data.item != null && Data.item.limitModule != null)
                {
                    GameManager.Instance.Database.reupdateTimerCount();
                    var pTime = GameManager.Instance.Database.getTimerCountdownRestoreModule(Data.item);
               
                    if (pTime != null)
                    {
                        if (labelTimer && !pTime.LabelTimer.Contains(labelTimer))
                        {
                            labelTimer.gameObject.SetActive(true);
                            pTime.LabelTimer.Add(labelTimer);
                            addLabel = true;
                        }
                        isAddTimer = GameManager.Instance.addTimer(pTime);
                    }
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (isAddTimer)
            {
                var pTime = GameManager.Instance.Database.getTimerCountdownRestoreModule(Data.item);
                if (pTime != null)
                {                  
                    GameManager.Instance.removeTimer(pTime);
                }
            }
            if (labelTimer)
            {
                var pTime = GameManager.Instance.Database.getTimerCountdownRestoreModule(Data.item);
                if (pTime != null)
                {
                    pTime.LabelTimer.Remove(labelTimer);
                }
            }
        }
        public override ItemRequireInfo Data { get => base.Data; set {
                base.Data = value;
                if (onFillData != null)
                {
                    onFillData.Invoke(value);
                }
                quantity.text = value.quantity.ToString() +( value.quantityRequire != 0 ?( "/ " + value.quantityRequire) : "");
            } }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
