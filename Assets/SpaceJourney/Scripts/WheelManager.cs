using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using System;

namespace EazyEngine.Space.UI
{
    public class WheelManager : Singleton<WheelManager>,EzEventListener<GameDatabasePropertyChanged<int>>
    {
        public EazyParallax parallax;
        public ItemWheel[] items;
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

        public float delayResult = 1;

        protected BaseItemGameInstancedArray data;
        // Start is called before the first frame update
        void Start()
        {
            labelPrice.text = priceTicket.ToString();
            labelTicketGet.text = quantityTicketCanGet.ToString();
            iconprice.sprite2D = itemExchangeTicket.CateGoryIcon;
            List<BaseItemGameInstanced> pInfos = new List<BaseItemGameInstanced>();
            for(int i = 0; i< GameDatabase.Instance.ItemWheelConfig.Length; ++i)
            {
                GameDatabase.Instance.ItemWheelConfig[i].CurrentLevel = GameManager.Instance.Database.CurrentLevelWheel;
                items[i].Data = new BaseItemGameInstancedArray() { infos =  new ItemWheelInfoConfig[] { GameDatabase.Instance.ItemWheelConfig[i] } };
            }
            data = new BaseItemGameInstancedArray() { infos = GameDatabase.Instance.ItemWheelConfig };
            for(int i = 0; i < parallax.Elements.Length; ++i)
            {
                parallax.Elements[i].GetComponent<ItemWheel>().Data = data;
            }
            
        }

        private void OnEnable()
        {

            updateTimeLeft();
            updateWheelChance();
            EzEventManager.AddListener(this);
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }

        // Update is called once per frame
        void Update()
        {
            if(timeCountDown.TotalMilliseconds > 0)
            {
                timeCountDown -= TimeSpan.FromSeconds(Time.deltaTime);
                if (!cacheTextFreeCountDown)
                {
                    cacheTextFreeCountDown = btnFree.transform.Find("time").GetComponent<UILabel>();
                }
                cacheTextFreeCountDown.text = timeCountDown.ToString(@"hh\:mm\:ss");
            }
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
            isRolling = false;
            updateWheelChance();
            updateTimeLeft();
            TopLayer.Instance.boxReward.show();
            var pStorage = GameManager.Instance.Database.getComonItem(data.infos[cacheResult].item.ItemID);
            pStorage.Quantity+= data.infos[cacheResult].Quantity;
            if (typeof(IExtractItem).IsAssignableFrom(data.infos[cacheResult].item.GetType()))
            {
                ((IExtractItem)data.infos[cacheResult].item).disableExtracItem();
            }
            EzEventManager.TriggerEvent(new RewardEvent() { item = new BaseItemGameInstanced() { item = data.infos[cacheResult].item,quantity = data.infos[cacheResult].Quantity} });
        }
        bool isRolling = false;
        public void startRoll()
        {
            updateWheelChance();
            btnRoll.isEnabled = false;
            btnFree.isEnabled = false;
            parallax.startRoll();
            isRolling = true;
            StartCoroutine(result());

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
            GameManager.Instance.Database.lastimeWheelFree = System.DateTime.Now;
            GameManager.Instance.SaveGame();
            startRoll();
            updateTimeLeft();
        }
        protected UILabel cacheTextFreeCountDown;
        protected TimeSpan timeCountDown;
        public void updateTimeLeft()
        {
            var pTime =  GameManager.Instance.Database.lastimeWheelFree.AddHours(8) - System.DateTime.Now;
            btnFree.isEnabled =!( pTime.Seconds > 0);
            btnFree.transform.Find("Label").gameObject.SetActive(!(pTime.Seconds > 0));
            btnFree.transform.Find("time").gameObject.SetActive((pTime.Seconds > 0));
            if ( pTime.TotalSeconds > 0)
            {
                btnFree.transform.Find("time").GetComponent<UILabel>().text = pTime.ToString(@"hh\:mm\:ss");
            }
            timeCountDown = pTime;
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
            for(int i = data.infos.Length-1; i >= 0; --i)
            {
                pCurrentPercent -= data.infos[i].percent / 100.0f;
                if (pRandom > pCurrentPercent)
                {
                    cacheResult = i;
                    return;
                }
            }
        }
        int cacheResult = 0;
        public IEnumerator result()
        {
            yield return new WaitForSeconds(delayResult);
            parallax.isForever = false;
            random();
            parallax.Elements[3].GetComponent<ItemWheel>().fixItem(cacheResult);
        }

        public void OnEzEvent(GameDatabasePropertyChanged<int> eventType)
        {
            if(eventType.nameProperty == "CurrentLevelWheel")
            {
                for (int i = 0; i < GameDatabase.Instance.ItemWheelConfig.Length; ++i)
                {
                    GameDatabase.Instance.ItemWheelConfig[i].CurrentLevel = GameManager.Instance.Database.CurrentLevelWheel;
                    items[i].Data = new BaseItemGameInstancedArray() { infos = new ItemWheelInfoConfig[] { GameDatabase.Instance.ItemWheelConfig[i] } };
                }
                for (int i = 0; i < parallax.Elements.Length; ++i)
                {
                    parallax.Elements[i].GetComponent<ItemWheel>().Data = data;
                }

            }
        }
    }
}
