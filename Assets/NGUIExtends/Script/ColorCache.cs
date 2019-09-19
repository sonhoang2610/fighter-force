using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCache : MonoBehaviour {

    public Color[] colors;
     UIWidget widget;
    UIWidget Widget
    {
        get
        {
            return widget ? widget : widget = GetComponent<UIWidget>();
        }
    }
    public void setColorIndex(int index)
    {
        if(colors  != null && index < colors.Length)
        {
            Widget.color = colors[index];
        }
    }
}
