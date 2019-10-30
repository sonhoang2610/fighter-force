using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.UI;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class ItemGameInstanced
    {
        public ItemGame item;
        public int quantity;
    }
    public class ItemButtonInGame : BaseItem<ItemGameInstanced>
    {
        public UI2DSprite iconButton;
        public UILabel quantity;

        public override ItemGameInstanced Data {
            get => base.Data;
            set
            {
                iconButton.sprite2D = value.item.iconShop;
                quantity.text = value.quantity.ToString();
                base.Data = value;
            }
        }

        public void useController()
        {
            EzEventManager.TriggerEvent(new InputButtonTrigger(Data.item.ItemID, Data.item.categoryItem));
            var pItem = GameManager.Instance.Database.getComonItem(Data.item.ItemID);
            pItem.Quantity--;
            Data.quantity--;
            if(Data.quantity == 0)
            {
                gameObject.SetActive(false);
            }
            Data = Data;
        }
     
    }
}
