using UnityEngine;
using System;
using Sirenix.OdinInspector;


[System.Serializable]
public class BezierSplineRaw
{

    [HideInInspector]
    public static int slectedIndexx = -1;
    [SerializeField]
    [ListDrawerSettings(HideAddButton =true,HideRemoveButton = true)]
    private Vector3[] points = new Vector3[] {
            new Vector3(0f, 3.0f, 0f),
         new Vector3(0f, 2.5f, 0f),
         new Vector3(0f, 0.5f, 0f),
         new Vector3(0f, 0f, 0f)
        };

    [SerializeField]
    
    [HideInInspector]
    public BezierControlPointMode[] modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
             BezierControlPointMode.Free, BezierControlPointMode.Free, BezierControlPointMode.Free
        };
    public void inverseTransform(Transform pTrans) {
        for (int i = 0; i < points.Length; ++i)
        {
            points[i] = pTrans.TransformPoint(points[i]);
        }
     
    }
    public void FlipX()
    {
        for (int i = 1; i < points.Length; ++i)
        {
            points[i] = new Vector3(points[i].x * 2 - points[0].x, points[i].y, points[i].z);
        }
    }

    public void setDelta(Vector2 pDelta)
    {
        for (int i = 0; i < points.Length; ++i) {
            points[i] += (Vector3) pDelta;
        }
    }

    public void rotation(Vector2 pDirection)
    {
       var pAngle =  Vector2.Angle(Vector2.up, pDirection);
        for (int i = 1; i < points.Length; ++i)
        {
            points[i] = points[0]+(Vector3)((Vector2)(points[i] - points[0])).Rotate(pAngle);
        }
    }
    [SerializeField]
    [HideInInspector]
    private bool loop;

    [SerializeField]
    [HorizontalGroup("group1")]
    private bool lineMode = true;

    [SerializeField]
    [HorizontalGroup("group1")]
    [Tooltip("if true khi move 1 point các point sau nó cũng sẽ move theo")]
    private bool relative = true;

    [SerializeField]
    [HideInInspector]
    private bool snap;

    [SerializeField]
    [HideInInspector]
    private Vector2 snapGrid;
    float totalDistanceLine;


    private void Awake()
    {
        //BezierEnviroment envi = gameObject.GetComponent<BezierEnviroment>();
        //if (envi == null)
        //{
        //    envi = gameObject.AddComponent<BezierEnviroment>();
        //}
    }

    private void Start()
    {

    }

    public bool Loop
    {
        get
        {
            return loop;
        }
        set
        {
            loop = value;
            if (value == true)
            {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    public int ControlPointCount
    {
        get
        {
            return points.Length;
        }
    }

    public Vector3 GetControlPoint(int index)
    {
        return points[index];
    }

    public float totalLength()
    {
        float pDistance = 0;
        Vector3 pCurrentPos = GetPoint(0);
        for (int i = 0; i < 101; ++i)
        {
            Vector3 pPointNext = GetPoint(i * 0.01f);
            pDistance += Vector3.Distance(pPointNext, pCurrentPos);
            pCurrentPos = pPointNext;
        }
        return pDistance;
    }

    public void SetControlPoint(int index, Vector3 point,bool noDealtaMainNode = false)
    {
        if (Snap)
        {
            point.x = snapGrid.x == 0 ? point.x : (((int)(point.x / snapGrid.x)) * snapGrid.x);
            point.y = snapGrid.y == 0 ? point.y : (((int)(point.y / snapGrid.y)) * snapGrid.y);
        }
        if (index % 3 == 0)
        {
            Vector3 delta = point - points[index];
            if (noDealtaMainNode)
            {
                delta = Vector3.zero;
            }
            if (loop)
            {
                if (index == 0)
                {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1)
                {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else
                {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length)
                {
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        totalDistanceLine = 0;
        for (int i = 0; i < points.Length - 3; i += 3)
        {
            totalDistanceLine += Vector3.Distance(points[i], points[i + 3]);
        }
        if (!noDealtaMainNode)
        {
            EnforceMode(index);
        }
    }

    public BezierControlPointMode GetControlPointMode(int index)
    {
        return modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;
        if (loop)
        {
            if (modeIndex == 0)
            {
                modes[modes.Length - 1] = mode;
            }
            else if (modeIndex == modes.Length - 1)
            {
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if ( !loop && ( modeIndex == 0 || index == points.Length -2))
        {
            return;
        }
        if( index == points.Length - 1)
        {
            return;
        }
        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0)
            {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= points.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = points.Length - 2;
            }
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }

    public int CurveCount
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    public bool LineMode
    {
        get
        {
            return lineMode;
        }

        set
        {
            lineMode = value;
        }
    }

    public bool Snap
    {
        get
        {
            return snap;
        }

        set
        {
            snap = value;
        }
    }

    public Vector2 SnapGrid
    {
        get
        {
            return snapGrid;
        }

        set
        {
            snapGrid = value;
        }
    }

    public bool Relative
    {
        get
        {
            return relative;
        }

        set
        {
            relative = value;
        }
    }
    public int currentNode = 0;
    public Vector3 GetPoint(float t)
    {
        if (!LineMode)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            currentNode = i / 3;
            Vector3 point = Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t);
            return point;
        }
        if (totalDistanceLine == 0)
        {
            totalDistanceLine = 0;
            for (int i = 0; i < points.Length - 3; i += 3)
            {
                totalDistanceLine += Vector3.Distance(points[i], points[i + 3]);
            }
        }
        float currentdistance = t * totalDistanceLine;
        Vector3 pointDestiny = Vector3.zero;
        int lastIndex = 0;
        for (int i = 3; i < points.Length; i += 3)
        {
            float pDistance = Vector3.Distance(points[i], points[i - 3]);
            lastIndex = i;
            if (currentdistance <= pDistance)
            {
                currentNode = (lastIndex / 3) - 1;
                break;
            }
            else
            {
                if (i != points.Length - 1)
                {
                    currentdistance -= pDistance;
                }
            }
        }
        pointDestiny = Vector3.LerpUnclamped(points[lastIndex - 3], points[lastIndex], currentdistance/Vector3.Distance(points[lastIndex - 3], points[lastIndex]));
        return pointDestiny;
    }

    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t);
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public void AddCurveWithPoint(Vector3 newPoint)
    {
        SetControlPoint(points.Length - 1, newPoint);
        points[points.Length - 2] = newPoint.changeDestinationByDirection(points[points.Length - 4], 1);
        points[points.Length - 3] = points[points.Length - 4].changeDestinationByDirection(newPoint, 1);

        totalDistanceLine = 0;
        for (int i = 0; i < points.Length - 3; i += 3)
        {
            totalDistanceLine += Vector3.Distance(points[i], points[i + 3]);
        }
    }

    public void AddCurve()
    {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop)
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
 
    }

    public void deleteLast()
    {
        if (points.Length < 3) return;
        Array.Resize(ref points, points.Length - 3);
    }

    ////public void Reset()
    ////{
    ////    points = new Vector3[] {
    ////        new Vector3(0f, 0f, 0f)
    ////    };
    ////    modes = new BezierControlPointMode[] {
    ////        BezierControlPointMode.Free
    ////    };
    ////}
}