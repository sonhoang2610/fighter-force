using EazyEngine.Space;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseFPSBehavior : MonoBehaviour
{
    public bool negative = false;
    protected int callFps = 0;
    // Start is called before the first frame update
    private void OnEnable()
    {
        callFps++;
       if(!negative) SceneManager.Instance.markDirtySlowFps();
       else SceneManager.Instance.removeDirtySlowFps();
    }

    // Update is called once per frame
    private void OnDisable()
    {
        for (int i = 0; i < callFps; ++i)
        {
            if (!negative) SceneManager.Instance.removeDirtySlowFps();
            else SceneManager.Instance.markDirtySlowFps();
        }
        callFps = 0;
    }
}
