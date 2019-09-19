using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Timer;

public class TimeControlBehavior : MonoBehaviour
{
    TimeControllerElement _time;
    public TimeControllerElement time
    {
        get
        {
            if(_time != null)
            {
                return _time;
            }
            _time = GetComponent<TimeControllerElement>();
            if(_time == null)
            {
                _time = gameObject.AddComponent<TimeControllerElement>();
                _time._groupName = TimeKeeper.Instance.getTimeLineIndex("Global");
            }
            return _time;
        }
    }
    public TimeControllerElement timeRaw
    {
        get
        {
            if (_time != null)
            {
                return _time;
            }
         
            if (_time == null)
            {
                _time = gameObject.AddComponent<TimeControllerElement>();
            }
            return _time;
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

