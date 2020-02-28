using NodeCanvas.Framework;
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
        public SpriteRenderer spriteEffect;

        protected int cacheLevel = -1;
        public override SkillInfoInstanced Data { get => base.Data;
            set {
                if(Data != null)
                {
                    if (value.info.info.ItemID == Data.info.info.ItemID)
                    {
                        if(cacheLevel < value.info.CurrentLevelSkill )
                        {
                            showEffect();
                            cacheLevel = value.info.CurrentLevelSkill;
                        }
                    }
                    else
                    {
                        cacheLevel = value.info.CurrentLevelSkill;
                        if (spriteEffect.gameObject.activeSelf)
                        {
                            disableEffect();
                        }
                    }
                }
                else
                {
                    cacheLevel = value.info.CurrentLevelSkill;
                }
             
                base.Data = value;
                if (value.info.CurrentLevelSkill > 0)
                {
                    model.sprite2D = value.info.Info.iconShop;
               
                }
                else
                {
                    model.sprite2D = value.info.Info.iconShopDisable;
                }
                if (spriteEffect)
                {
                    spriteEffect.GetComponent<IBlackboard>().SetValue("enable", value.info.Info.iconShop);
                    spriteEffect.GetComponent<IBlackboard>().SetValue("disable", value.info.Info.iconShopDisable);
                    float pX = value.info.Info.iconShop.bounds.size.x*100 / (model.localSize.x );
                    float pY = value.info.Info.iconShop.bounds.size.y*100 / (model.localSize.y );
                    spriteEffect.transform.localScale = new Vector3(100.0f/ pX, 100.0f / pY, 1);
                }
            }
        }

        protected float delayDisable = 0;
        [ContextMenu("test")]
        public void showEffect()
        {
            spriteEffect.gameObject.SetActive(true);
            model.alpha = 0;
            delayDisable = 2;
        }
        
        // Start is called before the first frame update
        void Start()
        {

        }
        public void disableEffect()
        {
            model.alpha = 1;
            spriteEffect.gameObject.SetActive(false);
        }
        // Update is called once per frame
        void Update()
        {
            if(delayDisable > 0)
            {
                delayDisable -= Time.deltaTime;
                if(delayDisable <= 0)
                {
                    disableEffect();
                }
            }
        }
    }
}
