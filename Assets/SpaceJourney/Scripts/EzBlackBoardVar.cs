using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using NodeCanvas.Framework;
#if UNITY_EDITOR
using UnityEditor;
using EazyEngine.Space;
public class EzBlackBoardCreator
{
    [MenuItem("Assets/Create/EazyEngine/BlackBoard")]
    public static void CreateMyAssetBlackBoard()
    {
        EzBlackBoardVar asset = ScriptableObject.CreateInstance<EzBlackBoardVar>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        AssetDatabase.CreateAsset(asset, path + "/EzBlackBoard.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif
namespace EazyEngine.Space
{
    public class EzBlackBoardVar : SerializedScriptableObject
    {
        public SkillInfoInstance abc;

    }
}
