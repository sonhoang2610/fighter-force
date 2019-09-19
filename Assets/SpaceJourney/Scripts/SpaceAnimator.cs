using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public enum TargetTrigger
{
    Animator,
    UnityEvent,
    Both
}

[System.Serializable]
public class TriggerControl
{
    public string _trigger;
    public TargetTrigger targetTrigger;
    [ShowIf("@targetTrigger == TargetTrigger.UnityEvent || targetTrigger == TargetTrigger.Both ")]
    public UnityEvent _onTrigger;
}
[System.Serializable]
public class SpaceAnimator 
{
    public Animator model;
    public TriggerControl[] triggerInfos;

    public void SetTrigger(string pString)
    {
        bool isFound = false;
        for(int i = 0; i < triggerInfos.Length; ++i)
        {
           if( triggerInfos[i]._trigger == pString)
            {
                isFound = true;
                if (triggerInfos[i].targetTrigger == TargetTrigger.UnityEvent)
                {
                    triggerInfos[i]._onTrigger.Invoke();
                }
                else if(model && (triggerInfos[i].targetTrigger == TargetTrigger.Animator))
                {
                    model.SetTrigger(pString);
                }else
                {
                    if (model)
                    {
                        model.SetTrigger(pString);
                    }
                    triggerInfos[i]._onTrigger.Invoke();
                }
            }
        }
        if (!isFound && model)
        {
            model.SetTrigger(pString);
        }
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
