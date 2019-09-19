using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyCustomAction;
using System;

public class NGUILabelCountAction : EazyFloatAction {
    UILabel label = null;
    public Action<UILabel, int> lateUpdateCount = null;
    public Action<UILabel, long> lateUpdateCountLong = null;
    public bool isInt = true;
    public NGUILabelCountAction()
    {
    }

    public static NGUILabelCountAction to(int pTo, float pUnit, bool calculByTime = true)
    {
        NGUILabelCountAction action = new NGUILabelCountAction();
        action.setTo(pTo, pUnit, calculByTime);
        action.isInt = true;
        return action;
    }
    public static NGUILabelCountAction to(long pTo, float pUnit, bool calculByTime = true)
    {
        NGUILabelCountAction action = new NGUILabelCountAction();
        action.setTo(pTo, pUnit, calculByTime);
        action.isInt = true;
        return action;
    }

    public NGUILabelCountAction from(int pFrom)
    {
        setFrom(pFrom);
        return this;
    }

    public NGUILabelCountAction onUpdate(Action<UILabel, int> action)
    {
        lateUpdateCount = action;
        return this;
    }

    public NGUILabelCountAction onUpdate(Action<UILabel, long> action)
    {
        lateUpdateCountLong = action;
        return this;
    }


    public static NGUILabelCountAction to(float pTo, float pUnit, bool calculByTime = true)
    {
        NGUILabelCountAction action = new NGUILabelCountAction();
        action.setTo(pTo, pUnit, calculByTime);
        action.isInt = false; 
        return action;
    }

    public NGUILabelCountAction from(float pFrom)
    {
        setFrom(pFrom);
        return this;
    }
    public override void extendCallBack(GameObject pObject)
    {
        if (label == null && pObject != null)
        {
            label = pObject.GetComponent<UILabel>();
        }
        if (isInt)
        {
            label.text = ((int)_current).ToString();
            if (lateUpdateCount != null)
            {
                lateUpdateCount(label, (int)_current);
            }
            if(lateUpdateCountLong != null)
            {
                lateUpdateCountLong(label,(long) _current);
            }
        }
        else
        {
            label.text = (_current).ToString();
        }
    }
}


public class NGUIColorAction : EazyColor3F
{
    UIWidget label = null;
    public NGUIColorAction()
    {
    }

    public static NGUIColorAction to(Vector3 pTo, float pUnit, bool calculByTime = true)
    {
        NGUIColorAction action = new NGUIColorAction();
        action.setTo(pTo, pUnit, calculByTime);
        return action;
    }


    public NGUIColorAction from(Vector3 pFrom)
    {
        setFrom(pFrom);
        return this;
    }

    public override void setUpAction(RootMotionController pRoot)
    {
        if(_typeActionBehavior < TypeBehaviorAction.FROM)
        {
            Color myColor  = (label ? label : label = pRoot.GetComponent<UIWidget>()).color;
            _from = new Vector3(myColor.r * 255.0f, myColor.g * 255.0f, myColor.b * 255.0f);
        }
        base.setUpAction(pRoot);
    }


    public override void extendCallBack(GameObject pObject)
    {
        if (label == null && pObject != null)
        {
            label = pObject.GetComponent<UIWidget>();
        }
        label.color = new Color(_current.x/255.0f, _current.y / 255.0f, _current.z / 255.0f, 1);
    }
}
