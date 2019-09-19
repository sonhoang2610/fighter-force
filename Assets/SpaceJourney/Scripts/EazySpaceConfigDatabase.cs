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
        
    }
}