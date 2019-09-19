using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDragPage : MonoBehaviour
{
    public float threshHoldRelativeWithPanel = 0.5f;
    public UIScrollView scrollView;


    float threshHold;
    UIPanel mPanel;
    CustomCenterChild centerChild;
    private void Awake()
    {
        centerChild = scrollView.GetComponentInChildren<CustomCenterChild>();
        mPanel = scrollView.GetComponent<UIPanel>();
        Vector3[] corners = mPanel.localCorners;
        threshHold = Vector3.Distance(corners[0], corners[2]) * threshHoldRelativeWithPanel;
    }
    bool mPressed = false;
    public Vector2 lastDelta;
    bool _block = false;
    bool _timerUnBlock = false;
    void OnPress(bool pressed)
    {
        mPressed = pressed;
        if (!mPressed)
        {
            _timerUnBlock = true;
            if (Mathf.Abs(lastDelta.x) > Mathf.Abs(threshHold))
            {
                if (lastDelta.x > 0)
                {
                    centerChild.previousPage();
                }
                else
                {
                    centerChild.nextPage();
                }
            }
            lastDelta = Vector2.zero;
        }
    }

    /// <summary>
    /// Drag the object along the plane.
    /// </summary>

    void OnDrag(Vector2 delta)
    {
      
        if (mPressed)
        {
            lastDelta += delta;
            if (Mathf.Abs(lastDelta.x) > Mathf.Abs(threshHold))
            {
                _block = true;
            }
        }
    }

}
