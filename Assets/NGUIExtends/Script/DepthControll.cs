using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthControll : MonoBehaviour {
    [SerializeField]
    UIWidget _targetIntial;
    [SerializeField]
    UIPanel _targetPanel;
    [SerializeField]
    int snap = 1;
    [SerializeField]
    bool onUpdate = true;
    UIWidget _rect;
    UIPanel _panel;

    public UIWidget rect
    {
        get
        {
            return _rect;
        }

        set
        {
            _rect = value;
        }
    }

    private void Awake()
    {
        rect = GetComponent<UIWidget>();
        _panel = GetComponent<UIPanel>();
    }
    public void setDepthUp(UIWidget target)
    {
        rect.depth = target.depth + 1;
    }

    public void setDepthDown(UIWidget target)
    {
        rect.depth = target.depth - 1;
    }

    private void OnEnable()
    {
        if (_targetIntial)
        {
            rect.depth = _targetIntial.depth + snap;
        }
        if (_targetPanel)
        {
            _panel.depth = _targetPanel.depth + snap;
        }
    }

    private void Update()
    {
        if (onUpdate)
        {
            if (_targetIntial)
            {
                rect.depth = _targetIntial.depth + snap;
            }
            if (_targetPanel)
            {
                _panel.depth = _targetPanel.depth + snap;
            }
        }
    }
}
