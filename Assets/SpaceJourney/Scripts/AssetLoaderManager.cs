using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EazyEngine.Tools;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class SubAssetInfo
    {
        public string name;
        public float percent;
        public SubAssetInfo[] subAssets;
        private float currentPercent;

        public float CurrentPercent
        {
            get
            {
                if (subAssets.Length > 0)
                {
                    float percentChild = 0;
                    for (int i = 0; i < subAssets.Length; ++i)
                    {
                        percentChild += subAssets[i].CurrentPercent * subAssets[i].percent;
                    }
                    return percentChild + currentPercent;
                }
                return currentPercent;
            }
            set =>
              currentPercent = value;
        }
        public void clearProgress()
        {
            currentPercent = 0;
            for (int i = 0; i < subAssets.Length; ++i)
            {
                subAssets[i].clearProgress();
            }
        }
        public SubAssetInfo getJob(List<string> path)
        {
            var pExist = System.Array.Exists(subAssets, x => x.name == path[0]);
            if (pExist)
            {
                if(path.Count == 1)
                {
                    return System.Array.Find(subAssets, x => x.name == path[0]);
                }
                else
                {
                    var pAsset = System.Array.Find(subAssets, x => x.name == path[0]);
                    path.RemoveAt(0);
                    return pAsset.getJob(path);
                }
            }
            return null;
        }
    }
    [System.Serializable]
    public struct TriggerLoadAsset
    {
        public string name;
        public float percent;
    }
    public class AssetLoaderManager : Singleton<AssetLoaderManager>, EzEventListener<TriggerLoadAsset>
    {
        [System.NonSerialized]
        public List<string> destroyJob = new List<string>();
       // [DrawWithUnity] 
        public SubAssetInfo[] subAssets;
        private void OnEnable()
        {
            EzEventManager.AddListener(this);
        }
        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }
        protected override void Awake()
        {
            base.Awake();
            //isNew = !GameManager.Instance.loadedAssets.Exists(x => x == IDAssets);
            //if (isNew)
            //{

            //}
        }

        public void clearJob()
        {
            destroyJob.Sort((x1, x2) => x1.Length.CompareTo(x2.Length));
            for(int i  = 0; i < destroyJob.Count; ++i)
            {
               var pJob = AssetLoaderManager.Instance.getJob(destroyJob[i]);
                if(pJob != null)
                {
                    pJob.clearProgress();
                }
            }
        }
        public float getPercentJob(string pID)
        {
            List<string> path = new List<string>();
            path.AddRange(pID.Split('/'));
           
            var pExist = System.Array.Exists(subAssets, x => x.name == path[0]);
            if (pExist)
            {
                if (path.Count == 1)
                {
                   var pJobFirst=  System.Array.Find(subAssets, x => x.name == path[0]);
                    return pJobFirst.CurrentPercent;
                }
                var pJob = System.Array.Find(subAssets, x => x.name == path[0]);
                path.RemoveAt(0);
                var pCurrentJob = pJob.getJob(path);
                return pCurrentJob.CurrentPercent;
            }
            return 1;
        }
        public SubAssetInfo getJob(string pID)
        {
            List<string> path = new List<string>();
            path.AddRange(pID.Split('/'));
            var pExist = System.Array.Exists(subAssets, x => x.name == path[0]);
            if (pExist)
            {
              
                var pJobFirst = System.Array.Find(subAssets, x => x.name == path[0]);
                if (path.Count == 1)
                {
                    return pJobFirst;
                }
                path.RemoveAt(0);
                var pCurrentJob = pJobFirst.getJob(path);
                return pCurrentJob;
            }
            return null;
        }
        public void OnEzEvent(TriggerLoadAsset eventType)
        {
            List<string> path = new List<string>();
            path.AddRange( eventType.name.Split('/'));
            var pExist = System.Array.Exists(subAssets, x => x.name == path[0]);
            if (pExist)
            {
               var pJob = System.Array.Find(subAssets, x => x.name == path[0]);
                path.RemoveAt(0);
               var pCurrentJob = pJob.getJob(path);
                pCurrentJob.CurrentPercent = eventType.percent;
            }
        }
    }

}