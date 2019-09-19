using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class BaseBox<TItem, TData> : MonoBehaviour where TItem : BaseItem<TData> where TData : class
{
    public GameObject attachMent;
    public TItem prefabItem;
    public ObservableList<TData> _infos = new ObservableList<TData>();
   // [HideInEditorMode]
    public List<TItem> items = new List<TItem>();
    public Dictionary<TData, TItem> Item = new Dictionary<TData, TItem>();
    public List<EventDelegate> onDataAction = new List<EventDelegate>();
    public virtual ObservableList<TData> DataSource
    {
        set
        {
            if (value != _infos)
            {
                _infos = value;
                setUpCallBack();
                _infos.CollectionChanged();
            }
            _infos = value;
            if (attachMent)
            {
                SendMessage("Reposition", attachMent, SendMessageOptions.DontRequireReceiver);
            }
        }
        get
        {
            return _infos;
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
                return items[i];
            }
        }
        return null;
    }

    public TItem obtainItemNewData(TData pData,int index)
    {
        for (int i = 0; i < items.Count; ++i)
        {
            if (!items[i].Using)
            {
                items[i].Index = index;
                setDataItem(pData, items[i]);
                return items[i];
            }
        }
        var pItem = Instantiate<TItem>(prefabItem, attachMent.transform);
        pItem.Index = index;
        setDataItem(pData, pItem);
        pItem.onData = onDataAction;
        items.Add(pItem);
        return pItem;
    }

    public void onCollectionChange()
    {
        Item.Clear();
        for (int i = 0; i < items.Count; ++i)
        {
            items[i].Using = false;
            items[i].hide();
        }
        List<TData> initData = new List<TData>();
        initData.AddRange(DataSource.ToArray());
        int index = 0;
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
                pItem.Index = index;
                pItem.show();
                pItem.Using = true;
                initData.RemoveAt(i);
                index++;
            }
        }
        for (int i = 0; i < initData.Count; ++i)
        {
            var pItem = obtainItemNewData(initData[i],index);

            if (!Item.ContainsKey(initData[i]))
            {
                Item.Add(initData[i], pItem);
            }
            else
            {
                Item[initData[i]] = pItem;
            }
            index++;
            pItem.show();
            pItem.Using = true;
        }
        if (attachMent)
        {
            attachMent.SendMessage("Reposition", SendMessageOptions.DontRequireReceiver);
        }
    }
}
