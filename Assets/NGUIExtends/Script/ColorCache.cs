using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCache : MonoBehaviour {

    public Color[] colors;
    public bool isOutlineLabel;
     UIWidget widget;
    UIWidget Widget
    {
        get
        {
            return widget ? widget : widget = GetComponent<UIWidget>();
        }
    }
    UILabel label;
    UILabel Label
    {
        get
        {
            return label ? label : label = GetComponent<UILabel>();
        }
    }

    public void setBehaviorIndex(int pIndex)
    {
        setColorIndex(pIndex);
    }

    public void setColorIndex(int index)
    {
        if (isOutlineLabel)
        {
            if (colors != null && index < colors.Length)
            {
                Label.effectColor = colors[index];
            }
        }
        else
        {
            if (colors != null && index < colors.Length)
            {
                Widget.color = colors[index];
            }
        }
     
    }
}
