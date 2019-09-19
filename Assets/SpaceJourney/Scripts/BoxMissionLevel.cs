using System.Collections;
using System.Collections.Generic;
using EazyEngine.Tools;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class BoxMissionLevel : BaseBox<MissionLevelItem,MissionItemInstanced>, EzEventListener<UIMessEvent>
    {
        private void OnEnable()
        {
            refreshInfo();
            EzEventManager.AddListener(this);
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }
        public void OnEzEvent(UIMessEvent eventType)
        {
            if (eventType.Event.StartsWith("ChangeLanguage"))
            {
                refreshInfo();
            }
        }

        public void refreshInfo()
        {
          var pInfo =  GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard).infos;
            List<MissionItemInstanced> pMissions = pInfo.Missions;
            if (pMissions.Count == 0)
            {
                var pMissionDefaults = GameDatabase.Instance.getMissionForLevel(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard);
                for (int i  =0; i < pMissionDefaults.Length; ++i)
                {
                    pMissions.Add(new MissionItemInstanced() { mission = pMissionDefaults[i].mission, process = 0 ,rewards = pMissionDefaults[i].rewards });
                }
            }
            DataSource = pMissions.ToObservableList();
        }


        public void chooseMode(int index)
        {
            GameManager.Instance.ChoosedHard = index;
            refreshInfo();
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
