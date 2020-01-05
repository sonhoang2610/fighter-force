using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EazyEngine.Tools;
using EazyEngine.Tools.Space;

namespace EazyEngine.Space
{
    public enum StateCharacter
    {
        Idle,
        Birth,
        AliveOutSide,
        AliveInSide,
        Death,
    }
    public struct CharacterChangeState
    {
        public Character target;
        public StateCharacter previousState;
    }
    public struct DeathEvent
    {
        public Health target;
    }
    public class Character : TimeControlBehavior
    {
        [HideLabel]
        public SpaceAnimator machine;    
        public GameObject modelObject;
        [HideInEditorMode]
        public Health _health;
        public CharacterHandleWeapon handleWeapon;
        CharacrerAbility[] abilities;
        public PlaneInfoConfig _info;
        public EnemyType enemyType;
        private StateCharacter currentState =  StateCharacter.Birth;

        public StateCharacter CurrentState { get => currentState; set => currentState = value; }
        [HideInEditorMode]
        public GameObject originalPreb;
        public Health[] AvailableTargets
        {
            get
            {
                if (_health)
                {
                    return _health.getAllAvailableHealth();
                }

                return new Health[] {  };
            }
        }

        public EnemyType EnemyType { get => enemyType; set {
                enemyType = value;
                if (!_health)
                {
                    _health.GetComponent<Health>();
                }
                _health.onDeath.RemoveListener(enemyTypeDeath);
                _health.onDeath.AddListener(enemyTypeDeath);
            } }
        public void enemyTypeDeath()
        {
            if(EnemyType ==  EnemyType.BOSS || EnemyType == EnemyType.MINIBOSS)
            {
                SoundManager.Instance.PlaySound(AudioGroupConstrant.BossExplore);
                CameraShake.Instance.shakeAmount = 0.25f;
                CameraShake.Instance.shakeDuration = 0.2f;
            }
        }
        protected PlaneInfoToCoppy[] infoPreload;
        public void setDataPreload(PlaneInfoToCoppy[] pInfos)
        {
            infoPreload = pInfos;
           // for(int i = 0; i )
        }
        [Button("random")]
        public void randomInfo()
        {
            int index = Random.Range(0, infoPreload.Length);

        }

        public void changeState (StateCharacter state)
        {
            var pOldState = currentState;
            currentState = state;
            EzEventManager.TriggerEvent(new CharacterChangeState() { target = this , previousState = pOldState });
            if(currentState == StateCharacter.Birth)
            {
                if (!LevelManger.InstanceRaw)
                {
                    changeState(StateCharacter.AliveOutSide);
                }
                else 
                {
                    if (LevelManger.Instance.mainPlayCamera.Rect(0.95f).Contains(transform.position))
                    {
                        changeState(StateCharacter.AliveInSide);
                    }
                    else
                    {
                        changeState(StateCharacter.AliveOutSide);
                    }
                }
                
            }
        }
        public void addChild(Character child) {
            for (int i = 0; i < abilities.Length; ++i)
            {
                abilities[i].addChild(child);
            }
        }
        public void SetTrigger(string pTrigger)
        {
            machine.SetTrigger(pTrigger);
        }
        protected float factorHP = 1;
        public void setFactorHP(float pFactor)
        {
            if (_info != null)
            {
                _health.MaxiumHealth *= (int)pFactor;
                _health.currentHealth *= (int)pFactor;
                _health.InitialHealth *= (int)pFactor;
            }
            factorHP = pFactor;
        }
        public void setData(PlaneInfoConfig pData)
        {
            _info = pData;
            if (getAbility("Hp", pData.info.currentAbility) != null)
            {
                _health.MaxiumHealth =(int)( getAbility("Hp", pData.info.currentAbility).CurrentUnit* factorHP);
                _health.currentHealth = (int)(getAbility("Hp", pData.info.currentAbility).CurrentUnit* factorHP);
                _health.InitialHealth = (int)(getAbility("Hp", pData.info.currentAbility).CurrentUnit *factorHP);
                _health.Deffense = getAbility("Defense", pData.info.currentAbility).CurrentUnit;
                var pSpeed = getAbility("SpeedFire", pData.info.currentAbility);
                if (pSpeed != null && pData.GetType() == typeof(SupportPlaneInfoConfig))
                {
                    handleWeapon.setFactorSpeedWeapon(((float)pSpeed.CurrentUnit) / 100.0f);
                }
            }
            handleWeapon.FixDamage = getAbility("Damage", pData.info.currentAbility).CurrentUnit;
    

        }
        [System.NonSerialized]
        public CharacterInstancedConfig mainInfo;

        public void setDataConfig(CharacterInstancedConfig pInfo)
        {
            if (!_health) _health = GetComponent<Health>();
            mainInfo = pInfo;
            _health.InitialHealth =(int) pInfo.Health;
            _health.MaxiumHealth = (int)pInfo.Health;
            _health.Deffense =(int) pInfo.Defense;
            _health.currentHealth = (int)pInfo.Health;

            if (!handleWeapon) handleWeapon = GetComponent<CharacterHandleWeapon>();
            if (handleWeapon)
            {
                handleWeapon.FixDamage = (int)pInfo.DamgageBasic;
                handleWeapon.setData(pInfo.weapon);
            }
            for (int i = 0; i < pInfo.propEdits.Length; ++i)
            {
                for (int j = 0; j < pInfo.propEdits[i].propEdits.Length; ++j)
                {
                    var pProp = pInfo.propEdits[i].propEdits[j];
                    pProp.setValue(gameObject);
                }
            }
            if (pInfo.DamgeSelf > 0)
            {
                var pDamage = GetComponent<DamageOnTouch>();
                {
                    if(pDamage)
                         pDamage.DamageCausedProp =(int) pInfo.DamgeSelf;
                };
            }
        }

        
        public AbilityConfig getAbility(string pID,List<AbilityConfig> abilities)
        {
            for(int i = 0; i < abilities.Count; ++i)
            {
                if(abilities[i]._ability.ItemID == pID)
                {
                    return abilities[i];
                }
            }
            return null;
        }
        public Dictionary<string, float> factorItem = new Dictionary<string, float>();

        public float getFactorWithItem(string pID)
        {
            if (!factorItem.ContainsKey(pID))
            {
                return 1;
            }
            return factorItem[pID];
        }
        public void setFactorWithItemId(string pId,float factor)
        {
            if (!factorItem.ContainsKey(pId))
            {
                factorItem.Add(pId, 1);
            }
            factorItem[pId] = factor;
        }
        private void Awake()
        {
            abilities = GetComponentsInChildren<CharacrerAbility>();
            _health = GetComponent<Health>();
            if (_health)
            {
                _health.onDeath.AddListener(delegate
                {
                    changeState(StateCharacter.Death);
                });
                _health.onRevie.AddListener(delegate
                {
                    changeState(StateCharacter.Birth);
                });
            }
            handleWeapon = GetComponent<CharacterHandleWeapon>();
            for (int i = 0; i < abilities.Length; ++i)
            {
                abilities[i].initialAbility();
            }
 
           
        }
        
        private void OnEnable()
        {
            if (GameManager.Instance.isPlaying && LevelManger.InstanceRaw != null)
            {
                LevelManger.Instance._charList.Add(this);
            }
        }

        [Button("refresh")]
        public void refresh()
        {
            if (mainInfo != null)
            {
                setDataConfig(mainInfo);
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance.IsDestroyed()) return;
            if (GameManager.Instance.isPlaying && LevelManger.InstanceRaw && LevelManger.Instance._charList.Contains(this))
            {
                LevelManger.Instance._charList.Remove(this);
            }
        }
        private void Start()
        {
        }
        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < abilities.Length; ++i)
            {
                if (abilities[i].enabled)
                {
                    abilities[i].EveryFrame(time.deltaTime);
                }
            }
        }
        private void LateUpdate()
        {
           
            if (transform.hasChanged)
            {
                if(CurrentState == StateCharacter.AliveOutSide)
                {
                    if (LevelManger.Instance.mainPlayCamera.Rect(0.95f).Contains(transform.position))
                    {
                        changeState(StateCharacter.AliveInSide);
                    }
                }
                else if (CurrentState == StateCharacter.AliveInSide)
                {
                    if (!LevelManger.Instance.mainPlayCamera.Rect(0.95f).Contains(transform.position))
                    {
                        changeState(StateCharacter.AliveOutSide);
                    }
                }
            }
        }
        public bool registerListen()
        {
            throw new System.NotImplementedException();
        }
        
    }
}
