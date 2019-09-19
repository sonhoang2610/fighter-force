using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class NGUIParameterInt : MonoBehaviour {
    UIButton button;
    [SerializeField]
    bool isIgnoreButton;

    [SerializeField]
    GameObject _target;

    [SerializeField]
    string nameComponent;
    [SerializeField]
    string nameEventDelegate;

    public UIButton Button
    {
        get
        {
            return button ? button : button = GetComponent<UIButton>();
        }
    }

    public int[] Parameters
    {
        get
        {
            return parameters;
        }

        set
        {
            parameters = value;
        }
    }

    [SerializeField]
    int[] parameters = null;

    public void updateParameters()
    {
        if (Button && Button.onClick.Count > 0 && !isIgnoreButton)
        {
            for (int i = 0; i < Button.onClick.Count; i++)
            {
                if (Button.onClick[i].parameters != null && Button.onClick[i].parameters.Length > 0)
                {
                    for (int j = 0; j < Parameters.Length; j++)
                    {
                        if (j < Button.onClick[i].parameters.Length && Button.onClick[i].parameters[j].expectedType == typeof(int))
                        {
                            Button.onClick[i].parameters[j].value = Parameters[j];
                        }
                    }
                }
            }
        }
        if (nameEventDelegate != "" && _target != null && nameComponent != "")
        {
            Component compo = _target.GetComponent(nameComponent);
            EventDelegate _event = null;
            List<EventDelegate> _events = null;
            PropertyInfo prop = compo.GetType().GetProperty(nameEventDelegate);
            if (prop != null)
            {
                if (prop.GetValue(compo, null).GetType() == typeof(EventDelegate))
                {
                    _event = (EventDelegate)prop.GetValue(compo, null);
                }
                else if (prop.GetValue(compo, null).GetType() == typeof(List<EventDelegate>))
                {
                    _events = (List<EventDelegate>)prop.GetValue(compo, null);
                }
            }
            if (_event == null)
            {
                FieldInfo field = compo.GetType().GetField(nameEventDelegate);
                if (field != null)
                {
                    if (field.GetValue(compo).GetType() == typeof(EventDelegate))
                    {
                        _event = (EventDelegate)field.GetValue(compo);
                    }
                    else if (field.GetValue(compo).GetType() == typeof(List<EventDelegate>))
                    {
                        _events = (List<EventDelegate>)field.GetValue(compo);
                    }
                }
            }
            if (_event != null)
            {
                for (int j = 0; j < Parameters.Length; j++)
                {
                    if (j < _event.parameters.Length && _event.parameters[j].expectedType == typeof(int))
                    {
                        _event.parameters[j].value = Parameters[j];
                    }
                }

            }
            if (_events != null)
            {
                for (int i = 0; i < _events.Count; i++)
                {
                    if (_events[i].parameters != null && _events[i].parameters.Length > 0)
                    {
                        for (int j = 0; j < Parameters.Length; j++)
                        {
                            if (j < _events[i].parameters.Length && _events[i].parameters[j].expectedType == typeof(int))
                            {
                                _events[i].parameters[j].value = Parameters[j];
                            }
                        }
                    }
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        if (Button && Button.onClick.Count > 0)
        {
            for (int i = 0; i < Button.onClick.Count; i++)
            {
                if (Button.onClick[i].parameters != null && Button.onClick[i].parameters.Length > 0)
                {
                    for (int j = 0; j < Parameters.Length; j++)
                    {
                        if (j < Button.onClick[i].parameters.Length && Button.onClick[i].parameters[j].expectedType == typeof(int))
                        {
                            Button.onClick[i].parameters[j].value = Parameters[j];
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateParameters();
    }

    private void Awake()
    {
        updateParameters();
    }

}
