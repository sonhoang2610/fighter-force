using EazyEngine.Space;
using MK.Glow.Legacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKGlowBehavior : MonoBehaviour
{
    protected int callBloom = 0;
    private void OnEnable()
    {
        callBloom++;
        SceneManager.Instance.markDirtyBloomMK();
    }

    private void OnDisable()
    {
        for (int i = 0; i < callBloom; ++i)
        {
            SceneManager.Instance?.removeDirtyBloomMK();
        }
        callBloom = 0;
    }
}
