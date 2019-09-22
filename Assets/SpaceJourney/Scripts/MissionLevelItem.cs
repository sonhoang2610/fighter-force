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
        public UILabel rewardQuantity;
        public UI2DSprite iconReward;
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
