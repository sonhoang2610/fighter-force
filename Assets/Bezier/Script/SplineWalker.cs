using UnityEngine;
using UnityEngine.Events;
using EazyCustomAction;

[System.Serializable]
public class EventOnRunTime
{
    [SerializeField]
    float percent;
    [SerializeField]
    UnityEvent onUpdate;

    bool isInvoke = false;

    public void apply(float percent)
    {
        if(percent < this.percent)
        {
            isInvoke = false;
        }
        if (!isInvoke && percent >= this.percent)
        {
            if (onUpdate != null)
            {
                onUpdate.Invoke();
            }
        }
    }
}

public class SplineWalker : MonoBehaviour
{

    [SerializeField]
    Transform model;
    public BezierSpline spline;

    public float duration;

    public bool lookForward;
    public bool isLocal = false;
    public bool isFlip = false;

    public SplineWalkerMode mode;
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 1f), new Keyframe(1f, 0f));

    private float progress;
    private bool goingForward = true;
    Vector3 startPoint;
    bool moving = false;
    [SerializeField]
    public UnityEvent onStart;
    [SerializeField]
    public UnityEvent onComplete;
    [SerializeField]
    EventOnRunTime onUpdate;
    
    bool enableMove = true;
    public bool IsFlip
    {
        get
        {
            return isFlip;
        }

        set
        {
            isFlip = value;
        }
    }

    public Transform Model
    {
        get
        {
            return model ? model : model = transform;
        }
        
    }

    public void flipp()
    {
        isFlip = !IsFlip;
    }

    public void setSpline(BezierSpline pSpline)
    {
        spline = pSpline;
    }

    public void restart()
    {
        transform.position = startPoint;
    }

    [ContextMenu("jump")]
    public void startMove(bool isUpdate = true)
    {
        enableMove = true;
        progress = 0;
        moving = isUpdate;
        startPoint = transform.position;
        onStart.Invoke();
 
    }

    public void stopMove()
    {
        moving = false;
        enableMove = false;
    }

    public void Movement()
    {
        if (!enableMove) return;
        if (goingForward)
        {
            progress += Time.deltaTime / duration;
            if (progress > 1f)
            {
                if (onComplete != null)
                {
                    onComplete.Invoke();
                }
                if (mode == SplineWalkerMode.Once)
                {
                    progress = 1f;
                    moving = false;
                }
                else if (mode == SplineWalkerMode.Loop)
                {
                    progress -= 1f;
                }
                else
                {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        }
        else
        {
            progress -= Time.deltaTime / duration;
            if (progress < 0f)
            {
                progress = -progress;
                goingForward = true;
            }
        }
        float realprogress = curve.Evaluate(progress);
        Vector3 position = spline.GetPoint(realprogress, isLocal, IsFlip);
        if (isLocal)
        {
            transform.position = startPoint + position;
        }
        else
        {
            transform.position = position;
        }

        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
    }

    public void onRevie()
    {
        progress = 0;
        transform.position = startPoint;
        moving = false;
    }

    private void Update()
    {
        if (moving)
        {
            if (goingForward)
            {
                progress += Time.deltaTime / duration;

            }
            else
            {
                progress -= Time.deltaTime / duration;
                if (progress < 0f)
                {
                    progress = -progress;
                    goingForward = true;
                }
            }
            float realprogress = curve.Evaluate(progress);
            Vector3 position = spline.GetPoint(realprogress, isLocal, IsFlip);
            if (isLocal)
            {
                transform.position = startPoint + position;
            }
            else
            {
                transform.position = position;
            }
            if(onUpdate != null)
            {
                onUpdate.apply(progress);
            }
            if (progress > 1f)
            {
                if (mode == SplineWalkerMode.Once)
                {
                    if (onComplete != null)
                    {
                        onComplete.Invoke();
                    }
                    progress = 1f;
                    moving = false;
                }
                else if (mode == SplineWalkerMode.Loop)
                {
                    if (onComplete != null)
                    {
                        onComplete.Invoke();
                    }
                    progress -= 1f;
                }
                else
                {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
            if (lookForward)
            {
                Model.RotationDirect2D(spline.GetDirection(progress));
            }
        }
    }
}