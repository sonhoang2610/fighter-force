using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using EazyEngine.Space;
#endif
namespace EazyEngine.Space
{
#if UNITY_EDITOR
    using UnityEditor;
    public class TableItemInfoCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/TableSupportItem")]
        public static void CreateMyAsset()
        {
            TableSupportItem asset = ScriptableObject.CreateInstance<TableSupportItem>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/TableSupportItem.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif


    [System.Serializable]
    public class TableSupportItem : BaseTableGeneric<ItemGame>
    {
    }
}
