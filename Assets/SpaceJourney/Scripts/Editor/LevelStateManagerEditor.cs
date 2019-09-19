﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

[CustomEditor(typeof(LevelStateManager))]
public class LevelStateManagerEditor : OdinEditor
{
    private const int stepsPerCurve = 10;
    private const float directionScale = 0.5f;
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;
    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    private BezierSplineRaw spline;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private int selectedIndex = -1;

    protected override void OnDisable()
    {
        base.OnDisable();
        selectedIndex = -1;
        if (spline != null)
        {
            BezierSplineRaw.slectedIndexx = -1;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
        //{
        //    DrawSelectedPointInspector();
        //}
    }
    //private void DrawSelectedPointInspector()
    //{

    //    EditorGUI.BeginChangeCheck();
    //    BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
    //    if (EditorGUI.EndChangeCheck())
    //    {
    //        Undo.RecordObject(target, "Change Point Mode");
    //        spline.SetControlPointMode(selectedIndex, mode);
    //        EditorUtility.SetDirty(target);
    //    }
    //}
    LevelStateManager mainState;
    int currentState = 0;
    int currentMove  = 0;
    bool clicked = false;
    private void OnSceneGUI()
    {
        mainState = (target as LevelStateManager);
        if (!mainState.gameObject.scene.IsValid()) return;
        if (mainState.states == null) return;
        for (int k = 0; k < mainState.states.Length; ++k)
        {
            if (mainState.states[k].isExpaned)
            {
                float size = HandleUtility.GetHandleSize(mainState.states[k].formatInfo.startSpawnPos);
                size *= 3;
               
                Vector3 point = mainState.states[k].formatInfo.startSpawnPos;
                if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
                {
                    clicked = true;
                    Repaint();
                }
                if (clicked)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 oldPos = point;
                    point = Handles.DoPositionHandle(point, handleRotation);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(mainState, "Start Point");

                        mainState.states[k].formatInfo.startSpawnPos = point;
                        mainState.changeStartValue(k, oldPos, point);
                        EditorUtility.SetDirty(mainState);
                    }
                }
                for (int g = 0; g < mainState.states[k].moveInfos.Length; g++)
                {
                    currentMove = g;
                    currentState = k;
                    spline = mainState.states[k].moveInfos[g].splineRaw;
                    handleTransform = mainState.transform;
                    handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                        handleTransform.rotation : Quaternion.identity;

                    Vector3 p0 = ShowPoint(0);
                    for (int i = 1; i < spline.ControlPointCount; i += 3)
                    {
                        Vector3 p1 = ShowPoint(i);
                        Vector3 p2 = ShowPoint(i + 1);
                        Vector3 p3 = ShowPoint(i + 2);

                        Handles.color = Color.red;
                        if (!spline.LineMode)
                        {
                            Handles.DrawLine(p0, p1);
                            Handles.DrawLine(p2, p3);
                            Handles.DrawBezier(p0, p3, p1, p2, Color.cyan, null, 2f);
                        }
                        else
                        {
                            Handles.DrawLine(p0, p3);
                        }
                        p0 = p3;
                    }
                }
            }
          
        }
    }

    private void ShowDirections()
    {
        //Handles.color = Color.green;
        //Vector3 point = spline.GetPoint(0f);
        //Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        //int steps = stepsPerCurve * spline.CurveCount;
        //for (int i = 1; i <= steps; i++) {
        //	point = spline.GetPoint(i / (float)steps);
        //	Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        //}
    }
    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);
        if (index == 0)
        {
            size *= 2.5f;
        }
        else if (index % 3 == 0)
        {
            size *= 2f;
        }
        Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
        if (index % 3 == 0 || !spline.LineMode)
        {
            if (index != 0)
            {
                if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
                {
                    selectedIndex = index;
                    BezierSplineRaw.slectedIndexx = selectedIndex;
                    Repaint();
                }
            }
        }
        if (selectedIndex == index && index != 0)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 oldPos = point;
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(mainState, "Move Point");
           
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
                BezierSplineRaw.slectedIndexx = 1;
                mainState.changeValue(currentState,currentMove, index, oldPos,point);
                EditorUtility.SetDirty(mainState);
            }
        }
        return point;
    }
}
