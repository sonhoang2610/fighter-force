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

public class HomeMissile : MonoBehaviour, ISetTarget, IMovementProjectile, IgnoreObject
{
    public int countFindTarget = -1;
    public float speed = 5;
    public float rotatingSpeed = 200;
    public float limitRotate = 200;
    public GameObject target;
    protected Health  healTarget;
    public TranformExtension.FacingDirection facingDefault = TranformExtension.FacingDirection.DOWN;
    public bool autoTarget;
    public bool useSpeedMain = false;
    public float range;
    public bool isBlockMove;
    public Vector2 defaultDirection = Vector2.up;
    public LayerMask TargetLayer;
    public UnityEvent onFindTarget;
    protected Rigidbody2D rb;
    Health _health;
    protected int currentCountFindTarget;
    List<GameObject> _listIgnoreCollider = new List<GameObject>();
    protected float currentRotate = 0;
    protected Vector2 CachePosLastTarget;
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
        currentCountFindTarget = countFindTarget;
    }

    public void forceFoundTarget()
    {
        target = null;
        findTargetMinDistance();
    }

    private void OnDisable()
    {
        target = null;
    }

    public virtual Character[] findChar()
    {
        return LevelManger.Instance._charList.ToArray(); 
    }

    public virtual void findTargetMinDistance()
    {
        if (target && target.gameObject.activeSelf) return;
 
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
                        healTarget = target.GetComponent<Health>();
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
        {
            if (target && (_listIgnoreCollider.Contains(target) || healTarget.CurrentHealth <= 0 || healTarget.Invulnerable || !healTarget.Collider2D || !healTarget.Collider2D.enabled))
            {
                target = null;
            }
            if (target && target.gameObject.activeSelf)
            {
               
                if (currentRotate > limitRotate) { rb.angularVelocity = 0; return; }
                CachePosLastTarget = target.transform.position;
                Vector2 point2Target = (Vector2)transform.position - (Vector2)target.transform.position;

                point2Target.Normalize();

                float value = Vector3.Cross(point2Target, -transform.up).z;
                if (facingDefault == TranformExtension.FacingDirection.UP)
                {
                    value = Vector3.Cross(point2Target, transform.up).z;
                }

                rb.angularVelocity = rotatingSpeed * (value);
                currentRotate += Mathf.Abs(rotatingSpeed * value) * Time.deltaTime;

            }
            else
            {
                rb.angularVelocity = 0;
                if (isUpdateAble() && autoTarget)
                {
                    if (target != null && currentCountFindTarget > 0)
                    {
                        currentCountFindTarget--;
                    }
                    findTargetMinDistance();
                }
            }
        }
        if(!target || !target.gameObject.activeSelf) {
            Vector2 pDirectionPoint = defaultDirection;
            Vector2 point2Target = -defaultDirection;

            point2Target.Normalize();

            float value = Vector3.Cross(point2Target, -transform.up).z;
            if (facingDefault == TranformExtension.FacingDirection.UP)
            {
                value = Vector3.Cross(point2Target, transform.up).z;
            }
            rb.angularVelocity = rotatingSpeed * (value);
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

    public void KillProcess()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        enabled = false;
    }

    public void setTarget(GameObject pObject)
    {
        target = pObject;
        healTarget = target.GetComponent<Health>();
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
    public bool blockRotation= true;

    public GameObject[] IgnoreObjects { get => _listIgnoreCollider.ToArray(); set {
            _listIgnoreCollider.Clear();
            _listIgnoreCollider.AddRange(value); } }

    public bool isBlockRotation()
    {
        return blockRotation;
    }

    public int getIndex()
    {
        return 0;
    }

    public void IgnoreGameObject(GameObject pObject)
    {
        _listIgnoreCollider.Add(pObject);
    }

    public void ClearIgnoreList()
    {
        _listIgnoreCollider.Clear();
    }
}
