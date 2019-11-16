
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.Events;
using EazyEngine.Space.UI;

namespace EazyEngine.Space
{
    public class ItemButtonOutGame : BaseItem<ItemOutGameInfo>
    {
        public UI2DSprite iconButton;
        public UILabel quantity;
        public UnityEvent onChoose;
        public UnityEvent onUnchoose;
        protected bool isChoosed = false;

        public override ItemOutGameInfo Data
        {
            get => base.Data;
            set
            {
                iconButton.sprite2D = value.item.quantity > 0 ? value.item.item.iconShop : value.item.item.iconShopDisable ;
                quantity.text = value.item.quantity.ToString();
                quantity.transform.parent.gameObject.SetActive(value.item.quantity > 0);
                GetComponent<UIButton>().isEnabled = value.item.quantity > 0;
                if (value.isChoosed)
                {
                    onChoose.Invoke();
              
                }
                else
                {
                    onUnchoose.Invoke();
                }
                onExecute();
                base.Data = value;
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

    }
}
