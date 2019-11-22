using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using EazyEngine.Space;
public class ITemGameCreator
{
    [MenuItem("Assets/Create/EazyEngine/Space/Item")]
    public static void CreateMyAsset()
    {
        ItemGame asset = ScriptableObject.CreateInstance<ItemGame>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        AssetDatabase.CreateAsset(asset, path + "/ItemGame.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif
namespace EazyEngine.Space
{
    [System.Serializable]
    public class ItemGame : BaseItemGame
    {
        public bool isActive = false;
#if UNITY_EDITOR
        public string coolDownLabel
        {
            get
            {
                return "Cooldown Time " + CoolDownTime;
            }
        }

        public string durationLabel
        {
            get
            {
                return "Duration Time " + Duration;
            }
        }

        public string animationTimeLabel
        {
            get
            {
                return "Animation Time " + AnimationTime;
            }
        }
#endif
        [HideLabel]
        [FoldoutGroup("@coolDownLabel")]
        public UnitDefineLevel coolDownTime = new UnitDefineLevel(10);
        [HideLabel]
        [FoldoutGroup("@durationLabel")]
        public UnitDefineLevel duration = new UnitDefineLevel(5);
        [HideLabel]
        [FoldoutGroup("@animationTimeLabel")]
        public UnitDefineLevel animationTime = new UnitDefineLevel(0);
        public FlowCanvas.FlowScript controller;
        [HideIf("isActive")]
        public FlowCanvas.FlowScript condition;

        public virtual float CoolDownTime { get => coolDownTime.getUnit(0); }
        public virtual float Duration
        {
            get
            {
                return duration.getUnit(0);
            }
        }
        public virtual float AnimationTime
        {
            get
            {
                return animationTime.getUnit(0);
            }
        }
    }
}
