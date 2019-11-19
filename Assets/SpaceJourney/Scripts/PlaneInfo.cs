using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using Sirenix.OdinInspector;
using EazyEngine.Tools;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor;

public class PlaneInfoCreator
{
    [MenuItem("Assets/Create/EazyEngine/Space/Plane")]
    public static void CreateMyAsset()
    {
        PlaneInfo asset = ScriptableObject.CreateInstance<PlaneInfo>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        AssetDatabase.CreateAsset(asset, path + "/Plane.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif

namespace EazyEngine.Space {
    [System.Serializable]
    public struct RankConfig
    {
        public int levelUpRank;
    }
    [System.Serializable]
    public class RequireCondition
    {
        public BaseItemGame craftItem;
        public int quantityRequire;
    }
    [System.Serializable]
    public class PlaneInfoConfig
    {
        [InlineEditor]
        public PlaneInfo info;
        [System.NonSerialized]
        protected int limitUpgrade;
        public int LimitUpgrade
        {
            set
            {
                limitUpgrade = value;
            }
            get
            {
                return Mathf.Min(info.limitUpgrade, limitUpgrade);
            }
        }

        [ShowInInspector]
        public Dictionary<string, int> upgradeSkill = new Dictionary<string, int>();
        [ShowInInspector]
        public Dictionary<string, int[]> upgradeExtraAbility = new Dictionary<string, int[]>();

        public static PlaneInfoConfig CloneDefault(PlaneInfo pInfo,int pLevel)
        {
             Dictionary<string, int> pUpgradeSkill = new Dictionary<string, int>();
            for(int i = 0; i < pInfo.skills.Count; ++i)
            {
                pUpgradeSkill.Add(pInfo.skills[i].Info.ItemID, 1);
            }
            var pInfoPlane  = new PlaneInfoConfig()
            {
                info = pInfo,
                limitUpgrade = 80,
                currentLevel = pLevel,
                upgradeSkill = pUpgradeSkill
            };
            pInfoPlane.ExtraInfo();
            return pInfoPlane;
        }

        public int getNextStepAbilityUpgrade(string pID)
        {
            for(int i = 0; i < info.currentAbility.Count; ++i)
            {
                if(info.currentAbility[i]._ability.ItemID == pID)
                {
                   return info.currentAbility[i].NextUnit - info.currentAbility[i].CurrentUnit;
                }
            }
            return 0;
        }
        [EzSerializeField]
        public int currentLevel = 0;

        public bool isMax
        {
            get
            {
                int index = info.configRank.Length;
                if(CurrentLevel >= info.configRank[index - 1].levelUpRank)
                {
                    return true;
                }
                while(index > 0 && CurrentLevel < info.configRank[index-1].levelUpRank)
                {
                    index--;
                }
                if(index == 0)
                {
                    return false;
                }
                if(CurrentLevel == info.configRank[index-1].levelUpRank)
                {
                    return true;
                }
                return false;
            }
        }

        public int Rank
        {
            get
            {
                int index = info.configRank.Length - 1;
                while(index > 0 && CurrentLevel <= info.configRank[index].levelUpRank)
                {
                    index--;
                }
                return index;
            }
        }
        public void ExtraInfo()
        {
	        info.setLevel(CurrentLevel);

            for (int i = 0; i < info.skills.Count; ++i)
            {

                info.skills[i].CurrentLevelSkill = upgradeSkill.ContainsKey(info.skills[i].info.ItemID)
                    ? upgradeSkill[info.skills[i].info.ItemID]
                    : (info.skills[i].isEnabled ? 1 : 0);
            }

            for (int i = 0; i < info.currentAbility.Count; ++i)
            {
                info.currentAbility[i].ExtraDamage = 0;
            }
            for (int i = 0; i < info.currentAbility.Count; ++i)
            {
                if(upgradeExtraAbility== null)
                {
                    upgradeExtraAbility = new Dictionary<string, int[]>();
                }
                if (!upgradeExtraAbility.ContainsKey(info.currentAbility[i]._ability.ItemID)) continue;
                int pExtra = 0;
                for(int j  = 0; j < upgradeExtraAbility[info.currentAbility[i]._ability.ItemID].Length; ++j)
                {
                    pExtra += (int)(((info.currentAbility[i].getUnitAtLevel(upgradeExtraAbility[info.currentAbility[i]._ability.ItemID][j]+1) - info.currentAbility[i].getUnitAtLevel(upgradeExtraAbility[info.currentAbility[i]._ability.ItemID][j])))*((float)(info.upgradeIncreaseRate.getUnit(upgradeExtraAbility[info.currentAbility[i]._ability.ItemID][j]))-100)/100.0f);
                }
                info.currentAbility[i].ExtraDamage = upgradeExtraAbility.ContainsKey(info.currentAbility[i]._ability.ItemID) ? pExtra : 0;
            }
        }
        public PlaneInfo Info {
            get {
                info.setLevel(CurrentLevel);
                for (int i = 0; i < info.skills.Count; ++i)
                {
                    info.skills[i].CurrentLevelSkill = upgradeSkill.ContainsKey(info.skills[i].info.ItemID) ? upgradeSkill[info.skills[i].info.ItemID] : (info.skills[i].isEnabled ? 1:0) ;
                }
                return info;
            }
            set => info = value; }

        public Dictionary<string, int> UpgradeSkill { get {
                if(upgradeSkill == null)
                {
                    upgradeSkill = new Dictionary<string, int>();
                }
                return upgradeSkill;
            } set => upgradeSkill = value; }
     

        public int CurrentLevel { get => currentLevel; set {
                for (int i = 0; i < info.skills.Count; ++i)
                {
                    if (upgradeSkill.ContainsKey(info.skills[i].Info.ItemID))
                    {
                        if (upgradeSkill[info.skills[i].Info.ItemID] == 0)
                        {
                            if (info.skills[i].requireLevelUnlock <= value)
                            {
                                upgradeSkill[info.skills[i].Info.ItemID] = 1;
                            }
                        }
                    }
                    else
                    {
                        if (info.skills[i].requireLevelUnlock <= value)
                        {
                            upgradeSkill.Add(info.skills[i].Info.ItemID, 1);
                        }
                    }
                }
                currentLevel = value;
                info.setLevel(CurrentLevel);
            }
        }

        public string[] planes()
        {
            return LoadAssets.loadAsset<GameDatabase>("GameDatabase", "Variants/Database/").findItemsNameTable(CategoryItem.PLANE);
        }
    }
    [System.Serializable]
    public class SupportPlaneInfoConfig : PlaneInfoConfig
    {
        public static SupportPlaneInfoConfig CloneDefaultSp(PlaneInfo pInfo)
        {
            Dictionary<string, int> pUpgradeSkill = new Dictionary<string, int>();
            for (int i = 0; i < pInfo.skills.Count; ++i)
            {
                pUpgradeSkill.Add(pInfo.skills[i].Info.ItemID, 1);
            }
            var pInfoPlane = new SupportPlaneInfoConfig()
            {
                info = pInfo,
                currentLevel = 20,
                upgradeSkill = pUpgradeSkill
            };
            pInfoPlane.ExtraInfo();
            return pInfoPlane;
        }
    }
    [System.Serializable]
    public struct EzBool
    {
        public bool value;
    }
    [System.Serializable]
    public class PlaneInfo : BaseItemGame
    {

        public Character modelPlane;
        public int limitUpgrade = 80;
        public int RankPlane = 0;
        [System.NonSerialized, OdinSerialize]
        public List<SkillInfoInstance> skills = new List<SkillInfoInstance>();
        public List<AbilityConfig> currentAbility = new List<AbilityConfig>();
        public RequireCondition conditionUnlock;
        public RankConfig[] configRank = new RankConfig[] {  new RankConfig() { levelUpRank = 20 }, new RankConfig() { levelUpRank = 40 } , new RankConfig() { levelUpRank = 60 } };
        [FoldoutGroup("Upgrade")]
#if UNITY_EDITOR
        [InfoBox("Config ti le bao kich")]
#endif
        public UnitDefineLevel upgradeRateCriticalConfig;
        [FoldoutGroup("Upgrade")]
#if UNITY_EDITOR
        [InfoBox("Config ti le tang bao nhieu moi lan bao kich tinh theo 100%( 1.5 lan = 150)")]
#endif
        public UnitDefineLevel upgradeIncreaseRate;
        public void setLevel(int pLevel)
	    {
            upgradeRateCriticalConfig.setLevel(pLevel);
            upgradeIncreaseRate.setLevel(pLevel);
            for (int i = 0; i < currentAbility.Count; ++i)
            {
                currentAbility[i].CurrentLevel = pLevel;
            }
            for (int i = 0; i < skills.Count; ++i)
            {
                skills[i].CurrentLevelPlaneOwner = pLevel;
            }
        }
    }
}