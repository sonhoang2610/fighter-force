using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyCustomAction;
using UnityEngine.Events;
using Spine.Unity;

[EazyActionNew("NGUI Fade",true,false,typeof(UIWidget))]
public class NGUIFadeAction : EazyFloatAction {

    UIWidget widget = null;
    UIPanel panel = null;
    public UnityAction actionOnComplete = null;

    public NGUIFadeAction setOnComplete(UnityAction action)
    {
        actionOnComplete = action;
        return this;
    }
    public NGUIFadeAction()
    {
    }

    public static NGUIFadeAction to(float pTo, float pUnit, bool calculByTime = true)
    {
        NGUIFadeAction action = new NGUIFadeAction();
        action.setTo(pTo, pUnit, calculByTime);
        return action;
    }

    public NGUIFadeAction from(float pFrom)
    {
        setFrom(pFrom);
        return this;
    }

    public override void extendCallBack(GameObject pObject)
    {
        if ( panel == null && widget == null && pObject != null)
        {
            widget = pObject.GetComponent<UIWidget>();
        }
        if (widget == null && panel == null)
        {
            panel = pObject.GetComponent<UIPanel>();
        }
        if (widget)
        {
            widget.alpha = _current;
        }
        if (panel)
        {
            panel.alpha = _current;
        }
        if (percentage >= 1)
        {
            if (actionOnComplete != null)
            {
                actionOnComplete.Invoke();
            }
        }
    }

    public override void setUpAction(RootMotionController pRoot)
    {
        if (_typeActionBehavior < TypeBehaviorAction.FROM)
        {
            widget = pRoot.GetComponent<UIWidget>();
            if (widget)
            {
                _from = widget.alpha;
            }
            if (!widget)
            {
                _from = (panel ? panel : panel = pRoot.GetComponent<UIPanel>()).alpha;
            }
        }
        base.setUpAction(pRoot);

    }
}

[EazyActionNew("NGUI Height", true, false, typeof(UIWidget))]
public class NGUIHeight : EazyFloatAction
{

    UIWidget widget = null;
    public UnityAction actionOnComplete = null;

    public NGUIHeight setOnComplete(UnityAction action)
    {
        actionOnComplete = action;
        return this;
    }
    public NGUIHeight()
    {
    }

    public static NGUIHeight to(float pTo, float pUnit, bool calculByTime = true)
    {
        NGUIHeight action = new NGUIHeight();
        action.setTo(pTo, pUnit, calculByTime);
        return action;
    }

    public NGUIHeight from(float pFrom)
    {
        setFrom(pFrom);
        return this;
    }

    public override void extendCallBack(GameObject pObject)
    {
        if (widget == null && pObject != null)
        {
            widget = pObject.GetComponent<UIWidget>();
        }
        widget.height =(int) _current;
        if (percentage >= 1)
        {
            if (actionOnComplete != null)
            {
                actionOnComplete.Invoke();
            }
        }
    }

    public override void setUpAction(RootMotionController pRoot)
    {
        if (_typeActionBehavior < TypeBehaviorAction.FROM)
        {
            _from = (widget ? widget : widget = pRoot.GetComponent<UIWidget>()).height;
        }
        base.setUpAction(pRoot);

    }
}


public class SkeletonFadeAction : EazyFloatAction
{

    Spine.Unity.SkeletonAnimation ske = null;
    Spine.Unity.SkeletonMecanim skemecan = null;
    public UnityAction actionOnComplete = null;

    public SkeletonFadeAction setOnComplete(UnityAction action)
    {
        actionOnComplete = action;
        return this;
    }
    public SkeletonFadeAction()
    {
    }

    public static SkeletonFadeAction to(float pTo, float pUnit, bool calculByTime = true)
    {
        SkeletonFadeAction action = new SkeletonFadeAction();
        action.setTo(pTo, pUnit, calculByTime);
        return action;
    }

    public SkeletonFadeAction from(float pFrom)
    {
        setFrom(pFrom);
        return this;
    }

    public override void extendCallBack(GameObject pObject)
    {
        if (ske)
        {
            ske.skeleton.A = _current;
        }
        if (skemecan)
        {
            skemecan.skeleton.A = _current;
        }
        if (percentage >= 1)
        {
            if (actionOnComplete != null)
            {
                actionOnComplete.Invoke();
            }
        }
    }

    public override void setUpAction(RootMotionController pRoot)
    {
        if (_typeActionBehavior < TypeBehaviorAction.FROM)
        {
            ske = pRoot.GetComponent<SkeletonAnimation>();
            if (ske)
            {
                _from = ske.Skeleton.A;
            }
            skemecan = pRoot.GetComponent<SkeletonMecanim>();
            if (skemecan)
            {
                _from = skemecan.Skeleton.A;
            }
        }
        base.setUpAction(pRoot);

    }
}