using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public  class ButtonChangeValue : Attribute
{
    public string pathCompare;
    public ButtonChangeValue(string pPathCompare)
    {
        pathCompare = pPathCompare;
    }
}
