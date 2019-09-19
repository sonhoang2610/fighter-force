using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
#if UNITY_EDITOR
    using UnityEditor;
    public class TablePlaneInfoCreator
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
    public class TablePlane : BaseTableGeneric<PlaneInfo>
    {
    }
}



