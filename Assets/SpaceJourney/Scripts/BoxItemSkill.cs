using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class BoxItemSkill : BaseBox<ItemButtonOutGame,BaseItemGameInstanced>
    {
        private void OnEnable()
        {
            //GameManager.Instance.Database.getComonItem();
           var pItems = GameDatabase.Instance.getAllItem(CategoryItem.SP_ITEM);
            List<BaseItemGameInstanced> pItemReal = new List<BaseItemGameInstanced>();
            for(int i = 0; i < pItems.Length; ++i)
            {
               var pItemExist = GameManager.Instance.Database.getComonItem(pItems[i].ItemID);
                if(pItemExist == null)
                {
                    pItemExist = new BaseItemGameInstanced() { item = pItems[i], quantity = 0 };
                }
                pItemReal.Add(pItemExist);
            }
            DataSource = pItemReal.ToObservableList();
            Invoke(nameof(chooseDefault), 0.25f);
          
        }

        public void chooseDefault()
        {
            for (int i = 0; i < 2; ++i)
            {
                if (i < items.Count)
                {
                    items[i].setChoose(true);
                    items[i].onExecute();
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
