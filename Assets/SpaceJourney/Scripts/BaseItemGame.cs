using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using System;

public enum CategoryItem { 
    PLANE,
    SP_PLANE,
    SP_ITEM,
    SKILL,
    ABILITY,
    CRAFT,
    COIN,
    CRYSTAL,
    ENERGY,
    COMON = CRAFT | COIN | CRYSTAL| ENERGY,
    MISSION,
    WATCH,
    IAP
}
namespace EazyEngine.Space
{
    using Sirenix.Serialization;
#if UNITY_EDITOR
    using UnityEditor;
    public class ItemBaseCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/ItemBase")]
        public static void CreateMyAsset()
        {
            BaseItemGame asset = ScriptableObject.CreateInstance<BaseItemGame>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/BaseItemGame.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
    
    public class TableItemBaseCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/TablePlane")]
        public static void CreateMyAsset()
        {
            TablePlane asset = ScriptableObject.CreateInstance<TablePlane>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/TablePlane.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif
    [System.Serializable]
    public class LimitedModule
    {
        public int limitInInventory = 20;
        public bool isRestoreAble;
        [ShowIf("isRestoreAble")]
        public float timeToRestore = 1200;
        [ShowIf("isRestoreAble")]
        public int quantityRestore = 1;
    }
    [System.Serializable]
    public class BaseItemGame : EzScriptTableObject
    {
        public string itemID;
        public I2String displayNameItem;
        public I2String descriptionItem;
        public CategoryItem categoryItem;
        public Sprite iconShop;
        public Sprite iconShopDisable;
        public Sprite iconGame;
        public Sprite iconGameDisable;
        public GameObject model;
        [System.NonSerialized,OdinSerialize]
        public LimitedModule limitModule;
        public virtual string Desc
        {
            get
            {
                return descriptionItem.Value;
            }
        }

        public Sprite CateGoryIcon
        {
            get
            {
                var pSprite = GameDatabase.Instance.getIconCateGory(categoryItem);
                if (!pSprite)
                {
                    pSprite = iconShop;
                }
                if (!pSprite)
                {
                    pSprite = iconGame;
                }
                return pSprite;
            }
        }

        public string ItemID { get {
                if(itemID == "BoxElitePack1")
                {
                    return "BoxElitePack01";
                }
                if (itemID == "BoxElitePack2")
                {
                    return "BoxElitePack02";
                }
                return itemID;
            } set => itemID = value; }
    }

}