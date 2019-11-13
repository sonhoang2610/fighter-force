using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class ItemBtnChoosePlane : BaseItem<PlaneInfoConfig>
    {
        public UI2DSprite icon;

        public override PlaneInfoConfig Data { get => base.Data; set {
                icon.sprite2D = value.info.iconShop;
                if( value.CurrentLevel <= 0)
                {
                    icon.sprite2D = value.info.iconShopDisable;
                }
                var pCheck = GetComponent<CheckUnlockPlane>();
                pCheck.infos.clear();
                System.Array.Resize(ref pCheck.infos, 1);
                pCheck.infos[0] = value.info;
                pCheck.check();
                base.Data = value;
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
