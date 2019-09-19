#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.OdinInspector.Editor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
public class ButtonChangeValueDrawer : OdinAttributeDrawer<ButtonChangeValue>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        this.CallNextDrawer(label);

        //UnityEditor.EditorUtility.SetDirty((Object)Property.SerializationRoot.ValueEntry.WeakSmartValue);
        if (GUILayout.Button("Add Point"))
        {
            ((BezierSplineRaw)Property.BaseValueEntry.WeakSmartValue).AddCurve();
            UnityEditor.EditorUtility.SetDirty((Object)Property.SerializationRoot.ValueEntry.WeakSmartValue);
        }
        if (GUILayout.Button("Delete Last"))
        {
            ((BezierSplineRaw)Property.BaseValueEntry.WeakSmartValue).deleteLast();
            UnityEditor.EditorUtility.SetDirty((Object)Property.SerializationRoot.ValueEntry.WeakSmartValue);
        }

    }
}
#endif