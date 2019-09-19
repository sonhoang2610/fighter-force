using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class ExpanedListEvent : FoldoutGroupAttribute
{
    public string methodNameExpaned,methodNameCollapsed;

    public ExpanedListEvent(string groupName, string pmethodExpaned = "", string pmethodcollapsed= "", bool expanded = true, int order = 0):base(groupName,expanded,order)
    {
        methodNameExpaned = pmethodExpaned;
        methodNameCollapsed = pmethodcollapsed;
    }

}
