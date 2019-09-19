using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PageElement), true)]
[CanEditMultipleObjects]
public class PageElementEditor : Editor {

    public override void OnInspectorGUI()
    {
        PageElement page = target as PageElement;
        NGUIEditorTools.DrawEvents("Choosed", page, page.ListEventChoose, false);
        NGUIEditorTools.DrawEvents("Unchoosed", page, page.ListEventUnChoosed, false);
        NGUIEditorTools.DrawEvents("Update Index", page, page.ListEventUpdateIndex, false);
        NGUIEditorTools.DrawEvents("Turn More", page, page.ListEventTurnMore, false);
        serializedObject.ApplyModifiedProperties();
    }
}
