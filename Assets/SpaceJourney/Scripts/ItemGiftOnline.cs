using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
    [System.Serializable]
    public class ItemGiftOnlineInfo {
        [System.NonSerialized]
        public int status;
        [System.NonSerialized]
        public bool isNext;
        [System.NonSerialized]
        public int timeLeft;
        public BaseItemGameInstanced mainData;
        public double time;
        public ItemGiftOnlineInfo(BaseItemGameInstanced pData,int pstatus)
        {
            mainData = pData;
            status = pstatus;
        }
    }
    public class ItemGiftOnline : BaseItem<ItemGiftOnlineInfo>
    {
        public UITexture iconItem;
        public UILabel quantity;
        public UI2DSprite iconCateGory;
        public UI2DSprite iconStatus;
        public Sprite[] statusSprites;
        public GameObject iconDaNhan;
        public UILabel leftTime;
        public ParticleSystem effect;
        protected bool Effecting = false;
        protected System.Action callBackEffect;
        public void claim(System.Action pcallBackEffect)
        {
            callBackEffect = pcallBackEffect;
            Effecting = true;
            GetComponent<Animator>().SetTrigger("Play");
        }
        
        

        public void claimReal()
        {
            callBackEffect();
            effect.gameObject.SetActive(true);
            effect.Play();
            var pItem = GameManager.Instance.Database.getComonItem(Data.mainData.item.ItemID);
            Data.status = 1;
            pItem.Quantity += Data.mainData.quantity;
            Effecting = false; 
        }

        private void OnDisable()
        {
            if (Effecting && Using)
            {
                var pItem = GameManager.Instance.Database.getComonItem(Data.mainData.item.ItemID);
                pItem.Quantity += Data.mainData.quantity;
                Effecting = false;
            }
        }
        public override ItemGiftOnlineInfo Data { get { return base.Data; } set {
                iconItem.mainTexture = value.mainData.item.iconShop.texture;
                quantity.text = StringUtils.addDotMoney( value.mainData.quantity);
                iconCateGory.sprite2D = value.mainData.item.CateGoryIcon;
                iconStatus.sprite2D = statusSprites[value.status];
                if (value.isNext)
                {
                    iconStatus.sprite2D = statusSprites[2];
                }
                iconStatus.MakePixelPerfect();
                if (value.status == 0)
                {
                    iconItem.GetComponent<NGUIEditMaterial>().setEffectAmount(0, 0);
                    iconDaNhan.gameObject.SetActive(false);

                }
                else
                {
                    iconItem.GetComponent<NGUIEditMaterial>().setEffectAmount(0, 1);
                    iconDaNhan.gameObject.SetActive(true);
                }
                base.Data = value;
            } }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            double ptime = Data.time - (double)GameManager.Instance.giftOnlineModule.onlineTime;
            if(ptime < 0)
            {
                ptime = 0;
            }
            TimeSpan time = TimeSpan.FromSeconds(ptime);

            //here backslash is must to tell that colon is
            //not the part of format, it just a character that we want in output
            leftTime.text = time.ToString(@"hh\:mm\:ss");
        }
    }

}
