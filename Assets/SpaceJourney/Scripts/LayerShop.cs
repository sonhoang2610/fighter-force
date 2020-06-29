using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EasyMobile;

namespace EazyEngine.Space.UI
{
    public class LayerShop : BaseBox<ItemShop, ShopItemInfo>
    {

        public string cateGoryItemLoad;
        public string targetShop;
        public BoxPackageInfo boxInfo;
        protected bool firstEnable = true;


        private void OnEnable()
        {
            if (firstEnable) { firstEnable = false; return; }
            var pShop = LoadAssets.LoadShop(targetShop);
            if (pShop)
            {
                DataSource = pShop.getAllItemCateGoryID(cateGoryItemLoad).ToObservableList();
            }
        }
        public override void Awake()
        {
            base.Awake();
            var pShop = LoadAssets.LoadShop(targetShop);
            if (pShop)
            {
                DataSource = pShop.getAllItemCateGoryID(cateGoryItemLoad).ToObservableList();
            }
        }
  
        public void claim(ShopItemInfo pItemInfo)
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
            EzEventManager.TriggerEvent(new RewardEvent(new BaseItemGameInstanced() { quantity = 1, item = pItemInfo.itemSell }));
            GameManager.Instance.SaveGame();

        }
        public void onBuy(object pItem)
        {
            var pItemInfo = (ShopItemInfo)pItem;
            var pItemStorage = GameManager.Instance.Database.getComonItem(pItemInfo.getPrice(0)[0][0].item.ItemID);
            if (pItemStorage.Quantity >= pItemInfo.getPrice(0)[0][0].quantity)
            {
                pItemStorage.Quantity -= pItemInfo.getPrice(0)[0][0].quantity;
                EazyAnalyticTool.LogEvent("BuyItemShop", "Name", pItemInfo.itemSell.ItemID);
                claim(pItemInfo);
            }
            else if (pItemStorage.item.ItemID == "IAP")
            {
                if(pItemInfo.itemSell.GetType() == typeof(ComboPackage))
                {
                    ExtraPackageDatabase.Instance.executePackage((ComboPackage)pItemInfo.itemSell);
                    return;
                }
                GameManager.Instance.showInapp(pItemInfo.itemSell.ItemID.ToLower(), delegate (bool pSuccess, IAPProduct product)
                {
                    if (product.Id == pItemInfo.itemSell.ItemID.ToLower())
                    {
                        if (pSuccess)
                        {
                            EazyAnalyticTool.LogEvent("BuyItemShop", "Name", pItemInfo.itemSell.ItemID);
                            claim(pItemInfo);
                        }
                        else
                        {
                            Debug.Log("Failed Buy");
                        }
                    }
                });
            }
            else
            {
                HUDLayer.Instance.showDialogNotEnoughMoney(pItemStorage.item.displayNameItem.Value, delegate
                {
                    ShopManager.Instance.showBoxShop(pItemStorage.item.categoryItem.ToString());
                    HUDLayer.Instance.BoxDialog.close();
                });
            }
        }
        public void onShowInfo(object pItem)
        {
            var pItemInfo = (ShopItemInfo)pItem;
            if (boxInfo)
            {
                boxInfo.GetComponent<UIElement>().show();
                if (typeof(IExtractItem).IsAssignableFrom(((ItemPackage) pItemInfo.itemSell).items[0].item.GetType()))
                {
                    boxInfo.setData( (ItemPackage)((ItemPackage) pItemInfo.itemSell).items[0].item);
                }
                else
                {
                    boxInfo.setData( ((ItemPackage) pItemInfo.itemSell));
                }
     
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
