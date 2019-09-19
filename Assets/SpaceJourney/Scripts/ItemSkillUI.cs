using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{

    public class SkillInfoInstanced 
    {
        public  SkillInfoInstance info;
        public int currentLevel = 0;
    }
    public class ItemSkillUI : BaseItem<SkillInfoInstanced>
    {
        public UI2DSprite model;
        public override SkillInfoInstanced Data { get => base.Data;
            set {
                base.Data = value;
                if (value.info.isEnabled)
                {
                    model.sprite2D = value.info.Info.iconShop;
                }
                else
                {
                    model.sprite2D = value.info.Info.iconShopDisable;
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
