using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;

namespace EazyEngine.Tools
{
#if UNITY_EDITOR
    using UnityEditor;
    using EazyEngine.Space;
    public class PositionDatabaseCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/Position")]
        public static void CreateMyAsset()
        {
            PositionDatabase asset = ScriptableObject.CreateInstance<PositionDatabase>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/Positions.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif
    [System.Serializable]
    public struct ChooseMapInfoElement
    {
        public Vector3 pos;
        public bool isBoss;

    }
    public class PositionDatabase : EzScriptTableObject
    {
        public ChooseMapInfoElement[] pos;
    }
}
