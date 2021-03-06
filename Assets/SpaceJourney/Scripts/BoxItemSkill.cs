﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class ItemOutGameInfo
    {
        public BaseItemGameInstanced item;
        public bool isChoosed;
    }
    public class BoxItemSkill : BaseBox<ItemButtonOutGame, ItemOutGameInfo>
    {
        private void OnEnable()
        {
            //GameManager.Instance.Database.getComonItem();
           var pItems = GameDatabase.Instance.getAllItem(CategoryItem.SP_ITEM);
            List<ItemOutGameInfo> pItemReal = new List<ItemOutGameInfo>();
            for(int i = 0; i < pItems.Length; ++i)
            {
               var pItemExist = GameManager.Instance.Database.getComonItem(pItems[i].ItemID);
                pItemReal.Add(new ItemOutGameInfo() {
                    item = pItemExist,
                    isChoosed = (i < 2 && pItemExist.Quantity >0) ? true : false 
                });
            }
            DataSource = pItemReal.ToObservableList();
          
        }

        public void chooseDefault()
        {
            //for (int i = 0; i < 2; ++i)
            //{
            //    if (i < items.Count && items[i].Data.Quantity > 0)
            //    {
            //        items[i].setChoose(true);
            //        items[i].onExecute();
            //    }else if(i < items.Count)
            //    {
            //        items[i].setChoose(false);
            //        items[i].onExecute();
            //    }
            //}
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
