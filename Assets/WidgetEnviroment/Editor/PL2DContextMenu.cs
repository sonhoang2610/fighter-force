using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

/// <summary>
/// This editor helper class makes it easy to create and show a context menu.
/// It ensures that it's possible to add multiple items with the same name.
/// </summary>

public class PL2DContextMenu
{
    static List<string> mEntries = new List<string>();
    static GenericMenu mMenu;

    /// <summary>
    /// Clear the context menu list.
    /// </summary>

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

public class PL2DContexMenuDraw
{
    public virtual void onDraw()
    {
      
    }
}


