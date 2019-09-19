using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EazyEngine.Tools
{
    /// <summary>
    /// A base class, meant to be extended depending on the use (simple, multiple object pooler), and used as an interface by the spawners.
    /// Still handles common stuff like singleton and initialization on start().
    /// DO NOT add this class to a prefab, nothing would happen. Instead, add SimpleObjectPooler or MultipleObjectPooler.
    /// </summary>
    /// 
    public class PoolerContainer
    {
        public GameObject parrent = null;
        public List<GameObject> poolObjects = null;
    }
    public class ObjectPooler : TimeControlBehavior
    {
        /// singleton pattern
        public static ObjectPooler Instance;
        /// if this is true, the pool will try not to create a new waiting pool if it finds one with the same name.
        // public bool MutualizeWaitingPools = false;

        /// <summary>
        /// On awake we fill our object pool
        /// </summary>
        protected virtual void Awake()
        {
            Instance = this;
            FillObjectPool();
        }
        public static Dictionary<GameObject, PoolerContainer> poolManager = new Dictionary<GameObject, PoolerContainer>();
        public Dictionary<GameObject, PoolerContainer> poolLocal = new Dictionary<GameObject, PoolerContainer>();
        public virtual GameObject getHostPooler()
        {
            return null;
        }
        /// <summary>
        /// Creates the waiting pool or tries to reuse one if there's already one available
        /// </summary>
        protected virtual void CreateWaitingPool(GameObject pObject)
        {
            if (pObject == null) return;
            var pGroup = GameManager.Instance.getGroupPrefab(pObject);
            bool isInitialized = false;
            PoolerContainer pContainer = null;
            if (pGroup != null)
            {
                for (int i = 0; i < pGroup.prefabBullet.Count; ++i)
                {
                    if (poolManager.ContainsKey(pGroup.prefabBullet[i]))
                    {
                        isInitialized = true;
                        pContainer = poolManager[pGroup.prefabBullet[i]];
                    }
                }
            }
            else if(poolManager.ContainsKey(pObject))
            {
                isInitialized = true;
                pContainer = poolManager[pObject];
            }
            if (!isInitialized)
            {
                PoolerContainer container = new PoolerContainer() { parrent = new GameObject(DetermineObjectPoolName()), poolObjects = new List<GameObject>() };
                poolManager.Add(pObject, container);
                pContainer = container;
            }
            if (!poolLocal.ContainsKey(pObject))
            {
                poolLocal.Add(pObject, pContainer);
            }
        }

        /// <summary>
        /// Determines the name of the object pool.
        /// </summary>
        /// <returns>The object pool name.</returns>
        protected virtual string DetermineObjectPoolName()
        {
            return ("[ObjectPooler] " + this.name);
        }

        /// <summary>
        /// Implement this method to fill the pool with objects
        /// </summary>
        public virtual void FillObjectPool()
        {
            return;
        }

        /// <summary>
        /// Implement this method to return a gameobject
        /// </summary>
        /// <returns>The pooled game object.</returns>
        public virtual GameObject GetPooledGameObject()
        {
            return null;
        }
        public virtual GameObject GetLastOriginal()
        {
            return null;
        }
    }
}