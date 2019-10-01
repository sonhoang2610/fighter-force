using EazyEngine.Tools;
using EazyEngine.Tools.Space;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class WeaponInstanceInfo
    {
        public Weapon IntialWeapon = null;
        [HideInInspector]
        public Weapon IntancedWeapon = null;
        public GameObject AttachmentWeapon = null;
        public GameObject AnchorGunToRotate = null;

    }
    public enum AttackStatus
    {
        Start,
        Running,
        Complete
    }
    public struct AttackEvent
    {
        public AttackStatus status;
        public GameObject owner;
        public AttackEvent(AttackStatus pStatus, GameObject pOwner)
        {
            status = pStatus;
            owner = pOwner;
        }
    }

    public enum ChangeWeaponCondition
    {
        NeverChange,
        ChangeWhenNotEnough,
        ChangeWhenAllStopAttack
    }

    [System.Serializable]
    public class WeaponGroupIntance
    {
        public string nameLabel()
        {
            return triggerToForceChange.Count > 0 ? triggerToForceChange[0] : "None";
        }
        public WeaponInstanceInfo[] weapons;
        public bool isUseOnInnital = true;
        public List<string> triggerToForceChange = new List<string>();
        public List<string> triggerToForceDisable = new List<string>();
        public bool removeOnStop = false;
        public bool skipChangeWeapon = false;
        public UnityEvent onStop;
        public UnityEvent onStart;

        private int activeWeapon = 0;

        public int ActiveWeapon { get => activeWeapon; set => activeWeapon = value; }
    }
    [Flags]
    [System.Serializable]
    public enum TriggerWeaponState
    {
        None,
        EnableSkill,
        DisableSkill,
    }
    public class CharacterHandleWeapon : CharacrerAbility, IListenerTriggerAnimator, IRespawn, EzEventListener<MessageGamePlayEvent>, EzEventListener<CharacterChangeState>
    {
        public bool registerDamagedDownBooster = false;
        public bool registerListenAnimatorTrigger = false;
        public bool registerMessEvent = false;
        [ShowIf("registerMessEvent")]
        public bool registerMessGameChangeWeapon = false;
        public bool shoortOnStart = false;
        public bool insideScreenRequire = false;
        public float factorDamage = 1;
        public float factorSpeed = 1;
        public ChangeWeaponCondition conditionChangeWeapon = ChangeWeaponCondition.NeverChange;
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "nameLabel")]
        public WeaponGroupIntance[] DatabaseWeapon;

        protected bool isShooting = false;
        protected List<Weapon> countShooting = new List<Weapon>();
        [EazyEngine.Tools.ReadOnly]
        [ListDrawerSettings(HideAddButton = true)]
        public List<Weapon> _currentWeapons;
        [HideInEditorMode]
        public List<Weapon[]> _storageWeapons;

        public GameObject[] damageOnTouchExtension;

        public List<CharacterHandleWeapon> handleChilds = new List<CharacterHandleWeapon>();

        Character _char;
        protected GameObject _targetDirection;
        protected bool _isRandom;
        protected bool isInit = false;
        protected int indexGroup = 0;
        protected int startLevelBooster = 0;
        protected bool firstShoot = true;
        [HideInEditorMode]
        private int fixDamage = 0;

        protected WeaponInstancedConfig[] cacheWeaponData;
        protected Dictionary<Weapon, Weapon> weaponPaths = new Dictionary<Weapon, Weapon>();
        public void setData(WeaponInstancedConfig[] pDatas)
        {
            cacheWeaponData = pDatas;
            if (isInit)
            {
                for (int j = 0; j < _storageWeapons.Count; ++j)
                {
                    var pWeapons = _storageWeapons[j];
                    foreach (var pWeapon in pWeapons)
                    {
                        if (cacheWeaponData != null)
                        {
                            foreach (var pWeaponInfo in cacheWeaponData)
                            {
                                if (pWeaponInfo.target == weaponPaths[pWeapon])
                                {
                                    pWeapon.setData(pWeaponInfo);
                                }
                            }
                        }
                    }
                }
            }
        }

        public WeaponInstanceInfo[] InitialWeapons
        {
            get
            {
                List<WeaponInstanceInfo> weaponInit = new List<WeaponInstanceInfo>();
                for (int i = 0; i < DatabaseWeapon.Length; ++i)
                {
                    if (DatabaseWeapon[i].isUseOnInnital)
                    {
                        weaponInit.AddRange(DatabaseWeapon[i].weapons);
                    }
                }
                return weaponInit.ToArray();
            }
        }
        public void onRespawn()
        {
            if (isShooting)
            {
                ShootStop();
            }
            for (int i = 0; i < _storageWeapons.Count; ++i)
            {
                for (int j = 0; j < _storageWeapons[i].Length; ++j)
                {
                    var pRess = _storageWeapons[i][j].GetComponents<IRespawn>();
                    foreach (var pRes in pRess)
                    {
                        pRes.onRespawn();
                    }
                }
            }
            _targetDirection = null;
            firstShoot = true;
            isSupering = false;
            currentDurationSuper = 0;
            isInit = false;
            initialAbility();
        }
        public bool IsRandom
        {
            set
            {
                _isRandom = value;
                for (int i = 0; i < _currentWeapons.Count; ++i)
                {
                    _currentWeapons[i].IsRandom = value;
                }
            }
            get
            {
                return _isRandom;
            }
        }
        public int FixDamage
        {
            get => fixDamage; set
            {
                if (_storageWeapons != null && value != 0)
                {
                    for (int j = 0; j < _storageWeapons.Count; ++j)
                    {
                        for (int i = 0; i < _storageWeapons[j].Length; ++i)
                        {
                            _storageWeapons[j][i].FixDamage = value;
                        }
                    }
                }
                for (int i = 0; i < damageOnTouchExtension.Length; ++i)
                {
                    var pDamages = damageOnTouchExtension[i].GetComponentsInChildren<DamageOnTouch>();
                    foreach (var pDamage in pDamages)
                    {
                        pDamage.DamageCaused = value;
                    }
                }
                fixDamage = value;
            }
        }

        public int CacheLevelBooster
        {
            get => cacheLevelBooster; set
            {
                cacheLevelBooster = value;
                EzEventManager.TriggerEvent<MessageGamePlayEvent>(new MessageGamePlayEvent("ChangeBooster", gameObject, value));
            }
        }

        public void setTarget(GameObject pTargetDirection)
        {
            _targetDirection = pTargetDirection;
            for (int i = 0; i < _currentWeapons.Count; ++i)
            {
                _currentWeapons[i].TargetDirection = _targetDirection;
            }
        }
        private void Awake()
        {
            var pHealth = GetComponent<Health>();
            if (pHealth && registerDamagedDownBooster)
            {
                pHealth.onTakenDamage.AddListener(onDamaged);
            }
        }

        public void onDamaged(int pDamaged)
        {
            if (pDamaged > 0)
            {
                if (startLevelBooster == 6)
                {
                    CacheLevelBooster--;
                    if (CacheLevelBooster <= 0)
                    {
                        CacheLevelBooster = 0;
                    }
                }
                else
                {
                    int pLEvel = startLevelBooster - 1;
                    if (pLEvel <= 0)
                    {
                        pLEvel = 0;
                    }
                    booster("Booster" + pLEvel);
                }
            }
        }
        public override void addChild(Character pChild)
        {
            base.addChild(pChild);
            handleChilds.Add(pChild.GetComponent<CharacterHandleWeapon>());
        }
        public override void initialAbility()
        {
            for (int i = 0; i < damageOnTouchExtension.Length; ++i)
            {
                var pDamages = damageOnTouchExtension[i].GetComponentsInChildren<DamageOnTouch>();
                foreach (var pDamage in pDamages)
                {
                    pDamage.FactorDamage = factorDamage;
                    pDamage.DamageCaused = FixDamage;
                }
            }
            if (_currentWeapons == null)
            {
                _currentWeapons = new List<Weapon>();
            }
            _char = GetComponent<Character>();
            var Health = GetComponent<Health>();
            if (Health)
            {
                Health.onDeath.AddListener(delegate
                {
                    ShootStop();
                });
            }
            base.initialAbility();
            if (_currentWeapons == null)
            {
                _currentWeapons = new List<Weapon>();
            }
            if (_storageWeapons == null)
            {
                _storageWeapons = new List<Weapon[]>();
            }
            if (_storageWeapons.Count == 0)
            {
                for (int j = 0; j < DatabaseWeapon.Length; ++j)
                {
                    List<Weapon> pWeapons = new List<Weapon>();
                    for (int i = 0; i < DatabaseWeapon[j].weapons.Length; ++i)
                    {
                        var pWeaponInstanced = DatabaseWeapon[j].weapons[i];
                        var pWeapon = pWeaponInstanced.IntialWeapon;
                        if (pWeaponInstanced.IntialWeapon.transform.parent == null)
                        {

                            pWeapon = Instantiate(pWeaponInstanced.IntialWeapon, pWeaponInstanced.AttachmentWeapon.transform);
                            pWeapon.name = pWeaponInstanced.IntialWeapon.name;
                            weaponPaths.Add(pWeapon, pWeaponInstanced.IntialWeapon);
                        }
                        if (cacheWeaponData != null)
                        {
                            foreach (var pWeaponInfo in cacheWeaponData)
                            {
                                if (pWeaponInfo.target == pWeaponInstanced.IntialWeapon)
                                {
                                    pWeapon.setData(pWeaponInfo);
                                }
                            }
                        }
                        DatabaseWeapon[j].weapons[i].IntancedWeapon = pWeapon;
                        pWeapon.init();
                        pWeapon.Owner = _char;
                        pWeapon.time._groupName = _char.time._groupName;
                        if (FixDamage != 0)
                        {
                            pWeapon.FixDamage = FixDamage;
                        }
                        pWeapon.FactorSpeed = factorSpeed;
                        pWeapon.anchorRotation = pWeaponInstanced.AnchorGunToRotate;
                        pWeapon.parrentGroup = DatabaseWeapon[j];
                        if (DatabaseWeapon[j].isUseOnInnital)
                        {
                            _currentWeapons.Add(pWeapon);
                        }
                        pWeapons.Add(pWeapon);
                        pWeapon.initDone();
                    }
                    _storageWeapons.Add(pWeapons.ToArray());
                }
            }
            else
            {
                _currentWeapons.Clear();
                for (int j = 0; j < DatabaseWeapon.Length; ++j)
                {
                    List<Weapon> pWeapons = new List<Weapon>();
                    for (int i = 0; i < DatabaseWeapon[j].weapons.Length; ++i)
                    {

                        var pWeapon = _storageWeapons[j][i];

                        if (DatabaseWeapon[j].isUseOnInnital)
                        {
                            _currentWeapons.Add(pWeapon);
                        }
                        pWeapons.Add(pWeapon);
                    }
                }
            }


            if (shoortOnStart && !insideScreenRequire)
            {
                ShootStart();
            }
            isInit = true;
        }
        public void triggerStopWeapon(Weapon pWeapon)
        {
            if (pWeapon.parrentGroup == null) return;
            if (!_currentWeapons.Contains(pWeapon)) return;
            if (!countShooting.Contains(pWeapon)) return;
            if (countShooting.Count > 0)
            {
                if (name.StartsWith("MainPlane6"))
                {
                    Debug.Log("here + " + countShooting.Count + "a" + _currentWeapons.Count);
                }

                if (pWeapon.parrentGroup.removeOnStop)
                {
                    _currentWeapons.Remove(pWeapon);
                }
                if (pWeapon.parrentGroup.ActiveWeapon == 0)
                {
                    pWeapon.parrentGroup.onStop.Invoke();
                }
                countShooting.Remove(pWeapon);
                if (countShooting.Count != _currentWeapons.Count && name.StartsWith("MainPlane6"))
                {
                    Debug.Log("here");
                }

                if (conditionChangeWeapon != ChangeWeaponCondition.ChangeWhenAllStopAttack)
                {
                    if (countShooting.Count <= 0)
                    {
                        ShootStopComon();
                    }
                }
                else if (countShooting.Count <= 0)
                {
                    int pBreak = 0;
                    do
                    {
                        pBreak++;
                        indexGroup++;
                        if (indexGroup >= DatabaseWeapon.Length)
                        {
                            indexGroup = 0;
                        }
                        if (pBreak > 100)
                        {
                            break;
                        }
                    } while (DatabaseWeapon[indexGroup].skipChangeWeapon);
                    for (int j = 0; j < DatabaseWeapon[indexGroup].weapons.Length; ++j)
                    {
                        var pWeapon1 = _storageWeapons[indexGroup][j];
                        if (!_currentWeapons.Contains(pWeapon1) && pWeapon1.gameObject.activeSelf && pWeapon1.gameObject.activeInHierarchy)
                        {
                            _currentWeapons.Add(pWeapon1);
                            if (isShooting)
                            {
                                pWeapon1.InputStartResult(ref countShooting);
                            }
                        }
                    }

                }

            }
        }
        public void triggerNotEngough(Weapon pWeapon)
        {
            if (!_currentWeapons.Contains(pWeapon)) return;
            if (!countShooting.Contains(pWeapon)) return;
            if (conditionChangeWeapon == ChangeWeaponCondition.ChangeWhenNotEnough)
            {
                _currentWeapons.Remove(pWeapon);
                countShooting.Remove(pWeapon);
                if (_currentWeapons.Count == 0)
                {
                    for (int i = 0; i < _storageWeapons.Count; ++i)
                    {
                        bool pOK = false;
                        for (int j = 0; j < _storageWeapons[i].Length; ++j)
                        {

                            if (_storageWeapons[i][j].isReady() && (_storageWeapons[i][j].CurrentState == WeaponState.WeaponIdle || _storageWeapons[i][j].CurrentState == WeaponState.WeaponStop))
                            {
                                pOK = true;
                                _currentWeapons.Add(_storageWeapons[i][j]);
                                bool result = _storageWeapons[i][j].InputStartResult(ref countShooting);
                            }
                        }
                        if (pOK)
                        {
                            break;
                        }
                    }
                    if (_currentWeapons.Count == 0)
                    {
                        for (int i = 0; i < InitialWeapons.Length; ++i)
                        {
                            _currentWeapons.Add(InitialWeapons[i].IntancedWeapon);
                        }
                        ShootStop();
                    }
                }

            }

        }
        public void ShootStart()
        {
            if (_currentWeapons == null || isShooting) return;
            if (!isShooting)
            {
                EzEventManager.TriggerEvent(new AttackEvent(AttackStatus.Start, gameObject));
            }
            if (_currentWeapons.Count > 0)
            {
                isShooting = true;
                for (int i = _currentWeapons.Count - 1; i >= 0; --i)
                {
                    bool result = _currentWeapons[i].InputStartResult(ref countShooting);
                }

            }
        }
        public void ShootStopComon()
        {
            if (_currentWeapons == null) return;
            countShooting.Clear();
            isShooting = false;
            EzEventManager.TriggerEvent(new AttackEvent(AttackStatus.Complete, gameObject));
        }
        public void ShootStop()
        {
            if (_currentWeapons == null) return;
            for (int i = _currentWeapons.Count - 1; i >= 0; --i)
            {
                _currentWeapons[i].InputStop();
            }

            ShootStopComon();
        }
        public void setOverrideWeapon(WeaponInstanceInfo[] pWeapons)
        {
            //overriding = true;
            //if(_currentWeapons != null)
            //{
            //    for(int  i = 0; i < _currentWeapons.Count; ++i)
            //    {
            //        _currentWeapons[i].InputStop();
            //        savedWeaponBeforeOverride.Add(_currentWeapons[i]);
            //    }
            //    _currentWeapons.Clear();
            //}
            //else
            //{
            //    _currentWeapons = new List<Weapon>();
            //}
            //for (int i = 0; i < pWeapons.Length; ++i)
            //{
            //    var pWeapon = pWeapons[i].IntialWeapon;
            //    if (pWeapon.transform.parent == null)
            //    {
            //        if (pWeapons[i].IntialWeapon.transform.parent == null)
            //        {
            //            pWeapon = Instantiate(pWeapons[i].IntialWeapon, pWeapons[i].AttachmentWeapon.transform);
            //        }
            //        pWeapon.init();
            //        pWeapon.Owner = _char;
            //        pWeapon.time._groupName = _char.time._groupName;
            //        pWeapon.anchorRotation = pWeapons[i].AnchorGunToRotate;
            //    }
            //    if (pWeapons[i].isUseOnInnital)
            //    {
            //        _currentWeapons.Add(pWeapon);
            //    }
            //    else
            //    {
            //        //pWeapon.gameObject.SetActive(false);
            //    }
            //}
            //    for (int i = 0; i < _currentWeapons.Count; ++i)
            //{
            //    _currentWeapons[i].InputStart();
            //}
        }
        private void Start()
        {

        }
        private void Update()
        {
            if (insideScreenRequire && shoortOnStart && firstShoot)
            {
                if (LevelManger.InstanceRaw != null && LevelManger.Instance.mainPlayCamera.Rect().Contains(transform.position))
                {
                    ShootStart();
                    firstShoot = false;
                }
            }
        }
        public bool registerListen()
        {
            return registerListenAnimatorTrigger;
        }
        public void TriggerFromAnimator(AnimationEvent pEvent)
        {
            for (int i = 0; i < _currentWeapons.Count; ++i)
            {
                _currentWeapons[i].onTrigger(pEvent.stringParameter);
            }
        }
        public Weapon[] triggerAndGetReactionWeapon(string pTrigger, Blackboard pParentVars = null)
        {
            List<Weapon> pWeapons = new List<Weapon>();
            for (int i = _currentWeapons.Count - 1; i >= 0; --i)
            {
                var pBool = _currentWeapons[i].boosterGetReaction(pTrigger, pParentVars);
                if (pBool)
                {
                    pWeapons.Add(_currentWeapons[i]);
                }
            }
            for (int i = holdWeapon.Count - 1; i >= 0; --i)
            {
                if (!_currentWeapons.Contains(holdWeapon[i]))
                {
                    var pBool = holdWeapon[i].boosterGetReaction(pTrigger, pParentVars);
                    if (pBool)
                    {
                        pWeapons.Add(holdWeapon[i]);
                    }
                }

            }
            for (int i = 0; i < handleChilds.Count; ++i)
            {
                pWeapons.AddRange(handleChilds[i].triggerAndGetReactionWeapon(pTrigger, pParentVars));
            }
            return pWeapons.ToArray();
        }
        public Weapon[] triggerAndGetReactionWeapon(string pTrigger)
        {
            return triggerAndGetReactionWeapon(pTrigger, null);
        }
        [System.NonSerialized]
        [ShowInInspector]
        [HideInEditorMode]
        public List<Weapon> holdWeapon = new List<Weapon>();
        public void triggerChangeWeapon(string pTrigger)
        {
            triggerHandleWepaon(pTrigger);
            triggerAndGetReactionWeapon(pTrigger);
        }
        public TriggerWeaponState checkTriggerHandleWeapon(string pTrigger)
        {
            TriggerWeaponState result = TriggerWeaponState.None;
            for (int i = 0; i < DatabaseWeapon.Length; ++i)
            {
                if (DatabaseWeapon[i].triggerToForceChange.Contains(pTrigger))
                {
                    DatabaseWeapon[i].onStart.Invoke();
                    for (int j = 0; j < DatabaseWeapon[i].weapons.Length; ++j)
                    {
                        var pWeapon = _storageWeapons[i][j];
                        if (!_currentWeapons.Contains(pWeapon))
                        {
                            holdWeapon.Add(pWeapon);
                            _currentWeapons.Add(pWeapon);
                            if (isShooting)
                            {
                                bool pResult = pWeapon.InputStartResult(ref countShooting);
                                if (pResult)
                                {
                                    result = TriggerWeaponState.EnableSkill;
                                }
                            }
                        }
                    }
                }
            }
            for (int i = _currentWeapons.Count - 1; i >= 0; --i)
            {
                if (_currentWeapons[i].parrentGroup.triggerToForceDisable.Contains(pTrigger))
                {
                    _currentWeapons[i].InputStop();
                }
            }
            result |= TriggerWeaponState.DisableSkill;
            if (shoortOnStart && _currentWeapons.Count > 0)
            {
                if (!insideScreenRequire || LevelManger.Instance.mainPlayCamera.Rect().Contains(transform.position))
                {
                    ShootStart();
                }
            }
            for (int i = 0; i < handleChilds.Count; ++i)
            {
                handleChilds[i].triggerHandleWepaon(pTrigger);
            }
            return result;
        }

        public void triggerHandleWepaon(string pTrigger)
        {
            for (int i = 0; i < DatabaseWeapon.Length; ++i)
            {
                if (DatabaseWeapon[i].triggerToForceChange.Contains(pTrigger))
                {
                    DatabaseWeapon[i].onStart.Invoke();
                    for (int j = 0; j < DatabaseWeapon[i].weapons.Length; ++j)
                    {
                        var pWeapon = _storageWeapons[i][j];
                        if (!_currentWeapons.Contains(pWeapon))
                        {
                            holdWeapon.Add(pWeapon);
                            _currentWeapons.Add(pWeapon);
                            if (isShooting)
                            {
                                bool pResult = pWeapon.InputStartResult(ref countShooting);
                            }
                        }
                    }
                }
            }
            for (int i = _currentWeapons.Count - 1; i >= 0; --i)
            {
                if (_currentWeapons[i].parrentGroup.triggerToForceDisable.Contains(pTrigger))
                {
                    _currentWeapons[i].InputStop();
                }
            }
            if (shoortOnStart && _currentWeapons.Count > 0)
            {
                if (!insideScreenRequire || LevelManger.Instance.mainPlayCamera.Rect().Contains(transform.position))
                {
                    ShootStart();
                }
            }
            for (int i = 0; i < handleChilds.Count; ++i)
            {
                handleChilds[i].triggerHandleWepaon(pTrigger);
            }
        }
        protected float currentDurationSuper = 0;
        public void addBoosterDamage(float pFactor)
        {
            factorDamage += pFactor;
            for (int i = 0; i < _storageWeapons.Count; ++i)
            {
                for (int j = 0; j < _storageWeapons[i].Length; ++j)
                {
                    _storageWeapons[i][j].FactorDamage = factorDamage;
                }
            }
        }
        public void boosterDamage(float pFactor)
        {
            factorDamage = pFactor;
            for (int i = 0; i < _storageWeapons.Count; ++i)
            {
                for (int j = 0; j < _storageWeapons[i].Length; ++j)
                {
                    _storageWeapons[i][j].FactorDamage = pFactor;
                }
            }
        }
        public void booster(string pID)
        {
            Debug.Log(pID);

            if (pID.StartsWith("Booster"))
            {
                bool planSupering = false;
                string pBoosterString = pID.Remove(0, 7);
                int pLevelBooster = startLevelBooster;
                if (!isSupering)
                {
                    if (startLevelBooster > 0)
                    {
                        factorSpeed -= (startLevelBooster / 2 + startLevelBooster % 2) * 0.2f;
                        if (startLevelBooster == 6)
                        {
                            factorSpeed -= 0.2f;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(pBoosterString) && int.TryParse(pBoosterString, out pLevelBooster))
                {

                    if (!isSupering)
                    {
                        if (pLevelBooster == 6)
                        {
                            planSupering = true;
                            currentDurationSuper = 0;
                            setFactorSpeedWeapon(factorSpeed + (pLevelBooster / 2 + pLevelBooster % 2) * 0.2f + 0.2f);
                     
                        }
                        else
                        {
                            setFactorSpeedWeapon(factorSpeed + (pLevelBooster / 2 + pLevelBooster % 2) * 0.2f);
                            CacheLevelBooster = pLevelBooster;

                        }
             
                        startLevelBooster = pLevelBooster;
                    }
                    else if (pLevelBooster == 6)
                    {
                        currentDurationSuper = 0;
                    }
                    else
                    {
                        CacheLevelBooster = pLevelBooster;
                    }
                  
                }
                else
                {
                    if (isSupering)
                    {
                        CacheLevelBooster++;
                        if (CacheLevelBooster > 5)
                        {
                            CacheLevelBooster = 5;
                        }
                        return;
                    }
                    startLevelBooster++;

                    if (startLevelBooster > 5)
                    {
                        startLevelBooster = 5;
                    }
                    CacheLevelBooster = startLevelBooster;
                    setFactorSpeedWeapon(factorSpeed + (startLevelBooster / 2 + startLevelBooster % 2) * 0.2f);
                }


                if (!isSupering)
                {
                    triggerChangeWeapon("Booster" + startLevelBooster);
                    EzEventManager.TriggerEvent(new MessageGamePlayEvent("Booster" + startLevelBooster, gameObject));
                }
                if (planSupering)
                {
                    isSupering = true;

                }
                if (!isSupering)
                {
                    EzEventManager.TriggerEvent(new MessageGamePlayEvent("Normal", gameObject));
                }
            }
        }


        private void OnEnable()
        {
            if (registerMessEvent)
            {
                EzEventManager.AddListener<MessageGamePlayEvent>(this);
                EzEventManager.AddListener<CharacterChangeState>(this);
            }
        }

        private void OnDisable()
        {
            if (registerMessEvent)
            {
                EzEventManager.RemoveListener<MessageGamePlayEvent>(this);
                EzEventManager.RemoveListener<CharacterChangeState>(this);
            }
        }
        public void OnEzEvent(MessageGamePlayEvent eventType)
        {
            if (eventType._objects == null || (eventType._objects.Length > 0 && (GameObject)eventType._objects[0] == gameObject))
            {
                if (eventType._message.StartsWith("Booster"))
                {
                    string pBoosterString = eventType._message.Remove(0, 7);
                    int pLevelBooster = startLevelBooster;
                    if (!string.IsNullOrEmpty(pBoosterString) && int.TryParse(pBoosterString, out pLevelBooster))
                    {
                        startLevelBooster = pLevelBooster;
                    }
                    else
                    {
                        startLevelBooster++;
                        if (startLevelBooster > 3)
                        {
                            startLevelBooster = 3;
                        }
                    }
                    if (startLevelBooster != 6)
                    {
                        CacheLevelBooster = startLevelBooster;
                    }
                    triggerChangeWeapon("Booster" + startLevelBooster);
                }
                else
                if (registerMessGameChangeWeapon)
                {
                    triggerChangeWeapon(eventType._message);
                }
            }
        }
        public void setFactorSpeedWeapon(float pSpeedFactor)
        {
            if (_storageWeapons != null)
            {
                for (int i = 0; i < _storageWeapons.Count; ++i)
                {
                    for (int j = 0; j < _storageWeapons[i].Length; ++j)
                    {
                        _storageWeapons[i][j].FactorSpeed = pSpeedFactor;
                    }
                }
            }
            factorSpeed = pSpeedFactor;
        }
        protected int cacheLevelBooster;
        protected bool isSupering = false;
        private void LateUpdate()
        {
            if (isSupering)
            {
                currentDurationSuper += Time.deltaTime;
                if (currentDurationSuper > 5)
                {
                    isSupering = false;
                    booster("Booster" + CacheLevelBooster);
                }
            }
            if (holdWeapon.Count > 0)
            {
                holdWeapon.Clear();
            }
        }

        public void OnEzEvent(CharacterChangeState eventType)
        {
            if (eventType.target != null && eventType.target.gameObject == gameObject)
            {
                if (eventType.target.CurrentState == StateCharacter.AliveOutSide && isShooting)
                {
                    ShootStop();
                }
            }
        }
    }
}
