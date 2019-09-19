using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using EazyEngine.Space;

[CustomEditor(typeof(WeaponTrajectorPath))]

public class WeaponTrajectorPathEditor : OdinEditor
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
    private WeaponTrajectorPath module;
    private int selectedIndex = -1;
    private void OnSceneGUI()
    {
        module = (target as WeaponTrajectorPath);
        spline = module.spline;
        handleTransform = module.transform;
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
                if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
                {
                    selectedIndex = index;
                    BezierSplineRaw.slectedIndexx = selectedIndex;
                    Repaint();
                }
            
        }
        if (selectedIndex == index && index != 0)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 oldPos = point;
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(module, "Move Point");

                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
                BezierSplineRaw.slectedIndexx = 1;
                EditorUtility.SetDirty(module);
            }
        }
        return point;
    }
}
