using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public static class SpaceJourneySupport
    {
        public static LevelInfo CloneData(this LevelInfo pLevel)
        {
            var pInfo = new LevelInfo() {
                Missions = new List<MissionItemInstanced>(),
                score = pLevel.score,                
            };
            pInfo.Missions.AddRange(pLevel.Missions.ToArray());
            pInfo.InputConfig = pLevel.InputConfig;
            return pInfo;
        }
        public static LevelInfoInstance CloneData(this LevelInfoInstance pLevel)
        {
            var pInfo = new LevelInfoInstance() {
                hard = pLevel.hard,
                level = pLevel.level,
                infos = pLevel.infos.CloneData()
            };
            return pInfo;
        }
        
    }
}
