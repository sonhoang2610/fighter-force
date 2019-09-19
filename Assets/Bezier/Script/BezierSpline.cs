using UnityEngine;
using System;
using EazyCustomAction;

[ExecuteInEditMode]
[RequireComponent(typeof(BezierEnviroment))]
public class BezierSpline : MonoBehaviour
{

    [SerializeField]
    private Vector3[] points = new Vector3[] {
            new Vector3(0f, 0f, 0f)
        };

    [SerializeField]
    private BezierControlPointMode[] modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free
        };

    [SerializeField]
    private bool loop;

    [SerializeField]
    private bool lineMode;
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
        totalDistanceLine = 0;
        for (int i = 0; i < points.Length - 3; i += 3)
        {
            totalDistanceLine += Vector3.Distance(points[i], points[i + 3]);
        }
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

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = point - points[index];
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
        EnforceMode(index);
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
        if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
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

	public Vector3 GetPoint(float t, bool local = false,bool flip = false)
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
	        Vector3 point = Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t);
	        if(flip){
	        	point.x = - point.x;
	        }
            if (local)
            {
                return point;
            }
            return transform.TransformPoint(point);
        }
        float currentdistance = t * totalDistanceLine;
        float distance = 0;
        Vector3 pointDestiny = Vector3.zero;
        for (int i = 3; i < points.Length; i += 3)
        {
            float pDistance = Vector3.Distance(points[i], points[i - 3]);
            if (currentdistance <= distance + pDistance)
            {
                pointDestiny = Vector3.Lerp(points[i - 3], points[i], (currentdistance - distance) / pDistance);
                break;
            }
            distance += pDistance;
        }
	    if(flip){
		    pointDestiny.x = - pointDestiny.x;
	    }
        if (local)
        {
            return pointDestiny;
        }
        return transform.TransformPoint(pointDestiny);
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
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
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

    public void Reset()
    {
        points = new Vector3[] {
            new Vector3(0f, 0f, 0f)
        };
        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free
        };
    }
}