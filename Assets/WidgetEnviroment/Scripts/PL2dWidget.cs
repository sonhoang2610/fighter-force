using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;

//[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class PL2dWidget :  CacheBehaviour{
    RectTransform rectTrans;
    BoxCollider2D boxCollider2D;
    [SerializeField]
    int orderLayer = 0;
    [SerializeField]
    Vector2 sizeWidget;
    [SerializeField]
    bool autoColliderResize,autoRiszeByCollider,resizeByGrid,autoResizeTransform;
    [SerializeField]
    List<PL2dWidget> childs;
    Vector3 lockPos;
    bool islock;
    

    public void OnValidate()
    {
        if (SpriteRenderer)
        {
            SpriteRenderer.sortingOrder = OrderLayer;
        }
    }
    private void Awake()
    {
    }

    public List<PL2dWidget> Childs
    {
        get
        {
            return childs != null ? childs : childs = new List<PL2dWidget>();
        }

        set
        {
            childs = value;
        }
    }
    public RectTransform RectTrans
    {
        get { return rectTrans != null ? rectTrans : rectTrans = GetComponent<RectTransform>(); }
    }

    public BoxCollider2D BoxCollider2D
    {
        get
        {
            return boxCollider2D ? boxCollider2D : boxCollider2D = GetComponent<BoxCollider2D>();
        }
    }

    public Vector2 Size
    {
        get
        {
            return sizeWidget;
        }

        set
        {
            sizeWidget = value;
        }
    }

    public int OrderLayer
    {
        get
        {
            return orderLayer;
        }

        set
        {
            orderLayer = value;
        }
    }

    public bool ResizeByGrid
    {
        get
        {
            return resizeByGrid;
        }

        set
        {
            resizeByGrid = value;
        }
    }

    public Vector3 LockPos
    {
        get
        {
            return lockPos;
        }

        set
        {
            Islock = true;
            lockPos = value;
        }
    }

    public bool Islock
    {
        get
        {
            return islock;
        }

        set
        {
            islock = value;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (!Application.isPlaying && transform.hasChanged)
        {
            if (autoColliderResize)
            {
                if (SpriteRenderer != null)
                {
                    BoxCollider2D.size = Size;
                    if (resizeByGrid)
                    {
                        SpriteRenderer.size = Size;
                        if (Islock)
                        {
                            transform.position = LockPos;
                        }
                    }
                }
            }else if (autoRiszeByCollider)
            {
                Size = BoxCollider2D.size;
            }
            if (autoResizeTransform)
            {
                if (RectTranform)
                {
                    RectTranform.sizeDelta = Size;
                }
            }
        }
#endif
    }

    private void OnDestroy()
    {
        if (transform.parent != null)
        {
            PL2dWidget widgetParrent = transform.parent.GetComponent<PL2dWidget>();
            if (widgetParrent)
            {
                widgetParrent.Childs.Remove(this);
            }
        }
    }
}
