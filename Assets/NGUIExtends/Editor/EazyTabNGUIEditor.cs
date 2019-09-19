using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(EazyTabNGUI),true)]
[CanEditMultipleObjects]
public class EazyTabNGUIEditor : Editor {

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField( serializedObject.FindProperty("colorNormal"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("colorPressed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("includeChild"));
        EazyTabNGUI tab = target as EazyTabNGUI;
        UISprite sprite = tab.GetComponent<UISprite>();
        UI2DSprite sprite2d = tab.GetComponent<UI2DSprite>();
        if (sprite)
        {
            NGUIEditorTools.BeginContents(true);
            SerializedObject obj = new SerializedObject(sprite);
        //    obj.Update();
            SerializedProperty atlas = obj.FindProperty("mAtlas");
            // NGUIEditorTools.DrawSpriteField("Normal", obj, atlas, obj.FindProperty("mSpriteName"));
            // obj.ApplyModifiedProperties();
            
            NGUIEditorTools.DrawSpriteField("Nomal", serializedObject, atlas, serializedObject.FindProperty("normalSprite"), true);
            NGUIEditorTools.DrawSpriteField("Pressed", serializedObject, atlas, serializedObject.FindProperty("pressedSprite"), true);
   
            NGUIEditorTools.EndContents();
        }else if (sprite2d)
        {
            NGUIEditorTools.BeginContents(true);
            SerializedObject obj = new SerializedObject(sprite2d);
            NGUIEditorTools.DrawProperty("Nomal", serializedObject, "normalSprite2D");
            NGUIEditorTools.DrawProperty("Pressed", serializedObject, "pressedSprite2D");

        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isSetParameter"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("eventOnPressed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("eventOnUnPress"));
        serializedObject.ApplyModifiedProperties();
    }
}
