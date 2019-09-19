using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EazyEngine.Tools;

//namespace EazyEngine.Tools
//{

[System.Serializable]
public struct QueueEffect
{
    public string nameEffect;
    public GameObject[] prefab;
    public bool isIgnore;
    public int countPreload;
}
public class ParticleEnviroment : PoolManagerGeneric<ParticleEnviroment>
{

    public GameObject createEffect(GameObject pObject, Vector3 pos, int orderLayer = 0, bool isLocal = false)
    {
       GameObject pNewObject =   getObjectFromPool(pObject);
        if (!isLocal)
        {
            pNewObject.transform.position = pos;
        }
        else
        {
            pNewObject.transform.localPosition = pos;
        }
        if (orderLayer != 0)
        {
            var renders = pNewObject.GetComponentsInChildren<ParticleSystemRenderer>();
            foreach (var pRender in renders)
            {
                pRender.sortingOrder = orderLayer;
            }
        }
        pNewObject.SetActive(true);
        return pNewObject;
    }


}
