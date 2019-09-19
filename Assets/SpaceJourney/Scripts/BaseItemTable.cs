using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
#if UNITY_EDITOR
    using UnityEditor;
    public class BaseTableInfoCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/BaseItemTable")]
        public static void CreateMyAsset()
        {
            BaseItemTable asset = ScriptableObject.CreateInstance<BaseItemTable>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/BaseItemTable.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif

    [System.Serializable]
    public class BaseItemTable : BaseTableGeneric<BaseItemGame>
    {
    }
}



