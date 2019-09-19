using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventMono : MonoBehaviour {
    [SerializeField]
    UnityEvent  _onAwake,_onEnable,_onStart, _onDisable,_onTrigger2D;

    private void Awake()
    {
        _onAwake.Invoke();
    }

    private void OnEnable()
    {
        _onEnable.Invoke();
    }

    private void Start()
    {
        _onStart.Invoke();
    }

    private void OnDisable()
    {
        _onDisable.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _onTrigger2D.Invoke();
    }
}
