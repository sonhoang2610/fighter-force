using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Tools.Space;
using EazyCustomAction;

namespace EazyEngine.Space
{
    public class AIBrain : TimeControlBehavior, IRespawn,EzEventListener<MessageGamePlayEvent>
    {
        protected AIElementAttackInfo _info;
        protected float currentDelayAttack = 0;
        protected CharacterHandleWeapon _characterWeapon;
        protected bool _isStart = false;

        bool isInit = false,firstTime = true;

        public bool isStart
        {
            get
            {
                return _isStart;
            }
            set
            {
                _isStart = value;
            }
        }
        public void setInfo(AIElementAttackInfo pInfo)
        {
            _info = pInfo;
            if (_info._typeDirection == TypeDirection.MainPlayer && _characterWeapon)
            {
                _characterWeapon.setTarget(LevelManger.Instance.CurrentPlayer.gameObject);
            }
            if (_info._typeDirection == TypeDirection.Random && _characterWeapon)
            {
                _characterWeapon.IsRandom = true;
            }
        }
        public virtual bool isSightMainPlayer()
        {
            for(int i  = 0; i < _characterWeapon._currentWeapons.Count; ++i)
            {
                var hit = MMDebug.RayCast(transform.position, _characterWeapon._currentWeapons[i].directionWeapon, 10, LayerMask.GetMask(LayerMask.LayerToName(LevelManger.Instance.CurrentPlayer.gameObject.layer)), Color.red);
                if (hit)
                {
                    return true;
                }
            }
            return false;
        }
        public virtual void attack()
        {
            _characterWeapon.ShootStart();
            //if (_info.typeFire == TypeFire.Semi)
            //{
            //    _characterWeapon.ShootStop();
            //}
        }
        public virtual void EveryFrameThinking()
        {
            if (_info.conditionAttack == ConditionAttack.OnSight)
            {
                if (isSightMainPlayer())
                {
                    if (CanAttack)
                    {
                        attack();
                        int pCount = Random.Range((int)_info.countShoot.x, (int)_info.countShoot.y);
                        for (int i = 0; i < pCount; ++i)
                        {
                           // StartCoroutine(attackTimer(i == pCount - 1));
                            float pDelay = pCount * Random.Range(_info.delayPerAttack.x, _info.delayPerAttack.y);
                            if (firstTime)
                            {
                                pDelay = pCount * Random.Range(_info.firstDelay.x, _info.firstDelay.y);
                            }
                            RootMotionController.runAction(gameObject, EazyCustomAction.Sequences.create(DelayTime.create(pDelay), CallFunc.create(delegate
                            {
                                attack();
                            })));
                        }
                        currentDelayAttack = Random.Range(_info.reloadTime.x, _info.reloadTime.y);
                    }
                }
            }
            else
            {
                if (CanAttack)
                {
                    int pCount = Random.Range((int)_info.countShoot.x, (int)_info.countShoot.y);
                    for (int i = 0; i < pCount; ++i)
                    {
                      //  StartCoroutine(attackTimer(i == pCount - 1));
                        float pDelay = pCount * Random.Range(_info.delayPerAttack.x, _info.delayPerAttack.y);
                        if (firstTime)
                        {
                            pDelay = pCount * Random.Range(_info.firstDelay.x, _info.firstDelay.y);
                        }
                        RootMotionController.runAction(gameObject, EazyCustomAction.Sequences.create(DelayTime.create(pDelay), CallFunc.create(delegate
                        {
                            attack();
                        })));
                    }
                    currentDelayAttack = Random.Range(_info.reloadTime.x, _info.reloadTime.y);
                }
            }
            if (currentDelayAttack > 0)
            {
                currentDelayAttack -= time.deltaTime;
            }
        }
        public IEnumerator attackTimer(bool pLast)
        {
            yield return new WaitForSeconds(Random.Range(_info.delayPerAttack.x, _info.delayPerAttack.y));
            attack();
        }
        public virtual bool CanAttack
        {
            get
            {
                return currentDelayAttack <= 0 && _info != null;
            }
        }
        public virtual bool CanThinking
        {
            get; set;
        }
        protected virtual void Awake()
        {
            _characterWeapon = GetComponent<CharacterHandleWeapon>();
            if (_info != null && _info._typeDirection == TypeDirection.MainPlayer)
            {
                _characterWeapon.setTarget(LevelManger.Instance.CurrentPlayer.gameObject);
            }
            if (_info != null && _info._typeDirection == TypeDirection.Random)
            {
                _characterWeapon.IsRandom = true;
            }
        }


        public virtual void startBrain()
        {
            isStart = true;
        }
        public virtual void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }

        public virtual void OnEnable()
        {
            EzEventManager.AddListener(this);
            isInit = false;
        }

        private void LateUpdate()
        {
            if (!isInit)
            {
                if (_info != null && _info.conditionStart == ConditionMaChineStart.OnStart)
                {
                    startBrain();
                    isInit = true;
                }

            }
        }
        // Start is called before the first frame update
        public virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (!isStart && LevelManger.InstanceRaw != null && LevelManger.Instance.mainPlayCamera.Rect().Contains(transform.position) && _info != null && _info.conditionStart == ConditionMaChineStart.InSideScreen)
            {
                startBrain();
            }
            if (CanThinking && isStart)
            {
                EveryFrameThinking();
            }
        }

        public virtual void onRespawn()
        {
            isStart = false;
            isInit = false;
            firstTime = true;
            currentDelayAttack = 0;
        }

        public void OnEzEvent(MessageGamePlayEvent eventType)
        {
            if(eventType._objects == null  || eventType._objects.Length == 0 || eventType._objects[0] == (object)gameObject)
            {
                if(_info != null &&_info.conditionStart == ConditionMaChineStart.Trigger && !isStart && eventType._message == _info.triggerStringMachine)
                {
                    startBrain();
                }
                if (_info != null && _info.conditionAttack == ConditionAttack.Trigger && eventType._message == _info.triggerStringAttack && isStart)
                {
                    attack();
                }
            }
        }
    }
}
