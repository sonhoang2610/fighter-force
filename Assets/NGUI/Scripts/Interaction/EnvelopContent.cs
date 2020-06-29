//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2019 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>
/// This script is capable of resizing the widget it's attached to in order to
/// completely envelop targeted UI content.
/// </summary>

[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/Interaction/Envelop Content")]
[ExecuteInEditMode]
public class EnvelopContent : MonoBehaviour
{
    public Transform targetRoot;
    public int padLeft = 0;
    public int padRight = 0;
    public int padBottom = 0;
    public int padTop = 0;
    public int minW, minH;
    public bool ignoreDisabled = true;
    public float timeUpdate = -1;
    protected float currentTime = 0;
    private void Update()
    {
        if (!Application.isPlaying)
        {
            Execute();
        }
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                Execute();
                currentTime = timeUpdate;
            }
        }
    }
    

    bool mStarted = false;

    void Start()
    {
        if (timeUpdate > 0)
        {
            currentTime = timeUpdate;
        }

        mStarted = true;
        Execute();
    }

    void OnEnable() { if (mStarted) Execute(); }

    [ContextMenu("Execute")]
    public void Execute()
    {
        if (!targetRoot) return;
        if (targetRoot == transform)
        {
            Debug.LogError("Target Root object cannot be the same object that has Envelop Content. Make it a sibling instead.", this);
        }
        else if (NGUITools.IsChild(targetRoot, transform))
        {
            Debug.LogError("Target Root object should not be a parent of Envelop Content. Make it a sibling instead.", this);
        }
        else
        {
            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(transform.parent, targetRoot, !ignoreDisabled);
            float x0 = b.min.x + padLeft;
            float y0 = b.min.y + padBottom;
            float x1 = b.max.x + padRight;
            float y1 = b.max.y + padTop;

            UIWidget w = GetComponent<UIWidget>();
            //w.SetRect(x0, y0, x1 - x0, y1 - y0);
            w.width = Mathf.Max((int)Mathf.Abs(x0 - x1), minW);
            w.height = Mathf.Max((int)Mathf.Abs(y1 - y0), minH);
            BroadcastMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
            NGUITools.UpdateWidgetCollider(gameObject);
        }
    }
}
