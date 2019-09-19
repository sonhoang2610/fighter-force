using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
    using EazyEngine.Tools;
#if UNITY_EDITOR
    using UnityEditor;
    public class AbilityInfoCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/Ability")]
        public static void CreateMyAsset()
        {
            AbilityInfo asset = ScriptableObject.CreateInstance<AbilityInfo>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/Ability.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif

    public class AbilityInfo : BaseItemGame
    {
    }
}
