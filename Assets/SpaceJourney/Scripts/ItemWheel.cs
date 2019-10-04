using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public class BaseItemGameInstancedArray
    {
        public ItemWheelInfoConfig[] infos;
    }

    public class ItemWheel : BaseItem<BaseItemGameInstancedArray>
    {
        public UI2DSprite icon;
        public UI2DSprite iconShop;
        public UILabel quantity;
        public UILabel rate;

        protected bool isFixed = false;

        public void begin()
        {
            isFixed = false;
        }
        public override BaseItemGameInstancedArray Data { get => base.Data;
            set {
                base.Data = value;
                int randomIndex = Random.Range(0, value.infos.Length);
                icon .sprite2D= value.infos[randomIndex].item.CateGoryIcon;
                if (iconShop)
                {
                    iconShop.sprite2D = value.infos[randomIndex].item.iconShop;
                }

                quantity.text = "X " + StringUtils.addDotMoney( value.infos[randomIndex].Quantity);
                if (rate)
                {
                    rate.text = (value.infos[randomIndex].percent ).ToString() + "%";
                }
            }
        }

        public void random()
        {
            if (isFixed) return;
            int randomIndex = Random.Range(0, Data.infos.Length);
            icon.sprite2D = Data.infos[randomIndex].item.CateGoryIcon;
            quantity.text = "X " + StringUtils.addDotMoney( Data.infos[randomIndex].Quantity);
        }


        public void fixItem(int index)
        {
            isFixed = true;
            icon.sprite2D = Data.infos[index].item.CateGoryIcon;
            if (iconShop)
            {
                iconShop.sprite2D = Data.infos[index].item.iconShop;
            }
            quantity.text = "X " + Data.infos[index].Quantity.ToString();
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
