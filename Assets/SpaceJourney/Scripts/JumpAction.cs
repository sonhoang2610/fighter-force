using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class JumpAction : MonoBehaviour
{
    protected bool start = false;
    public Vector3 startPos;
    public Vector3 endPos;
    public float time;
    public float height;
    public UnityEvent onComplete;
    protected float currentTime;
    public void startJump(Vector3 pStart, Vector3 end, float height, float t)
    {
        start = true;
        startPos = pStart;
        endPos = end;
        this.height = height + (Mathf.Max(pStart.y,end.y)- Mathf.Min(pStart.y, end.y));
        time = t;
    }
    public Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, levelDirecteion);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnDisable()
    {
        start = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            currentTime += Time.deltaTime;
            if(currentTime >= time)
            {
                currentTime = time;
            }
            Vector3 pos = SampleParabola(startPos, endPos, height, currentTime/time);
            transform.position = pos;
            if(currentTime >= time)
            {
                start = false;
                onComplete.Invoke();
            }
        }
    }
}
