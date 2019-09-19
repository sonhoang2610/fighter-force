using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventBool : UnityEvent<bool> {
}
public class ToogleSlider : MonoBehaviour {
    [SerializeField]
    EazyFrameCache backGround, slide, icon;
    [SerializeField]
    GameObject posTrue, posFalse;
    [SerializeField]
    bool value;
    [SerializeField]
    EventBool eventOnChange;

    public bool Value
    {
        get
        {
            return value;
        }

        set
        {
            this.value = value;
            updateValue();
        }
    }

    public void turnValue()
    {
        Value = !Value;
        updateValue();
        eventOnChange.Invoke( Value);
    }

    public void updateValue()
    {
        slide.transform.position = Value ? posTrue.transform.position : posFalse.transform.position;
        slide.setFrameIndex(Value ? 1 : 0);
        backGround.setFrameIndex(Value ? 1 : 0);
        if (icon)
        {
            icon.setFrameIndex(Value ? 1 : 0);
        }
    }

    private void Awake()
    {
        updateValue();
    }
}
