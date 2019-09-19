using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EazyEngine.Tools;
using System.Linq;

namespace EazyEngine.Space {
    public class ItemEnviroment : PoolManagerGeneric<ItemEnviroment> { 
        public GameObject getItem(GameObject pObject)
        {
            return getObjectFromPool(pObject);
        }
    }
}
