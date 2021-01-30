using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.Networking;

namespace EazyEngine.Space.UI
{
    using System;
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
        public int lastDateAds;
    }
    public class BoxDailyGift : BaseBox<ItemDailyGift, ItemDailyGiftInfo>
    {
        public UIButton btn1, btn2;
        public BaseItemGame watchItem;
        protected DailyGiftDataBase databse;
        private DateTime time;
        private bool isGetTime;
        public AudioClip sfxClaim;
        private bool firstTime = false;

        public bool FirstTime { get => firstTime; set => firstTime = value; }
        public DateTime Time { get => time; set => time = value; }
        public bool IsGetTime { get => isGetTime; set => isGetTime = value; }

        private void OnEnable()
        {
            if (databse == null)
            {
                databse = GameDatabase.Instance.databaseDailyGift;
            }
            if (!FirstTime)
            {
                TopLayer.Instance.LoadingAds.gameObject.SetActive(true);
                StartCoroutine(checkModule());
            }
            else
            {
                reload();
            }
            FirstTime = false;
        }

        IEnumerator checkModule()
        {
            if (!TopLayer.Instance.LoadingAds.gameObject.activeSelf)
            {
                GetComponent<UIElement>().close();
                yield return null;
            }
            yield return checkInternetConnection(delegate (bool pResult)
            {
                if (pResult)
                {
                    Time = TimeExtension.GetNetTime(ref isGetTime);
                }
                
            });
            if (!IsGetTime)
            {
                yield return new  WaitForSeconds(1);
                yield return checkModule();
            }
            else
            {
                reload();
                TopLayer.Instance.LoadingAds.gameObject.SetActive(false);
            }
        }
        IEnumerator checkInternetConnection(Action<bool> action)
        {
            UnityWebRequest www = new UnityWebRequest("http://google.com");
            yield return www;
            action(www.error == null);
            www.Dispose();
        }
        public void reload()
        {
            btn1.isEnabled = !(GameManager.Instance.dailyGiftModule.lastDate == Time.DayOfYear);
            btn2.isEnabled = !(GameManager.Instance.dailyGiftModule.lastDateAds == Time.DayOfYear);
            for (int i = 0; i < databse.item.Count; ++i)
            {
                int status = 0;

                databse.item[i].isNext = false;
                if (i <= GameManager.Instance.dailyGiftModule.currentDay)
                {
                    status = 1;

                }
                else if (i == GameManager.Instance.dailyGiftModule.currentDay + 1 && GameManager.Instance.dailyGiftModule.lastDate != Time.DayOfYear && GameManager.Instance.dailyGiftModule.lastDateAds != Time.DayOfYear)
                {
                    databse.item[i].isNext = true;
                }
                databse.item[i].status = status;
            }
            DataSource = databse.item.ToObservableList();
        }

        public void claim()
        {
      
            if (GameManager.Instance.dailyGiftModule.lastDate != Time.DayOfYear)
            {
                bool claimed = false;
                if (Time.DayOfYear == GameManager.Instance.dailyGiftModule.lastDateAds || Time.DayOfYear == GameManager.Instance.dailyGiftModule.lastDate)
                {
                    claimed = true;
                }
                var pData = DataSource[!claimed ? GameManager.Instance.dailyGiftModule.currentDay + 1 : GameManager.Instance.dailyGiftModule.currentDay];
                var pReward = GameManager.Instance.Database.getComonItem(pData.mainData.item.ItemID);
                SoundManager.Instance.PlaySound(sfxClaim, Vector3.zero);
                pReward.Quantity += pData.mainData.quantity;
                pData.status = 1;
                TopLayer.Instance.boxReward.show();
                if (typeof(IExtractItem).IsAssignableFrom(pData.mainData.item.GetType()))
                {
                    ((IExtractItem)pData.mainData.item).disableExtracItem();
                }
                EzEventManager.TriggerEvent(new RewardEvent() { item = pData.mainData });
                GameManager.Instance.dailyGiftModule.lastDate = Time.DayOfYear;
                if (!claimed)
                {
                    GameManager.Instance.dailyGiftModule.currentDay++;
                }
                GameManager.Instance.SaveGame();

                reload();
            }
        }
        public void claimX2()
        {
            GameManager.Instance.showRewardAds(watchItem.ItemID, resultClaimX2);
        }

        public void resultClaimX2(ResultStatusAds pSucess)
        {
            if (pSucess == ResultStatusAds.Success)
            {
                if (GameManager.Instance.dailyGiftModule.lastDateAds != Time.DayOfYear)
                {
                    bool claimed = false;
                    if (Time.DayOfYear == GameManager.Instance.dailyGiftModule.lastDateAds || Time.DayOfYear == GameManager.Instance.dailyGiftModule.lastDate)
                    {
                        claimed = true;
                    }
                     var pData = DataSource[!claimed ? GameManager.Instance.dailyGiftModule.currentDay + 1 : GameManager.Instance.dailyGiftModule.currentDay ];
                    var pReward = GameManager.Instance.Database.getComonItem(pData.mainData.item.ItemID);
                    SoundManager.Instance.PlaySound(sfxClaim, Vector3.zero);
                    pReward.Quantity += pData.mainData.quantity;
                    pData.status = 1;
                    TopLayer.Instance.boxReward.show();
                    if (typeof(IExtractItem).IsAssignableFrom(pData.mainData.item.GetType()))
                    {
                        ((IExtractItem)pData.mainData.item).disableExtracItem();
                    }
                    EzEventManager.TriggerEvent(new RewardEvent() { item = new BaseItemGameInstanced() { item = pData.mainData.item, quantity = pData.mainData.quantity  } });
                    GameManager.Instance.dailyGiftModule.lastDateAds = Time.DayOfYear;
                    if (!claimed)
                    {
                        GameManager.Instance.dailyGiftModule.currentDay++;
                    }
             
                    GameManager.Instance.SaveGame();
                    reload();
                }
            }
        }
    }
}
