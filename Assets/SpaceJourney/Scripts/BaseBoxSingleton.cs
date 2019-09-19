using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoxSingleton<T,TItem,TData> : BaseBox<TItem, TData> where TItem : BaseItem<TData> where TData : class where T : BaseBoxSingleton<T, TItem, TData>
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    public override void Awake()
    {
        instance = (T)this;
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
