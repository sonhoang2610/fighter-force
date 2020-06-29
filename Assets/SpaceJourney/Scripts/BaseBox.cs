using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System;
using EazyEngine.Space;

[System.Serializable]
public class UnityEventFloat : UnityEvent<float>
{

}
public class BaseBox<TItem, TData> : MonoBehaviour where TItem : BaseItem<TData> where TData : class
{
    public GameObject attachMent;
    public TItem prefabItem;
    public ObservableList<TData> _infos = new ObservableList<TData>();
    public bool loadAsync = false;
    [ShowIf("loadAsync")]
    public string idJob;
    [ShowIf("loadAsync")]
    public UnityEvent onCompleteAsync;
    [ShowIf("loadAsync")]
    public UnityEventFloat onLoadingAsync;
    // [HideInEditorMode]
    public List<TItem> items = new List<TItem>();
    public Dictionary<TData, TItem> Item = new Dictionary<TData, TItem>();
    public List<EventDelegate> onDataAction = new List<EventDelegate>();
    private Comparison<TData> comparison;

    public virtual void resortItem()
    {
        attachMent.SendMessage("Reposition", SendMessageOptions.DontRequireReceiver);
        attachMent.SendMessage("SortAlphabetically", attachMent, SendMessageOptions.DontRequireReceiver);
       // SendMessage("Reposition", attachMent, SendMessageOptions.DontRequireReceiver);
     //   SendMessage("SortAlphabetically", attachMent, SendMessageOptions.DontRequireReceiver);
    }
    public virtual void onCompleteFirstAsync()
    {
        onCompleteAsync.Invoke();
        if (!string.IsNullOrEmpty(idJob))
        {
            EzEventManager.TriggerAssetLoaded(new TriggerLoadAsset() { name = idJob, percent = 1 });
        }
    }
    public virtual ObservableList<TData> DataSource
    {
        set
        {
            value.Comparison = Comparison;
            if (value != _infos)
            {
                _infos = value;
                setUpCallBack();
                _infos.CollectionChanged();
            }
            _infos = value;
            if (attachMent)
            {

                resortItem();
            }
        }
        get
        {
            return _infos;
        }
    }
    public Comparison<TData> Comparison
    {
        get
        {
            return comparison;
        }

        set
        {
            comparison = value;
            if(DataSource != null)
            {
                DataSource.Comparison = value;
            }
        }
    }
    public void reloadData()
    {
        _infos.CollectionChanged();
    }

    public virtual void setDataItem(TData pData, TItem pItem)
    {
        pItem.Data = pData;
    }

    public void setUpCallBack()
    {
        _infos.OnCollectionChange += onCollectionChange;
    }

    public virtual void Awake()
    {
        setUpCallBack();
    }


    public TItem obtainItemExistData(TData pData)
    {
        for (int i = 0; i < items.Count; ++i)
        {
            if (items[i]._data == pData)
            {
                items[i].Using = true;
                items[i].Data = pData;
                if (items[i].Dirty)
                {
                    showNewItem(items[i]);
;                }
                return items[i];
            }
        }
        return null;
    }

    public virtual void showNewItem(TItem pItem)
    {

    }

    public TItem obtainItemNewData(TData pData,int index)
    {
        for (int i = 0; i < items.Count; ++i)
        {
            if (!items[i].Using)
            {
                items[i].Index = index;
                items[i].Dirty = true;
                setDataItem(pData, items[i]);
                showNewItem(items[i]);
                return items[i];
            }
        }

        var pItem = Instantiate<TItem>(prefabItem, attachMent.transform);
        pItem.Dirty = true;
        pItem.Index = index;
        setDataItem(pData, pItem);
        showNewItem(pItem);
        pItem.onData = onDataAction;
        items.Add(pItem);
        return pItem;
    }
    public virtual bool isItemEffect()
    {
        return false;
    }
    public IEnumerator onCollectionChangeAsync()
    {
        Item.Clear();
        for (int i = 0; i < items.Count; ++i)
        {
            items[i].Dirty = !items[i].Using;
            items[i].Using = false;
            items[i].hide();
        }
        List<TData> initData = new List<TData>();
        Dictionary<TData, int> indexBoard = new Dictionary<TData, int>();
        for (int i = 0; i < DataSource.Count; ++i)
        {
            indexBoard.Add(DataSource[i], i);
        }
        initData.AddRange(DataSource.ToArray());
        for (int i = initData.Count - 1; i >= 0; --i)
        {
            var pItem = obtainItemExistData(initData[i]);
            if (pItem)
            {

                if (!Item.ContainsKey(initData[i]))
                {
                    Item.Add(initData[i], pItem);
                }
                else
                {
                    Item[initData[i]] = pItem;
                }
                pItem.Index = indexBoard[initData[i]];
                setDataItem(initData[i], pItem);
                pItem.show(!isItemEffect());
                pItem.Using = true;
                initData.RemoveAt(i);
            }
        }
        for (int i = 0; i < initData.Count; ++i)
        {
            yield return new WaitForEndOfFrame();
            var pItem = obtainItemNewData(initData[i], indexBoard[initData[i]]);
            if (!Item.ContainsKey(initData[i]))
            {
                Item.Add(initData[i], pItem);
            }
            else
            {
                Item[initData[i]] = pItem;
            }
            pItem.show(!isItemEffect());
            pItem.Using = true;
            onLoadingAsync.Invoke((float)(i + 1) / (float)initData.Count);
            if (!string.IsNullOrEmpty(idJob))
            {
                EzEventManager.TriggerAssetLoaded(new TriggerLoadAsset() { name = idJob, percent = (float)(i + 1) / (float)initData.Count });
            }
        }
        if (attachMent)
        {
            resortItem();
        }
        onCompleteFirstAsync();
      
    }
    public void onCollectionChange()
    {
        if (loadAsync && AssetLoaderManager.Instance.getPercentJob("Main") < 1)
        {
            if (DataSource.Count > 0)
            {
                StartCoroutine(onCollectionChangeAsync());
            }
            return;
        }
        Item.Clear();
        for (int i = 0; i < items.Count; ++i)
        {
            items[i].Dirty = !items[i].Using;
            items[i].Using = false;
            items[i].hide();
        }
        List<TData> initData = new List<TData>();
        Dictionary<TData, int> indexBoard = new Dictionary<TData, int>();
        for(int i = 0; i < DataSource.Count; ++i)
        {
            indexBoard.Add(DataSource[i], i);
        }
        initData.AddRange(DataSource.ToArray());
        for (int i = initData.Count - 1; i >= 0; --i)
        {
            var pItem = obtainItemExistData(initData[i]);
            if (pItem)
            {

                if (!Item.ContainsKey(initData[i]))
                {
                    Item.Add(initData[i], pItem);
                }
                else
                {
                    Item[initData[i]] = pItem;
                }
                pItem.Index = indexBoard[initData[i]];
                setDataItem(initData[i], pItem);
                pItem.show(!isItemEffect());
                pItem.Using = true;
                initData.RemoveAt(i);
            }
        }
        for (int i = 0; i < initData.Count; ++i)
        {
            var pItem = obtainItemNewData(initData[i], indexBoard[initData[i]]);

            if (!Item.ContainsKey(initData[i]))
            {
                Item.Add(initData[i], pItem);
            }
            else
            {
                Item[initData[i]] = pItem;
            }
            pItem.show(!isItemEffect());
            pItem.Using = true;
        }
        if (attachMent)
        {
            resortItem();
        }
        
    }
}
