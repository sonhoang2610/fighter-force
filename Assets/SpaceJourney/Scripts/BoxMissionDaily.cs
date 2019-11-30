using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class  BoxMissionGeneric: BaseBox<ItemMissionGame, MissionItemInstanced>
    {
        public int sortCompare(MissionItemInstanced pData1, MissionItemInstanced pData2)
        {
            if(pData1.Claimed  && !pData2.Claimed)
            {
                return 1;
            }
            else if(!pData1.Claimed && pData2.Claimed)
            {
                return -1;
            }
            else
            {
                return pData2.Process.CompareTo(pData1.Process);
            }
        }

        public override void setDataItem(MissionItemInstanced pData, ItemMissionGame pItem)
        {
            base.setDataItem(pData, pItem);
            pItem.transform.SetSiblingIndex(pItem.Index);
        }
        public virtual void OnEnable()
        {
            Comparison = sortCompare;
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
    [System.Serializable]
    public class BoxMissionDaily : BoxMissionGeneric
    {
        public override void OnEnable()
        {
            base.OnEnable();
           var pInfo = MissionContainer.Instance.Info;
           DataSource = pInfo.dailyMission.missionProcessing.missions.ToObservableList();
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
