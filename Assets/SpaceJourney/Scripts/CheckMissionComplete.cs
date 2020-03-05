using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space.UI
{
    public class CheckMissionComplete : MonoBehaviour,EzEventListener<MissionEvent>
    {
        public bool moduleDaily;
        public bool moduleAchievement;
        public GameObject noti;
        public void OnEzEvent(MissionEvent eventType)
        {
            checkSelf();
        }

        public void checkSelf()
        {
            bool showNoti = false;
            if (moduleDaily)
            {
                var pAllMissionDaily = GameManager.Instance.Database.missionContainerInfo.dailyMission.missionProcessing.Missions;
                foreach (var pMission in pAllMissionDaily)
                {
                    if (pMission.process >= 1 && !pMission.Claimed)
                    {
                        showNoti = true;
                        break;
                    }
                }
            }
            if (moduleAchievement)
            {
                var pAllMissionMain = GameManager.Instance.Database.missionContainerInfo.mainMission.Missions;
                foreach (var pMission in pAllMissionMain)
                {
                    if (pMission.process >= 1 && !pMission.Claimed)
                    {
                        showNoti = true;
                        break;
                    }
                }
            }
            noti.gameObject.SetActive(false);
            if (showNoti)
            {
                noti.gameObject.SetActive(true);
            }
        }
        private void OnEnable()
        {
            EzEventManager.AddListener(this);
            checkSelf();
        }
        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
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
