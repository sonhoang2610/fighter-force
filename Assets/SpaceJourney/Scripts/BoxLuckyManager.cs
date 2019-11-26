using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI {
    public class BoxLuckyManager : MonoBehaviour
    {
        public BoxExtract boxExtract;
        public ItemPackage[] allPackage;

        // Start is called before the first frame update
        void Start()
        {
            List<BaseItemGameInstanced> pItems = new List<BaseItemGameInstanced>();
            for(int i = 0; i < allPackage.Length; ++i)
            {
                for(int j =0; j < allPackage[i].items.Length; ++j)
                {
                    if (typeof(ItemPackage).IsAssignableFrom(allPackage[i].items[j].item.GetType()))
                    {
                        var pPackageExist = System.Array.Find(allPackage, x => x.itemID == allPackage[i].items[j].item.itemID);
                        if(pPackageExist == null)
                        {
                            System.Array.Resize(ref allPackage, allPackage.Length + 1);
                            allPackage[allPackage.Length - 1] = (ItemPackage) allPackage[i].items[j].item;
                        }
                        continue;
                    }
                    else
                    {
                        var pItemExist = pItems.Find(x => x.item.ItemID == allPackage[i].items[j].item.ItemID);
                        if (pItemExist == null)
                        {
                            pItems.Add(new BaseItemGameInstanced()
                            {
                                item = allPackage[i].items[j].item,
                                quantity = (int)allPackage[i].items[j].quantity.y
                            });
                        }
                        else
                        {
                            pItemExist.quantity = Mathf.Max(pItemExist.quantity, (int)allPackage[i].items[j].quantity.y);
                        }
                    }
                
                }
            }
            pItems = pItems.Shuffle();
            boxExtract.DataSource = pItems.ToObservableList();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
