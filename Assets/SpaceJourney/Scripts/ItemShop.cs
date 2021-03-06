﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using System;
using EasyMobile;

namespace EazyEngine.Space.UI
{
    public class ItemShop : BaseItem<ShopItemInfo>, EzEventListener<GameDatabaseInventoryEvent>,EzEventListener<EventTimer>
    {
        public UILabel nameItem,description;
        public UI2DSprite iconItem;
        public GameObject attamentModel;
        public UI2DSprite compareRenderQueue;
        public UILabel price;
        public UI2DSprite itemExchangeIcon;
        public UIButton buttonBuy;
        public GameObject btnInfo;
        public bool loadBySelf = false;
        [ShowIf("loadBySelf")]
        public string nameTargetShop = "ShopMain";
        public string itemIDToLoad;
        public UnityEventObject onDetail;
        public bool timerFree = false;
        [ShowIf("timerFree")]
        public UIButton btnBuyFree;
        [ShowIf("timerFree")]
        public GameObject boxTimer;


        protected float delayInit = 0.2f;
        public bool useIfClaim = true;
        public override ShopItemInfo Data
        {
            get => base.Data;
            set
            {
                base.Data = value;
                var payment = value.getPrice(0)[0][0];
                if (price)
                {
                    price.text = value.FormartString.ToString();
                }
                if(payment.item.ItemID == "IAP")
                {
                    price.text = LoadAssets.loadAsset<IAPSetting>("IAPSetting", "Variants/Database/").getInfo(value.itemSell.ItemID).Price;
                }
                if (itemExchangeIcon)
                {
                    itemExchangeIcon.sprite2D = payment.item.iconShop;
                }
                if (description)
                {
                    description.text = value.itemSell.descriptionItem.Value;
                }
                nameItem.text = value.itemSell.displayNameItem.Value;
                if (iconItem)
                {
                    iconItem.sprite2D = value.itemSell.iconShop;
                }
                if (attamentModel)
                {
                    attamentModel.transform.DestroyChildren();
                    if (compareRenderQueue)
                    {
                        var pObjectNew = Instantiate(value.itemSell.model, attamentModel.transform);
                        var pRender = pObjectNew.GetComponentInChildren<RenderQueueModifier>();
                        pRender.setTarget(compareRenderQueue);
                    }
                }
                if (payment.item)
                {
                    var pItemExchange = GameManager.Instance.Database.getComonItem(payment.item);
                    if (buttonBuy && pItemExchange.quantity >= payment.quantity)
                    {
                      //  buttonBuy.isEnabled = true;
                    }
                    else
                    {
                      //  buttonBuy.isEnabled = false;
                    }
                }

                if (btnInfo)
                {
                    bool pShow = false;
                    if (typeof(IExtractItem).IsAssignableFrom(value.itemSell.GetType()))
                    {
                        var pItems = ((ItemPackage) value.itemSell).items;
                        if (pItems.Length == 1)
                        {
                            if (typeof(IExtractItem).IsAssignableFrom(pItems[0].item.GetType()))
                            {
                                pShow = true;
                            }
                        }else if (pItems.Length > 1)
                        {
                            pShow = true;
                        }
                    }

                    btnInfo.gameObject.SetActive(pShow);
                }
             
             
             
             
             
             
             
            }
        }

        protected ShopDatabase shop;
        protected ShopItemInfo item;
        protected virtual void Awake()
        {
            if (timerFree)
            {
                labelTimer = boxTimer.GetComponentInChildren<UILabel>();
            }
            if (loadBySelf)
            {
                var pShop = LoadAssets.LoadShop( nameTargetShop);
                var pShopItem = pShop.getInfoItem(itemIDToLoad);
                if (pShopItem != null)
                {
                    item = pShopItem;
                    shop = pShop;
                    Data = pShopItem;
                }


            }
        }
        public void claim()
        {
            var pItemInfo = item;
            if (useIfClaim)
            {
                TopLayer.Instance.boxReward.show();
                if (typeof(IExtractItem).IsAssignableFrom(pItemInfo.itemSell.GetType()))
                {
                    BaseItemGameInstanced[] pItems = ((IExtractItem)pItemInfo.itemSell).ExtractHere();
                    foreach (var pItemAdd in pItems)
                    {
                        var pCheckStorage = GameManager.Instance.Database.getComonItem(pItemAdd.item);
                        pCheckStorage.Quantity += pItemAdd.Quantity;
                    }
                }
                else
                {
                    var pCheckStorage = GameManager.Instance.Database.getComonItem(pItemInfo.itemSell);
                    pCheckStorage.Quantity++;
                }
                EzEventManager.TriggerEvent(new RewardEvent(new BaseItemGameInstanced() { quantity = 1, item = item.itemSell }));
                GameManager.Instance.SaveGame();
            }
        }
        public virtual void buy()
        {
            if (loadBySelf)
            {
                if (shop != null && item != null)
                {
                  
                    var pItem = GameManager.Instance.Database.getComonItem(item.getPrice(0)[0][0].item.ItemID);
                    if (pItem.Quantity >= item.getPrice(0)[0][0].quantity)
                    {
                        pItem.Quantity -= item.getPrice(0)[0][0].quantity;
                        claim();
                    }else if(pItem.item.ItemID == "IAP")
                    {
                        GameManager.Instance.showInapp(item.itemSell.ItemID.ToLower(), delegate (bool pSuccess, IAPProduct product)
                        {
                            if(product.Id == item.itemSell.ItemID.ToLower())
                            {
                                if (pSuccess)
                                {
                                    claim();
                                }
                            }
                        });
                    }
                    else
                    {
                        HUDLayer.Instance.showDialogNotEnoughMoney(pItem.item.displayNameItem.Value, delegate
                        {
                            ShopManager.Instance.showBoxShop(pItem.item.categoryItem.ToString());
                            HUDLayer.Instance.BoxDialog.close();
                        });
                    }
                  
                }
            }
            else
            {
                onExecute();
            }
        }
        protected UILabel labelTimer;
        public virtual void free()
        {
            if (useIfClaim)
            {
                claim();
            }
            var pTimer = GameManager.Instance.Database.timers.getTimer(nameTargetShop + "/" + itemIDToLoad);
            if (pTimer == null)
            {
                pTimer = new TimeCountDown() { lastimeWheelFree = System.DateTime.Now, key = nameTargetShop + "/" + itemIDToLoad,length = item.moduleLimitBuy.timeToRestore };
                GameManager.Instance.addTimer(pTimer);
            }
        
            GameManager.Instance.SaveGame();
            updateTime();
            Data = Data;
        }

        public virtual void watch()
        {
            if (loadBySelf)
            {
                if (shop != null && item != null)
                {
                    var pPrice = item.getPrice(0);
                    if (pPrice.Length > 0)
                    {
                        for(int  i = 0; i < pPrice.Length; ++i)
                        {
                            if (pPrice[i].Length == 1)
                            {
                                var pItemExchange = pPrice[i][0].item;
                                if (pItemExchange.categoryItem == CategoryItem.WATCH)
                                {
                                    GameManager.Instance.showRewardAds(pItemExchange.ItemID, callBackResultWatch);
                                }
                            }
                        }
                     
                    }
                }
            }
        }

        public void callBackResultWatch(bool pSuccess)
        {
            if (pSuccess)
            {
                free();
            }
        }

        public virtual void showDetail()
        {
            onDetail.Invoke(Data);
        }


        // Start is called before the first frame update
        void Start()
        {

        }
        protected UILabel cacheTextFreeCountDown;
        // Update is called once per frame
        void Update()
        {
            //if (timerFree && timeCountDown.TotalMilliseconds > 0)
            //{
            //    timeCountDown -= TimeSpan.FromSeconds(Time.deltaTime);
            //    if (!cacheTextFreeCountDown)
            //    {
            //        cacheTextFreeCountDown = boxTimer.GetComponentInChildren<UILabel>();
            //    }
            //    cacheTextFreeCountDown.text = timeCountDown.ToString(@"hh\:mm\:ss");
            //    if (timeCountDown.TotalMilliseconds <= 0)
            //    {
            //        boxTimer.gameObject.SetActive(false);
            //        btnBuyFree.gameObject.SetActive(true);
            //    }
            //}
        }

        public void OnEzEvent(GameDatabaseInventoryEvent eventType)
        {
            if (eventType.behavior == BehaviorDatabase.CHANGE_QUANTITY_ITEM || eventType.behavior == BehaviorDatabase.NEWITEM)
            {
                if (item == null) return;
                if (eventType.item.item == item.getPrice(0)[0][0].item)
                {
                    if (eventType.item.Quantity >= item.getPrice(0)[0][0].quantity)
                    {
                       // buttonBuy.isEnabled = true;
                    }
                    else
                    {
                       // buttonBuy.isEnabled = false;
                    }
                }
            }
        }

        public void updateTime()
        {
            var pTimerCounting = GameManager.Instance.Database.timers.Find(x => x.key == nameTargetShop + "/" + itemIDToLoad);
            if(pTimerCounting != null)
            {
                if (!pTimerCounting.LabelTimer.Contains(labelTimer))
                {
                    pTimerCounting.LabelTimer.Add(labelTimer);
                }
           
                if (timerFree)
                {
                    btnBuyFree.gameObject.SetActive(false);
                    boxTimer.gameObject.SetActive(true);
                }
            
            }
            else
            {
                if (timerFree)
                {
                    btnBuyFree.gameObject.SetActive(true);
                    boxTimer.gameObject.SetActive(false);
                }
            }
        }
        protected TimeSpan timeCountDown;
        protected bool isInit = false;
        private void LateUpdate()
        {
            delayInit -= Time.deltaTime;
        }
        private void OnEnable()
        {
         
            EzEventManager.AddListener<GameDatabaseInventoryEvent>(this);
            EzEventManager.AddListener<EventTimer>(this);
            updateTime();
            if (delayInit <= 0)
            {
                if (Data != null)
                {
                    Data = Data;
                }
            }
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener<GameDatabaseInventoryEvent>(this);
            EzEventManager.RemoveListener<EventTimer>(this);
        }

        public void OnEzEvent(EventTimer eventType)
        {
            if(eventType.key == nameTargetShop + "/" + itemIDToLoad)
            {
                if(eventType.state == TimerState.Complete && timerFree)
                {
                    btnBuyFree.gameObject.SetActive(true);
                    boxTimer.gameObject.SetActive(false);
                }
            }
        }
    }
}
