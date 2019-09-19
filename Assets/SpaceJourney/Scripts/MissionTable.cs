using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
#if UNITY_EDITOR
    using UnityEditor;
    public class MissionTableCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/MissionTable")]
        public static void CreateMyAsset()
        {
            MissionTable asset = ScriptableObject.CreateInstance<MissionTable>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/MissionTable.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif

    [System.Serializable]
    public class MissionTable : BaseTableGeneric<MissionItem>
    {
    }
}
