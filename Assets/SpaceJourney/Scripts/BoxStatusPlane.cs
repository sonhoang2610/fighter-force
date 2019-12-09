using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class BoxStatusPlane : BaseBox<ItemStatusPlane, StatusInfo>
    {
        public StatusInfo[] database;

        public TimeControlBehavior timeLine;
        public UIRoot root;
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

        public override void resortItem()
        {
                base.resortItem();
        }
        public override void showNewItem(ItemStatusPlane pItem)
        {
            base.showNewItem(pItem);
            updatePosPlayer(pItem.transform);
            if (!pItem.name.Contains("[block]"))
            {
                pItem.name += "[block]";
            }
            if (!listUpdateFollowPlayer.ContainsKey(pItem.transform))
            {
                listUpdateFollowPlayer.Add(pItem.transform,0.5f);
            }
            listUpdateFollowPlayer[pItem.transform] = 0.5f;
        }
        protected Dictionary<Transform,float> listUpdateFollowPlayer = new Dictionary<Transform,float>();

        public void updatePosPlayer(Transform trans)
        {
            Vector2 posPlayer = LevelManger.Instance.CurrentPlayer.transform.position;
            posPlayer.y /= LevelManger.Instance.mainPlayCamera.orthographicSize;
            posPlayer.x /= (LevelManger.Instance.mainPlayCamera.orthographicSize * Screen.width / Screen.height);
            posPlayer.x *= (GUIManager.Instance.CamGUI.orthographicSize * Screen.width / Screen.height);
            posPlayer.y *= GUIManager.Instance.CamGUI.orthographicSize;
       
            trans.position = posPlayer + (Vector2)GUIManager.Instance.CamGUI.transform.position;
        }

        // Start is called before the first frame update
        void Start()
        {
            attachMent.GetComponent<UIGrid>().isSortBlock = true;
        }

        // Update is called once per frame
        void Update()
        {
            List<Transform> listDelete = new List<Transform>();
            for (int i = 0; i < listUpdateFollowPlayer.Count; ++i)
            {
                var pKey = listUpdateFollowPlayer.Keys.ElementAt(i);
                if (listUpdateFollowPlayer[pKey] > 0)
                {
                    listUpdateFollowPlayer[pKey] -= timeLine.time.deltaTime;
                 
                }
                if(listUpdateFollowPlayer[pKey] <= 0)
                {
                    listDelete.Add(pKey);
                }
            }
            foreach(var pElement in listDelete)
            {
                listUpdateFollowPlayer.Remove(pElement);
                pElement.name = pElement.name.Replace("[block]", "");
            }
            if(listDelete.Count > 0) { resortItem(); }
        }
    }
}
