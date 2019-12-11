using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System;

public class BaseBox<TItem, TData> : MonoBehaviour where TItem : BaseItem<TData> where TData : class
{
    public GameObject attachMent;
    public TItem prefabItem;
    public ObservableList<TData> _infos = new ObservableList<TData>();
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
        Debug.Log("ez type" +typeof(TItem).ToString());
        for (int i = 0; i < items.Count; ++i)
        {
            if (!items[i].Using)
            {
                Debug.Log("ez here1");
                items[i].Index = index;
                Debug.Log("ez here2");
                items[i].Dirty = true;
                Debug.Log("ez here3");
                setDataItem(pData, items[i]);
                Debug.Log("ez here4");
                showNewItem(items[i]);
                return items[i];
            }
        }
        Debug.Log("ez here6");

     var pItem = Instantiate<TItem>(prefabItem, attachMent.transform);
        Debug.Log("ez here7");
        pItem.Dirty = true;
        pItem.Index = index;
        setDataItem(pData, pItem);
        showNewItem(pItem);
        pItem.onData = onDataAction;
        items.Add(pItem);
        return pItem;
    }

    public void onCollectionChange()
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
                pItem.show();
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
            pItem.show();
            pItem.Using = true;
        }
        if (attachMent)
        {
            resortItem();
        }
        
    }
}
