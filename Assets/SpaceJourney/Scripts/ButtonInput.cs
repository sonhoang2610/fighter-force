using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Space;
using Sirenix.OdinInspector;
#if  UNITY_EDITOR
using UnityEditor;
#endif

public struct InputButtonTrigger
{
    public string trigger;
    public int cateGory;
    public int indexTarget;
    public InputButtonTrigger(string pID,CategoryItem pCateGory,int pTarget = 0)
    {
        trigger = pID;
        indexTarget = pTarget;
        cateGory =(int) pCateGory;
    }
}



