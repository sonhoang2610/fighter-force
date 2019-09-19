using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NGUIAnchorExtends
{
    public static UIRect.AnchorPoint GetAnchorPoint(this UIRect rect, int index)
    {
        if (index == 0)
        {
            return rect.leftAnchor;
        }
        else if (index == 1)
        {
            return rect.topAnchor;
        }
        else if (index == 2)
        {
            return rect.rightAnchor;
        }
        else
        {
            return rect.bottomAnchor;
        }
    }
}