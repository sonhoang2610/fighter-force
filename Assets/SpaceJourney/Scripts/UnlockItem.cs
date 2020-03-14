using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    [CreateAssetMenu(fileName = "UnlockItem", menuName ="EazyEngine/UnlockItem")]
    public class UnlockItem : BaseItemGame
    {
        public BaseItemGameInstanced[] itemEqual;
        public PlaneInfo planeUnlock;
    }
}