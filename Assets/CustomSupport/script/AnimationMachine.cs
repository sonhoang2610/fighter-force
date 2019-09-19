﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMachine : StateMachineBehaviour {
    public bool _isLoopForever;
    public int _loopTime;
    public int _resetState;
    private int _currentloop = 0;
    public float _currentSClip = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _currentloop = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _currentSClip = stateInfo.normalizedTime;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
     
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(layerIndex);
        if (clips.Length > 0 && stateInfo.normalizedTime + Time.deltaTime/ clips[0].clip.length >= 1)
        {
            _currentloop++;
            if (_currentloop >= _loopTime)
            {
                animator.SetInteger("state", _resetState);
                if (animator.gameObject.GetComponent<RootMotionController>())
                {
                    animator.gameObject.GetComponent<RootMotionController>().enableRootMotion = TypeRoot.NONE;
                }
            }
        }
    }

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if (_currentloop == _loopTime)
    //    {
    //        animator.SetInteger("state", 0);
    //    }
    //}
}
