using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using EazyEngine.Tools;
public class EnableJob : MonoBehaviour
{

    public string idJob;
    public GameObject[] objectEnable;
    private void Awake()
    {
        StartCoroutine(enableJob());
    }

    public IEnumerator enableJob()
    {
        for(int i = 0; i < objectEnable.Length; ++i)
        {
            yield return new WaitForEndOfFrame();
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
