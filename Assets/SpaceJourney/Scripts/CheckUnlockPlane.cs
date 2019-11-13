using EazyEngine.Space;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EazyEngine.Tools;

namespace EazyEngine.Space.UI
{
    public class CheckUnlockPlane : MonoBehaviour,EzEventListener<GameDatabaseInventoryEvent>
    {
        public PlaneInfo[] infos;

        public UnityEvent onAbleUnlock,onUnAbleUnlock;

        protected ShopDatabase shop;
        protected ShopDatabase Shop
        {
            get
            {
                if(shop == null)
                {
                    shop = LoadAssets.LoadShop("UpgradePlane");
                }
                return shop;
            }
        }
        public void check()
        {
            bool pNotice = false;
            for(int i = 0; i < Shop.items.Length; ++i)
            {
              var pItem =  System.Array.Find(infos, x => x.itemID == Shop.items[i].itemSell.itemID);
                if (pItem)
                {

                    var pItemNowPlane = GameManager.Instance.Database.planes.Find(x => x.info.itemID == pItem.itemID);
                    var pItemNowSpPlane = GameManager.Instance.Database.spPlanes.Find(x => x.info.itemID == pItem.itemID);
                    if (pItem.categoryItem == CategoryItem.PLANE)
                    {
                        if (pItemNowPlane == null || pItemNowPlane.CurrentLevel == 0)
                        {
                            var price = Shop.items[i].getPrice(1);
                            if (price != null && price.Length > 1)
                            {
                                var pExist = GameManager.Instance.Database.getComonItem(price[0][1].item);
                                if (pExist.Quantity >= price[0][1].Quantity)
                                {
                                    pNotice = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (pItem.categoryItem == CategoryItem.SP_PLANE)
                    {
                        if (pItemNowSpPlane == null || pItemNowSpPlane.CurrentLevel == 0)
                        {
                            var price = Shop.items[i].getPrice(1);
                            if (price != null && price.Length > 1)
                            {
                                var pExist = GameManager.Instance.Database.getComonItem(price[0][1].item);
                                if (pExist.Quantity >= price[0][1].Quantity)
                                {
                                    pNotice = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (pNotice)
            {
                onAbleUnlock.Invoke();
            }
            else
            {
                onUnAbleUnlock.Invoke();
            }
        }

        private void OnEnable()
        {
            check();
            EzEventManager.AddListener(this);   
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnEzEvent(GameDatabaseInventoryEvent eventType)
        {
            check();
        }
    }
}
