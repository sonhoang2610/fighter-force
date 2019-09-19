using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAbility : MonoBehaviour
{
    public MoveInfo _moveInfo;


    public virtual void setInfoMove(MoveInfo pInfo,int pIndex)
    {
        _moveInfo = pInfo;
        if (_moveInfo != null)
        {
            RootMotionController.runAction(gameObject, EazyCustomAction.Sequences.create(EazyCustomAction.DelayTime.create(_moveInfo.DelayStart + pIndex * pInfo.RowDelay), BezierWalkAction.create(pInfo.splineRaw, pInfo.durationMove, !pInfo.speedBase).loop(_moveInfo.loop)));
        }
    }

    public virtual void onComplete()
    {

    }

}
