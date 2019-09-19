using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EazyCustomAction;

public class BezierEditorTools  {
    public static void addPointCurves(BezierSpline spline, Vector3 point)
    {
        spline.AddCurve();      
        spline.AddCurveWithPoint(point);
    }



    static public bool showHandles
    {
        get
        {
#if UNITY_4_3 || UNITY_4_5
			if (showHandlesWithMoveTool)
			{
				return UnityEditor.Tools.current == UnityEditor.Tool.Move;
			}
			return UnityEditor.Tools.current == UnityEditor.Tool.View;
#else
            return UnityEditor.Tools.current == UnityEditor.Tool.Rect;

#endif
        }
    }
}

public class BezierMenuContext
{
    static List<string> mEntries = new List<string>();
    static GenericMenu mMenu;

    /// <summary>
    /// Clear the context menu list.
    /// </summary>
    public static void addItemBezierEnviroment()
    {
        Clear();
        AddItem("Add/Point Curve", false, delegate (object param) {
            GameObject pObject = Selection.activeGameObject;
            List<object> objects = ( List<object>)param;
            Vector3 point = (Vector2)objects[0];
            SceneView sceneView = (SceneView)objects[1];
            point.y = sceneView.camera.pixelHeight - point.y;
            point = sceneView.camera.ScreenToWorldPoint(point);
            point = pObject.transform.InverseTransformPoint(point);
            point.z = 0;
            BezierSpline spline = pObject.GetComponent<BezierSpline>();
            if (spline != null)
            {
   
                BezierEditorTools.addPointCurves(spline, point);
            }
        },new List<object> { Event.current.mousePosition, SceneView.currentDrawingSceneView });
    }

    static public void Clear()
    {
        mEntries.Clear();
        mMenu = null;
    }
    static public void AddItem(string item, bool isChecked, GenericMenu.MenuFunction2 callback, object param)
    {
        if (callback != null)
        {
            if (mMenu == null) mMenu = new GenericMenu();
            int count = 0;

            for (int i = 0; i < mEntries.Count; ++i)
            {
                string str = mEntries[i];
                if (str == item) ++count;
            }
            mEntries.Add(item);

            if (count > 0) item += " [" + count + "]";
            mMenu.AddItem(new GUIContent(item), isChecked, callback, param);
        }
    }
    static public void Show()
    {
        if (mMenu != null)
        {
            mMenu.ShowAsContext();
            mMenu = null;
            mEntries.Clear();
        }
    }
}
