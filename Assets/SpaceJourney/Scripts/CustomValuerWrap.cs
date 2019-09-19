using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public class CustomValuerWrap : CustomValueDrawerAttribute
{
    public CustomValuerWrap(string pMethodName):base(pMethodName)
    {
    }
}
