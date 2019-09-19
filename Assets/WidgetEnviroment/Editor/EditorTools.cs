using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EazyEngine.Editor
{
    public static class EditorTools
    {

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
        public static void resortWidgetUp(object pObject)
        {
            int nextOrder = resortPLWidget(pObject);
            GameObject gameObject = ((GameObject)pObject);
            PL2dWidget select = gameObject.GetComponent<PL2dWidget>();
            select.OrderLayer = nextOrder;
            select.OnValidate();
        }

        public static void resortWidgetDown(object pObject)
        {
            int nextOrder = resortPLWidget(pObject, 1);
            GameObject gameObject = ((GameObject)pObject);
            PL2dWidget select = gameObject.GetComponent<PL2dWidget>();
            PL2dWidget parrentWidget = gameObject.transform.parent.GetComponent<PL2dWidget>();
            select.OrderLayer = parrentWidget.OrderLayer + 1;
            select.OnValidate();
        }
        static int resortPLWidget(object pObject, int pStartIndex = 0)
        {
            GameObject gameObject = ((GameObject)pObject);
            PL2dWidget select = gameObject.GetComponent<PL2dWidget>();
            PL2dWidget parrentWidget = gameObject.transform.parent.GetComponent<PL2dWidget>();
            PL2dWidget[] plWdigets = parrentWidget.gameObject.GetComponentsInChildren<PL2dWidget>();
            Array.Sort(plWdigets, CompareLayer);

            int nextOrder = 1 + pStartIndex;
            for (int i = 0; i < plWdigets.Length; ++i)
            {
                if (parrentWidget != plWdigets[i] && select != plWdigets[i])
                {
                    plWdigets[i].OrderLayer = parrentWidget.OrderLayer + nextOrder;
                    plWdigets[i].OnValidate();
                    nextOrder++;
                }
            }
            return parrentWidget.OrderLayer + nextOrder; ;

        }

        public static Type getTypeCustomContextMenu(Type pType)
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assems)
            {
                foreach (Type t in a.GetTypes())
                {
                    foreach (object obs in t.GetCustomAttributes(typeof(CustomPL2DContextmenu), true))
                    {
                        CustomPL2DContextmenu att = (CustomPL2DContextmenu)obs;
                        if (att._type == pType)
                        {
                            return t;
                        }
                    }
                }
            }
            return null;
        }

        private static int CompareLayer(PL2dWidget x, PL2dWidget y)
        {
            return x.OrderLayer.CompareTo(y.OrderLayer);
        }
    }
    public sealed class CustomPL2DContextmenu : Attribute
    {
        public Type _type;

        public CustomPL2DContextmenu(Type pType)
        {
            _type = pType;
        }

    }
}
