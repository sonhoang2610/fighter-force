using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
#if UNITY_EDITOR
    using UnityEditor;
    public class TableAbilitiesInfoCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/TableAbilities")]
        public static void CreateMyAsset()
        {
            TableAbilities asset = ScriptableObject.CreateInstance<TableAbilities>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/TableAbilities.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif

    [System.Serializable]
    public class TableAbilities : BaseTableGeneric<AbilityInfo>
    {
    }
}

