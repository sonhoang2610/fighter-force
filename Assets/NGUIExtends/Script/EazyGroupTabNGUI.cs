using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EazyGroupTabNGUI : MonoBehaviour {
    [SerializeField]
    private List<EazyTabNGUI> groupTab;
    [SerializeField]
    private List<Transform> groupLayer;
    [SerializeField]
    List<EventDelegate> onChooseIndex;
    [SerializeField]
    int currentTab;

    public int CurrentTab
    {
        get
        {
            return currentTab;
        }

        set
        {
            currentTab = value;
        }
    }

    public List<Transform> GroupLayer
    {
        get
        {
            return groupLayer;
        }
    }

    public List<EazyTabNGUI> GroupTab
    {
        get
        {
            if(groupTab == null)
            {
                groupTab = new List<EazyTabNGUI>();
            }
            return groupTab;
        }
    }
    
    public bool isLockOnEnable = false;
    bool isFirst = true;
    public bool isReload = false;
    public IEnumerator delayInit()
    {
        yield return new WaitForSeconds(0.1f);
        changeTab(0);
    }
    private void OnEnable()
    {
        reloadTabs();
        if (isReload)
        {
            changeTab(0);
      
        }
        else
        {
            if (!isFirst && !isLockOnEnable)
            {
                if (currentTab != 0) return;
                changeTab(0);
            }
        }
        
          
    }
    // Use this for initialization
     public virtual void  Start () {

        isFirst = false;

        reloadTabs();
        if (isReload)
        {
            StartCoroutine(delayInit());
        }
        else
        {
            if (currentTab != 0) return;
            StartCoroutine(delayInit());
        }
    }


    public void reloadTabs()
    {
        for (int i = 0; i < GroupTab.Count; i++)
        {
            EventDelegate newDelegate = new EventDelegate(this, "changeTab");
            newDelegate.parameters[0].value = i;
            EventDelegate.Add(GroupTab[i].Button.onClick, newDelegate);
            GroupTab[i].Index = i;
        }
    }

    public void changeTab(int index)
    {
        CurrentTab = index;
       for (int i = 0; i < GroupLayer.Count; i++)
        {
            if (GroupLayer[i] != null)
            {
                GroupLayer[i].gameObject.SetActive(false);
            }
        }
        if (GroupLayer.Count > 0 && index < GroupLayer.Count)
        {
            if (GroupLayer[index] != null)
            {
                GroupLayer[index].gameObject.SetActive(true);
                if (GroupLayer[index].GetComponent<EazyObject>())
                {
                    GroupLayer[index].GetComponent<EazyObject>().initIndex(index);
                }
            }
        }
        for (int i = 0; i < GroupTab.Count; i++)
        {
            if (i == index)
            {
                GroupTab[i].Pressed = (true);
            }
            else
            {
                GroupTab[i].Pressed = (false);
            }
        }
        if (onChooseIndex != null)
        {
            for (int i = 0; i < onChooseIndex.Count; ++i)
            {
                if (onChooseIndex[i].parameters != null && onChooseIndex[i].parameters.Length == 1 && onChooseIndex[i].parameters[0].expectedType == typeof(int))
                {
                    onChooseIndex[i].parameters[0].value = index;
                }
            }
            EventDelegate.Execute(onChooseIndex);
        }
    }
	
	// Update is called once per frame
	void Update () {
     
	}
}
