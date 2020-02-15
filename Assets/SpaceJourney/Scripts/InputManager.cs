using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Space;

public class InputManager : Singleton<InputManager> {

    public bool isTouch
    {
        get { return Input.touchCount > 0 || Input.GetMouseButton(0); }
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
    private void LateUpdate()
    {
        if (LevelManger.InstanceRaw != null && LevelManger.Instance.IsPlaying && LevelManger.Instance.IsMatching)
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
