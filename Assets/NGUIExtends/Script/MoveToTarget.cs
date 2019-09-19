using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyCustomAction;
using UnityEngine.Events;

[System.Serializable]
public struct TargetAction
{
    public string nameAction;
    public GameObject target;
    public float time;
    public bool isAwake;
    public bool isEnable;
    public bool isLoop ;
    public Vector2 delayBeforeAction;
    public UnityEvent onStartAction;
    public UnityEvent onFinishedAction;
}
public class MoveToTarget : MonoBehaviour
{
    [SerializeField]
    TargetAction[] actions;

    RootMotionController _root;

    public RootMotionController Root
    {
        get
        {
            return _root ? _root : _root = GetComponent<RootMotionController>();
        }

    }

    public void setTarget(GameObject pTarget, int index) {
        actions[0].target = pTarget;
    }

    public void runActionName(string name)
    {
        for (int i = 0; i < actions.Length; ++i)
        {
            if (actions[i].nameAction == name)
            {
                actions[i].onStartAction.Invoke();
                if (Root != null && actions[i].target != null)
                {
                    Vector3 cachePos = transform.position;
                    int index = i;
                     Root.runAction(EazyCustomAction.Sequences.create(DelayTime.create(UnityEngine.Random.Range( actions[i].delayBeforeAction.x, actions[i].delayBeforeAction.y)), EazyMove.to(new EazyReflectionSupport.EazyMethodInfo(actions[i].target,EazyReflectionSupport.TypeReflection.Properties,"Transform","position"), actions[i].time).setLocal(false),CallFunc.create(delegate {
                         if (actions[index].isLoop)
                         {
                             transform.position = cachePos;
                             runActionName(name);
                         }
                     })));
                }
                if (actions[i].time > 0)
                {
                    StartCoroutine(delayEvent(actions[i].time, actions[i].onFinishedAction));
                }
            }

        }
    }

    IEnumerator delayEvent(float pSec,UnityEvent pEvent)
    {
        yield return new WaitForSeconds(pSec);
        pEvent.Invoke();
    }

    IEnumerator delayIn(float pSec,string name)
    {
        yield return new WaitForSeconds(pSec);
        runActionName(name);
    }
    bool isFirst = true;
    private void OnEnable()
    {
        for (int i = 0; i < actions.Length; ++i)
        {
            if (actions[i].isEnable || (actions[i].isAwake && isFirst))
            {
               StartCoroutine( delayIn(0.1f, actions[i].nameAction));
            }
        }
        isFirst = false;
    }

    private void Awake()
    {
        isFirst = true;
    }
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
