using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class BoxStatusPlane : BaseBox<ItemStatusPlane, StatusInfo>
    {
        public StatusInfo[] database;

        public TimeControlBehavior timeLine;
        protected List<StatusInfo> cacheData = new List<StatusInfo>();
        public void addStatus(string pId, float pDuration)
        {
            for (int i = 0; i < database.Length; ++i)
            {
                if (database[i].id == pId)
                {
                    var pData = cacheData.Find(delegate (StatusInfo pInfo)
                    {
                        return pInfo.idOverride.checkExist(pId);
                    });
                    if (pData == null)
                    {
                        pData = cacheData.Find(delegate (StatusInfo pInfo)
                        {
                            return pInfo.id == pId;
                        });
                    }
                    if (pData == null)
                    {
                        pData = database[i];
                        cacheData.Add(database[i]);
                    }
                    pData.Duration = pDuration;
                    pData.CurrentDuration = pDuration;
                }
            }
            DataSource = cacheData.ToObservableList();
        }

        private void LateUpdate()
        {
            bool dirty = false;
            for (int i = cacheData.Count - 1; i >= 0; --i)
            {
                cacheData[i].CurrentDuration -= timeLine.time.deltaTime;
                if (cacheData[i].CurrentDuration <= 0)
                {
                    cacheData.RemoveAt(i);
                    dirty = true;
                }
            }
            if (dirty)
            {
                DataSource = cacheData.ToObservableList();
            }
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
