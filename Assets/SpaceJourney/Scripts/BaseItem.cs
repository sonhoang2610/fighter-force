using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseItem<T> : MonoBehaviour where T  : class
{
   // [HideInInspector]
    public List<EventDelegate> onData; 
    public T _data;
    public UnityEvent onUsing;
    [HideInInspector]
    private int index;
    public virtual T Data
    {
        set
        {
            _data = value;
        }
        get
        {
            return _data;
        }
    }
    protected bool _using;
    public bool Using
    {
        get
        {
            return _using;
        }
        set
        {
            _using = value;
            if (value && Dirty)
            {
                onUsing.Invoke();
            }
        }
    }
    public bool Dirty
    {
        get; set;
    }
    public virtual int Index { get => index; set {
            index = value;
        }
    }

    public virtual void onExecute()
    {
        for(int i = 0; i < onData.Count; ++i) { 

            if (onData[i].parameters.Length == 1)
            {
                onData[i].parameters[0].value = Data;
            }
      
        }

        if (onData.Count > 0)
        {
            onData[0].Execute();
        }
      
    }
    public virtual void onExecute1()
    {
        for(int i = 0; i < onData.Count; ++i) { 

            if (onData[i].parameters.Length == 1)
            {
                onData[i].parameters[0].value = Data;
            }
      
        }

        onData[1].Execute();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void show()
    {
        gameObject.SetActive(true);
    }
    public virtual void hide()
    {
        gameObject.SetActive(false);
    }
}
