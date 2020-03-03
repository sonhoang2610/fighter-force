using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    using EazyEngine.Space.UI;
    using Sirenix.OdinInspector;
#if UNITY_EDITOR
    using UnityEditor;

    public class AssetbundleManagerCreator
    {
        [MenuItem("Assets/Create/EazyEngine/Space/AssetbundleManager")]
        public static void CreateAsset()
        {
            CreatetorScriptTableObjectDTB.CreateMyAsset<AssetbundleManager>();
        }
    }
#endif
    [System.Serializable]
    public class ModuleAssetInfo
    {
        public string nameModule;
        public string nameDisplay; 
        public float sizeFile;
        public bool disableDownload = false;

        private float percent;
        public float Percent { get => percent; set => percent = value; }
    }
    [System.Serializable]
    public class DashBoardBundle
    {
        public string version = "1.0";
        public ModuleAssetInfo[] modules;
    }
    public class AssetbundleManager : ScriptableObject
    {
        public DashBoardBundle currentModule;
    }

}
