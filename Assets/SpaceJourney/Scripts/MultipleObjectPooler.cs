using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using EazyEngine.Space;

namespace EazyEngine.Tools
{
    public static class MMLists
    {
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }
    [Serializable]
    /// <summary>
    /// Multiple object pooler object.
    /// </summary>
    public class MultipleObjectPoolerObject
    {
        public GameObject GameObjectToPool;
        public int PoolSize;
        public bool PoolCanExpand = true;
        public bool Enabled = true;
        public int countFireToNext = 1;
        [HideInInspector]
        public int currentIndexFire = 0;
    }

    /// <summary>
    /// The various methods you can pull objects from the pool with
    /// </summary>
    public enum PoolingMethods { OriginalOrder, OriginalOrderSequential, RandomBetweenObjects, RandomPoolSizeBased, OriginalOrderBaseOnCountFire }

    /// <summary>
    /// This class allows you to have a pool of various objects to pool from.
    /// </summary>
    public class MultipleObjectPooler : ObjectPooler
    {
        /// the list of objects to pool
        public List<MultipleObjectPoolerObject> Pool;

       // [Information("A MultipleObjectPooler is a reserve of objects, to be used by a Spawner. When asked, it will return an object from the pool (ideally an inactive one) chosen based on the pooling method you've chosen.\n- OriginalOrder will spawn objects in the order you've set them in the inspector (from top to bottom)\n- OriginalOrderSequential will do the same, but will empty each pool before moving to the next object\n- RandomBetweenObjects will pick one object from the pool, at random, but ignoring its pool size, each object has equal chances to get picked\n- PoolSizeBased randomly choses one object from the pool, based on its pool size probability (the larger the pool size, the higher the chances it'll get picked)'...", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// the chosen pooling method
        //public PoolingMethods PoolingMethod = PoolingMethods.RandomPoolSizeBased;
      //  [Information("If you set CanPoolSameObjectTwice to false, the Pooler will try to prevent the same object from being pooled twice to avoid repetition. This will only affect random pooling methods, not ordered pooling.", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// whether or not the same object can be pooled twice in a row. If you set CanPoolSameObjectTwice to false, the Pooler will try to prevent the same object from being pooled twice to avoid repetition. This will only affect random pooling methods, not ordered pooling.
      //  public bool CanPoolSameObjectTwice = true;
        public int fixedIndex = -1;

        /// the actual object pool
        protected List<GameObject> _pooledGameObjects;
        protected List<GameObject> _pooledGameObjectsOriginalOrder;
        protected List<MultipleObjectPoolerObject> _randomizedPool;
        protected string _lastPooledObjectName;
        protected int _currentIndex = 0;

        public void enableIndexPool(int index)
        {
            Pool[index].Enabled = true;
        }

        /// <summary>
        /// Determines the name of the object pool.
        /// </summary>
        /// <returns>The object pool name.</returns>
        //protected override string DetermineObjectPoolName()
        //{
        //    return ("[MultipleObjectPooler] " + this.name);
        //}
        public override GameObject GetLastOriginal()
        {
            return lastOriginal;
        }

        public GameObject getObjectPool(GameObject pObject,int index)
        {
            if(poolLocal[pObject].poolObjects.Count > index)
            {
                _pooledGameObjects.Add(poolLocal[pObject].poolObjects[index]);
                return poolLocal[pObject].poolObjects[index];
            }
            return AddOneObjectToThePool(pObject);
        }
        /// <summary>
        /// Fills the object pool with the amount of objects you specified in the inspector.
        /// </summary>
        public override void FillObjectPool()
        {
            if (!LevelManger.InstanceRaw) return;
            for(int i = 0; i < Pool.Count; ++i)
            {
                CreateWaitingPool(Pool[i].GameObjectToPool);
            }
    
            // we initialize the pool
            _pooledGameObjects = new List<GameObject>();
           bool stillObjectsToPool = true;
            _pooledGameObjectsOriginalOrder = new List<GameObject>();

            // we store our poolsizes in a temp array so it doesn't impact the inspector
           var poolSizes = new int[Pool.Count];
            for (int i = 0; i < Pool.Count; i++)
            {
                poolSizes[i] = Pool[i].PoolSize;
            }

            // we go through our objects in the order they were in the inspector, and fill the pool while we find objects to add
            while (stillObjectsToPool)
            {
                stillObjectsToPool = false;
                for (int i = 0; i < Pool.Count; i++)
                {
                    for (int j = 0; j < Pool[i].countFireToNext; j++)
                    {
                        if (poolSizes[i] > 0)
                        {
                            AddOneObjectToThePool(Pool[i].GameObjectToPool);
                            poolSizes[i]--;
                            stillObjectsToPool = true;
                        }
                        else
                        {
                            goto final;
                        }
                    }
                }
            }
            final:
            _pooledGameObjectsOriginalOrder.AddRange(_pooledGameObjects);
        }

        /// <summary>
        /// Adds one object of the specified type to the object pool.
        /// </summary>
        /// <returns>The object that just got added.</returns>
        /// <param name="typeOfObject">The type of object to add to the pool.</param>
        protected virtual GameObject AddOneObjectToThePool(GameObject typeOfObject)
        {
            GameObject newGameObject = (GameObject)Instantiate(typeOfObject);
            newGameObject.gameObject.SetActive(false);
            newGameObject.transform.SetParent(poolLocal[typeOfObject].parrent.transform);
            newGameObject.name = typeOfObject.name;
            _pooledGameObjects.Add(newGameObject);
            return newGameObject;
        }
        protected GameObject lastOriginal;
        /// <summary>
        /// Gets a random object from the pool.
        /// </summary>
        /// <returns>The pooled game object.</returns>
        /// 
        public override GameObject GetPooledGameObject()
        {
            GameObject pooledGameObject = GetPooledGameObjectOriginalOrder();
            if (pooledGameObject != null)
            {
                _lastPooledObjectName = pooledGameObject.name;
            }
            else
            {
                _lastPooledObjectName = "";
            }

            return pooledGameObject;
        }

      
        

        public int FixedIndex { get => fixedIndex; set => fixedIndex = value; }
        

        /// <summary>
        /// Tries to find a gameobject in the pool according to the order the list has been setup in (one of each, no matter how big their respective pool sizes)
        /// </summary>
        /// <returns>The pooled game object original order.</returns>
        protected virtual GameObject GetPooledGameObjectOriginalOrder()
        {
            int newIndex;
            _currentIndex = FixedIndex > -1 ? FixedIndex : _currentIndex;
            // if we've reached the end of our list, we start again from the beginning
            if (_currentIndex >= _pooledGameObjectsOriginalOrder.Count)
            {
                ResetCurrentIndex();
            }

            MultipleObjectPoolerObject searchedObject = GetPoolObject(_pooledGameObjects[_currentIndex].gameObject);

            if (_currentIndex >= _pooledGameObjects.Count) { return null; }
            if (!searchedObject.Enabled) { _currentIndex++; return null; }
            lastOriginal = searchedObject.GameObjectToPool;
            // if the object is already active, we need to find another one
            if (_pooledGameObjects[_currentIndex].gameObject.activeInHierarchy)
            {
                GameObject findObject = FindInactiveObject(_pooledGameObjects[_currentIndex].gameObject.name, _pooledGameObjects);
                if (findObject != null)
                {
                    _currentIndex++;

                    return findObject;
                }

                // if its pool can expand, we create a new one
                if (searchedObject.PoolCanExpand)
                {
                    _currentIndex++;
                    return AddOneObjectToThePool(searchedObject.GameObjectToPool);
                }
                else
                {
                    // if it can't expand we return nothing
                    return null;
                }
            }
            else
            {
                // if the object is inactive, we return it
                newIndex = _currentIndex;
                _currentIndex++;
                return _pooledGameObjects[newIndex];
            }
        }


        protected virtual GameObject FindInactiveObject(string searchedName, List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                // if we find an object inside the pool that matches the asked type
                if (list[i].name.Equals(searchedName))
                {
                    // and if that object is inactive right now
                    if (!list[i].gameObject.activeInHierarchy)
                    {
                        // we return it
                        return list[i];
                    }
                }
            }
            return null;
        }

        protected virtual GameObject FindAnyInactiveObject(List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                // and if that object is inactive right now
                if (!list[i].gameObject.activeInHierarchy)
                {
                    // we return it
                    return list[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Finds an object in the pool based on its name, active or inactive
        /// Returns null if there's no object by that name in the pool
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="searchedName">Searched name.</param>
        protected virtual GameObject FindObject(string searchedName, List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                // if we find an object inside the pool that matches the asked type
                if (list[i].name.Equals(searchedName))
                {
                    // and if that object is inactive right now
                    return list[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns (if it exists) the MultipleObjectPoolerObject from the original Pool based on a GameObject.
        /// Note that this is name based.
        /// </summary>
        /// <returns>The pool object.</returns>
        /// <param name="testedObject">Tested object.</param>
        protected virtual MultipleObjectPoolerObject GetPoolObject(GameObject testedObject)
        {
            if (testedObject == null)
            {
                return null;
            }
            int i = 0;
            foreach (MultipleObjectPoolerObject poolerObject in Pool)
            {
                if (testedObject.name.Equals(poolerObject.GameObjectToPool.name))
                {
                    return (poolerObject);
                }
                i++;
            }
            return null;
        }

        protected virtual bool PoolObjectEnabled(GameObject testedObject)
        {
            MultipleObjectPoolerObject searchedObject = GetPoolObject(testedObject);
            if (searchedObject != null)
            {
                return searchedObject.Enabled;
            }
            else
            {
                return false;
            }
        }

        public virtual void EnableObjects(string name, bool newStatus)
        {
            foreach (MultipleObjectPoolerObject poolerObject in Pool)
            {
                if (name.Equals(poolerObject.GameObjectToPool.name))
                {
                    poolerObject.Enabled = newStatus;
                }
            }
        }

        public virtual void ResetCurrentIndex()
        {
            _currentIndex = 0;
        }
    }
}