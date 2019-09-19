using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SingleUnityLayer
{
    [SerializeField]
    private int m_LayerIndex;
    public int LayerIndex
    {
        get { return m_LayerIndex; }
    }

    public void Set(int _layerIndex)
    {
        if (_layerIndex > 0 && _layerIndex < 32)
        {
            m_LayerIndex = _layerIndex;
        }
    }

    public int Mask
    {
        get { return 1 << m_LayerIndex; }
    }
}
