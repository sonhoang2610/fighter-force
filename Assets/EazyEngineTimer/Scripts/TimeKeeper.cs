using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;

namespace EazyEngine.Timer
{

    [ExecuteInEditMode]
    public class TimeKeeper : PersistentSingleton<TimeKeeper>
    {
        public List<TimeController> timerGroup = new List<TimeController>();

        public string[] getAllTimerName()
        {
            string[] pNames = new string[timerGroup.Count+1];
            pNames[0] = "Root";
            for (int i = 1; i < pNames.Length; ++i)
            {
                pNames[i] = timerGroup[i-1].nameGroup;
            }
            return pNames;
        }

        public void addTimerGroup(TimeController pController)
        {
            if (!timerGroup.Contains(pController))
            {
                timerGroup.Add(pController);
            }
        }

        public void removeTimerGroup(TimeController pController)
        {
            timerGroup.Remove(pController);
        }


        public int getTimeLineIndex(string pString)
        {
            for(int i = 0; i < timerGroup.Count; ++i)
            {
                if(timerGroup[i].nameGroup == pString)
                {
                    return i + 1;
                }
            }
            return 1;
        }
        //public float getDeltaTime(int pGroup)
        //{
        //    if (pGroup == 0 || pGroup > timerGroup.Count)
        //    {
        //        return Time.deltaTime;
        //    }
        //    return timerGroup[pGroup].getDeltaTime();
        //}
        public TimeController getTimer(string pGroup)
        {
            foreach(var pTimer in timerGroup)
            {
                if(pTimer.nameGroup == pGroup)
                {
                    return pTimer;
                }
            }
            return null;
        }

        public TimeController getTimeController(int pGroup)
        {
            pGroup--;
            if (pGroup < 0 || pGroup >= timerGroup.Count)
            {
                return null;
            }
            return timerGroup[pGroup];
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
