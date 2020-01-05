using EazyEngine.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EazyEngine.Space
{
    [System.Serializable]
    public class AssetSelectorRef
    {
#if UNITY_EDITOR
        [AssetSelector]
        [OnValueChanged("refresh")]
        [SerializeField]
        private UnityEngine.Object asset;
        public Object Asset { get => asset; set => asset = value; }
#else
             public Object Asset { get; set; }
#endif

#if UNITY_EDITOR
        public void refresh()
        {
            bool pUpdated = false;
            for(int i = 0; i < AddressableDatabase.Instance.groups.Length; ++i)
            {
                var pAsset = System.Array.Find(AddressableDatabase.Instance.groups[i].elements, x => x.asset == Asset);
                if(pAsset != null)
                {
                    runtimeKey = pAsset.runtimeKey;
                    pUpdated = true;
                }
            }
            if (!pUpdated)
            {
                Debug.LogError("something wrong" + Asset.name);
            }
        }
#endif
        [Sirenix.OdinInspector.ReadOnly]
        public string runtimeKey;

   
    }
    [System.Serializable]
    public class AddressableElement
    {
#if UNITY_EDITOR
        [AssetSelector]
        public UnityEngine.Object asset;
#endif
        public string runtimeKey;
        public string pathLocal;
        public string pathRemote;
    }
    [System.Serializable]
    public class AddressableGroup
    {
        public AddressableElement[] elements;
    }
    [CreateAssetMenu(menuName = "EazyEngine/Space/AddressableDatabase")]
    public class AddressableDatabase : EzScriptTableObject
    {
        public static AddressableDatabase dataBase;
        public static AddressableDatabase Instance
        {
            get
            {
                if (dataBase != null)
                {
                    return dataBase;
                }
                dataBase = LoadAssets.loadAsset<AddressableDatabase>("AddressableDatabase", "Variants/Database/");
                return dataBase;
            }
        }
        public AddressableGroup[] groups;
        protected Dictionary<string, string> cachePath = new Dictionary<string, string>();
        public string loadPath(string pKey)
        {
            if (cachePath.ContainsKey(pKey))
            {
                return cachePath[pKey]; 
            }
            for (int i = 0; i < groups.Length; ++i)
            {
                var pAddress = System.Array.Find(groups[i].elements, x => x.runtimeKey == pKey);
                if(pAddress != null)
                {
                   int index = pAddress.pathLocal.IndexOf("Resources/");
                    var path = pAddress.pathLocal.Substring(index + 10, pAddress.pathLocal.Length - index - 10).Split('.')[0];
                    cachePath.Add(pKey, path);
                    return path;
                }
            }
            return "";
        }
        
#if UNITY_EDITOR
        public int indexGroup;
        [AssetSelector]
        public UnityEngine.Object[] assets;
        [Button("Gernerate")]
        public void generate()
        {
            for (int i = 0; i < assets.Length; ++i)
            {
                var pAdress = System.Array.Find(groups[indexGroup].elements, x => x.asset == assets[i]);
                if (pAdress == null)
                {
                    System.Array.Resize(ref groups[indexGroup].elements, groups[indexGroup].elements.Length + 1);
                    groups[indexGroup].elements[groups[indexGroup].elements.Length - 1] = new AddressableElement() { asset = assets[i] };
                }
            }
            generateKey();
        }
        [Button("Gernerate Key")]
        public void generateKey()
        {
            for (int i = 0; i < groups.Length; ++i)
            {
                for (int j = 0; j < groups[i].elements.Length; ++j)
                {
                    string pGuiID = "";
                    long pLocalID = 0;
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(groups[i].elements[j].asset, out pGuiID, out pLocalID))
                    {
                        
                        groups[i].elements[j].runtimeKey = pGuiID;
                        string path = AssetDatabase.GUIDToAssetPath(pGuiID);
                        if (!string.IsNullOrEmpty(path) && !path.Contains("Resources"))
                        {
                            path = "";
                        }
                        groups[i].elements[j].pathLocal = path;
                    }
                }
            }
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
