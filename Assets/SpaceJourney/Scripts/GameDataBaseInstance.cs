using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using EazyEngine.Space.UI;

namespace EazyEngine.Space
{

    //public static class SerializeData
    //{
    //    public static T Serialize<T>(T )
    //}

    public enum BehaviorDatabase { NEWITEM,CHANGE_QUANTITY_ITEM};
    public struct GameDatabaseInventoryEvent
    {
        public BaseItemGameInstanced item;
        public BehaviorDatabase behavior;

        public GameDatabaseInventoryEvent(BehaviorDatabase pBehavior, BaseItemGameInstanced pItem)
        {
            behavior = pBehavior;
            item = pItem;
        }

    }
    public struct GameDatabasePropertyChanged<T>
    {
        public string nameProperty;
        public T value;
        public T beforValue;
        public GameDatabasePropertyChanged(string pProperty, T pValue,T pBeforeValue)
        {
            nameProperty = pProperty;
            value = pValue;
            beforValue = pBeforeValue;
        }

    }
    [System.Serializable]
    public class TimeCountDown
    {
        public string key;
        public System.DateTime lastimeWheelFree;
        public double length;
        [System.NonSerialized]
        protected List<UILabel> labeltimers = new List<UILabel>();
        public List<UILabel> LabelTimer
        {
            get {
                if(labeltimers == null)
                {
                    labeltimers = new List<UILabel>();
                }
                return labeltimers;
            }
            set {
                labeltimers = value;
            }
        }
    }

    public static class GameDataBaseSupport
    {
        public static TimeCountDown getTimer(this List<TimeCountDown> pTimers, string pKey)
        {
            for(int i = 0; i < pTimers.Count; ++i)
            {
                if(pTimers[i].key == pKey)
                {
                    return pTimers[i];
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class GameDataBaseInstance
    {
        public void ExtraInfo()
        {
            foreach(var pPlane in planes)
            {
                pPlane.ExtraInfo();
            }
            foreach (var pPlane in spPlanes)
            {
                pPlane.ExtraInfo();
            }
            reupdateTimerCount();
        }

        public void reupdateTimerCount()
        {
            for(int i = timers.Count-1; i >=0; i--)
            {
                var pTime = timers[i];
                string[] splitStr = pTime.key.Split('/');
                if (pTime.key.StartsWith("MainInventory"))
                {
                    if (splitStr.Length > 1)
                    {
                        var pItem = getComonItem(splitStr[1]);
                        if (pItem != null)
                        {
                           pTime.length = pItem.item.limitModule.timeToRestore;
                            var pTimeChange = System.DateTime.Now - pTime.lastimeWheelFree;
                            if (pTimeChange.TotalSeconds > 0)
                            {
                                int pRestoreTime = (int)pTimeChange.TotalSeconds / (int)pItem.item.limitModule.timeToRestore;
                                if (pRestoreTime > 0)
                                {
                                    int pQuantityRestore = pRestoreTime * pItem.item.limitModule.quantityRestore + pItem.quantity;
                                    if (pQuantityRestore >= pItem.item.limitModule.limitInInventory)
                                    {
                                        pQuantityRestore = pItem.item.limitModule.limitInInventory;
                                        GameManager.Instance.removeTimer(pTime);
                                        timers.Remove(pTime);
                                    }
                                    else
                                    {
                                        int pSodu = (int)pTimeChange.TotalSeconds % (int)pItem.item.limitModule.timeToRestore;
                                        pTime.lastimeWheelFree = System.DateTime.Now.AddSeconds(-pSodu);
                                   
                                    }
                                    pItem.Quantity = pQuantityRestore;
                                    GameManager.Instance.SaveGame();
                                }
                            }
                        }
                    }
                }
                else
                {
                    var pTimeChange = System.DateTime.Now - pTime.lastimeWheelFree;
                    if (pTimeChange.TotalSeconds > pTime.length)
                    {
                        GameManager.Instance.removeTimer(timers[i]);
                        timers.RemoveAt(i);                   
                    }
                    //else
                    //{
                    //    pTime.length -= pTimeChange.TotalSeconds;
                    //}

                }
            }
        }

        public bool checkTimerCountdownResotreModule(BaseItemGame pItem)
        {
            foreach(var pTime in timers)
            {
                if(pTime.key.Contains(pItem.ItemID) && pTime.key.StartsWith("MainInventory"))
                {
                    return true;
                }
            }
            return false;
        }
        public TimeCountDown getTimerCountdownId(string pID)
        {
            foreach (var pTime in timers)
            {
                if (pTime.key == pID)
                {
                    return pTime;
                }
            }
            return null;
        }
        public TimeCountDown getTimerCountdownRestoreModule(BaseItemGame pItem)
        {
            foreach (var pTime in timers)
            {
                if (pTime.key.Contains(pItem.ItemID) && pTime.key.StartsWith("MainInventory"))
                {
                    return pTime;
                }
            }
            return null;
        }
        public List<PlaneInfoConfig> planes = new List<PlaneInfoConfig>();
        public List<SupportPlaneInfoConfig> spPlanes = new List<SupportPlaneInfoConfig>();
        public string selectedMainPlane = "";
        public string selectedSupportPlane1 = "";
        public string selectedSupportPlane2 = "";
        public Pos lastPlayStage = Pos.Zero;
        [HideInInspector]
        public int currentLevelWheel = 0;
        [HideInInspector]
        public int currentExpWheel = 0;
        [HideInInspector]
        public System.DateTime lastimeWheelFree;
        [HideInInspector]
        public System.DateTime lastOnline;
        [HideInInspector]
        public System.DateTime lastGoldWheel;
        [HideInInspector]
        public System.DateTime firstOnline;
        [HideInInspector]
        public int currentWheelToday = 0;
     //   [HideInInspector]
        [SerializeField]
        public List<TimeCountDown> timers = new List<TimeCountDown>();

        public List<BaseItemGameInstanced> items = new List<BaseItemGameInstanced>();
        public List<GiftOnlineModule> giftOnlineModules = new List<GiftOnlineModule>();
        public List<DailyGiftModule> dailyGiftModules = new List<DailyGiftModule>();

        public GiftOnlineModule getModuleGiftOnline(string pId)
        {
            if(giftOnlineModules == null) { giftOnlineModules = new List<GiftOnlineModule>(); }
            for(int i = 0; i < giftOnlineModules.Count; ++i)
            {
                if(giftOnlineModules[i].id == pId)
                {
                    if(giftOnlineModules[i].DateInYear != System.DateTime.Now.DayOfYear)
                    {
                        giftOnlineModules[i].calimedIndex = -1;
                        giftOnlineModules[i].onlineTime = 0;
                        giftOnlineModules[i].DateInYear = System.DateTime.Now.DayOfYear;
                    }
                    return giftOnlineModules[i];
                }
            }
            return null;
        }

        public void clearModules(string[] pIDs)
        {
            for(int i = 0; i < pIDs.Length; ++i)
            {
                for(int j = giftOnlineModules.Count-1; j >= 0; --j)
                {
                    if(giftOnlineModules[j].id == pIDs[i])
                    {
                         giftOnlineModules.RemoveAt(j);
                    }
                }
            }
        }
        public DailyGiftModule getModuleGiftDaily(string pId)
        {
            if (dailyGiftModules == null) { dailyGiftModules = new List<DailyGiftModule>(); }
            for (int i = 0; i < dailyGiftModules.Count; ++i)
            {
                if (dailyGiftModules[i].id == pId)
                {
                    return dailyGiftModules[i];
                }
            }
            return null;
        }

        public void clearDailyModules(string[] pIDs)
        {
            for (int i = 0; i < pIDs.Length; ++i)
            {
                for (int j = dailyGiftModules.Count - 1; j >= 0; --j)
                {
                    if (dailyGiftModules[j].id == pIDs[i])
                    {
                        dailyGiftModules.RemoveAt(j);
                    }
                }
            }
        }
        public BaseItemGameInstanced addItem(BaseItemGameInstanced pItem,bool ping = true)
        {
            items.Add(pItem);
            if (ping)
            {
                EzEventManager.TriggerEvent(new GameDatabaseInventoryEvent(BehaviorDatabase.NEWITEM, pItem));
            }
            return pItem;
        }
        public int CurrentWheelToday {
            get {
                if(lastGoldWheel.Date != System.DateTime.Now.Date)
                {
                    lastGoldWheel = System.DateTime.Now;
                    currentWheelToday = 0;
                }
                return currentWheelToday;
            }
            set {
                currentWheelToday = value;
            }
        }

        public int CurrentExpWheel { get => currentExpWheel;
            set {
                if(value > currentExpWheel && value >= GameDatabase.Instance.WheelLevelExp[CurrentLevelWheel + 1])
                {
                    CurrentLevelWheel++;
                    value -= GameDatabase.Instance.WheelLevelExp[CurrentLevelWheel];
                    EzEventManager.TriggerEvent(new GameDatabasePropertyChanged<int>("CurrentLevelWheel", CurrentLevelWheel, CurrentLevelWheel-1));
                }
                currentExpWheel = value;
            }
        }

        public int CurrentLevelWheel { get => currentLevelWheel; set => currentLevelWheel = value; }
        public string CacheSelectedMainPlane { get; set; }
        public string SelectedMainPlane { get => selectedMainPlane; set {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                if(value != selectedMainPlane)
                {
                    CacheSelectedMainPlane = value;
                }
                selectedMainPlane = value;
            }
        }
        public string CacheSelectedSpPlane { get; set; }
        public string SelectedSupportPlane1 { get => selectedSupportPlane1; set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                if (value != selectedSupportPlane1)
                {
                    CacheSelectedSpPlane = value;
                }
                selectedSupportPlane1 = value;
            }
        }
        public string SelectedSupportPlane2 { get => selectedSupportPlane2; set
            {
                if (value != selectedSupportPlane2)
                {
                    CacheSelectedSpPlane = value;
                }
                selectedSupportPlane2 = value;
            }
        }
        public PlaneInfoConfig getPlane(string pID)
        {
            for(int i = 0; i < planes.Count; ++i)
            {
                if(planes[i].Info.ItemID == pID)
                {
                    return planes[i];
                }
            }
            return null;
        }

        public BaseItemGameInstanced getComonItem(string pID)
        {
            for(int i = items.Count-1; i >= 0; --i)
            {
                if (items[i].item == null) {
                    items.RemoveAt(i);
                    continue;
                }
                if(items[i].item.ItemID == pID)
                {
                    return items[i];
                }
            }
            var pItem = new BaseItemGameInstanced() { item= GameDatabase.Instance.getItem(pID) ,quantity = 0};
            addItem(pItem, false);
            return pItem;
        }

        public BaseItemGameInstanced getComonItem(BaseItemGame pItem)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i].item == pItem)
                {
                    return items[i];
                }
            }
            var pNew = new BaseItemGameInstanced() { item = pItem, quantity = 0 };
            addItem(pNew,false);
            return pNew;
        }
    }
    [System.Serializable]
    public class BaseItemGameInstanced
    {
        public BaseItemGame item;
        public int quantity = 1;
        [System.NonSerialized]
        public bool isRequire = false;


        
        protected bool isFree = true;
        [System.NonSerialized]
        public int changeQuantity;
        private int beforeQuantity = 0;
        public bool IsFree
        {
            get
            {
                return isFree;
            }
            set
            {
                isFree = value;
            }
        }

        public virtual int Quantity { get => quantity;
            set {
                if (item.limitModule != null)
                {
                    if (value > item.limitModule.limitInInventory)
                    {
                        value = item.limitModule.limitInInventory;
                    }
                }
                changeQuantity = value - quantity;      
                quantity = value;
                
                if (changeQuantity != 0)
                {
                    EzEventManager.TriggerEvent(new GameDatabaseInventoryEvent(BehaviorDatabase.CHANGE_QUANTITY_ITEM, this));
                }
            }
        }

        protected int BeforeQuantity { get => beforeQuantity; set => beforeQuantity = value; }
    }
}
