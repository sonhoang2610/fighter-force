using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class ItemDailyGiftInfo
    {
        [System.NonSerialized]
        public int status;
        [System.NonSerialized]
        public bool isNext;
        public BaseItemGameInstanced mainData;
        public int day;
    }
    public class ItemDailyGift : BaseItem<ItemDailyGiftInfo>
    {
        public Sprite[] frameIndex;
        public UI2DSprite iconItem,box;
        public UILabel quantity,time;
        public GameObject iconRecieve, block,now;
  

        public override ItemDailyGiftInfo Data { get => base.Data; set {

                base.Data = value;
                iconItem.sprite2D = value.mainData.item.iconShop;
                quantity.text ="+"+ value.mainData.quantity.ToString();
                time.text =  value.day.ToString();
                iconRecieve.SetActive(value.status == 1);
                block.SetActive(value.status == 1);
                now.SetActive(value.isNext && value.status == 0);
            }
        }
        public override int Index { get => base.Index; set { box.sprite2D = frameIndex[value % 2]; base.Index = value; } }
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
