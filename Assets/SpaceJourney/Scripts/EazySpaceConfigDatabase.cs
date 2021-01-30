using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor;
public class CharacterConfig
{
    [MenuItem("Assets/Create/EazyEngine/Space/ConfigEnemies")]
    public static void CreateMyAsset()
    {
        EazySpaceConfigDatabase asset = ScriptableObject.CreateInstance<EazySpaceConfigDatabase>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        AssetDatabase.CreateAsset(asset, path + "/EazySpaceConfigDatabase.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif
namespace EazyEngine.Space
{

    public class EazySpaceConfigDatabase : EzScriptTableObject
    {
//        [Button("Export")]
//        public void ExportChar()
//        {
//#if UNITY_EDITOR
//            smallEnemiesObs = new CharacterInstancedConfigGroupSO[smallEnemies.Length];
//            for (int i = 0; i < smallEnemies.Length; ++i)
//            {
//                CharacterInstancedConfigGroupSO asset = ScriptableObject.CreateInstance<CharacterInstancedConfigGroupSO>();
//                string path = "Assets/SpaceJourney/Resources/Database/Enemy";
//                AssetDatabase.CreateAsset(asset, path + "/EnemySmall" + i + ".asset");
//                var context = new SerializationContext()
//                {
//                    StringReferenceResolver = new CharacterInstancedConfigGroup(),
//                };
//                var contextde = new DeserializationContext()
//                {
//                    StringReferenceResolver = new CharacterInstancedConfigGroup(),
//                };
//                asset.info = SerializationUtility.DeserializeValue<CharacterInstancedConfigGroup>(SerializationUtility.SerializeValue(smallEnemies[i], DataFormat.Binary, context), DataFormat.Binary, contextde);

//                smallEnemiesObs[i] = asset;
//                SerializedObject pObject = new SerializedObject(asset);
//                EditorUtility.SetDirty(asset);
//                pObject.ApplyModifiedProperties();
//            }

//            mediumEnemiesObs = new CharacterInstancedConfigGroupSO[mediumEnemies.Length];
//            for (int i = 0; i < mediumEnemies.Length; ++i)
//            {
//                CharacterInstancedConfigGroupSO asset = ScriptableObject.CreateInstance<CharacterInstancedConfigGroupSO>();
//                string path = "Assets/SpaceJourney/Resources/Database/Enemy";
//                AssetDatabase.CreateAsset(asset, path + "/EnemyMedium" + i + ".asset");
//                var context = new SerializationContext()
//                {
//                    StringReferenceResolver = new CharacterInstancedConfigGroup(),
//                };
//                var contextde = new DeserializationContext()
//                {
//                    StringReferenceResolver = new CharacterInstancedConfigGroup(),
//                };
//                asset.info = SerializationUtility.DeserializeValue<CharacterInstancedConfigGroup>(SerializationUtility.SerializeValue(mediumEnemies[i], DataFormat.Binary, context), DataFormat.Binary, contextde);

//                mediumEnemiesObs[i] = asset;
//                SerializedObject pObject = new SerializedObject(asset);
//                EditorUtility.SetDirty(asset);
//                pObject.ApplyModifiedProperties();
//            }
//            EditorUtility.SetDirty(this);
//            AssetDatabase.SaveAssets();
//#endif
//        }


        [TabGroup("Small")]
        [ListDrawerSettings(OnBeginListElementGUI = "small", OnEndListElementGUI =("smallend"))]
        public CharacterInstancedConfigGroupSO[] smallEnemiesObs;

        public void small(int index)
        {
#if UNITY_EDITOR
            EditorGUILayout.BeginHorizontal();
            //Sirenix.Utilities.Editor.SirenixEditorGUI.dra
            if(smallEnemiesObs[index] != null)
            smallEnemiesObs[index].info.preview = (Texture2D) EditorGUILayout.ObjectField("", smallEnemiesObs[index].info.preview,typeof(Texture2D),true,GUILayout.Width(80));
#endif
        }
        public void smallend(int index)
        {
#if UNITY_EDITOR
            EditorGUILayout.EndHorizontal();
#endif
        }
        [TabGroup("Medium")]
        [ListDrawerSettings(OnBeginListElementGUI = "medium", OnEndListElementGUI = ("mediumend"))]
        public CharacterInstancedConfigGroupSO[] mediumEnemiesObs;
        public void medium(int index)
        {
#if UNITY_EDITOR
            EditorGUILayout.BeginHorizontal();
            //Sirenix.Utilities.Editor.SirenixEditorGUI.dra
            if(mediumEnemiesObs[index] != null && mediumEnemiesObs[index].info.preview)
           EditorGUILayout.ObjectField("", mediumEnemiesObs[index].info.preview, typeof(Texture2D), true, GUILayout.Width(80));
#endif
        }
        public void mediumend(int index)
        {
#if UNITY_EDITOR
            EditorGUILayout.EndHorizontal();
#endif
        }
        [TabGroup("MiniBoss")]
        public CharacterInstancedConfigGroupSO[] miniBosssObs;
        [TabGroup("Boss")]
        public CharacterInstancedConfigGroupSO[] bosssObs;

#if UNITY_EDITOR
        protected void convertLegacyBullet(WeaponInstancedConfig weapon)
        {
            for(int g = 0; g < weapon.bullets.Length; ++g)
            {
                weapon.bullets[g].prefabRef.Asset = weapon.bullets[g].prefab;
                weapon.bullets[g].prefabRef.refresh();
            }
        }

        protected void addAddressBullet(WeaponInstancedConfig weapon ,List<UnityEngine.Object> allAsset)
        {
            for (int g = 0; g < weapon.bullets.Length; ++g)
            {
                allAsset.Add(weapon.bullets[g].prefab);
            }
        }
        [Button("Mark Address Asset")]
        public void addAddressableAsset()
        {
            List<CharacterInstancedConfigGroupSO> allAsset = new List<CharacterInstancedConfigGroupSO>();
            allAsset.AddRange(smallEnemiesObs);
            allAsset.AddRange(mediumEnemiesObs);
            allAsset.AddRange(miniBosssObs);
            allAsset.AddRange(bosssObs);
            List<UnityEngine.Object> assets = new List<Object>();
            for(int i = 0; i < allAsset.Count; ++i)
            {
                assets.Add(allAsset[i].info.easy.Target);
                var pListEasy = allAsset[i].info.easy.weapon;
                for (int j = 0; j < pListEasy.Length; ++j)
                {
                    assets.Add(pListEasy[j].target);
                    addAddressBullet(pListEasy[j], assets);
                }
                assets.Add(allAsset[i].info.medium.Target);
                var pListMedium = allAsset[i].info.medium.weapon;
                for (int j = 0; j < pListMedium.Length; ++j)
                {
                    assets.Add(pListMedium[j].target);
                    addAddressBullet(pListMedium[j], assets);
                }
                assets.Add(allAsset[i].info.hard.Target);
                var pListHard = allAsset[i].info.hard.weapon;
                for (int j = 0; j < pListHard.Length; ++j)
                {
                    assets.Add(pListHard[j].target);
                    addAddressBullet(pListHard[j], assets);
                }
                EditorUtility.SetDirty(allAsset[i]);
            }
            AddressableDatabase.Instance.assets = assets.ToArray();
            AddressableDatabase.Instance.indexGroup = 1;
            AddressableDatabase.Instance.generate();
        }

        [Button("Convert Legacy to Addressable")]
        public void convertAddressable()
        {
            List<CharacterInstancedConfigGroupSO> allAsset = new List<CharacterInstancedConfigGroupSO>();
            allAsset.AddRange(smallEnemiesObs);
            allAsset.AddRange(mediumEnemiesObs);
            allAsset.AddRange(miniBosssObs);
            allAsset.AddRange(bosssObs);
            for (int i = 0; i < allAsset.Count; ++i)
            {
                allAsset[i].info.easy.targetRef.Asset = allAsset[i].info.easy.Target;
                allAsset[i].info.easy.targetRef.refresh();
                allAsset[i].info.medium.targetRef.Asset = allAsset[i].info.medium.Target;
                allAsset[i].info.medium.targetRef.refresh();
                allAsset[i].info.hard.targetRef.Asset = allAsset[i].info.hard.Target;
                allAsset[i].info.hard.targetRef.refresh();
                var pListEasy = allAsset[i].info.easy.weapon;
                for (int j = 0; j < pListEasy.Length; ++j)
                {
                    pListEasy[j].targetRef.Asset = pListEasy[j].target;
                    pListEasy[j].targetRef.refresh();
                    convertLegacyBullet(pListEasy[j]);
                }
                var pListMedium = allAsset[i].info.medium.weapon;
                for (int j = 0; j < pListMedium.Length; ++j)
                {
                    pListMedium[j].targetRef.Asset = pListMedium[j].target;
                    pListMedium[j].targetRef.refresh();
                    convertLegacyBullet(pListMedium[j]);
                }
                var pListHard = allAsset[i].info.hard.weapon;
                for (int j = 0; j < pListHard.Length; ++j)
                {
                    pListHard[j].targetRef.Asset = pListHard[j].target;
                    pListHard[j].targetRef.refresh();
                    convertLegacyBullet(pListHard[j]);
                }
                EditorUtility.SetDirty(allAsset[i]);
            }

        }


        [Button("Assign Address1")]
        public void assignID()
        {
            List<CharacterInstancedConfigGroupSO> allAsset = new List<CharacterInstancedConfigGroupSO>();
            allAsset.AddRange(smallEnemiesObs);
            allAsset.AddRange(mediumEnemiesObs);
            allAsset.AddRange(miniBosssObs);
            allAsset.AddRange(bosssObs);
            for (int i = 0; i < allAsset.Count; ++i)
            {
                 var pAddress =  ((GameObject)allAsset[i].info.hard.targetRef.Asset).GetComponent<AdressableObject>();
                if (pAddress && pAddress.uniqueID != allAsset[i].info.hard.targetRef.runtimeKey)
                {
                    pAddress.uniqueID = allAsset[i].info.hard.targetRef.runtimeKey;
                    EditorUtility.SetDirty(pAddress);
                }

             
            }
        }
        [Button("Assign Address2")]
        public void assignID2()
        {
            List<CharacterInstancedConfigGroupSO> allAsset = new List<CharacterInstancedConfigGroupSO>();
            allAsset.AddRange(smallEnemiesObs);
            allAsset.AddRange(mediumEnemiesObs);
            allAsset.AddRange(miniBosssObs);
            allAsset.AddRange(bosssObs);
            for (int i = 0; i < allAsset.Count; ++i)
            {
                var pAddress1 = ((GameObject)allAsset[i].info.medium.targetRef.Asset).GetComponent<AdressableObject>();
                if (pAddress1 && pAddress1.uniqueID != allAsset[i].info.medium.targetRef.runtimeKey)
                {
                    pAddress1.uniqueID = allAsset[i].info.medium.targetRef.runtimeKey;
                    EditorUtility.SetDirty(pAddress1);
                }


            }
        }
        [Button("Assign Address3")]
        public void assignID3()
        {
            List<CharacterInstancedConfigGroupSO> allAsset = new List<CharacterInstancedConfigGroupSO>();
            allAsset.AddRange(smallEnemiesObs);
            allAsset.AddRange(mediumEnemiesObs);
            allAsset.AddRange(miniBosssObs);
            allAsset.AddRange(bosssObs);
            for (int i = 0; i < allAsset.Count; ++i)
            {
                var pAddress2 = ((GameObject)allAsset[i].info.easy.targetRef.Asset).GetComponent<AdressableObject>();
                if (pAddress2 && pAddress2.uniqueID != allAsset[i].info.easy.targetRef.runtimeKey)
                {
                    pAddress2.uniqueID = allAsset[i].info.easy.targetRef.runtimeKey;
                    EditorUtility.SetDirty(pAddress2);
                }

            }
        }
        [Button("Find issue")]
        public void detectError()
        {

            List<CharacterInstancedConfigGroupSO> allAsset = new List<CharacterInstancedConfigGroupSO>();
            allAsset.AddRange(smallEnemiesObs);
            allAsset.AddRange(mediumEnemiesObs);
            allAsset.AddRange(miniBosssObs);
            allAsset.AddRange(bosssObs);
            List<string> pIDs = new List<string>();
            List<AdressableObject> adresss = new List<AdressableObject>();
            for (int i = 0; i < allAsset.Count; ++i)
            {
                var pAddress2 = ((GameObject)allAsset[i].info.easy.targetRef.Asset).GetComponent<AdressableObject>();
                if (pAddress2)
                {
                    adresss.Add(pAddress2);
                }
                var pAddress1 = ((GameObject)allAsset[i].info.medium.targetRef.Asset).GetComponent<AdressableObject>();
                if (pAddress1)
                {
                    adresss.Add(pAddress1);
                }
                var pAddress = ((GameObject)allAsset[i].info.hard.targetRef.Asset).GetComponent<AdressableObject>();
                if (pAddress)
                {
                    adresss.Add(pAddress);
                }
            }
            for(int i = 0; i < adresss.Count; ++i)
            {
                if (!pIDs.Contains(adresss[i].uniqueID))
                {
                    pIDs.Add(adresss[i].uniqueID);
                }
                else
                {
                    Debug.LogError("something wrong" + adresss[i].name + " and " + adresss[pIDs.IndexOf(adresss[i].uniqueID)].name );
                }
            }
        }
#endif
    }
}