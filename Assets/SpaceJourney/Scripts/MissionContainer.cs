using EazyEngine.Tools;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
    [System.Serializable]
    public class MissionModule
    {
        public FlowCanvas.FlowScript condition;
        [OdinSerialize]
        public Dictionary<string, object> VariableDict = new Dictionary<string, object>();
        public MissionItemInstanced[] missions;
    }
    [CreateAssetMenu( menuName = "EazyEngine/Space/MissionDatabase", order =0)]
    public class MissionDatabase : EzScriptTableObject
    {
        [OdinSerialize,System.NonSerialized]
        public MissionModule[] dailyMissions;
        public MissionItemInstanced[] achievements;
    }
}
