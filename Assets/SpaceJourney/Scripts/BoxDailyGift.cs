using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space.UI
{
#if UNITY_EDITOR
    using UnityEditor;

    public class DailyGiftDataBaseCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/DailyGiftDataBase")]
        public static void CreateAsset()
        {
            CreatetorScriptTableObjectDTB.CreateMyAsset<DailyGiftDataBase>();
        }
    }
#endif
    [System.Serializable]
    public class DailyGiftModule
    {
        public string id;
        public int currentDay = -1;
        public int lastDate;
    }
    public class BoxDailyGift : BaseBox<ItemDailyGift,ItemDailyGiftInfo>
    {
        public UIButton btn1, btn2;
        public BaseItemGame watchItem;
        protected DailyGiftDataBase databse;
        private void OnEnable()
        {
            if (databse == null)
            {
                databse = GameDatabase.Instance.databaseDailyGift;
            }
            reload();
            //bool isDirty = false;
            //for (int i = 0; i < databse.item.Length; ++i)
            //{
            //    if (databse.item[i].time < GameManager.Instance.giftOnlineModule.onlineTime && i > GameManager.Instance.giftOnlineModule.calimedIndex)
            //    {
            //        currentEffect++;
            //        items[i].claim(delegate {
            //            currentEffect--;
            //            if (currentEffect <= 0)
            //            {
            //                reload();
            //            }
            //        });
            //        GameManager.Instance.giftOnlineModule.calimedIndex = i;
            //        isDirty = true;
            //    }
            //}
            //if (isDirty)
            //{
            //    GameManager.Instance.SaveGame();
            //}

        }

        public void reload()
        {
            if (GameManager.Instance.dailyGiftModule.lastDate == System.DateTime.Now.DayOfYear) {
                btn1.isEnabled = false;
                btn2.isEnabled = false;
            }
                for (int i = 0; i < databse.item.Count; ++i)
            {
                int status = 0;

                databse.item[i].isNext = false;
                if (i <= GameManager.Instance.dailyGiftModule.currentDay)
                {
                    status = 1;
     
                }
                else if (i == GameManager.Instance.dailyGiftModule.currentDay + 1 && GameManager.Instance.dailyGiftModule.lastDate != System.DateTime.Now.DayOfYear)
                {
                    databse.item[i].isNext = true;
                }
                databse.item[i].status = status;
            }
            DataSource = databse.item.ToObservableList();
        }

        public void claim()
        {
            btn1.isEnabled = false;
            btn2.isEnabled = false;
            if(GameManager.Instance.dailyGiftModule.lastDate != System.DateTime.Now.DayOfYear)
            {
                var pData = DataSource[GameManager.Instance.dailyGiftModule.currentDay+1];
                var pReward = GameManager.Instance.Database.getComonItem(pData.mainData.item.itemID);
                pReward.Quantity += pData.mainData.quantity;
                pData.status = 1;
                TopLayer.Instance.boxReward.show();
                EzEventManager.TriggerEvent(new RewardEvent() { item = pData.mainData });
                GameManager.Instance.dailyGiftModule.lastDate = System.DateTime.Now.DayOfYear;
                GameManager.Instance.dailyGiftModule.currentDay++;         
                GameManager.Instance.SaveGame();
      
                reload();
            }
        }
        public void claimX2()
        {
            GameManager.Instance.showRewardAds(watchItem.itemID, resultClaimX2);
        }

        public void resultClaimX2(bool pSucess)
        {
            if (pSucess)
            {
                btn1.isEnabled = false;
                btn2.isEnabled = false;
                if (GameManager.Instance.dailyGiftModule.lastDate != System.DateTime.Now.DayOfYear)
                {
                    var pData = DataSource[GameManager.Instance.dailyGiftModule.currentDay + 1];
                    var pReward = GameManager.Instance.Database.getComonItem(pData.mainData.item.itemID);
                    pReward.Quantity += pData.mainData.quantity * 2;
                    pData.status = 1;
                    TopLayer.Instance.boxReward.show();
                    EzEventManager.TriggerEvent(new RewardEvent() { item = new BaseItemGameInstanced() { item = pData.mainData.item, quantity = pData.mainData.quantity * 2 } });
                    GameManager.Instance.dailyGiftModule.lastDate = System.DateTime.Now.DayOfYear;
                    GameManager.Instance.dailyGiftModule.currentDay++;
                    GameManager.Instance.SaveGame();
                    reload();
                }
            }
        }
    }
}
