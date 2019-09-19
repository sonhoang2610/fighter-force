using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
#if UNITY_EDITOR
    using UnityEditor;
    public class MissionItemCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/MissionItem")]
        public static void CreateMyAsset()
        {
            MissionItem asset = ScriptableObject.CreateInstance<MissionItem>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            AssetDatabase.CreateAsset(asset, path + "/MissionItem.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
#endif

    [System.Serializable]
    public class MissionItem : BaseItemGame
    {
        public FlowCanvas.FlowScript checkComplete;
    }

    [System.Serializable]
    public class MissionItemInstanced
    {
        public MissionItem mission;
        public float process;
        public RewardInfo[] rewards;
    }
    [System.Serializable]
    public class RewardInfo
    {
        public BaseItemGame item;
        public int quantity;
    }
}