using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space.UI
{
    public class BoxPackageInfo : BaseBox<ItemWheel, BaseItemGameInstancedArray>
    {
        public UI2DSprite icon;
        public UILabel title;
        public void setData(ItemPackage pPack)
        {
            title.text = pPack.displayNameItem.Value;
            icon.sprite2D = pPack.iconShop;
            List<BaseItemGameInstancedArray> pDatas = new List<BaseItemGameInstancedArray>();
            for(int i  = 0; i < pPack.items.Length; ++i)
            {
                var pItemDrop = pPack.items[i];
                pDatas.Add(new BaseItemGameInstancedArray() { infos = new ItemWheelInfoConfig[] { new ItemWheelInfoConfig() { item = pItemDrop .item,percent = pItemDrop.percent,quantityStart =(int) pItemDrop.quantity.x} } });
            }
            DataSource = pDatas.ToObservableList();
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
