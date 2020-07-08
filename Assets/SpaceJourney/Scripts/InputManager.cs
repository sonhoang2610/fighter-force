﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Space;

public class InputManager : Singleton<InputManager> {

    public bool isTouch
    {
        get { return !_blockTouch && (Input.touchCount > 0 || Input.GetMouseButton(0)); }
    }

    public Vector3 TouchPos
    {
        get
        {
#if ( UNITY_ANDROID || UNITY_IPHONE ) && !UNITY_EDITOR
            if (isTouch)
            {
                return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
            return Vector3.zero;
#endif
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        fixedTimeDefault = Time.fixedDeltaTime;
    }
    protected bool cacheTouch = false;
    protected float fixedTimeDefault;

    bool _blockTouch = false;
    public bool BlockTouch
    {
        get
        {
            return _blockTouch;
        }

        set
        {
            _blockTouch = value;
            if (value)
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = fixedTimeDefault;
            }
        }
    }
    private void LateUpdate()
    {
        if (LevelManger.InstanceRaw != null && LevelManger.Instance.IsPlaying && LevelManger.Instance.IsMatching && !BlockTouch)
        {
            if (cacheTouch != isTouch)
            {
                cacheTouch = isTouch;
                if (!isTouch)
                {
                    Time.timeScale = 0.3f;
                    Time.fixedDeltaTime = 0.3f * Time.fixedDeltaTime;
                }
                else
                {
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = fixedTimeDefault;
                }
            }
        }
    }
}
