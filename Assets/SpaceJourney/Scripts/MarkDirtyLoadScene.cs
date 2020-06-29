using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space{
public class MarkDirtyLoadScene : MonoBehaviour
{
    public void MarkDirtyScene(int pDirty)
    {
            SceneManager.Instance.addloading(pDirty);
    }
}
}
