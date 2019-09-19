using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EazyEngine.Timer;

public class TimeControlSrializeBehavior : SerializedMonoBehaviour
{
    TimeControllerElement _time;
    public TimeControllerElement time
    {
        get
        {
            if (_time != null)
            {
                return _time;
            }
            _time = GetComponent<TimeControllerElement>();
            if (_time == null)
            {
                _time = gameObject.AddComponent<TimeControllerElement>();
                _time._groupName = 1;
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
