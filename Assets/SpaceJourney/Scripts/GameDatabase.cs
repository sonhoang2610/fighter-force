using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
    using EazyEngine.Space.UI;

#if UNITY_EDITOR
    using UnityEditor;
    public class GameDatabaseInfoCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/GameDatabase")]
        public static void CreateMyAsset()
        {
            GameDatabase asset = ScriptableObject.CreateInstance<GameDatabase>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/GameDatabase.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif
    [System.Serializable]
    public class ItemWheelInfoConfig
    {
        public BaseItemGame item;
        public float percent = 0;
        public int quantityStart = 1;
        public AlgrothimPriceItem algrothimTypeUpgrade;
        [ShowIf("algrothimTypeUpgrade", AlgrothimPriceItem.StepConstrainEachLevel)]
        public int priceStep;
        [ShowIf("algrothimTypeUpgrade", AlgrothimPriceItem.ConstrainDefineFromeSpecifiedLevel)]
        public SpecifiedLevelStepUnit[] priceDefines = new SpecifiedLevelStepUnit[] { new SpecifiedLevelStepUnit() { levelRequire = 0, unit = 0 } };

        protected int currentLevel = 0;
        public int CurrentLevel
        {
            set
            {
                currentLevel = value;
            }
        }

        public int Quantity
        {
            get
            {
                if (algrothimTypeUpgrade == AlgrothimPriceItem.StepConstrainEachLevel)
                {
                    return quantityStart + currentLevel * priceStep;
                }
                for (int i = priceDefines.Length - 2; i >= 0; --i)
                {
                    if (currentLevel > priceDefines[i].levelRequire)
                    {
                        return priceDefines[i + 1].unit;
                    }
                }
                return quantityStart;
            }
        }
    }
    [System.Serializable]
    public class CateGoryConfig
    {
        public CategoryItem category;
        public Sprite icon;
    }
    [System.Serializable]
    public class LevelConfigScene
    {
        public int level;
        public string scene;
    }
    [System.Serializable]
    public class MissionConfig
    {
        public int level;
        [ListDrawerSettings(AddCopiesLastElement = true)]
        public List<MissionArray> missions = new List<MissionArray>();
    }
    [System.Serializable]
    public class MissionArray
    {
        [ListDrawerSettings(AddCopiesLastElement =true)]
        public MissionItemInstanced[] missions;
    }

    [System.Serializable]
    public class LevelMoneyDropInfo
    {
        public Vector2 totalMoney;
        public Vector2 totalCoin;
        [LabelText("So sao nhan du 100% tien")]
        public int requireStar;
    }
    [System.Serializable]
    public class LevelMoneyDropInfos : List<LevelMoneyDropInfo>
    {
    }

    [System.Serializable]
    public struct MapInfo
    {
        public int level;
        public I2String mapName;
        public I2String ZoneName;
    }



    [System.Serializable]
    public class GameDatabase : EzScriptTableObject
    {
        [HideLabel]
        [InlineEditor]
        [FoldoutGroup("Table")]
        public BaseTable[] tables;

        [SerializeField]
        [FoldoutGroup("Category")]
        private CateGoryConfig[] cateGoryConfig;


        [SerializeField]
        [FoldoutGroup("Level")]
        private LevelConfigScene[] levelConfigScene;
        [SerializeField]
        [FoldoutGroup("Level")]
      //  [ListDrawerSettings(AddCopiesLastElement = true)]
        private MapInfo[] mapInfos;
        [SerializeField]
        [FoldoutGroup("Wheel")]
        public float timeWheelFree = 28800;

        [SerializeField]
        [FoldoutGroup("Wheel")]
        private int[] wheelLevelExp = new int[] { 0,10,20,30,40 };

        [SerializeField]
        [FoldoutGroup("Wheel")]
        private ItemWheelInfoConfig[] itemWheelConfig = new ItemWheelInfoConfig[] {
            new ItemWheelInfoConfig(){percent = 5},
            new ItemWheelInfoConfig(){percent = 5},
            new ItemWheelInfoConfig(){percent = 2.5f},
            new ItemWheelInfoConfig(){percent = 2.5f},
            new ItemWheelInfoConfig(){percent = 5},
            new ItemWheelInfoConfig(){percent = 10},
            new ItemWheelInfoConfig(){percent = 15},
            new ItemWheelInfoConfig(){percent = 10 },
            new ItemWheelInfoConfig(){percent = 10},
            new ItemWheelInfoConfig(){percent =15},
            new ItemWheelInfoConfig(){percent = 10},
            new ItemWheelInfoConfig(){percent = 10}
        };

        [SerializeField]
        [FoldoutGroup("Wheel")]
        private int[] configWheelGold = new int[] {
           1000,2000,3000,4000,5000,6000,7000,8000,9000,10000
        };
        [FoldoutGroup("Wheel")]
        public ItemWheelInfoConfig[] ItemWheelConfig
        {
            get
            {
                return itemWheelConfig;
            }
        }

        [FoldoutGroup("Mission", false)]
        [ListDrawerSettings(AddCopiesLastElement =true)]
        public MissionConfig[] missionConfig;

        [FoldoutGroup("Drop Money States", false)]
        [ListDrawerSettings(ShowIndexLabels = true,AddCopiesLastElement = true)]
        public LevelMoneyDropInfos[] dropMonyeconfig;
        [InlineEditor]
        public GiftOnlineDataBase databaseGiftOnline;
        [InlineEditor]
        public DailyGiftDataBase databaseDailyGift;
        [InlineEditor]
        public IAPSetting IAPSetting;

        public MissionItemInstanced[] getMissionForLevel(int pLevel,int hard)
        {
            for(int i = 0; i < missionConfig.Length; ++i)
            {
                if(missionConfig[i].level == pLevel )
                {
                    return missionConfig[i].missions[hard].missions;
                }
            }
            return new MissionItemInstanced[0];
        }

        public Sprite getIconCateGory(CategoryItem pCate)
        {
            for(int i = 0; i < cateGoryConfig.Length; ++i)
            {
                if(cateGoryConfig[i].category == pCate)
                {
                    return cateGoryConfig[i].icon;
                }
            }
            return null;
        }

        public  string[] findItemsNameTable(CategoryItem pCategoryItem)
        {
            for(int i = 0; i < tables.Length; ++i)
            {
                if(tables[i].cateGory == pCategoryItem)
                {
                    return tables[i].getAllIDItems();
                }
            }
            return new string[0];
        }

        public BaseItemGame getItem(string pID,CategoryItem pCateGory)
        {
            for (int i = 0; i < tables.Length; ++i)
            {
                if (tables[i].cateGory == pCateGory)
                {
                    var pItem = tables[i].getItem(pID);
                    if(pItem != null)
                    {
                        return pItem;
                    }
                }
            }
            return null;
        }

        public BaseItemGame getItem(string pID)
        {
            for (int i = 0; i < tables.Length; ++i)
            {
                    var pItem = tables[i].getItem(pID);
                    if (pItem != null)
                    {
                        return pItem;
                    }
                
            }
            return null;
        }

        public BaseItemGame[] getAllItem( CategoryItem pCateGory)
        {
            for (int i = 0; i < tables.Length; ++i)
            {
                if (tables[i].cateGory == pCateGory)
                {
                    return tables[i].getAllItem();
                }
            }
            return null;
        }

        public MapInfo getMapInfo(int index)
        {
            for(int i = 0; i < mapInfos.Length; ++i)
            {
                if(mapInfos[i].level == index)
                {
                    return mapInfos[i];
                }
            }
            return mapInfos[0];
        }
        public static GameDatabase dataBase;
        public static GameDatabase Instance
        {
            get
            {
                if(dataBase!= null)
                {
                    return dataBase;
                }
                dataBase = LoadAssets.loadAsset<GameDatabase>("GameDatabase", "Variants/Database/");
                return dataBase;
            }
        }

        public LevelConfigScene[] LevelConfigScene { get => levelConfigScene; set => levelConfigScene = value; }
        public int[] ConfigWheelGold { get => configWheelGold; set => configWheelGold = value; }
        public int[] WheelLevelExp { get => wheelLevelExp; set => wheelLevelExp = value; }

        public string LevelScene(int index)
        {
            string pScene = "Zone1";
           for(int i =0; i < levelConfigScene.Length; ++i)
            {
                if(levelConfigScene[i].level <= index)
                {
                    pScene = levelConfigScene[i].scene;
               
                }
                else
                {
                    break;
                }
            }
            return pScene;
        }
    }

}
