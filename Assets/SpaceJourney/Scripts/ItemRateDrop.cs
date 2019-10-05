using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class ItemRateDrop : BaseItem<ItemRateDropInfo>
    {
        public UI2DSprite icon;
        public UI2DSprite iconShop;
        public UILabel quantity;
        public UILabel rate;


        public override ItemRateDropInfo Data
        {
            get => base.Data;
            set
            {
                base.Data = value;
                icon.sprite2D = value.item.CateGoryIcon;
                if (iconShop)
                {
                    iconShop.sprite2D = value.item.iconShop;
                }

                quantity.text =  StringUtils.convertMoneyAndAddText ((int)(value.quantity.x)) +"-"+ StringUtils.convertMoneyAndAddText((int)(value.quantity.y));
                if (rate)
                {
                    rate.text = (value.percent).ToString() + "%";
                }
            }
        }

 


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

