using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventUnity : MonoBehaviour {
    public UnityEvent _event;

    public void excute()
    {
        _event.Invoke();
    }
}
