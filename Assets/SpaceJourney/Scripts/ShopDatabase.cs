using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using FlowCanvas.Macros;
using Sirenix.OdinInspector;


namespace EazyEngine.Space
{
    using Sirenix.Serialization;
#if UNITY_EDITOR
    using UnityEditor;
    public class ShopDatabaseInfoCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/ShopDatabase")]
        public static void CreateMyAsset()
        {
            ShopDatabase asset = ScriptableObject.CreateInstance<ShopDatabase>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/ShopDatabase.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif

    public enum AlgrothimPriceItem
    {
        StepConstrainEachLevel,
        ConstrainDefineFromeSpecifiedLevel,
    }

    public class SpecifiedLevelStepUnitGeneric<T>
    {
        public int levelRequire;
        public T unit;
    }
    [System.Serializable]
    public class SpecifiedLevelStepUnit : SpecifiedLevelStepUnitGeneric<int>
    {

    }

    [System.Serializable]
    public class SpecifiedLevelStepMultipeUnit : SpecifiedLevelStepUnitGeneric<PaymentWay[]>
    {
    }
    [System.Serializable]
    public class PaymentWay
    {
        public int groupID = 0;
        [HideLabel]
        public BaseItemGameInstanced exchangeItems;
        public int addUnitLevel = 0;
        public bool requireOnly = false;
    }



    [System.Serializable]
    public class ShopItemInfo
    {
        public string LabelName
        {
            get
            {
                if (itemSell == null)
                {
                    return "none";
                }
                return itemSell.ItemID;
            }
        }
        public string idCategoryInShop = "Common";
        public bool isVisible = true;
        public BaseItemGame itemSell;
        public LimitedModule moduleLimitBuy;
        public bool isLimit = false;
        [ShowIf("isLimit")]
        public int limitUpgrade = 1;
        //[HideLabel]
        //public PaymentWay[] wayDefault;
        //public AlgrothimPriceItem algrothimTypeUpgrade = AlgrothimPriceItem.StepConstrainEachLevel;
        //[ShowIf("ableUpgrade")]
        //[ShowIf("algrothimTypeUpgrade", AlgrothimPriceItem.StepConstrainEachLevel)]
        //public int quantityStep = 0;
        //[ShowIf("ableUpgrade")]
        //[ShowIf("algrothimTypeUpgrade", AlgrothimPriceItem.ConstrainDefineFromeSpecifiedLevel)]
        [ListDrawerSettings(AddCopiesLastElement = true)]
        public SpecifiedLevelStepMultipeUnit[] priceDefines = new SpecifiedLevelStepMultipeUnit[] { new SpecifiedLevelStepMultipeUnit() { levelRequire = 0, unit = new PaymentWay[] { new PaymentWay() } } };
        public int sortGroupPrice(PaymentWay pA,PaymentWay pB)
        {
            return pA.groupID.CompareTo(pB.groupID);
        }
        public BaseItemGameInstanced[][] getPrice(int pLevel)
        {
            List<BaseItemGameInstanced[]> pPrice = new List<BaseItemGameInstanced[]>();
            for (int i = priceDefines.Length - 1; i >= 0; --i)
            {
                if (priceDefines[i].levelRequire <= pLevel)
                {
                    System.Array.Sort(priceDefines[i].unit, sortGroupPrice);
                    List<BaseItemGameInstanced> pPayment = new List<BaseItemGameInstanced>();
                    int pID = 0;
                    bool first = true;
                    for(int j = 0; j < priceDefines[i].unit.Length; ++j)
                    {
                        if(pID != priceDefines[i].unit[j].groupID && !first)
                        {
                            pPrice.Add(pPayment.ToArray());
                            pPayment.Clear();
                        }
                        pID = priceDefines[i].unit[j].groupID;
                        pPayment.Add(new BaseItemGameInstanced() { isRequire = priceDefines[i].unit[j].requireOnly,   item = priceDefines[i].unit[j].exchangeItems.item, quantity = priceDefines[i].unit[j].exchangeItems.quantity + priceDefines[i].unit[j].addUnitLevel* (pLevel- priceDefines[i].levelRequire )});
                        first = false;
                    }
                    if(pPayment.Count > 0)
                    {
                        pPrice.Add(pPayment.ToArray());
                    }
                    break;
                }
            }
            return pPrice.ToArray();

        }
        public int getPriceFirst(int pLelvel)
        {
            return getPrice(pLelvel)[0][0].quantity;
        }
        public int getPriceSecond(int pLelvel)
        {
            return getPrice(pLelvel)[1][0].quantity;
        }
        public BaseItemGameInstanced getPriceFirstItem(int pLelvel)
        {
            return getPrice(pLelvel)[0][0];
        }
        public BaseItemGameInstanced getPriceSecondItem(int pLelvel)
        {
            return getPrice(pLelvel)[1][0];
        }
        public BaseItemGameInstanced[] getPriceFirstItems(int pLelvel)
        {
            return getPrice(pLelvel)[0];
        }
        public BaseItemGameInstanced[] getPriceSecondItems(int pLelvel)
        {
            return getPrice(pLelvel)[1];
        }
        public I2String formatString = new I2String() { normalString = "{0}" };
        public string FormartString
        {
            get
            {
                return string.IsNullOrEmpty(formatString.Value) ? getPriceFirst(0).ToString() : string.Format(formatString.Value, getPriceFirst(0).ToString());
            }
        }
    }

    public class ShopDatabase : EzScriptTableObject
    {
        public string nameShop;
        [ListDrawerSettings(AddCopiesLastElement = true, ListElementLabelName = "LabelName")]
        public ShopItemInfo[] items;

        public bool ContainItem(BaseItemGame pItem)
        {
            for (int i = 0; i < items.Length; ++i)
            {
                if (items[i].itemSell == pItem)
                {
                    return true;
                }
            }
            return false;
        }

        public ShopItemInfo getInfoItem(string pItemID)
        {
            for (int i = 0; i < items.Length; ++i)
            {
                if (items[i].itemSell.ItemID == pItemID)
                {
                    return items[i];
                }
            }
            return null;
        }

        public ShopItemInfo[] getAllItemCateGoryID(string pID)
        {
            List<ShopItemInfo> array = new List<ShopItemInfo>();
            for (int i = 0; i < items.Length; ++i)
            {
                if (items[i].idCategoryInShop == pID && items[i].isVisible)
                {
                    array.Add(items[i]);
                }
            }
            return array.ToArray();
        }
    }
}
