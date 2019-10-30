
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.Events;

namespace EazyEngine.Space
{
    public class ItemButtonOutGame : BaseItem<BaseItemGameInstanced>
    {
        public UI2DSprite iconButton;
        public UILabel quantity;
        public UnityEvent onChoose;
        public UnityEvent onUnchoose;
        protected bool isChoosed = false;

        public override BaseItemGameInstanced Data
        {
            get => base.Data;
            set
            {
                iconButton.sprite2D = value.quantity > 0 ? value.item.iconShop : value.item.iconShopDisable ;
                quantity.text = value.quantity.ToString();
                quantity.transform.parent.gameObject.SetActive(value.quantity > 0);
                GetComponent<UIButton>().isEnabled = value.quantity > 0;
                base.Data = value;
            }
        }

        public void choose()
        {
            isChoosed = !isChoosed;
            if (isChoosed)
            {
                onChoose.Invoke();
            }
            else
            {
                onUnchoose.Invoke();
            }
        }
        public void setChoose(bool pBool)
        {
            isChoosed = pBool;
            if (isChoosed)
            {
                onChoose.Invoke();
            }
            else
            {
                onUnchoose.Invoke();
            }
        }

    }
}
