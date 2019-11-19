using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoxResizeUI : MonoBehaviour
{
    [SerializeField]
    UIWidget _content = null, _self;
    [SerializeField]
    UIWidget[] moreContent;
    [SerializeField]
    float _offsetOut = 0;
    [SerializeField]
    float _offsetOutY = 0;
    [SerializeField]
    Vector2 _min;
    Vector2 _oldSize;
    bool isDirty = false;

    [ContextMenu("reUpdate")]
    public void reUpdate()
    {
        isDirty = true;
    }
    public UIWidget Self
    {
        get
        {
            return _self ? _self : _self = GetComponent<UIWidget>();
        }
    }

    public UIWidget Content
    {
        get
        {
            return _content;
        }

        set
        {
            _content = value;
        }
    }

    // Use this for initialization
    void Start()
    {

    }
    protected float minX = 0, maxX = 0;
    [ContextMenu("Test")]
    public void calculatorMoreContent()
    {
        minX = 0;
        maxX = 0;
        for (int j = 0; j < _content.worldCorners.Length; ++j)
        {
            Vector3 point = transform.InverseTransformPoint(_content.worldCorners[j]);
            if (minX == 0 || minX >= point.x)
            {
                minX = point.x;
            }
            if (maxX == 0 || maxX <= point.x)
            {
                maxX = point.x;
            }
        }
        for (int i = 0; i < moreContent.Length; ++i)
        {
    
            for (int j  = 0; j < moreContent[i].localCorners.Length; ++j)
            {
                if (!moreContent[i].gameObject.activeSelf) continue;
                Vector3 point = transform.InverseTransformPoint(moreContent[i].worldCorners[j]);
                if (minX ==0 || minX >= point.x)
                {
                    minX = point.x;
                }
                if(maxX == 0 || maxX  <= point.x)
                {
                    maxX = point.x;
                }
                if (moreContent[i].dirtyChange)
                {
                    isDirty = true;
                }
            }
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Content && Self)
        {

            //Vector2 newSize = new Vector2(_content.localSize.x + _offsetOut, Self.localSize.y);
            calculatorMoreContent();
            if ((/*Content.dirtyChange*/  /*||*/ true))
            {   
                isDirty = false;
                // Self.SetRect(0, 0, _content.localSize.x + _offsetOut, Self.localSize.y);
                Self.width = (int)((maxX-minX) + _offsetOut);
                Self.height = (int)(Content.localSize.y + _offsetOutY);
                _oldSize = Content.localSize;
            }
            if (Self.width < _min.x)
            {
                Self.width = (int)(_min.x);
            }
            if (Self.height < _min.y)
            {
                Self.height = (int)(_min.y);
            }
        }

    }
}
