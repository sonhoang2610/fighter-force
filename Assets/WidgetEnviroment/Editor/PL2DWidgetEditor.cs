using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using EazyEngine.Editor;


[CanEditMultipleObjects]
[CustomEditor(typeof(PL2dWidget))]
public class PL2DWidgetEditor : Editor
{
    bool isdown;
    Vector3 nowPos;
    private void OnSceneGUI()
    {
        if (Selection.gameObjects.Length != 1) return;
        if (!EditorTools.showHandles) return;

        PL2dWidget widget = target as PL2dWidget;
        Rect rect = UnityEditor.Tools.handleRect;
        rect.x = widget.transform.position.x - rect.width / 2;
        rect.y = widget.transform.position.y - rect.height / 2;
        // widget.Size = rect.size;
        // rect.center = Selection.gameObjects[0].transform.position;
        Event e = Event.current;
        int id = GUIUtility.GetControlID(FocusType.Passive);
        EventType type = e.GetTypeForControl(id);
        Vector3 mousePosition = Event.current.mousePosition;
        SceneView sceneView = SceneView.currentDrawingSceneView;
        mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
        mousePosition = sceneView.camera.ScreenToWorldPoint(mousePosition);
       
        switch (type)
        {
            case EventType.Repaint:
  
                break;
            case EventType.MouseDrag:
                if (isdown)
                {
                    if (rect.size != widget.Size)
                    {
                        widget.Size = new Vector2((int)rect.size.x, (int)rect.size.y);
                        widget.LockPos = nowPos;
                    }
                }
                break;
            case EventType.MouseUp:
                isdown = false;
                widget.Islock = false;
                if (GUIUtility.hotControl == id && rect.Contains(mousePosition))
                {
                    if (e.button == 1)
                    {
                        var components = Selection.activeGameObject.GetComponents<MonoBehaviour>();
                        Type[] pTypes = new Type[components.Length];
                        for(int i = 0; i < components.Length; ++i)
                        {
                            pTypes[i] = components[i].GetType();
                        }
                        PL2DContextMenu.Clear();
                        foreach (Type pType in pTypes)
                        {
                            Type pTypeDraw = EditorTools.getTypeCustomContextMenu(pType);
                            if (pTypeDraw != null)
                            {
                                PL2DContexMenuDraw draw = (PL2DContexMenuDraw)Activator.CreateInstance(pTypeDraw);
                                draw.onDraw();
                            }
                        }
                        PL2DContextMenu.Show();
                    }
                }
                break;
            case EventType.MouseDown:
                isdown = true;
                nowPos = widget.gameObject.transform.position;
                if (e.button == 1 && rect.Contains(mousePosition))
                {
                    GUIUtility.hotControl = id;
                }
                break;
        }
    }
}
[CustomPL2DContextmenu(typeof(PL2dWidget))]
public class PL2DWidgetContextMenu : PL2DContexMenuDraw
{
    public override void onDraw()
    {
        base.onDraw();
        PL2DContextMenu.AddItem("Selected/Push Front", false, EditorTools.resortWidgetUp, Selection.activeGameObject);
        PL2DContextMenu.AddItem("Selected/Push Back", false, EditorTools.resortWidgetDown, Selection.activeGameObject);
        PL2DContextMenu.AddItem("Selected/Duplicate", false, delegate (object param) {
            GameObject objectSelect = (GameObject)param;
            GameObject newGameObject = null;
            PrefabType prefabType = PrefabUtility.GetPrefabType(objectSelect);

            if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.ModelPrefabInstance)
            {
                GameObject prefab = (GameObject)PrefabUtility.GetPrefabParent(objectSelect);
                newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

                //Copy any unapplied changes from the source prefab to the new one.
                PropertyModification[] propMods = PrefabUtility.GetPropertyModifications(objectSelect);
                PrefabUtility.SetPropertyModifications(newGameObject, propMods);
            }
            newGameObject.transform.position = objectSelect.transform.position;
            newGameObject.transform.parent = objectSelect.transform.parent;
            Selection.activeGameObject = newGameObject;
        }, Selection.activeGameObject);
    }
}
