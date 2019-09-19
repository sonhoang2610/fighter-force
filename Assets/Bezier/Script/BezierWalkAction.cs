using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyCustomAction;
using UnityEngine.Events;
public class BezierWalkAction : EazyFloatAction
{
    public BezierSplineRaw spline;
    public System.Action<int> onNodeMoveEvent;
    protected int currentNode = -1;

    public BezierWalkAction setNodeEvent(System.Action<int> pEvent)
    {
        onNodeMoveEvent = pEvent;
        return this;
    }
    public override void extendCallBack(GameObject pObject)
    {
        base.extendCallBack(pObject);
        pObject.transform.localPosition = spline.GetPoint(_current);
        if(currentNode != spline.currentNode)
        {
            currentNode = spline.currentNode;
            if (onNodeMoveEvent != null)
            {
                onNodeMoveEvent(currentNode);
            }
        }
    }
    public static BezierWalkAction create(BezierSplineRaw pSpline,float pUnit,bool pCalculByTime = true)
    {
        BezierWalkAction pAction = new BezierWalkAction();
        pAction.spline = pSpline;
        pAction.setTo(1, pUnit, pCalculByTime);
        pAction.setFrom(0);
        return pAction;
    }
    public override void setUpAction(RootMotionController pRoot)
    {
        base.setUpAction(pRoot);
        if (!_calculByTime)
        {
            float pLength = spline.totalLength();
            _time = pLength / _unit;
        }
        
    }
}
