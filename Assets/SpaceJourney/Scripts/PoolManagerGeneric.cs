using EazyEngine.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class PrefabInfo
{
    [TableColumnWidth(100)]
    public GameObject key;
    [TableColumnWidth(100)]
    [HideLabel]
    public PrefabInfoMain info;

    public PrefabInfo(GameObject pKey, PrefabInfoMain pInfo)
    {
        key = pKey;
        info = pInfo;
    }
}
[System.Serializable]
public class PrefabInfoMain
{
    [VerticalGroup("group2")]
    public bool isIgnore;
    [VerticalGroup("group2")]
    public int countPreload;
    [HideInEditorMode]
    [VerticalGroup("group2")]
    public ObjectPooler pooler;
}
[System.Serializable]
public class DictionnaryPrefabInfo 
{
    [TableList]
    public List<PrefabInfo> _array = new List<PrefabInfo>();
    public DictionnaryPrefabInfo Keys
    {
        get
        {
            return this;
        }
    }

    public int Count
    {
        get
        {
            return _array.Count;
        }
    }

    public GameObject ElementAt(int index)
    {
        return _array[index].key;
    }

    public PrefabInfoMain this[GameObject key]
    {
        get
        {
            return _array.Find(x => x.key == key).info;
        }
        set
        {
            int index = _array.FindIndex(x => x.key == key);
            if(index < 0)
            {
                _array.Add(new PrefabInfo(key, value));
                index = _array.Count - 1;
            }
            _array[index].info = value;
        }
    }

    public bool ContainsKey(GameObject key)
    {
       return _array.Find(x => x.key == key) == null ? false : true;
    }


    public void Add(GameObject key,PrefabInfoMain pInfo)
    {
        this[key] = pInfo;
    }
    public void Remove(GameObject key)
    {
        int index = _array.FindIndex(x => x.key == key);
        _array.RemoveAt(index);
    }
}
public class PoolManagerGeneric<T> : Singleton<T> where T : Component
{
    [SerializeField]
    [HideLabel]
    public DictionnaryPrefabInfo _storage;
    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < _storage.Keys.Count; ++i)
        {
            if (!_storage.ElementAt(i)) continue;
            GameObject pObjectNew = new GameObject();
            pObjectNew.name = "[Pool]" + _storage.Keys.ElementAt(i).name;
            pObjectNew.transform.parent = transform;
            pObjectNew.transform.localPosition = Vector3.zero;
            var pooler = pObjectNew.AddComponent<SimpleObjectPooler>();
            pooler.onNewGameObjectCreated = (onNewCreateObject);
            pooler.GameObjectToPool = _storage.Keys.ElementAt(i);
            pooler.PoolSize = _storage[_storage.Keys.ElementAt(i)].countPreload;
            pooler.FillObjectPool();
            _storage[_storage.Keys.ElementAt(i)].pooler = pooler;
        }
    }
    public virtual void onNewCreateObject(GameObject pObject,GameObject pOriginal)
    {

    }
  
    public GameObject getObjectFromPool(GameObject pObject)
    {
        if(_storage.ContainsKey(pObject) && _storage[pObject].pooler == null)
        {
            _storage.Remove(pObject);
        }
        if (!_storage.ContainsKey(pObject))
        {
            GameObject pObjectNew = new GameObject();
            pObjectNew.name = "[Pool]" + pObject.name;
            pObjectNew.transform.parent = transform;
            pObjectNew.transform.localPosition = Vector3.zero;
            var pooler = pObjectNew.AddComponent<SimpleObjectPooler>();
            pooler.onNewGameObjectCreated = (onNewCreateObject);
            pooler.GameObjectToPool = pObject;
            pooler.PoolSize =1;
            pooler.FillObjectPool();
            _storage.Add(pObject, new PrefabInfoMain());
            _storage[pObject].pooler = pooler;
        }

        return _storage[pObject].pooler.GetPooledGameObject();
    }
}