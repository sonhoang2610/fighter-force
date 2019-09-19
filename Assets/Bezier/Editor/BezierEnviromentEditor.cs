using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierEnviroment))]
public class BezierEnviromentEditor : Editor {
    private void OnEnable()
    {
        //if (Selection.gameObjects.Length > 0)
        //{
        //    GameObject gameObject = Selection.gameObjects[0];
        //    RectTransform rect = gameObject.GetComponent<RectTransform>();
        //    if (rect == null)
        //    {
        //         rect = gameObject.AddComponent<RectTransform>();
        //    }
        //    rect.sizeDelta = new Vector2(1, 1);
        //}

    }



    private void OnSceneGUI()
    {
        if (Selection.gameObjects.Length != 1) return;
        if (!BezierEditorTools.showHandles) return;

        Rect rect = UnityEditor.Tools.handleRect;
        rect.center = Selection.gameObjects[0].transform.position;
        Event e = Event.current;
        int id = GUIUtility.GetControlID(FocusType.Passive);
        EventType type = e.GetTypeForControl(id);
        Vector3 mousePosition = Event.current.mousePosition;
        SceneView sceneView = SceneView.currentDrawingSceneView;
        mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
        mousePosition = sceneView.camera.ScreenToWorldPoint(mousePosition);

        Vector2 point = e.mousePosition;
        //GUIUtility.
        switch (type)
        {
            case EventType.MouseUp:
                if (GUIUtility.hotControl == id && rect.Contains(mousePosition))
                {
                    if (e.button == 1)
                    {
                        BezierMenuContext.addItemBezierEnviroment();
                        BezierMenuContext.Show();
                    }
                }
                break;
            case EventType.MouseDown:
                if (e.button == 1 && rect.Contains(mousePosition))
                {
                    GUIUtility.hotControl = id;
                }
                break;
        }
    }
}
