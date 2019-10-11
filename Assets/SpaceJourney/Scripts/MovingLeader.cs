using DG.Tweening;
using EazyEngine.Space;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
[System.Serializable]

public class ObservableListGroupElement : ObservableList<GroupElement>
{

}

public class MovingLeader : TimeControlBehavior
{

    [HideInEditorMode]
    public ObservableListGroupElement elements = new ObservableListGroupElement();
    [HideInInspector]
    public MoveInfo _moveInfo;
    [HideInInspector]
    public LevelState _parentState;
    [HideInInspector]
    public GroupManager _manager;
    [HideInInspector]
    public int currentStep = 0;
    bool stopRotation = true;
    protected int setUpIndex;
    [SerializeField]
    [HideInEditorMode]
    protected bool completeAllandWait = false;

    private void OnEnable()
    {
        completeAllandWait = false;
        setUpIndex = 0;
        stopRotation = true;
        currentStep = 0;
    }


    public virtual void setInfoMove(MoveInfo pInfo, int indexLeader)
    {
        _moveInfo = pInfo;
        setUpIndex = indexLeader;
        for (int i = 0; i < elements.Count; ++i)
        {
            float pSpeed = pInfo.speedBase ? pInfo.durationMove : (pInfo.splineRaw.totalLength() / pInfo.durationMove);
            elements[i].speedSpringPoss = pSpeed < elements[i].speedSpringPoss ? elements[i].speedSpringPoss : pSpeed;
        }
        if (_moveInfo.lookType == TypeLook.LookDirection)
        {
            stopRotation = false;
        }
        if (_moveInfo != null)
        {
            RootMotionController.stopAllAction(gameObject);
            for (int j = 0; j < elements.Count; ++j)
            {
                elements[j].SendMessage("moveAfter", _moveInfo.DelayStart + indexLeader * pInfo.RowDelay,SendMessageOptions.DontRequireReceiver);
             }
            RootMotionController.runAction(gameObject, EazyCustomAction.Sequences.create(EazyCustomAction.DelayTime.create(_moveInfo.DelayStart + indexLeader * pInfo.RowDelay), BezierWalkAction.create(pInfo.splineRaw, pInfo.durationMove, !pInfo.speedBase).setNodeEvent(
                delegate(int pNode)
                {
                    for(int i = 0; i < _moveInfo.onCompleteNode.Length; ++i)
                    {
                        if(_moveInfo.onCompleteNode[i].nodeListen == pNode || _moveInfo.onCompleteNode[i].nodeListen == -1)
                        {
                            for(int  j = 0; j < elements.Count; ++j)
                            {
                                _moveInfo.onCompleteNode[i].onComplate.Invoke(elements[j].gameObject);
                              
                            }
                        }
                    }
                }
                ).loop(_moveInfo.loop).setCurve(pInfo.curvesMoving), EazyCustomAction.CallFunc.create(onComplete)).setCustomDeltaTime(()=>time.deltaTime));
        }
        //if (_moveInfo != null)
        //{
        //    RootMotionController.runAction(gameObject, EazyCustomAction.Sequences.create(EazyCustomAction.DelayTime.create(_moveInfo.DelayStart + indexLeader * pInfo.RowDelay), BezierWalkAction.create(pInfo.splineRaw, pInfo.durationMove, !pInfo.speedBase).loop(_moveInfo.loop)).setCustomDeltaTime(() => time.deltaTime));
        //}
        transform.hasChanged = false;
    }

    Vector2 oldPos = Vector2.zero;
    private void LateUpdate()
    {
        if (transform.hasChanged && !stopRotation)
        {
            Vector2 pDirection = ((Vector2)transform.localPosition - oldPos).normalized;
            transform.RotationDirect2D(pDirection, TranformExtension.FacingDirection.DOWN);
            oldPos = transform.localPosition;
        }
    }
    public void triggerRightPositionElement()
    {
        if (completeAllandWait)
        {
            onComplete();
        }
    }
    public bool rightPlaceAll()
    {
        for(int i  = 0; i < elements.Count; ++i)
        {
            if (!elements[i].RightPlace && elements[i].gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }


    public virtual void onComplete()
    {
        stopRotation = true;
        if (_moveInfo == _parentState.moveInfos[_parentState.moveInfos.Length - 1])
        {
            if (!rightPlaceAll())
            {
                completeAllandWait = true;
                return;
            }
        }     
        for (int i = 0; i < _moveInfo.conditionComplete.Length; ++i)
        {
            if (_moveInfo.conditionComplete[i].condition == ConditionEndMoveType.EndMove)
            {
                nextStep();
            }
        }

    }
    public void nextStep()
    {
        _moveInfo.onComplete.Invoke(_manager.gameObject);
        for(int i = 0; i < elements.Count; ++i)
        {
            _moveInfo.onComplete.Invoke(elements[i].gameObject);
        }
        currentStep++;
        if (currentStep < _parentState.moveInfos.Length)
        {
            _moveInfo = _parentState.moveInfos[currentStep];
            for (int i = 0; i < _moveInfo.conditionComplete.Length; ++i)
            {
                if (_moveInfo.conditionComplete[i].condition == ConditionEndMoveType.DestroyAll)
                {
                    if (_parentState.totalDeath >= _parentState.formatInfo.quantity)
                    {
                        nextStep();
                        return;
                    }
                }
                else if (_moveInfo.conditionComplete[i].condition == ConditionEndMoveType.DestroyQuantity)
                {
                    if (_parentState.totalDeath >= _moveInfo.conditionComplete[i].destroyQuantity)
                    {
                        nextStep();
                        return;
                    }
                }
            }
            setInfoMove(_moveInfo, setUpIndex);
        }
        else
        {
            onCompleteStateForthis();
        }
    }
    public void onCompleteStateForthis()
    {
        _manager.onComplete(this);

    }
    public void OnEzEvent(MessageGamePlayEvent eventType)
    {
        for (int i = 0; i < _moveInfo.conditionComplete.Length; ++i)
        {
            if (_moveInfo.conditionComplete[i].condition == ConditionEndMoveType.CustomTrigger && _moveInfo.conditionComplete[i].triggerString == eventType._message)
            {
                nextStep();
            }
        }
    }
}
