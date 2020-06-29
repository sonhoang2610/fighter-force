using EazyEngine.Space;
using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ListenJob
{
    public string JobID;
    public GameObject objectEnable;
}
public class EnableListenJob : MonoBehaviour,EzEventListener<TriggerLoadAsset>
{
    [System.NonSerialized]
    public string currentJob;
    [System.NonSerialized]
    public string jobDone;
    public ListenJob[] jobs;
    private void Awake()
    {
        StartCoroutine(enableJob());
    }
    public IEnumerator enableJob()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < jobs.Length; ++i)
        {
            currentJob = jobs[i].JobID;
            jobs[i].objectEnable.SetActive(true);
            while (currentJob != jobDone)
            {
                yield return new WaitForEndOfFrame();
            }
            jobs[i].objectEnable.SetActive(false);

        }
    }
    private void OnEnable()
    {
        EzEventManager.AddListener(this);
    }
    private void OnDisable()
    {
        EzEventManager.RemoveListener(this);
    }

    public void OnEzEvent(TriggerLoadAsset eventType)
    {
        if(!string.IsNullOrEmpty(currentJob) && eventType.name.Contains(currentJob) && AssetLoaderManager.Instance.getPercentJob(currentJob) >= 1)
        {
            jobDone = currentJob;
        }
    }
}

