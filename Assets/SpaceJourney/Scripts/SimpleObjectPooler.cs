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
        /// the number of objects we'll add to the pool
        public int PoolSize = 20;
        [HideInEditorMode]
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
        /// <summary>
        /// Fills the object pool with the gameobject type you've specified in the inspector
        /// </summary>
        public override void FillObjectPool()
        {
            if (!LevelManger.InstanceRaw) return;
            if (GameObjectToPool == null) return;
            CreateWaitingPool(GameObjectToPool);
            if (dontDestroyOnload)
            {
                DontDestroyOnLoad(poolLocal[GameObjectToPool].parrent);
            }
            _pooledGameObjects = poolLocal[GameObjectToPool].poolObjects;
            if (poolLocal[GameObjectToPool].poolObjects.Count < PoolSize)
            {

                // we add to the pool the specified number of objects
                for (int i = 0; i < PoolSize; i++)
                {
                    AddOneObjectToThePool();
                }
            }
            StartCoroutine(delayCheckSpawnPool());
       
        }

        IEnumerator delayCheckSpawnPool()
        {
            if(RemainPoolSize > 0)
            {
                AddOneObjectToThePoolRemainTime();
                RemainPoolSize--;
            }
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(delayCheckSpawnPool());
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
                    if (_pooledGameObjects[i].name.StartsWith("[block]") ){ continue; }
                    TrailRenderer[] trails = null;
                    if ((trails = _pooledGameObjects[i].GetComponentsInChildren<TrailRenderer>()) != null)
                    {
                        foreach(var pTrail in trails)
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
                onNewGameObjectCreated(newGameObject,GameObjectToPool);
            }
            if(LevelManger.InstanceRaw != null)
            {
                if (LevelManger.Instance.IsPlaying)
                {
                    newGameObject.gameObject.SetActive(false);
                }
                else
                {
                   // newGameObject.gameObject.SetActive(false);
                    StartCoroutine(delayActionDeactive(newGameObject));

                }
            }
            else
            {
                newGameObject.gameObject.SetActive(false);
            }
      
            newGameObject.transform.SetParent(poolLocal[GameObjectToPool].parrent.transform);
            newGameObject.transform.position = new Vector3(9000, 9000, 9000 );
            newGameObject.name = GameObjectToPool.name + "-" + _pooledGameObjects.Count;
            if (timeRaw != null) {
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
        protected virtual GameObject AddOneObjectToThePoolRemainTime()
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
            if (LevelManger.InstanceRaw != null)
            {
                if (LevelManger.Instance.IsMatching)
                {
                    newGameObject.gameObject.SetActive(false);
                }
                else
                {
                    // newGameObject.gameObject.SetActive(false);
                    StartCoroutine(delayActionDeactive(newGameObject));

                }
            }
            else
            {
                newGameObject.gameObject.SetActive(false);
            }

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