using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Space;

namespace EazyEngine.Space.UI
{
    public class BoxShopDialog : Singleton<BoxShopDialog>
    {
        public UILabel title, content,price;
        public UI2DSprite icon, iconExchange;
        public UIButton btnYes;

        protected BaseItemGame cacheItem;
        public void showBoxWithItem(BaseItemGame item)
        {
            cacheItem = item;
            title.text = "SHOPPING";
            icon.sprite2D = item.iconShop;
           var pShop = LoadAssets.LoadShop("MainShop");
            var pItemShop = pShop.getInfoItem(item.ItemID);
            price.text = "25.000";
            iconExchange.sprite2D = GameManager.Instance.Database.getComonItem("Coin").item.iconShop;
            GetComponent<UIElement>().show();
        }

        public void buy()
        {
           var pCoinExist =  GameManager.Instance.Database.getComonItem("Coin");
           if( pCoinExist.Quantity > 25000)
            {
                pCoinExist.Quantity -= 25000;
               var pItemExist = GameManager.Instance.Database.getComonItem(cacheItem.ItemID);
                pItemExist.Quantity += 3;
                TopLayer.Instance.boxReward.show();

                EzEventManager.TriggerEvent(new RewardEvent() { item = new BaseItemGameInstanced() { item = cacheItem ,quantity = 3} });
                GetComponent<UIElement>().close();
            }
            else
            {
                GetComponent<UIElement>().close();
                HUDLayer.Instance.showDialogNotEnoughMoney("Coin", delegate
                {
                    ShopManager.Instance.showBoxShop("Coin");
                    HUDLayer.Instance.BoxDialog.close();
                }, delegate {
                    HUDLayer.Instance.BoxDialog.close();
                    GetComponent<UIElement>().show();
                });
            }
        }
    }
}
