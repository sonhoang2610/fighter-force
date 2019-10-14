using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class MissionLevelItem : BaseItem<MissionItemInstanced>
    {
        public UILabel content;
        public EazyFrameCache status;
        public GameObject iconComplete;
        public GameObject rewardLayer;
        public UILabel rewardQuantity,rewardQuantity1;
        public UI2DSprite iconReward,iconReward1;
        public override MissionItemInstanced Data { get => base.Data;
            set
            {
                base.Data = value;
                content.text = value.mission.descriptionItem.Value;
                status.setFrameIndex(value.process >= 1 ? 1 : 0);
                iconComplete.SetActive(value.process >= 1);
                rewardLayer.SetActive(value.process < 1);
                iconReward.sprite2D = value.rewards[0].item.iconShop;
                rewardQuantity.text =StringUtils.addDotMoney( value.rewards[0].quantity);
                if (value.rewards.Length > 1)
                {
                    iconReward1.sprite2D = value.rewards[1].item.iconShop;
                    rewardQuantity1.text =StringUtils.addDotMoney( value.rewards[1].quantity);
                }
                else
                {
                    iconReward1.gameObject.SetActive(false);
                    rewardQuantity1.gameObject.SetActive(false);
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
