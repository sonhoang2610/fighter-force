using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EazyEngine.Space.UI
{
    public class BoxAchievement : BoxMissionGeneric
    {
        public override void OnEnable()
        {
            base.OnEnable();
            var pInfo = MissionContainer.Instance.Info;
            DataSource = pInfo.mainMission.missions.ToObservableList();
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
