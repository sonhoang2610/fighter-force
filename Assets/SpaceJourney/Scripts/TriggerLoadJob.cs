using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using EazyEngine.Tools;

public class TriggerLoadJob : MonoBehaviour
{
    public TriggerLoadAsset trigger;
    public void Trigger()
    {
        EzEventManager.TriggerAssetLoaded(trigger);
    }
}
