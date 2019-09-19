using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
#if UNITY_EDITOR
    using UnityEditor;
    public class TableSkillInfoCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/TableSkill")]
        public static void CreateMyAsset()
        {
            TableSkill asset = ScriptableObject.CreateInstance<TableSkill>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/TableSkill.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif

    [System.Serializable]
    public class TableSkill : BaseTableGeneric<SkillInfo>
    {
    }
}




