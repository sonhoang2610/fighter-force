using EazyEngine.Space;
//using MK.Glow.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKGlowBehavior : MonoBehaviour
{
    public int countMarkDirty = 1;
    protected int currentBloom = 0;
    private void OnEnable()
    {
        currentBloom+= countMarkDirty;
        for (int i = 0; i < currentBloom; ++i)
        {
            SceneManager.Instance.markDirtyBloomMK();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < currentBloom; ++i)
        {
            SceneManager.Instance?.removeDirtyBloomMK();
        }
        currentBloom = 0;
    }
}
