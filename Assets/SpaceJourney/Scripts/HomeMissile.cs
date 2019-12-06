using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Space;
using UnityEngine.Events;
using EazyEngine.Tools.Space;

public interface ISetTarget
{
    void setTarget(GameObject pObject);
}
public class HomeMissile : MonoBehaviour,EzEventListener<DamageTakenEvent>, ISetTarget, IMovementProjectile
{
    public int countFindTarget = -1;
    public float speed = 5;
    public float rotatingSpeed = 200;
    public float limitRotate = 200;
    public GameObject target;
    public TranformExtension.FacingDirection facingDefault = TranformExtension.FacingDirection.DOWN;
    public bool autoTarget;
    public bool useSpeedMain = false;
    public float range;
    public bool isBlockMove;
    public LayerMask TargetLayer;
    public UnityEvent onFindTarget;
    protected Rigidbody2D rb;
    Health _health;
    protected int currentCountFindTarget;
    List<GameObject> _listIgnoreCollider = new List<GameObject>();
    private Collider2D _cacheCollider;
    private bool isSelfDamage;
    private DamageOnTouch _dameOnTouch;
    protected float currentRotate = 0;
    public bool IsEnable()
    {
        return enabled;
    }
    public void Revie()
    {
        if (_health == null) _health = GetComponent<Health>();
        _health.Revive();
        currentRotate = 0;
    }
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
    private void OnEnable()
    {
        currentRotate = 0;
        if (!_dameOnTouch)
        {
            _dameOnTouch = GetComponent<DamageOnTouch>();
        }
       // target = null;
        currentCountFindTarget = countFindTarget;
        _cacheCollider = null;
        isSelfDamage = false;
        _dameOnTouch.ClearIgnoreList();
        _listIgnoreCollider.Clear();
        EzEventManager.AddListener(this);
    }

    private void OnDisable()
    {
        EzEventManager.RemoveListener(this);
        target = null;
    }

    public virtual Character[] findChar()
    {
        return LevelManger.Instance._charList.ToArray(); 
    }

    public virtual void findTargetMinDistance()
    {
        if (target && target.gameObject.activeSelf) return;
        if (autoTarget)
        {
            target = null;
            var _chars = findChar();
            float distance = 0;
            for (int i = 0; i < _chars.Length; ++i)
            {
                float pDistance = Vector2.Distance(_chars[i].transform.position, transform.position);
                bool insideScreen = LevelManger.Instance.mainPlayCamera.Rect().Contains(_chars[i].transform.position);
                if(!_chars[i].gameObject.activeSelf || _chars[i].AvailableTargets.Length <= 0)continue;
                if (!_listIgnoreCollider.Contains(_chars[i].gameObject) && Layers.LayerInLayerMask(_chars[i].gameObject.layer, TargetLayer) && pDistance < range && insideScreen) 
                {
                    if (pDistance < distance || distance == 0)
                    {
                        distance = pDistance;
                        target = _chars[i].gameObject;
                        onFindTarget.Invoke();
                        OnFindTarget();
                  
                    }
                }
            }
        }
    }

    public virtual void OnFindTarget()
    {
    }
    public virtual bool isUpdateAble()
    {
        return countFindTarget == -1 || currentCountFindTarget > 0;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isUpdateAble()) return;
        if (target && target.gameObject.activeSelf)
        {
            if (currentRotate > limitRotate) { rb.angularVelocity = 0; return; }
            Vector2 point2Target = (Vector2)transform.position - (Vector2)target.transform.position;

            point2Target.Normalize();

            float value = Vector3.Cross(point2Target, -transform.up).z;
            if (facingDefault == TranformExtension.FacingDirection.UP)
            {
                value = Vector3.Cross(point2Target, transform.up).z;
            }

             rb.angularVelocity = rotatingSpeed * (value);
            currentRotate += Mathf.Abs(rotatingSpeed * value)*Time.deltaTime;
           
        }
        else
        {
            if(target != null)
            {
                if (currentCountFindTarget > 0)
                {
                    currentCountFindTarget--;
                }
            }
            rb.angularVelocity = 0;
            findTargetMinDistance();
        }
        rb.velocity = -transform.up * speed;
        if (facingDefault == TranformExtension.FacingDirection.UP)
        {
            rb.velocity = transform.up * speed;
        }
        else if (facingDefault == TranformExtension.FacingDirection.DOWN) {
            rb.velocity = -transform.up * speed;
        }
    }

    public void addIgnoreObject(Collider2D pCollider)
    {
        _listIgnoreCollider.Add(pCollider.gameObject);
        _dameOnTouch.IgnoreGameObject(pCollider.gameObject);
        GetComponent<Health>().showDeathEffect();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isSelfDamage)
        {
            isSelfDamage = false;
            _listIgnoreCollider.Add(collision.gameObject);
            findTargetMinDistance();
        }
        else
        {
            _cacheCollider = collision;
        }
    }

    void EzEventListener<DamageTakenEvent>.OnEzEvent(DamageTakenEvent eventType)
    {
        if(eventType.AffectedCharacter == eventType.Instigator == gameObject || (eventType.AffectedCharacter == null && eventType.Instigator == gameObject))
        {
            if (_cacheCollider != null)
            {
                addIgnoreObject(_cacheCollider);
                findTargetMinDistance();
                _cacheCollider = null;
            }
            else
            {
                isSelfDamage = true;
            }
        }
    }

    public void setTarget(GameObject pObject)
    {
        target = pObject;
    }

    public void setDirection(Vector2 pDir)
    {
        
    }

    public void setSpeed(float pSpeed)
    {
        if (useSpeedMain)
        {
            speed = pSpeed;
        }
    }

    public Vector2 Movement()
    {
        return Vector2.zero;
    }

    public bool isBlock()
    {
        return isBlockMove;
    }

    public bool isBlockRotation()
    {
        return false;
    }

    public int getIndex()
    {
        return 0;
    }
    //void OnTriggerEnter2D(Collider2D other)
    //{

    //    if (destroyOnCollider)
    //    {

    //        Destroy(this.gameObject, 0.02f);

    //    }

    //}
}
