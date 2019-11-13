using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
    using Sirenix.OdinInspector;
#if UNITY_EDITOR
    using UnityEditor;
    public class ItemPackageCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/ItemPackage")]
        public static void CreateMyAsset()
        {
            ItemPackage asset = ScriptableObject.CreateInstance<ItemPackage>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/ItemPackage.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif
    public interface IExtractItem
    {
        BaseItemGameInstanced[] ExtractHere(bool isNew = true);
        int CacheExtraItemCount();
        void disableExtracItem();
        bool alwayExtra();
    }
    [System.Serializable]
    public class ItemRateDropInfo
    {
        public BaseItemGame item;
        public Vector2 quantity = new Vector2(1, 1);
        public float percent;
    }
    public class ItemPackage : BaseItemGame, IExtractItem
    {

        public bool alwayExtras = false;
        public ItemRateDropInfo[] items;
        public Vector2 randomQuantity = new Vector2(1, 1);

        public bool alwayExtra()
        {
            return alwayExtras;
        }
        protected BaseItemGameInstanced[] cacheExtra;
        public BaseItemGameInstanced[] ExtractHere(bool isNew = true)
        {
            int pFirstBox = PlayerPrefs.GetInt("FirstBoxReward", 0);
            if(pFirstBox == 2 && isNew)
            {
                PlayerPrefs.SetInt("FirstBoxReward",3);
                var pItemCoin = GameDatabase.Instance.getItem("Coin", CategoryItem.COMON);
                var pItemCrystal = GameDatabase.Instance.getItem("Crystal", CategoryItem.COMON);
                var pItemcraft = GameDatabase.Instance.getItem("CraftPlane2", CategoryItem.COMON);
                List<BaseItemGameInstanced> pItemResultFake = new List<BaseItemGameInstanced>()
                {
                    new BaseItemGameInstanced(){item =pItemCoin,quantity= 10000 },
                    new BaseItemGameInstanced(){item =pItemCrystal,quantity= 100 },
                    new BaseItemGameInstanced(){item =pItemcraft,quantity= 10 }
                };
                cacheExtra = pItemResultFake.ToArray();
                return pItemResultFake.ToArray();


            }
            if (!isNew && cacheExtra != null)
            {
                return cacheExtra;
            }
            List<BaseItemGameInstanced> pItemResult = new List<BaseItemGameInstanced>();
            int pCount = Random.Range((int)randomQuantity.x, (int)randomQuantity.y+1);
            do
            {
                List<ItemRateDropInfo> pItems = new List<ItemRateDropInfo>();
                pItems.AddRange(items);
                int pRandom = Random.Range(0, 100);
                int pCurrent = 0;
                int indexBreak = 0;
                while (true)
                {
                    if (pCurrent >= 100)
                    {
                        break;
                    }
                    int index = Random.Range(0, pItems.Count);
                    pCurrent += (int)pItems[index].percent;
                    if (pRandom <= pCurrent)
                    {
                        if (!typeof(IExtractItem).IsAssignableFrom(pItems[index].item.GetType()) || !((IExtractItem)pItems[index].item).alwayExtra())
                        {
                            pItemResult.Add(new BaseItemGameInstanced() { item = pItems[index].item, quantity = Random.Range((int)pItems[index].quantity.x, (int)pItems[index].quantity.y + 1) });
                        }
                        else
                        {
                            pItemResult.AddRange(((IExtractItem)pItems[index].item).ExtractHere());
                        }
                        break;
                    }
                    else
                    {
                        pItems.RemoveAt(index);
                    }
                    indexBreak++;
                    if (indexBreak > 1000000)
                    {
                        break;
                    }
                }
                pCount--;
            } while (pCount > 0);
            cacheExtra = pItemResult.ToArray();
           return pItemResult.ToArray();
        }

        public int CacheExtraItemCount()
        {
            return cacheExtra== null ? 0: cacheExtra.Length;
        }

        public void disableExtracItem()
        {
            cacheExtra = null;
        }
    }
}
