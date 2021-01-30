using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RepositionParent : MonoBehaviour
{
    public UIWidget[] contents;

    public Vector3 cachePos;
    public bool freezeY;
    private void Awake()
    {
      
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    protected float minX = 0, maxX = 0;
    protected float cacheWidth;
    protected bool dirty = false;
    [ContextMenu("Test")]
    public void calculatorMoreContent()
    {
        minX = 0;
        maxX = 0;
        for (int i = 0; i < contents.Length; ++i)
        {

            for (int j = 0; j < contents[i].localCorners.Length; ++j)
            {
                if (!contents[i].gameObject.activeSelf) continue;
                Vector3 point = contents[i].localCorners[j];
                if (minX == 0 || minX >= point.x)
                {
                    minX = point.x;
                }
                if (maxX == 0 || maxX <= point.x)
                {
                    maxX = point.x;
                }
            }

        }
        if(cacheWidth != maxX - minX)
        {
            dirty = true;
            cacheWidth = maxX - minX;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            calculatorMoreContent();
            if (dirty)
            {
                float pMid = (maxX + minX) / 2;
                var pDelta = transform.TransformVector(new Vector3(pMid, 0, 0));
                transform.position = transform.parent.TransformPoint(cachePos) - pDelta;
                dirty = false;
            }
        }
        else
        {
            cachePos = transform.localPosition;
        }
    }
}
