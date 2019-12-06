using DigitalRuby.ThunderAndLightning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.Events;
using System;
using EazyEngine.Timer;

namespace EazyEngine.Space
{
    public class Projectile : PoolableObject
    {
        public UnityEventInt onIndexBullet;
        public float Speed = 1;
        public float delayMove = 0;
        [ShowIf("onSpeedConstraintFeature")]
        public bool _SpeedConstraint = true;
        [HideIf("_SpeedConstraint")]
        public float SpeedStart = 0;
        [HideIf("_SpeedConstraint")]
        public float durationToMax = 1;
        [HideIf("_SpeedConstraint")]
        public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });

        [System.NonSerialized]
        public BezierSplineRaw spline;

        [ShowInInspector]
        [HideInEditorMode]
        [System.NonSerialized]
        public Vector2 Direction;
        public Projectile[] childs;

        protected Character owner;
        protected Weapon Weapon;
        [SerializeField]
        protected bool ignoreMove = false;
        [SerializeField]
        [HideInEditorMode]
        [ShowInInspector]
        protected float currentSpeed = 0;
        public bool MovetoTarget = false;
        [ShowIf("MovetoTarget")]
        public UnityEvent onMoveToTarget;
        [HideInInspector]
        public Vector2 target;

        protected Weapon[] weaponChilds = null;

        protected float currentDelayMove = 0;
        protected IMovementProjectile[] movementModules;
        protected ISetTarget[] setTargetAbles;

        public void setTarget(GameObject pTarget)
        {
            foreach(var pSet in setTargetAbles)
            {
                pSet.setTarget(pTarget);
            }
        }

        private void Awake()
        {
            movementModules = GetComponents<IMovementProjectile>();
            setTargetAbles = GetComponents<ISetTarget>();
            System.Array.Sort<IMovementProjectile>(movementModules, new Comparison<IMovementProjectile>((i1, i2) => i2.getIndex().CompareTo(i1.getIndex())));
        }
        protected virtual bool onSpeedConstraintFeature()
        {
            return true;
        }
        [HideInEditorMode]
        [System.NonSerialized]
        public int Index;
        public void setIndex(int pIndex)
        {
            onIndexBullet.Invoke(pIndex);
            Index = pIndex;
        }

        protected BulletInstancedConfig data;
        public void setData(BulletInstancedConfig pData)
        {
            data = pData;
            Speed = pData.speedBullet;
            for(int i = 0; i < pData.propEdits.Length; ++i)
            {
                for(int  j = 0; j < pData.propEdits[i].propEdits.Length; ++j)
                {
                   var pProp =   pData.propEdits[i].propEdits[j];
                    pProp.setValue(gameObject);
                }
            }
       
        }

        protected DamageExtra[] cacheExtras = new DamageExtra[0];
        public DamageExtra[] CacheExtras
        {
            get
            {
                return cacheExtras;
            }
        }
        public void setDamage(int damageBasic,float FactorDamage,DamageExtra[] extras)
        {
            if (!damage)
            {
                damage = GetComponent<DamageOnTouch>();
            }
            if (damage)
            {
                cacheExtras = extras;
                damage.FactorDamage = FactorDamage;
                if (damageBasic != 0)
                {
                    damage.DamageCausedProp = damageBasic;
                }
                damage.ExtraDamge = extras;
                if(weaponChilds != null)
                {
                   foreach(var pWeapon in weaponChilds)
                    {
                        pWeapon.FixDamage = damageBasic;
                         if(extras!= null && extras.Length > 0)
                        {
                            pWeapon.extraDamage.Clear();
                            pWeapon.extraDamage.AddRange(extras);
                        }
                        pWeapon.FactorDamage = FactorDamage;
                    }
                }
            }
            foreach (var proj in childs)
            {
                proj.setDamage(damageBasic, FactorDamage, extras);
            }
        }
        protected DamageOnTouch damage;
        protected TimeController timer;
        protected Tween tween;
        protected override void OnEnable()
        {
            currentTime = 0;
            base.OnEnable();
            //transform.localEulerAngles = new Vector3(0, 0, 0);
            damage = GetComponent<DamageOnTouch>();
            weaponChilds = GetComponentsInChildren<Weapon>();
            if(weaponChilds != null)
            {
                foreach(var pWeapon in weaponChilds)
                {
                    pWeapon.initWithInfo(gameObject);
                    if (pWeapon.Owner)
                    {
                        pWeapon.initDone();
                    }
                
                }
            }
            if (delayMove == 0)
            {
                if (_SpeedConstraint)
                {
                    CurrentSpeed = Speed;
                }
                else
                {
                    CurrentSpeed = SpeedStart;
                    tween = DOTween.To(() => currentSpeed, x => currentSpeed = x, Speed, durationToMax).SetEase(curve);
                }
            }
            else{
                currentDelayMove = delayMove;
            }
            timer = TimeKeeper.Instance.getTimeController(time._groupName);
        }
   
        public bool IgnoreMove
        {
            get
            {
                return ignoreMove;
            }

            set
            {
                ignoreMove = value;
            }
        }
        [ShowInInspector]
        public virtual float CurrentSpeed
        {
            get
            {
                return currentSpeed;
            }

            set
            {
                if (currentSpeed != value)
                {
                    if (movementModules != null && movementModules.Length > 0)
                    {
                        for (int i = 0; i < movementModules.Length; ++i)
                        {
                            movementModules[i].setSpeed(value);
                        }
                    }
                }
                currentSpeed = value;
               
            }
        }

        public Character Owner
        {
            get
            {
                return owner;
            }

            set
            {
                owner = value;
            }
        }
        public virtual void setDirectionFromQuantenion(Quaternion direction)
        {
            Vector2 directon = Vector2.up.Rotate(direction.eulerAngles.z);
            setDirection(directon);
        }
        protected Vector2 cachePos;
        public virtual void setDirection(Vector2 direction)
        {
            setDirectionAxis(direction, Vector2.up);
        }
        protected Sequence seqX;
        public virtual void setDirectionAxis(Vector2 direction,Vector2 pAxis)
        {
      
            cachePos = transform.position;
            transform.RotationDirect2D(direction, TranformExtension.FacingDirection.DOWN);
            if (movementModules != null)
            {
                for (int i = 0; i < movementModules.Length; ++i)
                {
                    movementModules[i].setDirection(direction);
                }
            }
            Direction = direction.normalized;
            velocityX = UnityEngine.Random.Range(velocityXRandom.x, velocityXRandom.y);
            currentVelocityX = 0;
            if (isAngularX)
            {
                if(seqX != null) { seqX.Kill(); }
                seqX = DOTween.Sequence();
                seqX.AppendInterval(delayX);
                seqX.Append(DOTween.To(() => currentVelocityX, x => currentVelocityX = x, velocityX, UnityEngine.Random.Range(timeVelocityX.x, timeVelocityX.y)).SetEase(curveVelocityX));

                seqX.Play();
            }
        }
   

        public virtual void setSplineMove(BezierSplineRaw pSpline)
        {
            IgnoreMove = true;
            RootMotionController.stopAllAction(gameObject);
            RootMotionController.runAction(gameObject, BezierWalkAction.create(pSpline, Speed, false).setUpdateEvent(delegate (float a) {
                Vector2 direction = pSpline.GetVelocity(a);
                transform.RotationDirect2D(direction, TranformExtension.FacingDirection.DOWN);
                Direction = direction.normalized;
                IgnoreMove = false;
            } ));

       
     
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            if (!_SpeedConstraint)
            {
                tween.Kill();
            }
           if(seqX != null)
            {
                seqX.Kill();
                seqX = null;
            }
        }
        public virtual void setOwner(Character pOwner)
        {
            Owner = pOwner;
            time.timeLineParent = pOwner.time;
            var pWeapons = GetComponents<Weapon>();
            if (weaponChilds != null)
            {
                foreach (var pWeapon in weaponChilds)
                {
                    bool init = false;
                    if (!pWeapon.Owner)
                    {
                        init = true;
                    }
                    pWeapon.Owner = pOwner;
                    if (init && gameObject.activeSelf)
                    {
                        pWeapon.initDone();
                    }
                }
            }
            foreach (var proj in childs)
            {
                proj.setOwner(pOwner);
            }
        }

        public virtual void setWeapon(Weapon pWeapon)
        {
            Weapon = pWeapon;
            foreach (var proj in childs)
            {
                proj.setWeapon(pWeapon);
            }
        }
        [HideIf("isAngularX")]
        public bool moveSin = false;
        [ShowIf("moveSin")]
        [HideIf("isAngularX")]
        public float frequency = 5.0f;
        [ShowIf("moveSin")]
        [HideIf("isAngularX")]
        public float magnitude = 0.1f;
        [HideIf("moveSin")]
        public bool isAngularX;
        [HideIf("moveSin")]
        [ShowIf("isAngularX")]
        public float delayX = 0;
        [HideIf("moveSin")]
        [ShowIf("isAngularX")]
        public Vector2 velocityXRandom = new Vector2(1,1);
        [HideIf("moveSin")]
        [ShowIf("isAngularX")]
        public Vector2 timeVelocityX = new Vector2(0.5f,0.5f);
        [HideIf("moveSin")]
        [ShowIf("isAngularX")]
        public AnimationCurve curveVelocityX = new AnimationCurve(new Keyframe[] { new Keyframe(0,0), new Keyframe(1, 1) });
  
 

        protected float currentVelocityX = 0;
        protected float velocityX = 0;
        protected float currentTime = 0;
        protected Vector2 cacheStartPos;

        protected virtual void Movement()
        {
            if (MovetoTarget)
            {
                Direction = (target - (Vector2)transform.position).normalized;
                if (Vector2.Distance(target, (Vector2)transform.position) <= time.deltaTime * CurrentSpeed)
                {
                    onMoveToTarget.Invoke();
                }
            }
            Vector2 pOldVec = transform.position;
            Vector2 movement = Direction * (CurrentSpeed) * time.deltaTime;
            bool block = false;
            bool blockRotation = false;
            if (movementModules != null && movementModules.Length > 0)
            {
                for (int i = 0; i < movementModules.Length; ++i)
                {
                    movementModules[i].setSpeed(CurrentSpeed);
                     movement = movementModules[i].Movement();
                    if (movementModules[i].isBlockRotation())
                    {
                        blockRotation = true;
                    }
                    transform.Translate(movement, UnityEngine.Space.World);
                    cachePos = cachePos + movement;
                    if (movementModules[i].isBlock() && movementModules[i].IsEnable()) {
                        block = true;
                        break;
                    }
   
                }
      
            }
            if (!block)
            {
                 movement = Direction * (CurrentSpeed) * time.deltaTime;
          
                transform.Translate(movement, UnityEngine.Space.World);
                cachePos = cachePos + movement;
                currentTime += time.deltaTime;
                if (moveSin)
                {
                    transform.position = (Vector3)cachePos + transform.right * Mathf.Sin(currentTime * frequency) * magnitude;
                }
                else if (isAngularX)
                {
                    var pVecX = Vector2.Perpendicular(Direction);
                    transform.position += (Vector3)pVecX.normalized * currentVelocityX * time.deltaTime;
         
                    
                }
            }
          
            if (time.deltaTime != 0 && !moveSin && !IgnoreMove && !blockRotation)
            {
                transform.RotationDirect2D((Vector2)transform.position - pOldVec, TranformExtension.FacingDirection.DOWN);
            }
        }


        protected override void Update()
        {
            base.Update();
            if(currentDelayMove > 0)
            {
                currentDelayMove -= time.deltaTime;
                if(currentDelayMove<= 0)
                {
                    if (_SpeedConstraint)
                    {
                        CurrentSpeed = Speed;
                    }
                    else
                    {
                        CurrentSpeed = SpeedStart;
                        DOTween.To(() => currentSpeed, x => currentSpeed = x, Speed, durationToMax).SetEase(curve);
                    }
                }
                return;
            }
            if (!IgnoreMove)
            {
                Movement();
            }
        }
    }
}