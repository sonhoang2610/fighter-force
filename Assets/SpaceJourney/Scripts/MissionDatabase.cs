using EazyEngine.Tools;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    [CreateAssetMenu(menuName = "EazyEngine/Space/MissionDatabase", order = 0)]
    public class MissionDatabase : EzScriptTableObject
    {
        public static MissionDatabase dataBase;
        public static MissionDatabase Instance
        {
            get
            {
                if (dataBase != null)
                {
                    return dataBase;
                }
                dataBase = LoadAssets.loadAsset<MissionDatabase>("MissionDatabase", "Variants/Database/");
                return dataBase;
            }
        }
        [OdinSerialize, System.NonSerialized]
        public MissionModule[] dailyMissions;
        [OdinSerialize, System.NonSerialized]
        public MissionModule achievements;
    }
}