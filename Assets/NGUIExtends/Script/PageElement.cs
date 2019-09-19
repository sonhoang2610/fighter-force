    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageElement : MonoBehaviour
{
    [SerializeField]
    List<EventDelegate> _listEventChoose = null;
    [SerializeField]
    List<EventDelegate> _listEventUnChoosed = null;
    [SerializeField]
    List<EventDelegate> _listEventUpdateIndex = null;
    [SerializeField]
    List<EventDelegate> _listEventTurnMore = null;
    int index;
    [HideInInspector]
    public bool isChoose = false;

    public int Index
    {
        get
        {
            return index;
        }

        set
        {
            index = value;
            for (int i = 0; i < ListEventUpdateIndex.Count; ++i)
            {

                if (ListEventUpdateIndex[i].parameters != null && ListEventUpdateIndex[i].parameters.Length == 1)
                {
                    ListEventUpdateIndex[i].parameters[0].value = Index;
                }
            }
            EventDelegate.Execute(ListEventUpdateIndex);
        }
    }

    UIButton button = null;

    public UIButton Button
    {
        get
        {
            return button ? button : button = GetComponentInChildren<UIButton>();
        }

        set
        {
            button = value;
        }
    }

    public List<EventDelegate> ListEventChoose
    {
        get
        {
            return _listEventChoose;
        }

        set
        {
            _listEventChoose = value;
        }
    }

    public List<EventDelegate> ListEventUnChoosed
    {
        get
        {
            return _listEventUnChoosed;
        }

        set
        {
            _listEventUnChoosed = value;
        }
    }

    public List<EventDelegate> ListEventUpdateIndex
    {
        get
        {
            return _listEventUpdateIndex;
        }

        set
        {
            _listEventUpdateIndex = value;
        }
    }

    public List<EventDelegate> ListEventTurnMore
    {
        get
        {
            return _listEventTurnMore;
        }

        set
        {
            _listEventTurnMore = value;
        }
    }

    public void unChoosed()
    {
        isChoose = false;
        onUnChoosed();
        for (int i = 0; i < ListEventUnChoosed.Count; ++i)
        {

            if (ListEventUnChoosed[i].parameters  != null && ListEventUnChoosed[i].parameters.Length == 1)
            {
                ListEventUnChoosed[i].parameters[0].value = Index;
            }
        }
        EventDelegate.Execute(ListEventUnChoosed);
    }
    public void choosed()
    {
        isChoose = true;
        onChoosed();
        for (int i = 0; i < ListEventChoose.Count; ++i)
        {

            if (ListEventChoose[i].parameters != null && ListEventChoose[i].parameters.Length == 1)
            {
                ListEventChoose[i].parameters[0].value = Index;
            }
        }
        EventDelegate.Execute(ListEventChoose);
    }

    public void turnMore()
    {
        onTurnBtnMore();
        for (int i = 0; i < ListEventTurnMore.Count; ++i)
        {

            if (ListEventTurnMore[i].parameters != null && ListEventTurnMore[i].parameters.Length == 1)
            {
                ListEventTurnMore[i].parameters[0].value = Index;
            }
        }
        EventDelegate.Execute(ListEventTurnMore);
    }

    protected virtual void onChoosed()
    {

    }

    protected virtual void onUnChoosed()
    {

    }

    protected virtual void onTurnBtnMore()
    {

    }

    // Use this for initialization
    void Start()
    {
        //if (Button && Button.onClick.Count > 0)
        //{
        //    Button.onClick[0].parameters[0].value = index;
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
