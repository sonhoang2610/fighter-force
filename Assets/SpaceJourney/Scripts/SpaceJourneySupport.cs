using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public static class SpaceJourneySupport
    {
        public static int CountPassMode(int pMode)
        {
            var container = GameManager.Instance.container;
            int pCount = 0;
            for (int i = 0; i < container.levels.Count; ++i)
            {
                if (container.levels[i].isPassed && container.levels[i].hard == pMode)
                {
                    pCount++;
                }
            }
            return pCount;
        }
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
