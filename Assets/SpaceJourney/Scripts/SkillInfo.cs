using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EazyEngine.Space;
using EazyEngine.Tools;
using EazyReflectionSupport;
using NodeCanvas.Framework;
using Sirenix.Serialization;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;

public class SkillInfoCreator
{
    [MenuItem("Assets/Create/EazyEngine/Space/Skill")]
    public static void CreateMyAsset()
    {
        SkillInfo asset = ScriptableObject.CreateInstance<SkillInfo>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        AssetDatabase.CreateAsset(asset, path + "/Skill.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif

namespace EazyEngine.Space
{
    [System.Serializable]
    public class SkillInfo : ItemGame
    {
        public bool blackBoardExtra;
        [ShowIf("blackBoardExtra")]
        [OdinSerialize]
        public Dictionary<string, object> VariableDict = new Dictionary<string, object>();
        private int currentLevel = 0;
        public override float CoolDownTime => coolDownTime.getUnit(CurrentLevel);
        public override float Duration
        {
            get
            {
                return duration.getUnit(CurrentLevel);
            }
        }


        public override string Desc {
            get {
                var pStr = base.Desc;
                var pStrs = pStr.Split(' ');
                if (!VariableDict.ContainsKey("Duration"))
                {
                    VariableDict.Add("Duration", Duration);
                }
                else
                {
                    VariableDict["Duration"] = Duration;
                }
                foreach (var pString in pStrs)
                {
                    if (pString.StartsWith("$"))
                    {
                        var pVar = pString.Remove(0,1);
                        if (VariableDict.ContainsKey(pVar) && VariableDict[pVar] != null)
                        {
                            pStr = pStr.Replace(pString, VariableDict[pVar].ToString());
                        }
                    }
                }
          
                return pStr;
            }
        }
      
        public int CurrentLevel { get => currentLevel;
            set {
                currentLevel = value;
                for(int i = 0; i < VariableDict.Count; ++i)
                {
                    if (VariableDict.Values.ElementAt(i) == null) continue;
                    if(typeof(ILevelSetter).IsAssignableFrom(VariableDict.Values.ElementAt(i).GetType()))
                    {
                        ((ILevelSetter)VariableDict.Values.ElementAt(i)).setLevel(value);
                    }
                }
            }
        }
    }
    public enum EzType
    {
        Float,
        String,
    }

    [System.Serializable]
    public class SkillInfoInstance
    {
        [InlineEditor]
        public SkillInfo info;
        [OdinSerialize]
        public Dictionary<string, object> skillBlackBoard = new Dictionary<string, object>();

        public int requireLevelUnlock;
        public int limitUpgrade = 1;

        protected int limitUpgradeFromOut = 999;
        

        protected int currentLevelPlane;

        public int CurrentLevelPlane
        {
            set
            {
                currentLevelPlane = value;
            }

            get
            {
                return currentLevelPlane;
            }
        }

        public bool isEnabled
        {
            get
            {
                return currentLevelPlane >= requireLevelUnlock;
            }
        }
        public int CurrentLevelSkill
        {
            set
            {
                Info.CurrentLevel = value;
            }
            get
            {
                return Info.CurrentLevel;
            }
        }
        public SkillInfo Info
        {
            get
            {
                //   for(int i = 0; i < )
                return info;
            }
            set => info = value;
        }

        public int LimitUpgrade { get { return Mathf.Min(limitUpgrade, limitUpgradeFromOut); }  set => limitUpgradeFromOut = value; }
    }
}