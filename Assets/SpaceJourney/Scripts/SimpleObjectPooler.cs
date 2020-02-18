using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EazyEngine.Space;
using Sirenix.OdinInspector;

namespace EazyEngine.Tools
{
    [System.Serializable]
    public class SimpleObjectPooler : ObjectPooler
    {
        [ContextMenu("test")]
        public void test()
        {
#if UNITY_EDITOR
            UnityEditor.SerializedObject pOb = new UnityEditor.SerializedObject(this);
            var prop = pOb.FindProperty("m_Icon");

#endif
        }
        [HideInInspector]
        public System.Action<GameObject, GameObject> onNewGameObjectCreated;
        /// the game object we'll instantiate 
        public GameObject GameObjectToPool;

        public string backUpPath;
#if UNITY_EDITOR
        [Button("Generate Backup")]
        public void generateBackUp()
        {
            if (!GameObjectToPool) return;
            backUpPath = UnityEditor.AssetDatabase.GetAssetPath(GameObjectToPool);
        }
#endif
        public void recover()
        {
            if (GameObjectToPool) return;
            int index = backUpPath.IndexOf("Resources/");
            var path = backUpPath.Substring(index + 10, backUpPath.Length - index - 10).Split('.')[0];
            GameObjectToPool = Resources.Load<GameObject>(path);
        }
        /// the number of objects we'll add to the pool
        public int PoolSize = 20;
        public int RemainPoolSize = 0;
        /// if true, the pool will automatically add objects to the itself if needed
        public bool PoolCanExpand = true;
        public bool dontDestroyOnload = false;
        protected List<GameObject> _cachePreloadObject = new List<GameObject>();
        /// the actual object pool
        protected List<GameObject> _pooledGameObjects;

        public override bool isDontDestroyOnload()
        {
            return dontDestroyOnload;
        }
        public override GameObject GetLastOriginal()
        {
            return GameObjectToPool;
        }
        int _remainPoolsize = 0;
        /// <summary>
        /// Fills the object pool with the gameobject type you've specified in the inspector
        /// </summary>
        /// 
        public override void FillObjectPool()
        {
            // if (!LevelManger.InstanceRaw) return;
            recover();
            if (GameObjectToPool == null) return;
            CreateWaitingPool(GameObjectToPool);
            if (dontDestroyOnload)
            {
                DontDestroyOnLoad(poolLocal[GameObjectToPool].parrent);
            }
            _pooledGameObjects = poolLocal[GameObjectToPool].poolObjects;

            if ((poolLocal[GameObjectToPool].poolObjects.Count < PoolSize && SceneManager.Instance.isLoading && SceneManager.Instance.stateLoading == StateLoadingGame.PoolFirst))
            {
                if (PoolSize > 1)
                {
                    RemainPoolSize += PoolSize - 1;
                    PoolSize = 1;
                }
                SceneManager.Instance.addloading(1);
                SceneManager.Instance.poolRegisterLoading.Add(this);
            }
            else if (!SceneManager.Instance.isLoading)
            {
                var pRemain = ((PoolSize + RemainPoolSize) - poolLocal[GameObjectToPool].poolObjects.Count) <= 0 ? 0 : ((PoolSize + RemainPoolSize) - poolLocal[GameObjectToPool].poolObjects.Count);
                if (pRemain > 0)
                {
                    for (int i = 0; i < pRemain; i++)
                    {
                        AddOneObjectToThePool();
                    }
                }
            }
            //SceneManager.Instance.StartCoroutine(delayCheckSpawnPool());

        }
        public void FillObjectNow()
        {
            if (GameObjectToPool == null) return;
            CreateWaitingPool(GameObjectToPool);
            if (dontDestroyOnload)
            {
                DontDestroyOnLoad(poolLocal[GameObjectToPool].parrent);
            }
            _pooledGameObjects = poolLocal[GameObjectToPool].poolObjects;
            if (poolLocal[GameObjectToPool].poolObjects.Count < PoolSize)
            {
                for (int i = 0; i < PoolSize; i++)
                {
                    AddOneObjectToThePool();
                }
            }
            if (!GameManager.Instance.pendingObjects.Contains(GameObjectToPool))
            {
                GameManager.Instance.pendingObjects.Add(GameObjectToPool);
            }
        }

        public void FillRemain()
        {
            _remainPoolsize = ((PoolSize + RemainPoolSize) - poolLocal[GameObjectToPool].poolObjects.Count) <= 0 ? 0 : ((PoolSize + RemainPoolSize) - poolLocal[GameObjectToPool].poolObjects.Count);
            if (_remainPoolsize > 0)
            {
                if (SceneManager.Instance.isLoading)
                {
                    SceneManager.Instance.addloading(_remainPoolsize);
                }
            }
        }
        public IEnumerator delayCheckSpawnPool()
        {
            yield return new WaitForEndOfFrame();
            if (SceneManager.Instance.isLoading)
            {
                if (_remainPoolsize > 0)
                {
                    var pRemain = ((PoolSize + RemainPoolSize) - poolLocal[GameObjectToPool].poolObjects.Count);
                    var pCountDown = Mathf.Min(pRemain, SceneManager.Instance.loadObjectPerFrame);
                    if (pRemain > 0)
                    {
                        for(int i = 0; i < pCountDown; ++i)
                        {
                            AddOneObjectToThePoolRemainTime(false);
                        }                        
                    }
                    var pCountDownRaw = Mathf.Min(_remainPoolsize, SceneManager.Instance.loadObjectPerFrame);
                    _remainPoolsize -= pCountDownRaw;
                    for (int i = 0; i < pCountDownRaw; ++i)
                    {
                        SceneManager.Instance.loadingDirty(StateLoadingGame.PoolAfter);
                    }
                }

                if (_remainPoolsize > 0)
                {
                    SceneManager.Instance.StartCoroutine(delayCheckSpawnPool());
                }
            }
        }
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Determines the name of the object pool.
        /// </summary>
        /// <returns>The object pool name.</returns>
        //protected override string DetermineObjectPoolName()
        //{
        //    return ("[SimpleObjectPooler] " + (transform.parent != null ? (transform.parent.name + ".")  : "") + this.name);
        //}

        /// <summary>
        /// This method returns one inactive object from the pool
        /// </summary>
        /// <returns>The pooled game object.</returns>
        public override GameObject GetPooledGameObject()
        {
            // we go through the pool looking for an inactive object
            for (int i = 0; i < _pooledGameObjects.Count; i++)
            {
                if (!_pooledGameObjects[i].gameObject.activeInHierarchy)
                {
                    if (_pooledGameObjects[i].name.StartsWith("[block]")) { continue; }
                    TrailRenderer[] trails = null;
                    if ((trails = _pooledGameObjects[i].GetComponentsInChildren<TrailRenderer>()) != null)
                    {
                        foreach (var pTrail in trails)
                        {
                            pTrail.Clear();
                        }
                    }
                    // if we find one, we return it
                    return _pooledGameObjects[i];
                }
            }
            // if we haven't found an inactive object (the pool is empty), and if we can extend it, we add one new object to the pool, and return it		
            if (PoolCanExpand)
            {
                return AddOneObjectToThePool();
            }
            // if the pool is empty and can't grow, we return nothing.
            return null;
        }

        /// <summary>
        /// Adds one object of the specified type (in the inspector) to the pool.
        /// </summary>
        /// <returns>The one object to the pool.</returns>
        protected virtual GameObject AddOneObjectToThePool()
        {
            if (GameObjectToPool == null)
            {
                Debug.LogWarning("The " + gameObject.name + " ObjectPooler doesn't have any GameObjectToPool defined.", gameObject);
                return null;
            }
            GameObject newGameObject = (GameObject)Instantiate(GameObjectToPool);
            _cachePreloadObject.Add(newGameObject);
            if (onNewGameObjectCreated != null)
            {
                onNewGameObjectCreated(newGameObject, GameObjectToPool);
            }
            newGameObject.gameObject.SetActive(false);
            //if (LevelManger.InstanceRaw != null)
            //{
            //    if (LevelManger.Instance.IsPlaying)
            //    {
            //        newGameObject.gameObject.SetActive(false);
            //    }
            //    else
            //    {
            //       // newGameObject.gameObject.SetActive(false);
            //        StartCoroutine(delayActionDeactive(newGameObject));

            //    }
            //}
            //else
            //{
            //    newGameObject.gameObject.SetActive(false);
            //}

            newGameObject.transform.SetParent(poolLocal[GameObjectToPool].parrent.transform);
            newGameObject.transform.position = new Vector3(9000, 9000, 9000);
            newGameObject.name = GameObjectToPool.name + "-" + _pooledGameObjects.Count;
            if (timeRaw != null)
            {
                var pTime = newGameObject.GetComponent<EazyEngine.Timer.TimeControllerElement>();
                if (pTime == null)
                {
                    pTime = newGameObject.AddComponent<EazyEngine.Timer.TimeControllerElement>();
                    pTime._groupName = time._groupName;
                }
            }
            _pooledGameObjects.Add(newGameObject);
            return newGameObject;
        }
        public virtual GameObject AddOneObjectToThePoolRemainTime(bool isActive)
        {
            if (GameObjectToPool == null)
            {
                Debug.LogWarning("The " + gameObject.name + " ObjectPooler doesn't have any GameObjectToPool defined.", gameObject);
                return null;
            }
            bool pCacheMatching = LevelManger.Instance.IsMatching;
            if (pCacheMatching)
            {
                LevelManger.Instance.IsMatching = false;
            }
            GameObject newGameObject = (GameObject)Instantiate(GameObjectToPool);
            if (pCacheMatching)
            {
                LevelManger.Instance.IsMatching = true;
            }
            _cachePreloadObject.Add(newGameObject);
            if (onNewGameObjectCreated != null)
            {
                onNewGameObjectCreated(newGameObject, GameObjectToPool);
            }
            newGameObject.gameObject.SetActive(isActive);
            //if (LevelManger.InstanceRaw != null)
            //{
            //    if (LevelManger.Instance.IsMatching)
            //    {
            //        newGameObject.gameObject.SetActive(false);
            //    }
            //    else
            //    {
            //        // newGameObject.gameObject.SetActive(false);
            //        StartCoroutine(delayActionDeactive(newGameObject));

            //    }
            //}
            //else
            //{
            //    newGameObject.gameObject.SetActive(false);
            //}

            newGameObject.transform.SetParent(poolLocal[GameObjectToPool].parrent.transform);
            newGameObject.transform.position = new Vector3(9000, 9000, 9000);
            newGameObject.name = GameObjectToPool.name + "-" + _pooledGameObjects.Count;
            if (timeRaw != null)
            {
                var pTime = newGameObject.GetComponent<EazyEngine.Timer.TimeControllerElement>();
                if (pTime == null)
                {
                    pTime = newGameObject.AddComponent<EazyEngine.Timer.TimeControllerElement>();
                    pTime._groupName = time._groupName;
                }
            }
            _pooledGameObjects.Add(newGameObject);
            return newGameObject;
        }
        public IEnumerator delayActionDeactive(GameObject pObject)
        {
            yield return new WaitForSeconds(0.1f);
            pObject.SetActive(false);
        }
    }
}