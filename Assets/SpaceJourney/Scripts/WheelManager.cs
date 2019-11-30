using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using System;

namespace EazyEngine.Space.UI
{
    public class WheelManager : Singleton<WheelManager>,EzEventListener<GameDatabasePropertyChanged<int>>,EzEventListener<EventTimer>
    {
        public SpinWheel parallax;
       // public ItemWheel[] items;
        public BaseItemGame itemExchange;
        public BaseItemGame itemWatchExchange;
        public UIButton btnRoll;
        public UIButton btnFree;
        public UILabel level;
        public UILabel exp;
        public UI2DSprite expprocess;
        public UILabel limit;
        public UILabel countTicket;
        public UILabel countGoldRequire;
        public BaseItemGame itemExchangeTicket;
        public int priceTicket;
        public int quantityTicketCanGet;
        public UI2DSprite iconprice;
        public UILabel labelPrice;
        public UILabel labelTicketGet;
        public GameObject layerTicket;
        public GameObject layerGold;
        public GameObject effectBuyTicket;
        public List<BaseItemGameInstancedArray> cacheInfos = new List<BaseItemGameInstancedArray>();

        public float delayResult = 1;

       // protected BaseItemGameInstancedArray data;
        // Start is called before the first frame update
        void Start()
        {
            if (!cacheTextFreeCountDown)
            {
                cacheTextFreeCountDown = btnFree.transform.Find("time").GetComponent<UILabel>();
            }
            labelPrice.text = priceTicket.ToString();
            labelTicketGet.text = quantityTicketCanGet.ToString();
            iconprice.sprite2D = itemExchangeTicket.CateGoryIcon;
            List<BaseItemGameInstancedArray> pInfos = new List<BaseItemGameInstancedArray>();
            for (int i = 0; i < GameDatabase.Instance.ItemWheelConfig.Length; ++i)
            {
                GameDatabase.Instance.ItemWheelConfig[i].CurrentLevel = GameManager.Instance.Database.CurrentLevelWheel;
                pInfos.Add( new BaseItemGameInstancedArray() { infos = new ItemWheelInfoConfig[] { GameDatabase.Instance.ItemWheelConfig[i] } });
            }
            cacheInfos = pInfos;
            parallax.DataSource = pInfos.ToObservableList();
        }

        private void OnEnable()
        {

            updateTimeLeft();
            updateWheelChance();
            EzEventManager.AddListener< GameDatabasePropertyChanged<int>>(this);
            EzEventManager.AddListener<EventTimer>(this);
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener< GameDatabasePropertyChanged<int>>(this);
            EzEventManager.RemoveListener<EventTimer>(this);
        }

        // Update is called once per frame
        void Update()
        {
            //if(timeCountDown.TotalMilliseconds > 0)
            //{
            //    timeCountDown -= TimeSpan.FromSeconds(Time.deltaTime);
    
            //    cacheTextFreeCountDown.text = timeCountDown.ToString(@"hh\:mm\:ss");
            //}
        }

        public void askForWheel()
        {
            var pItemCoin = GameManager.Instance.Database.getComonItem("Coin");
            var pItemExchange = GameManager.Instance.Database.getComonItem(itemExchange);
            bool isAble = false;
            if(pItemExchange.Quantity > 0)
            {
                pItemExchange.Quantity--;
                isAble = true;
            }
            else if (GameManager.Instance.Database.CurrentWheelToday < 10 && pItemCoin.Quantity >= GameDatabase.Instance.ConfigWheelGold[GameManager.Instance.Database.CurrentWheelToday])
            {
                pItemCoin.Quantity-= GameDatabase.Instance.ConfigWheelGold[GameManager.Instance.Database.CurrentWheelToday];
                GameManager.Instance.Database.CurrentWheelToday++;
                isAble = true;
            }
            if (isAble)
            {
                startRoll();
                GameManager.Instance.Database.CurrentExpWheel++;
                GameManager.Instance.SaveGame();
            }
            else
            {

            }
            limit.text = GameManager.Instance.Database.CurrentWheelToday.ToString() + "/"+ "10";
            exp.text = GameManager.Instance.Database.CurrentExpWheel.ToString() + "/" + GameDatabase.Instance.WheelLevelExp[GameManager.Instance.Database.CurrentLevelWheel + 1].ToString();
            expprocess.fillAmount = (float)GameManager.Instance.Database.CurrentExpWheel / (float)GameDatabase.Instance.WheelLevelExp[GameManager.Instance.Database.CurrentLevelWheel + 1];
            level.text = (GameManager.Instance.Database.CurrentLevelWheel+1).ToString();
        }

        public void onStopRoll()
        {
           StartCoroutine( delayAction(0.3f, delegate
            {
                isRolling = false;
                updateWheelChance();
                updateTimeLeft();
                TopLayer.Instance.boxReward.show();
                var pStorage = GameManager.Instance.Database.getComonItem(cacheInfos[cacheResult].infos[0].item.ItemID);
                pStorage.Quantity += cacheInfos[cacheResult].infos[0].Quantity;
                if (typeof(IExtractItem).IsAssignableFrom(cacheInfos[cacheResult].infos[0].item.GetType()))
                {
                    ((IExtractItem)cacheInfos[cacheResult].infos[0].item).disableExtracItem();
                }
                EzEventManager.TriggerEvent(new RewardEvent() { item = new BaseItemGameInstanced() { item = cacheInfos[cacheResult].infos[0].item, quantity = cacheInfos[cacheResult].infos[0].Quantity } });
            }));
     
        }

        public IEnumerator delayAction(float pSec , System.Action onAction)
        {
            yield return new WaitForSeconds(pSec);
            onAction?.Invoke();
        }
        bool isRolling = false;
        public void startRoll()
        {
            GameManager.Instance.Database.collectionDailyInfo.spinTime++;
            GameManager.Instance.Database.collectionInfo.spinTime++;
            EzEventManager.TriggerEvent(new MessageGamePlayEvent("MissionDirty"));
            updateWheelChance();
            btnRoll.isEnabled = false;
            btnFree.isEnabled = false;
            random();
            parallax.itemNumber = cacheResult;
            parallax.startRoll();
            isRolling = true;

        }
        public void watch()
        {
            if(itemWatchExchange.categoryItem == CategoryItem.WATCH)
            {
                GameManager.Instance.showRewardAds(itemWatchExchange.ItemID, delegate (bool pBool)
                {
                    if (pBool)
                    {
                        startRoll();
                    }
                });
            }
        }

        public void startRollFree()
        {
           var pTime  = GameManager.Instance.addTimer(new TimeCountDown()
            {
                key = "WheelFree",
                lastimeWheelFree = System.DateTime.Now,
                length = GameDatabase.Instance.timeWheelFree,
            });
     
            GameManager.Instance.SaveGame();
            startRoll();
            updateTimeLeft();
        }
        protected UILabel cacheTextFreeCountDown;
        public void updateTimeLeft()
        {
            var pTimer = GameManager.Instance.Database.timers.Find(x => x.key == "WheelFree");
            //if(pTimer != null)
            //{
                var pTime = (pTimer!= null? pTimer.lastimeWheelFree.AddSeconds(pTimer.length) : System.DateTime.Now) - System.DateTime.Now;
                btnFree.isEnabled = !(pTime.Seconds > 0);
                btnFree.transform.Find("Label").gameObject.SetActive(!(pTime.Seconds > 0));
                btnFree.transform.Find("time").gameObject.SetActive((pTime.Seconds > 0));
                if (pTime.TotalSeconds > 0)
                {
                    btnFree.transform.Find("time").GetComponent<UILabel>().text = pTime.ToString(@"hh\:mm\:ss");
                }
                if (pTimer != null && !pTimer.LabelTimer.Contains(cacheTextFreeCountDown))
                {
                    pTimer.LabelTimer.Add(cacheTextFreeCountDown);
                }
            //}
          
        }



        public void updateWheelChance()
        {
            var pItemCoin = GameManager.Instance.Database.getComonItem("Coin");
            var pItemExchange = GameManager.Instance.Database.getComonItem(itemExchange);
            limit.text = GameManager.Instance.Database.CurrentWheelToday.ToString() + "/" + "10";
            exp.text = GameManager.Instance.Database.CurrentExpWheel.ToString() + "/" + GameDatabase.Instance.WheelLevelExp[GameManager.Instance.Database.CurrentLevelWheel + 1].ToString();
            expprocess.fillAmount = (float)GameManager.Instance.Database.CurrentExpWheel / (float)GameDatabase.Instance.WheelLevelExp[GameManager.Instance.Database.CurrentLevelWheel + 1];
            level.text = (GameManager.Instance.Database.CurrentLevelWheel+1).ToString();
           // btnRoll.isEnabled = pItemExchange.Quantity > 0 ||(GameManager.Instance.Database.CurrentWheelToday < 10  && pItemCoin.Quantity >=  GameDatabase.Instance.ConfigWheelGold[GameManager.Instance.Database.CurrentWheelToday]);
            layerTicket.gameObject.SetActive(false);
            layerGold.gameObject.SetActive(false);
            if (pItemExchange.Quantity > 0)
            {
                btnRoll.isEnabled = true;
                layerTicket.gameObject.SetActive(true);
                countTicket.text = pItemExchange.Quantity.ToString();
            }
            else if (GameManager.Instance.Database.CurrentWheelToday < 10 && pItemCoin.Quantity >= GameDatabase.Instance.ConfigWheelGold[GameManager.Instance.Database.CurrentWheelToday])
            {
                btnRoll.isEnabled = true;
                layerGold.gameObject.SetActive(true);
                countGoldRequire.text = GameDatabase.Instance.ConfigWheelGold[GameManager.Instance.Database.CurrentWheelToday].ToString();
            }
        }

        public void callBackBuyTicket(bool isSucess)
        {
            if (isSucess)
            {
                if (effectBuyTicket)
                {
                    effectBuyTicket.gameObject.SetActive(isSucess);
                }
                var pItemExchange = GameManager.Instance.Database.getComonItem(itemExchange);
                pItemExchange.Quantity += 10;
                GameManager.Instance.SaveGame();
                if (!isRolling)
                {
                    updateWheelChance();
                }
            }
        }

        public void buyTicket()
        {
            var pMoney = GameManager.Instance.Database.getComonItem(itemExchangeTicket.ItemID);
            if(pMoney.Quantity >= priceTicket)
            {
                pMoney.Quantity -= priceTicket;
                callBackBuyTicket(true);
            }
            else
            {
                HUDLayer.Instance.showDialogNotEnoughMoney(pMoney.item.displayNameItem.Value, delegate
                {
                    ShopManager.Instance.showBoxShop(pMoney.item.categoryItem.ToString());
                    HUDLayer.Instance.BoxDialog.close();
                });
            }
           
        }

        public void random()
        {
            float pCurrentPercent = 1;
            float pRandom = UnityEngine.Random.Range(0, 1.0f);
            for(int i = cacheInfos.Count-1; i >= 0; --i)
            {
                pCurrentPercent -= cacheInfos[i].infos[0].percent / 100.0f;
                if (pRandom > pCurrentPercent)
                {
                    cacheResult = i;
                    return;
                }
            }
        }
        int cacheResult = 0;

        public void OnEzEvent(GameDatabasePropertyChanged<int> eventType)
        {
            if(eventType.nameProperty == "CurrentLevelWheel")
            {
                List<BaseItemGameInstancedArray> pInfos = new List<BaseItemGameInstancedArray>();
                for (int i = 0; i < GameDatabase.Instance.ItemWheelConfig.Length; ++i)
                {
                    GameDatabase.Instance.ItemWheelConfig[i].CurrentLevel = GameManager.Instance.Database.CurrentLevelWheel;
                    pInfos.Add( new BaseItemGameInstancedArray() { infos = new ItemWheelInfoConfig[] { GameDatabase.Instance.ItemWheelConfig[i] } });
                }
                parallax.DataSource = pInfos.ToObservableList();
                cacheInfos = pInfos;
            }
        }

        public void OnEzEvent(EventTimer eventType)
        {
           if(eventType.key == "WheelFree")
            {
                if(eventType.state == TimerState.Complete)
                {
                    updateTimeLeft();
                }
            }
        }
    }
}
