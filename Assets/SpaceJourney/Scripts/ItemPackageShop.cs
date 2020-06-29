using EazyEngine.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public class UnityEventObject : UnityEvent<object>
    {

    }
    public class ItemPackageShop : ItemShop
    {
        public UILabel labelQuantity;
        public UIButton btnOpen;

        public override void OnEzEvent(GameDatabaseInventoryEvent eventType)
        {
            base.OnEzEvent(eventType);
            if(eventType.item.item.ItemID == Data.itemSell.ItemID)
            {
                if (labelQuantity)
                {
                    var pQuantity = GameManager.Instance.Database.getComonItem(Data.itemSell.ItemID).Quantity;
                    labelQuantity.transform.parent.gameObject.SetActive(pQuantity > 0);
                    labelQuantity.text = pQuantity.ToString();
                    if (pQuantity > 0)
                    {
                        buttonBuy.gameObject.SetActive(false);
                        btnOpen.gameObject.SetActive(true);
                    }
                    else
                    {
                        buttonBuy.gameObject.SetActive(true);
                        btnOpen.gameObject.SetActive(false);
                    }

                }
                buttonBuy.transform.parent.gameObject.SendMessage("Reposition", SendMessageOptions.DontRequireReceiver);
            }
        }

        public override ShopItemInfo Data { get => base.Data;
            set {
                base.Data = value;
                if (labelQuantity)
                {
                    var pQuantity = GameManager.Instance.Database.getComonItem(value.itemSell.ItemID).Quantity;
                    labelQuantity.transform.parent.gameObject.SetActive(pQuantity > 0);
                    labelQuantity.text = pQuantity.ToString();
                    if(pQuantity > 0)
                    {
                        buttonBuy.gameObject.SetActive(false);
                        btnOpen.gameObject.SetActive(true);
                    }
                    else
                    {
                        buttonBuy.gameObject.SetActive(true);
                        btnOpen.gameObject.SetActive(false);
                    }
                 
                }
                buttonBuy.transform.parent.gameObject.SendMessage("Reposition", SendMessageOptions.DontRequireReceiver);
            }
        }
        
        public void open()
        {
            EazyAnalyticTool.LogEvent("OpenPackage", "Name", Data.itemSell.ItemID);
            var pItem =  GameManager.Instance.Database.getComonItem(Data.itemSell.ItemID);
            pItem.Quantity--;
            claim();
            Data = Data;
        }
        public BoxPackageInfo boxInfo;
        public void showInfo(object pData)
        {
            boxInfo.GetComponent<UIElement>().show();
            boxInfo.setData((ItemPackage) ((ShopItemInfo)pData).itemSell);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

      
    }
}
