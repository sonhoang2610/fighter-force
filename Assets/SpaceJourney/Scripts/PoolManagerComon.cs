using EazyEngine.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PoolManagerComon : PoolManagerGeneric<PoolManagerComon> { 
    public GameObject createNewObject(GameObject pObject)
    {
        
        return getObjectFromPool(pObject);
    }
}
