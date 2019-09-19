using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EazyReflectionSupport;
using System.Reflection;

public abstract class EazyGetValueEditor<T> : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {       
        Rect current = position;
        current.height = 16;
        current.width = 100;
        if (GUI.Button(current, "show"))
        {
            SerializedProperty prop = property.FindPropertyRelative("typeReflection");
            EditorGUI.PropertyField(current, prop, new GUIContent());
            //position.width -= 100;
            //position.x += 100;
            TypeReflection typeReflection = (TypeReflection)prop.enumValueIndex;
            if (typeReflection != (int)TypeReflection.None)
            {
                prop = property.FindPropertyRelative("target");
                current = position;
                current.x += 100;
                current.height = 16;
                current.width /= 2;
                current.width -= 15;
                EditorGUI.ObjectField(current, prop, new GUIContent());
                string[] methoStrings = new string[0];
                EazyMethodInfo[] methos = null;
                if (prop != null && prop.objectReferenceValue != null)
                {
                    GameObject gameObject = (GameObject)prop.objectReferenceValue;
                    if (typeReflection == TypeReflection.Method)
                    {
                        methos = gameObject.getAllEzMethodReturnType(typeof(T));
                    }
                    else if (typeReflection == TypeReflection.Properties)
                    {
                        methos = gameObject.getAllEzPropertyreturnType(typeof(T));
                    }
                    else
                    {
                        methos = gameObject.getAllEzFieldreturnType(typeof(T));
                    }
                    methoStrings = methos.convertToStringMethods();
                }
                System.Array.Resize(ref methoStrings, methoStrings.Length + 1);
                methoStrings[methoStrings.Length - 1] = "None";

                current.x += position.width / 2;
                current.width = position.width / 2;
                prop = property.FindPropertyRelative("selected");
                int index = methoStrings.findIndex(prop.stringValue);
                if (index >= 0)
                {
                    index = EditorGUI.Popup(current, index, methoStrings);
                    prop.stringValue = methoStrings[index];
                    if (methos != null && index < methos.Length)
                    {
                        property.FindPropertyRelative("componentString").stringValue = methos[index].componentString;
                        property.FindPropertyRelative("methodString").stringValue = methos[index].methodString;
                    }
                }
                else
                {
                    prop.stringValue = "None";
                }

                current = position;
                current.y += 18;
                current.height = 18;
                prop = property.FindPropertyRelative("value");
                EditorGUI.PropertyField(current, prop, new GUIContent());
            }
            else
            {
                current = position;
                current.x += 20;
                prop = property.FindPropertyRelative("value");
                EditorGUI.PropertyField(current, prop, new GUIContent());
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty prop = property.FindPropertyRelative("typeReflection");
        TypeReflection typeReflection = (TypeReflection)prop.enumValueIndex;
        if (typeReflection != (int)TypeReflection.None)
        {
            return 36; 
        }
            return 18;
    }
}


