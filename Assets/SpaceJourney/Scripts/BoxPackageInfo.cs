using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space.UI
{
    public class BoxPackageInfo : BaseBox<ItemRateDrop, ItemRateDropInfo>
    {
        public UI2DSprite icon;
        public UILabel title;
        public void setData(ItemPackage pPack)
        {
            title.text = pPack.displayNameItem.Value;
            icon.sprite2D = pPack.iconShop;
            List<ItemRateDropInfo> pDatas = new List<ItemRateDropInfo>();
            for(int i  = 0; i < pPack.items.Length; ++i)
            {
                var pItemDrop = pPack.items[i];
                pDatas.Add(pItemDrop);
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
