using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EazyEngine.Space.UI
{
    public class ItemStorageRequire : BaseItem<RequireCondition>
    {
        public UI2DSprite icon;
        public UILabel quantity;
        public UIButton btnAddMore;
        public List<EventDelegate> onAddMore = new List<EventDelegate>();
        public override RequireCondition Data
        {
            get => base.Data; set
            {
                base.Data = value;
                if (value.craftItem)
                {
                    var pItem = GameManager.Instance.Database.getComonItem(value.craftItem);
                    icon.sprite2D = pItem.item.CateGoryIcon;
                    quantity.text = pItem.Quantity.ToString()+"/"+ value.quantityRequire;
                }
      
            }
        }

        public bool isEnough
        {
            get
            {
                var pItem = GameManager.Instance.Database.getComonItem(Data.craftItem);
                icon.sprite2D = pItem.item.CateGoryIcon;
                return pItem.Quantity >= Data.quantityRequire;
            }
        }

        public void clickMore()
        {
            EventDelegate.Execute(onAddMore);
        }
        // Start is called before the first frame update
        void Start()
        {
            if (btnAddMore)
            {
                btnAddMore.onClick.Clear();
                btnAddMore.onClick.AddRange(onAddMore);
            }
     
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
