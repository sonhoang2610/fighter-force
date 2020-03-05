using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
    using EazyEngine.Tools;
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
    public enum ExtraType
    {
        Random,
        Queue,
    }
    public class ItemPackage : BaseItemGame, IExtractItem
    {

        public bool alwayExtras = false;
        public ItemRateDropInfo[] items;
        public Vector2 randomQuantity = new Vector2(1, 1);
        public ExtraType extraType;
        public bool alwayExtra()
        {
            return alwayExtras;
        }
        protected BaseItemGameInstanced[] cacheExtra;
        public BaseItemGameInstanced[] ExtractHere(bool isNew = true)
        {
            if (isNew)
            {
                int pIndex = -1;
                if (ItemID == "BoxElite")
                {
                    pIndex = 1;
                }
                else if (ItemID == "BoxComon")
                {
                    pIndex = 0;
                }
                else if (ItemID == "BoxSupreme")
                {
                    pIndex = 2;
                }
                if (pIndex != -1)
                {
                    GameManager.Instance.Database.collectionInfo.addQuantityBoxOpen(1, pIndex);
                    GameManager.Instance.Database.collectionDailyInfo.addQuantityBoxOpen(1, pIndex);
                    EzEventManager.TriggerEvent(new MessageGamePlayEvent("MissionDirty"));
                }
            }
            int pFirstBox = PlayerPrefs.GetInt("FirstBoxReward", 0);
            int pSecondBox = PlayerPrefs.GetInt("SecondBox", 0);
            if (itemID == "BoxElite")
            {
                if (pFirstBox == 2 && isNew)
                {
                    PlayerPrefs.SetInt("FirstBoxReward", 3);
                    var pItemCoin = GameDatabase.Instance.getItem("Coin", CategoryItem.COMON);
                    var pItemCrystal = GameDatabase.Instance.getItem("Crystal", CategoryItem.COMON);
                    var pItemcraft = GameDatabase.Instance.getItem("CraftPlane2", CategoryItem.COMON);
                    List<BaseItemGameInstanced> pItemResultFake = new List<BaseItemGameInstanced>()
                {
                    new BaseItemGameInstanced(){item =pItemCoin,quantity= 5000 },
                    new BaseItemGameInstanced(){item =pItemCrystal,quantity= 50 },
                    new BaseItemGameInstanced(){item =pItemcraft,quantity= 10 }
                };
                    cacheExtra = pItemResultFake.ToArray();
                    return pItemResultFake.ToArray();


                }
                else if (pSecondBox == 1 && isNew)
                {
                    PlayerPrefs.SetInt("SecondBox", 2);
                    var pItemCoin = GameDatabase.Instance.getItem("Coin", CategoryItem.COMON);
                    var pItemCrystal = GameDatabase.Instance.getItem("Crystal", CategoryItem.COMON);
                    var pItemcraft = GameDatabase.Instance.getItem("CraftSpPlane3", CategoryItem.COMON);
                    List<BaseItemGameInstanced> pItemResultFake = new List<BaseItemGameInstanced>()
                {
                    new BaseItemGameInstanced(){item =pItemCoin,quantity= 5000 },
                    new BaseItemGameInstanced(){item =pItemCrystal,quantity= 50 },
                    new BaseItemGameInstanced(){item =pItemcraft,quantity= 10 }
                };
                    cacheExtra = pItemResultFake.ToArray();
                    return pItemResultFake.ToArray();
                }
            }
            if (!isNew && cacheExtra != null)
            {
                return cacheExtra;
            }
            List<BaseItemGameInstanced> pItemResult = new List<BaseItemGameInstanced>();
            int pCount = Random.Range((int)randomQuantity.x, (int)randomQuantity.y == 0 ? ((int)randomQuantity.y + 1) : (int)randomQuantity.y);
            if (extraType == ExtraType.Random)
            {
                do
                {
                    List<ItemRateDropInfo> pItems = new List<ItemRateDropInfo>();
                    pItems.AddRange(items);
                    int pRandom = Random.Range(0, 100);
                    int pCurrent = 0;
                    int indexBreak = 0;
                    while (true)
                    {
                        if (pCurrent >= 100 || pItems.Count == 0)
                        {
                            break;
                        }
                        int index = Random.Range(0, pItems.Count);
                        if (index >= pItems.Count)
                        {
                            continue;
                        }
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
            }
            else
            {
                int indexQueue = 0;
                for(int i = 0; i < pCount; ++i)
                {
                    int percent = Random.Range(0, 100);
                    if(percent < items[indexQueue].percent)
                    {
                        var pQuantity = Random.Range((int)items[indexQueue].quantity.x, (int)items[indexQueue].quantity.y);
                        if (!typeof(IExtractItem).IsAssignableFrom(items[indexQueue].item.GetType()) || !((IExtractItem)items[indexQueue].item).alwayExtra())
                        {
                            pItemResult.Add(new BaseItemGameInstanced()
                            {
                                item = items[indexQueue].item,
                                quantity = pQuantity
                            });
                        }
                        else
                        {
                            pItemResult.AddRange(((IExtractItem)items[indexQueue].item).ExtractHere());
                        }
                 
                        indexQueue++;
                        if(indexQueue >= items.Length)
                        {
                            indexQueue = 0;
                        }
                    }
                }
            }
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
