using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EazyReflectionSupport;

[CustomPropertyDrawer(typeof(EazyGetBoolValue))]
public class EazyGetBoolValueEditor : EazyGetValueEditor<bool>
{

}

[CustomPropertyDrawer(typeof(EazyGetFloatValue))]
public class EazyGetFloatValueEditor : EazyGetValueEditor<float>
{

}

