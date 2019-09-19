using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EazyReflectionSupport;
using System.Reflection;

[Serializable]
public abstract class EazyGetValue<T>
{
    public TypeReflection typeReflection;
    public T value;
    public GameObject target;
    public string selected;
    public string componentString, methodString;

    public T Value
    {
        get
        {
            if (typeReflection == TypeReflection.Method)
            {
                var component = target.GetComponent(componentString);
                return (T)component.GetType().GetMethod(methodString).Invoke(component,null);
            }
            else
            if (typeReflection == TypeReflection.Properties)
            {
                var component = target.GetComponent(componentString);
                return (T)component.GetType().GetProperty(methodString).GetValue(component,null);
            }
            else if (typeReflection == TypeReflection.Field)
            {
                var component = target.GetComponent(componentString);
                FieldInfo pField = component.GetType().GetField(methodString);
                return (T)pField.GetValue(component);
            }
            return value;
        }

        set
        {
            this.value = value;
        }
    }
}

