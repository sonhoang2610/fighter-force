using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ExecuteOn {START,ENABLE,UPDATE };
public enum AnchorType { ONE_TARGET,MULTI_TARGET};
[ExecuteInEditMode]
public class NguiAnchor : MonoBehaviour {
    private UIWidget widget; 


    // Use this for initialization
    public ExecuteOn _ExecuteOn;
    public Transform mainTarget;
    public Transform targetRight;
    public Vector2 anchorRight = new Vector2(1,0);
    public Transform targetLeft;
    public Vector2 anchorLeft = new Vector2(0, 0);
    public Transform targetTop;
    public Vector2 anchorTop = new Vector2(1, 0);
    public Transform targetBottom;
    public Vector2 anchorBottom = new Vector2(0, 0);

    Vector2 originalSize;

    public UIWidget Widget
    {
        get
        {
            return widget ? widget : widget = GetComponent<UIWidget>();
        }

        set
        {
            widget = value;
        }
    }

    private void OnEnable()
    {
        Widget.leftAnchor.target = null;
        Widget.rightAnchor.target = null;
        Widget.topAnchor.target = null;
        Widget.bottomAnchor.target = null;
        widget.ResetAnchors();
        Widget.MakePixelPerfect();
        originalSize = new Vector2(Widget.width, Widget.height);
    }
    void Start () {
        if (_ExecuteOn == ExecuteOn.START)
        {
            mathAchorPoint();
            Widget.leftAnchor.target = null;
            Widget.rightAnchor.target = null;
            Widget.topAnchor.target = null;
            Widget.bottomAnchor.target = null;
        }
    }

    void mathAchorPoint()
    {
        Widget.leftAnchor.target = mainTarget ? mainTarget: targetLeft;
        Widget.rightAnchor.target = mainTarget ? mainTarget : targetRight;
        Widget.topAnchor.target = mainTarget ? mainTarget : targetTop;
        Widget.bottomAnchor.target = mainTarget ? mainTarget : targetBottom;
        if (mainTarget)
        {
            Widget.leftAnchor.Set(anchorLeft.x, anchorLeft.y);
            Widget.rightAnchor.Set(anchorRight.x, anchorRight.y);
            Widget.topAnchor.Set(anchorTop.x, anchorTop.y);
            Widget.bottomAnchor.Set(anchorBottom.x, anchorTop.y);
        }
        else
        {
            if (targetLeft)
            {
                Widget.leftAnchor.Set(anchorLeft.x, anchorLeft.y);
                if (!targetRight)
                {
                    Widget.rightAnchor.Set(anchorLeft.x, anchorLeft.y + originalSize.x);
                    Widget.rightAnchor.target = targetLeft;
                }
            }
            if (targetRight)
            {
                Widget.rightAnchor.Set(anchorRight.x, anchorRight.y);
                if (!targetLeft)
                {
                    Widget.leftAnchor.target = targetRight;
                    Widget.leftAnchor.Set(anchorRight.x, anchorRight.y - originalSize.x);         
                }
            }
        }

        Widget.ResetAnchors();
        Widget.UpdateAnchors();
    }

	// Update is called once per frame
	void Update () {
	
	}
    private void LateUpdate()
    {
        if (_ExecuteOn == ExecuteOn.UPDATE)
        {
            mathAchorPoint();
        }
    }
}
