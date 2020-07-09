
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.Events;
using EazyEngine.Space.UI;

namespace EazyEngine.Space
{
    public class ItemButtonOutGame : BaseItem<ItemOutGameInfo>, EzEventListener<GameDatabaseInventoryEvent>
    {
        public UI2DSprite iconButton;
        public UILabel quantity;
        public UnityEvent onChoose;
        public UnityEvent onUnchoose;
        protected bool isChoosed = false;

        private void OnEnable()
        {
            EzEventManager.AddListener(this);
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }

        public override ItemOutGameInfo Data
        {
            get => base.Data;
            set
            {
                iconButton.sprite2D = value.item.quantity > 0 ? value.item.item.iconShop : value.item.item.iconShopDisable;
                quantity.text = value.item.quantity > 0 ? value.item.quantity.ToString() : "+";
                // quantity.transform.parent.gameObject.SetActive(value.item.quantity > 0);
                //   GetComponent<UIButton>().isEnabled = value.item.quantity > 0;

                if (value.isChoosed)
                {
                    onChoose.Invoke();

                }
                else
                {
                    onUnchoose.Invoke();
                }

                base.Data = value;
                onExecute();
            }
        }

        public void executeWrap()
        {
            if (Data.item.Quantity > 0)
            {
                choose();
            }
            else
            {
                BoxShopDialog.Instance.showBoxWithItem(Data.item.item);
            }
        }

        public void choose()
        {
            Data.isChoosed = !Data.isChoosed;
            Data = Data;
        }
        public void setChoose(bool pBool)
        {
            isChoosed = pBool;
            Data = Data;
        }

        public void OnEzEvent(GameDatabaseInventoryEvent eventType)
        {
            if (eventType.item.item.ItemID == Data.item.item.ItemID)
            {
                if (eventType.item.changeQuantity > 0 && eventType.item.Quantity - eventType.item.changeQuantity == 0)
                {
                    {
                        Data.isChoosed = true;
                        Data = Data;
                    }
                }
                else
                {
                    quantity.text = eventType.item.Quantity.ToString();
                }
            }
        }
    }
}
