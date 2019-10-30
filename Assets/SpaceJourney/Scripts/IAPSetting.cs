using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace EazyEngine.Tools
{
    using EazyEngine.Space;
    using EazyEngine.Space.UI;
#if UNITY_EDITOR
    using UnityEditor;

    public class DailyGiftDataBaseCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/IAPSetting")]
        public static void CreateAsset()
        {
            CreatetorScriptTableObjectDTB.CreateMyAsset<IAPSetting>();
        }
    }
#endif

    [System.Serializable]
    public class IAPBaseItem
    {
        public BaseItemGame item;
        public string ShopID = "MainShop";
    }

    [System.Serializable]
    public class IAPItem
    {
        public bool isCustom;
       [HideInInspector]
       [SerializeField]
        private string name;
        [HideInInspector]
        [SerializeField]
        private string id;
        [HideInInspector]
        [SerializeField]
        private string price;
        [HideInInspector]
        [SerializeField]
        private string des;
        [HideIf("isCustom")]
        [HideLabel]
        public IAPBaseItem item;
        [ShowInInspector]
        [EnableIf("isCustom")]
        public string Name { get =>( isCustom || item == null || item.item == null) ? name : item.item.displayNameItem.Value; set => name = value; }
        [ShowInInspector]
        [EnableIf("isCustom")]
        public string Id { get => (isCustom || item == null || item.item == null) ? id : item.item.ItemID; set => id = value; }
        [EnableIf("isCustom")]
        [ShowInInspector]
        public string Price { get {
                if (isLoadLocalize || isCustom || item == null || item.item == null)
                {
                    return price;
                }
                if (!string.IsNullOrEmpty(item.ShopID))
                {
                    var pShop = LoadAssets.LoadShop(item.ShopID);
                    if (pShop)
                    {
                        var pInfoPrice = pShop.getInfoItem(item.item.ItemID);
                        foreach (var pWay in pInfoPrice.getPrice(0))
                        {
                            if (pWay.Length == 1)
                            {
                                if (pWay[0].item.ItemID == "IAP")
                                {
                                    if (!pInfoPrice.formatString.Value.Contains("{0}"))
                                        return pInfoPrice.formatString.Value;
                                    return string.Format(pInfoPrice.formatString.Value, pWay[0].quantity.ToString());
                                }
                            }
                        }
                    }
                }
                return price;
            } set => price = value;
        }
        [ShowInInspector]
        [EnableIf("isCustom")]
        public string Des { get => (isCustom || item == null || item.item == null) ? des : item.item.descriptionItem.Value; set => des = value; }

        public bool isLoadLocalize { get; set; }
    }


    [System.Serializable]
    public class IAPSetting : ScriptableObject
    {
        [ListDrawerSettings(ShowIndexLabels =true,ListElementLabelName = "Name")]
        public IAPItem[] items;

        public IAPItem getInfo(string itemId)
        {
            for(int i = 0; i < items.Length; ++i)
            {
                if (items[i].Id.ToLower() == itemId.ToLower())
                {
                    return items[i];
                }
            }
            return null;
        }
    }
}
