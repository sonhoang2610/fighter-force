using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using EazyEngine.Tools;
using Sirenix.Serialization;
using FlowCanvas;
using NodeCanvas.Framework;

namespace EazyEngine.Space
{
    public enum WeaponState { WeaponIdle, WeaponStart, WeaponStop, WeaponUse, WeaponReloading, WeaponReloadComplete}
    public enum TypeFire {Auto,Semi}
    public enum BehaviourType { Normal, AnimationWait }
    [System.Serializable]
    public struct LevelBoosterInfo
    {
        public int layer;
        public string trigger;
        public string resuseAction;
        [ShowIf("@!string.IsNullOrEmpty( resuseAction)")]
        public bool includeChild ;
        public UnityEvent actions;
        public FlowScriptController flow;
    }
    

    public interface IgnoreObject
    {
         void IgnoreGameObject(GameObject pObject);
         void ClearIgnoreList();

        GameObject[] IgnoreObjects { get; set; }
    }
    [System.Serializable]
    public class UnityEventBool : UnityEvent<bool>
    {

    }
    [System.Serializable]
    public struct EffectInfo
    {
        public GameObject particleEffect;
        public bool onePerUse;
        public int orderlayer ;
        public float scale;
        public bool isLocal;
    }
    [System.Serializable]
    public class AnimationMachine
    {
        [FoldoutGroup("Start", Expanded = true)]
        public string AnimUse = "Shoot";
        [FoldoutGroup("Start", Expanded = true)]
        public UnityEvent onUse;
        [FoldoutGroup("Start", Expanded = true)]
        public string AnimReload = "Reload";
        [FoldoutGroup("Start", Expanded = true)]
        public UnityEvent onReload;

        [FoldoutGroup("Listen", Expanded = true)]
        public string TriggerShoot = "Shoot";
        [FoldoutGroup("Listen", Expanded = true)]
        public UnityEvent onListenShoot;
        [FoldoutGroup("Listen", Expanded = true)]
        public string TriggerShootDone = "ShootDone";
        [FoldoutGroup("Listen", Expanded = true)]
        public UnityEvent onListenShootDone;
        [FoldoutGroup("Listen", Expanded = true)]
        public string TriggerReloadDone = "ReloadDone";
        [FoldoutGroup("Listen", Expanded = true)]
        public UnityEvent onListenReloadDone;
    }
    public class Weapon : TimeControlBehavior,IRespawn, IgnoreObject
    {
        protected bool isPause = false;
        public bool IsPause { get => isPause; set => isPause = value; }
        [System.NonSerialized]
        public WeaponGroupIntance parrentGroup;
        [SerializeField]
        [ListDrawerSettings(ShowIndexLabels = true, Expanded = false, ListElementLabelName = "trigger")]
        public LevelBoosterInfo[] boosterInfos;
        [SerializeField]
        public LevelBoosterInfo[] transitionAction;
        protected int currentIndexBullet;
        public TypeFire typeFire = TypeFire.Semi;
        public bool startWithReloadFirst = false;
        [ShowIf("startWithReloadFirst")]
        public float timeReloadFirst = 0.1f;
        [ShowIf("startWithReloadFirst")]
        public bool randomReloadFirst = false;
        [ShowIf("startWithReloadFirst")]
        [ShowIf("randomReloadFirst")]
        public float randomTo = 0.1f;

        public bool prepareBulletWhenReload = false;
        [FoldoutGroup("Target Setting")]
        public float sizeRemoveTarget = -1;
        [FoldoutGroup("Target Setting")]
        public bool needTargetToFire = false;
        [FoldoutGroup("Target Setting")]
        public bool forceDirectionFollowGun = false;
        [FoldoutGroup("Target Setting")]
        [OnValueChanged("changeRequireFace")]
        public bool requireFaceTargetToFire = false;

        protected bool blockState = false;
        private void changeRequireFace()
        {
            if (!requireFaceTargetToFire)
            {
                onStartRotate = null;
            }
        }
        [FoldoutGroup("Target Setting")]
        [ShowIf("requireFaceTargetToFire")]
        public UnityEvent onStartRotate;
        [FoldoutGroup("Target Setting/Rotate Setting")]
        public GameObject anchorRotation;
        [FoldoutGroup("Target Setting/Rotate Setting")]
        public float rotationSpeedUnlock = 0.5f;
        [FoldoutGroup("Target Setting/Rotate Setting")]
        public float rotationSpeedd = 0.5f;
        [FoldoutGroup("Target Setting/Rotate Setting")]
        public Vector2 defaultFace = Vector2.up;
        [FoldoutGroup("Target Setting/Rotate Setting")]
        public Vector2 directionTargetDefault = Vector2.up;
        [FoldoutGroup("Target Setting/Rotate Setting")]
        public bool comebackDefaultWhenStop = false;
        [FoldoutGroup("Target Setting/Rotate Setting")]
        public bool stopTargetingWhenstop = false;
        [FoldoutGroup("Target Setting/Rotate Setting")]
        public bool allowRotationStart = false;
        protected bool rotating = false;
        public bool restartOnRestoreAmmo = false;
        public Vector2 initialOffset;
        public BehaviourType typeBehaviour = BehaviourType.Normal;
        public bool isRamdomTimeReload = false;
        [ShowIf("isRamdomTimeReload")]
        public Vector2 reloadRamdom = new Vector2(0.1f, 0.2f);
        [HideIf("isRamdomTimeReload")]
        public float timeReload;
        [ShowIfGroup("typeBehaviour", BehaviourType.AnimationWait)]
        [FoldoutGroup("typeBehaviour/Normal")]
        [HideLabel]
        public AnimationMachine machine;

    

        [FoldoutGroup("typeBehaviour/First"), ShowIfGroup("typeBehaviour", BehaviourType.AnimationWait)]

        [HideLabel]
        public AnimationMachine firstMachine;


        [FoldoutGroup("typeBehaviour/ReActive"), ShowIfGroup("typeBehaviour", BehaviourType.AnimationWait)]

        [HideLabel]
        public AnimationMachine reactiveMachine;

        //[HideInEditorMode]
        public int fixDamage = 0;
        public List<DamageExtra> extraDamage =  new  List < DamageExtra >() { new DamageExtra() { damageExtra = 0, type = DamageType.Normal } };
        protected Dictionary<string, Vector2> extraDamageDict = new Dictionary<string, Vector2>();
        protected WeaponGroup weaponParrent;
        public virtual void addDamageExtra(DamageExtra[] extra,string pStr ="")
        {
            if (string.IsNullOrEmpty(pStr))
            {
                extraDamage.AddRange(extra);
            }
            else
            {
                if (!extraDamageDict.ContainsKey(pStr))
                {
                    extraDamageDict.Add(pStr,new Vector2( extraDamage.Count, extra.Length));
                    extraDamage.AddRange(extra);
                }
            }

        }
        public virtual void pauseWeapon(bool pBool)
        {
            isPause = pBool;
        }
        public void setTriggerShootListenString(string pString)
        {
            firstMachine.TriggerShoot = pString;
            reactiveMachine.TriggerShoot = pString;
            machine.TriggerShoot = pString;
        }
        public void removeExtraDamge(string pID)
        {
            if (extraDamageDict.ContainsKey(pID) )
            {
                if (extraDamageDict.Count > extraDamageDict[pID].x) {
                    for (int i = (int)Mathf.Min(extraDamageDict[pID].y + extraDamageDict[pID].x, extraDamage.Count) - 1; i >= 0; --i) {
                        extraDamage.RemoveAt(i);
                    }
                }
            }
        }
        protected WeaponInstancedConfig data;
        private void Start()
        {
            if (allowRotationStart)
            {
                Rotating = true;
            }
        }
        public virtual void setData(WeaponInstancedConfig pData)
        {
            data = pData;
            extraDamage.Add( pData.extraDamge);
            for (int i = 0; i < pData.propEdits.Length; ++i)
            {
                for (int j = 0; j < pData.propEdits[i].propEdits.Length; ++j)
                {
                    var pProp = pData.propEdits[i].propEdits[j];
                    pProp.setValue(gameObject);
                }
            }
        }

        public virtual Vector2 directionWeapon
        {
            get
            {
               return  Vector2.up.Rotate(transform.rotation.eulerAngles.z);
            }
        }
        protected bool showInfoAnimationWaitReload()
        {
            return typeBehaviour == BehaviourType.AnimationWait;
        }
        [FoldoutGroup("Event Setting")]
        [SerializeField]
        public UnityEventBool onStartEvent;
        [FoldoutGroup("Event Setting")]
        public UnityEvent onUse;
        [FoldoutGroup("Event Setting")]
        public UnityEvent onIdle;

        public EffectInfo[] particleEffects;

     

        [HideInInspector]
        public List<GameObject> _listIgnoreObject = new List<GameObject>();
   
        protected Character _owner;
        protected float _currentTimReload;
        [SerializeField]
        [HideInEditorMode]
        protected WeaponState _currentState;
        [HideInEditorMode]
        public WeaponAmmo _ammo;
        [HideInEditorMode]
        public List<GameObject> targetDirection = new List<GameObject>();
        [HideInEditorMode]
        public float factorSpeed = 1;
        [HideInEditorMode]
        public float factorSpeedWeapon = 1;
        [HideInEditorMode]
        public float factorDamage = 1;
        protected bool _isRandom, _ignoreReloadAnim = false;
        protected bool isShootingActive = false;
        public void IgnoreGameObject(GameObject pObject)
        {
            _listIgnoreObject.Add(pObject);
        }
        public void ClearIgnoreList()
        {
            _listIgnoreObject.Clear();
        }
        public void onRestoreAmmo( int pRestoreCount)
        {
            if(pRestoreCount > 0 && isShootingActive && restartOnRestoreAmmo)
            {
                ReActive = true;
                changeState(WeaponState.WeaponStart);
            }
        }
        public virtual int currentTargetCount()
        {
            int pCount = 0;
            for(int i = targetDirection.Count-1 ; i >= 0; --i)
            {
                if (targetDirection[i].gameObject.activeSelf)
                {
                    pCount++;
                }
            }
            return pCount;
        }
        public virtual int countTargetNeeded()
        {
            return 1;
        }
        public virtual int getConsumeShoot()
        {
            return 1;
        }
        public bool IsRandom
        {
            get
            {
                return _isRandom;
            }

            set
            {
                _isRandom = value;
            }
        }
        public virtual Character Owner
        {
            get
            {
                return _owner;
            }

            set
            {
                _owner = value;
            }
        }

        public virtual void addTargetDirection(GameObject pObject)
        {
            targetDirection.Add(pObject);
            if(anchorRotation  && (CurrentState != WeaponState.WeaponIdle || !stopTargetingWhenstop))
            {
                Rotating = true;
            }
        }
        public GameObject TargetDirection
        {
            get
            {
                return targetDirection.Count > 0 ? targetDirection[0] : null;
            }

            set
            {
                targetDirection.Clear();
                targetDirection.Add( value);
            }
        }
        public WeaponState CurrentState
        {
            get
            {
                return _currentState;
            }

            set
            {
                _currentState = value;
            }
        }
        public GameObject[] IgnoreObjects
        {
            get
            {
                return _listIgnoreObject.ToArray();
            }
            set
            {
                _listIgnoreObject.Clear();
                _listIgnoreObject .AddRange( value);
            }
        }

        public float FactorSpeed { get => factorSpeed; set { factorSpeed = value; } }
        public float FactorSpeedWeapon { get => factorSpeedWeapon; set => factorSpeedWeapon = value; }
        public AnimationMachine Machine { get { return IsFirstActive  ? firstMachine : (ReActive ? reactiveMachine :  machine); }  set => machine = value; }

        public float FactorDamage { get => factorDamage; set { factorDamage = value; } }

        public void addFactorSpeed(float pFactor)
        {
            FactorSpeed += pFactor;
        }

        public void addFactorDamage(float pFactor)
        {
            FactorDamage += pFactor;
        }
        public virtual void init()
        {
           
            _ammo = GetComponent<WeaponAmmo>();
        }

        public virtual void initWithInfo(GameObject prop)
        {
            _ammo = GetComponent<WeaponAmmo>();
            Owner = prop.GetComponent<Projectile>().Owner;
        }

        public void InputStartWithReload()
        {
            if (CurrentState != WeaponState.WeaponIdle) return;
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy) return;
            if(parrentGroup != null)
            {
                parrentGroup.ActiveWeapon++;
            }
            isShootingActive = true;
            IsFirstActive = true;
            WaitingRotate = false;
            WaitingTarget = false;
            currentIndexBullet = 0;
            changeState(WeaponState.WeaponStart);
            //if (CurrentState != WeaponState.WeaponIdle) return;
            //isShootingActive = true;
            //_currentTimReload = timeReload;
            //changeState(WeaponState.WeaponReloading);
        }
        private bool isFirstActive = false;
        private bool reActive = false;
        public float TimeReload
        {
            get
            {
                return (IsFirstActive && startWithReloadFirst) ? timeReloadFirst : (isRamdomTimeReload ? Random.Range(reloadRamdom.x, reloadRamdom.y) : timeReload);
            }
        }

        public bool InputStartResult(ref int pCount)
        {
            if (CurrentState != WeaponState.WeaponIdle) return false;
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy) return false;
            pCount++;
            InputStart();
            return true;
        }
        public virtual void InputStart()
        {
            if (CurrentState != WeaponState.WeaponIdle) return;
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy) return;
            isShootingActive = true;
            IsFirstActive = true;
            WaitingRotate = false;
            WaitingTarget = false;
            UnlockFire = false;
            currentIndexBullet = 0;
            if (startWithReloadFirst)
            {
                changeState(WeaponState.WeaponReloading);
            }
            else
            {
                changeState(WeaponState.WeaponStart);
            }
       
        }

        public virtual void WeaponPrepare()
        {

        }

        public virtual void InputStop()
        {
            isShootingActive = false;
            WaitingRotate = false;
            WaitingTarget = false;
            UnlockFire = false;
            changeState(WeaponState.WeaponStop);
        }
        private void OnDisable()
        {
            if (needTargetToFire)
            {
                targetDirection.Clear();
            }
            InputStop();
        }
        public virtual void onTrigger(string pTrigger)
        {
            if (CurrentState == WeaponState.WeaponIdle) return;
            if(pTrigger == Machine.TriggerShoot)
            {
                Machine.onListenShoot.Invoke();
                if (onUse != null)
                {
                    onUse.Invoke();
                }
                WeaponUse();
                if ( string.IsNullOrEmpty(Machine.TriggerShootDone))
                {
                    changeState(WeaponState.WeaponReloading);
                }

            }
            else if(pTrigger == Machine.TriggerShootDone)
            {
                Machine.onListenShootDone.Invoke();
                changeState(WeaponState.WeaponReloading);
              
            }
            else if(pTrigger == Machine.TriggerReloadDone)
            {
                Machine.onListenReloadDone.Invoke();
               changeState(WeaponState.WeaponReloadComplete);
           
            }
        }

        public void triggerToOwner(string pTrigger)
        {
            if (Owner)
            {
                Owner.SetTrigger(pTrigger);
            }
        }

        public virtual void obtainBulletRelaoding()
        {

        }
        public WeaponFindTarget Radar { get; set; }

        public virtual void AnimUse()
        {
            if (_owner)
            {
                _owner.machine.SetTrigger(Machine.AnimUse);
            }
            Machine.onUse.Invoke();
        }

        public virtual void AnimReload()
        {
            _owner.machine.SetTrigger(Machine.AnimReload);
            Machine.onReload.Invoke();
        }
        public virtual void WeaponStart()
        {
            if (needTargetToFire)
            {
                if (!Radar)
                {
                    Radar = GetComponent<WeaponFindTarget>();
                }       
                Radar.RequestTarget(this);
            }
        }

        public virtual void changeState(WeaponState nextState)
        {
            if(blockState)
            {
                CurrentState = nextState;
                return;
            }
            if(CurrentState == WeaponState.WeaponUse && nextState == WeaponState.WeaponReloading)
            {
                IsFirstActive = false;
         
                if (typeFire == TypeFire.Semi)
                {
                    changeState(WeaponState.WeaponStop);
                    return;
                }
            }
            CurrentState = nextState;
            if (CurrentState == WeaponState.WeaponReloading)
            {
                if (typeBehaviour == BehaviourType.AnimationWait)
                {
                    if (!string.IsNullOrEmpty(Machine.AnimReload))
                    {
                        AnimReload();
                    }
                    else
                    {
                        _ignoreReloadAnim = true;
                        _currentTimReload = TimeReload;
                    }
                }
                if (prepareBulletWhenReload)
                {
                    obtainBulletRelaoding();
                }
            }
            if (CurrentState == WeaponState.WeaponStart)
            {
                WeaponStart();
                onStartEvent.Invoke(IsFirstActive);
                ShootRequest();    
            }
            else if (CurrentState == WeaponState.WeaponUse)
            {
                if (typeBehaviour == BehaviourType.AnimationWait && !string.IsNullOrEmpty(Machine.AnimUse))
                {
                    AnimUse();
                    if (string.IsNullOrEmpty(Machine.TriggerShoot))
                    {
                        if (onUse != null)
                        {
                            onUse.Invoke();
                        }
                        WeaponUse();
                        if ( string.IsNullOrEmpty(Machine.TriggerShootDone))
                        {
                            changeState(WeaponState.WeaponReloading);
                        }
                        _currentTimReload = TimeReload;
                    }
                }
                else 
                {
                    if (onUse != null)
                    {
                        onUse.Invoke();
                    }
                    WeaponUse();
                    if (typeBehaviour != BehaviourType.AnimationWait || string.IsNullOrEmpty(Machine.TriggerShootDone))
                    {
                        changeState(WeaponState.WeaponReloading);
                    }
                    _currentTimReload = TimeReload;
                }
            }
            else if (CurrentState == WeaponState.WeaponReloadComplete)
            {
                ReActive = false;
                changeState(WeaponState.WeaponStart);
            }
            else if (CurrentState == WeaponState.WeaponStop)
            {
                if(parrentGroup != null && parrentGroup.ActiveWeapon  > 0)
                {
                    parrentGroup.ActiveWeapon--;
                }
                if (Owner)
                {
                    Owner.GetComponent<CharacterHandleWeapon>().triggerStopWeapon(this);
                }
                if(onIdle != null)
                {
                    onIdle.Invoke();
                }
                WeaponIdle();
                changeState(WeaponState.WeaponIdle);
            }
        }
        public bool Rotating
        {
            get
            {
                return rotating;
            }

            set
            {
                rotating = value;
            }
        }
        [ShowInInspector]
        public bool BlockState { get => blockState; set => blockState = value; }
        public virtual bool IsFirstActive { get => isFirstActive; set => isFirstActive = value; }
        public virtual bool ReActive { get => reActive; set => reActive = value; }
        public virtual bool WaitingTarget { get => waitingTarget; set => waitingTarget = value; }
        public virtual bool UnlockFire { get => unlockFire; set => unlockFire = value; }
        public virtual bool WaitingRotate { get => waitingRotate; set => waitingRotate = value; }

        public virtual void WeaponIdle()
        {
            WaitingRotate = false;
            WaitingTarget = false;
            UnlockFire = false;
            if (Radar && stopTargetingWhenstop)
            {
                Radar.UnRequestTargetFinding(this);
                Radar = null;
                targetDirection.Clear();
            }
 
            if (_ammo)
            {
                _ammo.TotalRestore = 0;
            }
        }

        public bool isReady()
        {
            if (_ammo)
            {
                return _ammo.storageBullet >= getConsumeShoot();
            }
            else
            {
                return true;
            }
        }
        private bool unlockFire = false;

        private bool waitingTarget = false;
        private bool waitingRotate = false;
        public virtual void ShootRequest()
        {
            if (needTargetToFire && currentTargetCount()<=0)
            {
                if (!Radar)
                {
                    changeState(WeaponState.WeaponStop);
                }
                return;
            }

            if(!UnlockFire && requireFaceTargetToFire && needTargetToFire && Rotating)
            {
                if(onStartRotate != null)
                {
                    onStartRotate.Invoke();
                }
                WaitingRotate = true;
                return;
            }
            UnlockFire = true;
            if (_ammo)
            {
                if (_ammo.EnoughToFire(getConsumeShoot()))
                {
                    _ammo.changeStorage(-getConsumeShoot());
                    changeState(WeaponState.WeaponUse);
                }
                else
                {
                    if (Owner)
                    {
                        _owner.GetComponent<CharacterHandleWeapon>().triggerNotEngough(this);
                    }
                    if (!restartOnRestoreAmmo)
                    {
                        changeState(WeaponState.WeaponStop);
                    }
                    else
                    {
                        if (onIdle != null)
                        {
                            onIdle.Invoke();
                        }
                        WeaponIdle();
                    }
                }
            }
            else
            {
                changeState(WeaponState.WeaponUse);
            }
        }

        public virtual void WeaponUse()
        {
      
            EzEventManager.TriggerEvent(new AttackEvent(AttackStatus.Running, Owner != null ? Owner.gameObject : null)); 
            
        }
        
        
        
        protected virtual void LateUpdate()
        {
            if (BlockState) return;
            if(targetDirection.Count > 0 && sizeRemoveTarget > 0)
            {
                for(int i = targetDirection.Count -1; i >= 0; --i)
                {
                    if(Vector3.Distance(targetDirection[i].transform.position, transform.position) >= sizeRemoveTarget)
                    {
                        targetDirection.RemoveAt(i);
                    }
                }
            }
            if ((TargetDirection || comebackDefaultWhenStop) && anchorRotation && rotating )
            {
                if (anchorRotation.name.Contains("[block]"))
                {
                    if (!anchorRotation.name.StartsWith(gameObject.name)) {
                        return;
                    }
                }
                else
                {
                    anchorRotation.name = gameObject.name + "[block]" + anchorRotation.name;
                }

                //  isReady = false;
                Vector2 directon = directionTargetDefault;
                if (TargetDirection)
                {
                    directon = (TargetDirection.transform.position - transform.position).normalized;

                }
                float pAngle = Vector2.SignedAngle(defaultFace, directon);
                //  pAngle = -pAngle;
                if (anchorRotation.transform.parent.lossyScale.x < 0)
                {
                    pAngle = -pAngle;
                }
                var pRotation = Quaternion.Euler(0, 0, pAngle);
                var pDestiny = Quaternion.Inverse(anchorRotation.transform.parent.rotation) * pRotation;
                float from = (anchorRotation.transform.localRotation.eulerAngles.z);
                float to = (pDestiny.eulerAngles.z);
                float pSpeed = rotationSpeedd;
                if(UnlockFire)
                {
                    pSpeed = rotationSpeedUnlock;
                }
                anchorRotation.transform.localRotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(from, to, time.deltaTime * pSpeed));
               var dir =  (Vector2)(Quaternion.Euler(0, 0, anchorRotation.transform.localRotation.eulerAngles.z) * defaultFace);
                if (!UnlockFire && WaitingRotate)
                {
                    var hits = MMDebug.RayCastAll(anchorRotation.transform.position, dir.normalized, 20, LayerMask.GetMask("Player"), Color.red, true);
                    for (int i = 0; i < hits.Length; ++i)
                    {
                        if (TargetDirection&& hits[i].collider.gameObject == TargetDirection.gameObject)
                        {
                            UnlockFire = true;
                            ShootRequest();
                        }
                    }
                }
            }
            if (CurrentState == WeaponState.WeaponReloading)
            {
                if (typeBehaviour == BehaviourType.Normal || _ignoreReloadAnim)
                {
                    _currentTimReload -= time.deltaTime*FactorSpeed* FactorSpeedWeapon;
                    if (_currentTimReload <= 0)
                    {
                        changeState(WeaponState.WeaponReloadComplete);
                    }
                }
            }
            if (needTargetToFire)
            {
            
                for (int i = targetDirection.Count - 1; i >= 0; --i)
                {
                    if (!targetDirection[i].gameObject.activeSelf)
                    {
                        targetDirection.RemoveAt(i);
                    }
                }
                if (WaitingTarget && targetDirection.Count > 0)
                {
                    WaitingTarget = true;
                    ShootRequest();
                }
            }
        }



        public virtual void onRespawn()
        {
            ClearIgnoreList();
        }

        public GraphOwner flow { get { return GetComponent<GraphOwner>(); } }

        public virtual int FixDamage { get => fixDamage; set => fixDamage = value; }
        public WeaponGroup WeaponParrent { get => weaponParrent; set => weaponParrent = value; }

        public bool boosterGetReaction(string pTrigger)
        {
            if (flow)
            {
                flow.SendEvent<string>("Booster", pTrigger);
            }
            int index = -1;
            int layer = 0;
            for (int i = 0; i < boosterInfos.Length; ++i)
            {
                if (pTrigger == boosterInfos[i].trigger)
                {
                    index = i;
                    layer = boosterInfos[i].layer;
                }
            }
            if (!layerManager.ContainsKey(layer))
            {
                layerManager.Add(layer, "Default");
            }
            for (int i = 0; i < transitionAction.Length; ++i)
            {
                if (layerManager[layer] + "->" + pTrigger == transitionAction[i].trigger || (layerManager[layer] == transitionAction[i].trigger.Substring(0, layerManager[layer].Length) && transitionAction[i].trigger.EndsWith("Any")))
                {
                    transitionAction[i].actions.Invoke();
                }
            }
            layerManager[layer] = pTrigger;
            if (index >= 0 && index < boosterInfos.Length)
            {
                executeAction(index);
                return true;
            }
            return false;
        }

        public Dictionary<int, string> layerManager = new Dictionary<int, string>();
        public void booster(string pTrigger)
        {
            if (flow)
            {
                flow.SendEvent<string>("Booster", pTrigger);
            }
            int index = -1;
            int layer = 0;
            for (int i = 0; i < boosterInfos.Length; ++i)
            {
                if (pTrigger == boosterInfos[i].trigger)
                {
                    index = i;
                    layer = boosterInfos[i].layer;
                }
            }
            if (!layerManager.ContainsKey(layer))
            {
                layerManager.Add(layer, "Default");
            }
            for (int i = 0; i < transitionAction.Length; ++i)
            {
                if (layerManager[layer] + "->" + pTrigger == transitionAction[i].trigger)
                {
                    transitionAction[index].actions.Invoke();
                }
            }
            layerManager[layer] = pTrigger;
            if (index >=0 && index < boosterInfos.Length)
            {
                executeAction(index);
            }
        }

        public void executeAction(int index)
        {
            if (!string.IsNullOrEmpty( boosterInfos[index].resuseAction))
            {
                var pActions = boosterInfos[index].resuseAction.Split('+');
                for(int i = 0; i < pActions.Length; ++i)
                {
                    for (int j = 0; j < boosterInfos.Length; ++j)
                    {
                        if (pActions[i] == boosterInfos[j].trigger)
                        {
                            if (boosterInfos[j].includeChild)
                            {
                                executeAction(j);
                                break;
                            }
                            else
                            {
                                boosterInfos[j].actions.Invoke();
                            }
                        }
                    }
                }
            }
            boosterInfos[index].actions.Invoke();
        }
    }
}
