using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using EazyEngine.Tools;
public class EnableJob : MonoBehaviour
{

    public string idJob;
    public int countItemPreloadPerFrame = 3;
    public GameObject[] objectEnable;

    protected int currentPreload = 0;
    private void Awake()
    {
        StartCoroutine(enableJob());
    }

    public IEnumerator enableJob()
    {
        currentPreload = 0;
        for (int i = 0; i < objectEnable.Length; ++i)
        {
            if (currentPreload >= countItemPreloadPerFrame)
            {
                yield return new WaitForEndOfFrame();
                currentPreload = 0;
            }
            currentPreload++;
            objectEnable[i].gameObject.SetActive(true);
            if(i < objectEnable.Length - 1)
            {
                EzEventManager.TriggerAssetLoaded(new TriggerLoadAsset() { name = idJob, percent = (i + 1) / (objectEnable.Length) });
            }
            else
            {
                EzEventManager.TriggerAssetLoaded(new TriggerLoadAsset() { name = idJob, percent =1});
            }
           
        }
    }
}
