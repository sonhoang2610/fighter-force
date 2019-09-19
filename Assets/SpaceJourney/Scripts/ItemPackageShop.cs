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
        public override ShopItemInfo Data { get => base.Data;
            set {
                base.Data = value;
                if (labelQuantity)
                {
                    var pQuantity = GameManager.Instance.Database.getComonItem(value.itemSell.itemID).Quantity;
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
            }
        }
        
        public void open()
        {
          var pItem =  GameManager.Instance.Database.getComonItem(Data.itemSell.itemID);
            pItem.Quantity--;
            Data = Data;
            TopLayer.Instance.boxReward.show();
            EzEventManager.TriggerEvent(new RewardEvent(new BaseItemGameInstanced() { quantity = 1, item = Data.itemSell }));
            GameManager.Instance.SaveGame();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

      
    }
}
