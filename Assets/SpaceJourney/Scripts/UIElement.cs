using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EazyCustomAction;
using EazyEngine.Space;
using Sirenix.OdinInspector;
using EazyEngine.Tools;

[System.Serializable]
public class UITriggerBehavior
{
    public string trigger;
    public float delayToTween = 0;
    public string tweenID;
    public string lockID;
    public UnityEvent onTriggerEvent;
}
public class UIElement : MonoBehaviour,EzEventListener<UIMessEvent> {
    public bool listenTriggerUI = false;
    [ShowIf("listenTriggerUI")]
    public UITriggerBehavior[] listeners;
    [SerializeField]
    public string cateGory = "None";
    [SerializeField]
     public UnityEvent onEnableEvent;
    [SerializeField]
    public UnityEvent onDisableEvent;
    [SerializeField]
    UnityEvent onStartEvent;
    [SerializeField]
    UnityEvent onInitEvent;
    [SerializeField]
    UnityEvent onEnableLateUpdateEvent;
    [SerializeField]
    UnityEvent onCompleteTweenShow;

    public Action _actionOnClose;
    public int relative = 0;
    public void handleEffect(bool active)
    {
        //if (effect == EffectDisplay.FADE_SCALE)
        //{
        //    if (active)
        //    {
        //        transform.localScale = new Vector3(0, 0, 0);
        //        GetComponent<UIWidget>().alpha = 0;
        //        RootMotionController.runAction(gameObject, NGUIFadeAction.to(1, 0.5f));
        //        RootMotionController.runAction(gameObject, EazyScale.to(new Vector3(1, 1, 1), 0.5f).setEase(EaseCustomType.easeOutExpo));
        //    }
        //    else
        //    {

        //    }
        //}
    }

    public void showRelative()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        relative++;
    }

    public void hideRelative()
    {
        relative--;
        if(relative <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void resetTween()
    {
    }

    public void stepEnable()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }


    public virtual void show()
    {
        showElement(delegate {onCompleteTweenShow.Invoke();  }); 
    }

    public virtual void showElement(System.Action pComplete)
    {
        GameObject o;
        (o = gameObject).SetActive(true);
        RootMotionController.stopAllAction(o);
        if( UIElementManager.Instance.doAction(this, true,pComplete))
        {
            gameObject.GetComponent<RootMotionController>()._isRunOnRealTime = true;
        }
    }

    public virtual void close()
    {
        RootMotionController.stopAllAction(gameObject);
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        var pAction = UIElementManager.Instance.doAction(this, false);

        if (!pAction)
        {
            gameObject.SetActive(false);
        }

        _actionOnClose?.Invoke();
    }

    public void change()
    {
        if (gameObject.activeSelf)
        {
            close();
        }
        else
        {
            show();
        }
    }

    private bool isFirst = false;

    private void LateUpdate()
    {
        if (!isFirst) return;
        onEnableLateUpdateEvent.Invoke();
        isFirst = false;
    }
    private void OnEnable()
    {

        isFirst = true;
        onEnableEvent?.Invoke();
        onStartEvent?.Invoke();
    }

    private void OnDisable()
    {
        //if (listenTriggerUI)
        //{
        //    EzEventManager.RemoveListener(this);
        //}
        onDisableEvent?.Invoke();
    }
    protected virtual void Start()
    {
        onInitEvent.Invoke();
    }

    public IEnumerator delayAction(float pDelay,UnityEvent pEvent)
    {
        yield return new WaitForSeconds(pDelay);
        pEvent.Invoke();
    }
    private void Update()
    {
        DoUpdate();
    }

    // Update is called once per frame
    protected virtual void DoUpdate () {
    
	}
    private void OnDestroy()
    {
        if (listenTriggerUI)
        {
            EzEventManager.RemoveListener(this);
        }
    }
    private void Awake()
    {
        if (listenTriggerUI)
        {
            EzEventManager.AddListener(this);
        }
    }
    protected List<UITriggerBehavior> lockedBehavior = new List<UITriggerBehavior>();
    public void OnEzEvent(UIMessEvent eventType)
    {
       // RootMotionController.stopAllAction(gameObject);
       for(int i = 0; i < listeners.Length; ++i)
        {
            var pStrs = listeners[i].trigger.Split('|');
            if (pStrs.checkExist( eventType.Event))
            {
                bool ableRunUnlock = false;
                if(lockedBehavior.Contains(listeners[i]) )
                {
                    listeners[i].onTriggerEvent.Invoke();
                    continue;
                }
                else
                {
                    for(int j = lockedBehavior.Count-1; j >= 0;--j )
                    {
                        var pTrigger = lockedBehavior[j];
                        if (pTrigger.lockID.Contains("Lock") && listeners[i].lockID.Contains("Unlock"))
                        {
                            string pIDLock = pTrigger.lockID.Replace("Lock","");
                            string pIDUnlock = listeners[i].lockID.Replace("Unlock", "");
                            if (pIDLock == pIDUnlock)
                            {
                                ableRunUnlock = true;
                                lockedBehavior.Remove(pTrigger);
                            }
                        }
                    }
                }
                if (!listeners[i].lockID.Contains("Unlock") || ableRunUnlock)
                {
                    var pAction = UIElementManager.Instance.getTween(listeners[i].tweenID);
                    if (pAction != null)
                    {
                        List<EazyAction> actions = new List<EazyAction>();
                        if (listeners[i].delayToTween > 0)
                        {
                            actions.Add(DelayTime.create(listeners[i].delayToTween));
                        }
                        actions.Add(pAction);
                        Sequences pSeq = Sequences.create(actions.ToArray());
                        RootMotionController.runAction(gameObject, pSeq);
                    }
                }
                listeners[i].onTriggerEvent.Invoke();
                if (!string.IsNullOrEmpty(listeners[i].lockID) && listeners[i].lockID.Contains("Lock"))
                {
                    lockedBehavior.Add(listeners[i]);
                }
            }
        }
    }
}
