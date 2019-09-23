using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;

namespace EazyEngine.Space {
    public enum DamageType
    {
        Normal,
        PercentCurrentDamage,
        PecentMaxHp,
        PecentHp
    }
    [System.Serializable]
    public struct DamageExtra
    {
        [HorizontalGroup("1"), PropertyOrder(1)]
        [HideLabel]
        public DamageType type;
        [HorizontalGroup("1"),PropertyOrder(0)]
        public float damageExtra;
    }
    [System.Serializable]
    public struct DamageExtraVariant : ILevelSetter
    {
        [HorizontalGroup("1"), PropertyOrder(1)]
        [HideLabel]
        public DamageType type;
        [HorizontalGroup("1"), PropertyOrder(0)]
        public UnitDefineLevel valueExtra;
        [HorizontalGroup("1"), PropertyOrder(0)]
        public void setLevel(int pLevel)
        {
            valueExtra.setLevel(pLevel);
        }

        public DamageExtra toNormal()
        {
            return new DamageExtra() { damageExtra = valueExtra, type = this.type };
        }
        public DamageExtra[] toNormalArray()
        {
            return new DamageExtra[] { new DamageExtra() { damageExtra = valueExtra, type = this.type } };
        }
        public override string ToString()
        {
            return valueExtra!= null ?valueExtra.ToString() : "";
        }

    }
    [System.Serializable]
    public class DamageExtraVariants : List<DamageExtraVariant> , ILevelSetter
    {
        public void setLevel(int pLevel)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                this[i].setLevel(pLevel);
            }
        }

        public DamageExtra[] toNormalArray()
        {
            List<DamageExtra> pArray = new List<DamageExtra>();
            for(int i = 0; i < this.Count; ++i)
            {
                pArray.Add(this[i].toNormal());
            }
            return pArray.ToArray();
        }
    }
    public class DamageOnTouch : TimeControlBehavior,IRespawn, IgnoreObject
    {
        public LayerMask TargetMaskLayer;
        public bool ignoreOnDamaged = false;   
        public int DamageCaused = 10;
        public GameObject damagedEffect;
        public float durationForNextDame = 0.1f;
        public LayerMask TakenDamageMask = ~0; 
        public int DamageTakenWithEveryThing = 0;
        [HideInInspector]
        public DamageOnTouch parentDamage;
        [HideInInspector]
        public Health _health;
        protected Collider2D _collider;
        public DamageOnTouch[] damageChilds;

        [SerializeField]
        [HideInEditorMode]
        protected float factorDamage = 1;

        protected List<GameObjectIgnoreTime> nextDamageSameObject = new List<GameObjectIgnoreTime>();

        public bool getObjectIgnore(GameObject pObjectCompare, out GameObjectIgnoreTime pObject)
        {  
            for (int i = 0; i < nextDamageSameObject.Count; ++i)
            {
                if(nextDamageSameObject[i].pObject == pObjectCompare)
                {
                    pObject = nextDamageSameObject[i];
                    return true;
                }
            }
            pObject = new GameObjectIgnoreTime();
            return false;
        }
        public struct GameObjectIgnoreTime
        {
            public GameObject pObject;
            public float duration;
        }

        public float factorDamageSelfConfig = 1;

        private DamageExtra[] extraDamge;
        public DamageExtra[] extraDamageSelf;
        public float FactorDamage { get => factorDamage; set => factorDamage = value; }
        public GameObject[] IgnoreObjects
        {
            get
            {
                return _listIgnoreObject.ToArray();
            }
            set
            {
                _listIgnoreObject.Clear();
                _listIgnoreObject.AddRange(value);
            }
        }
        [ShowInInspector]
        public DamageExtra[] ExtraDamge { get => extraDamge; set {
                tableExtraDamage.Clear();
                extraDamge = value;
            } }
        protected Dictionary<string, int> tableExtraDamage = new Dictionary<string, int>();
        public void addExtraDamge(DamageExtra pDamage,string pID)
        {
            if (tableExtraDamage.ContainsKey(pID))
            {
                if (tableExtraDamage[pID] >= extraDamge.Length) return;
                extraDamge[tableExtraDamage[pID]] = pDamage;
                return;
            }
            System.Array.Resize(ref extraDamge, extraDamge.Length + 1);
            extraDamge[extraDamge.Length - 1] = pDamage;
            tableExtraDamage.Add(pID, extraDamge.Length - 1);
        }
        public List<DamageExtra> PExtras { get {
                if (pExtras == null) pExtras = new List<DamageExtra>();
                if (extraDamageSelf != null && ExtraDamge != null && pExtras.Count != extraDamageSelf.Length + ExtraDamge.Length)
                {
                    pExtras.Clear();
                    pExtras.AddRange(extraDamageSelf);
                    pExtras.AddRange(ExtraDamge);
                }
                return pExtras;
                    } set => pExtras = value; }

        private void Update()
        {
             for(int i = nextDamageSameObject.Count -1; i >= 0; --i)
            {
                GameObjectIgnoreTime pObject = nextDamageSameObject[i];
                pObject.duration -= time.deltaTime;
                nextDamageSameObject[i] = pObject;
                if (pObject.duration <= 0)
                {
                    nextDamageSameObject.RemoveAt(i);
                }
            }
        }
        protected virtual void Awake()
        {
            _health = GetComponent<Health>();
            for(int i = 0; i < damageChilds.Length; ++i)
            {
                damageChilds[i].parentDamage = this;
            }
            _collider = GetComponent<Collider2D>();
        }
        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            GameObjectIgnoreTime pObjectOut;
            if (getObjectIgnore(collision.gameObject,out pObjectOut)) return;
            if (!_collider || !_collider.isTrigger || !_collider.enabled) return;
            OnEazyTriggerEnter2D(gameObject, collision);
        }
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            GameObjectIgnoreTime pObjectOut;
            if (getObjectIgnore(collision.gameObject, out pObjectOut)) return;

            if (parentDamage)
            {
                parentDamage.OnEazyTriggerEnter2D(gameObject, collision);
            }
            if (!_collider || !_collider.isTrigger || !_collider.enabled) return;
            OnEazyTriggerEnter2D(gameObject, collision);
        }
        protected void OnEazyTriggerEnter2D(GameObject pObject, Collider2D collision)
        {
            Collider(pObject,collision);
        }
        List<DamageExtra> pExtras = new List<DamageExtra>();
        protected void Collider(GameObject pSelf, Collider2D collision)
        {
       
            if (!Layers.LayerInLayerMask(collision.gameObject.layer, TargetMaskLayer))
            {
                return;
            }
            ConnectLayerTrigger pConnect = null;
            if (LevelManger.InstanceRaw &&( pConnect = LevelManger.Instance.checkAssignedObject(collision.gameObject))  != null)
            {
                if(Layers.LayerInLayerMask(gameObject.layer, pConnect.layer))
                {
                    return;
                }
            }
            if (_listIgnoreObject.Contains(collision.gameObject)) return;
            if (damagedEffect)
            {
                var hits = MMDebug.RayCastAll(transform.position, (collision.transform.position - transform.position).normalized, 20, TargetMaskLayer, Color.red, true);
                foreach (var hit in hits)
                {
                    if (hit && hit.collider.gameObject == collision.gameObject)
                    {
                        ParticleEnviroment.Instance.createEffect(damagedEffect, hit.point, 3, false);
                    }
                }
            }
            Health health = collision.GetComponent<Health>();
            if (!health) return;
            GameObjectIgnoreTime pObjectOut;
            if (durationForNextDame >0 && !getObjectIgnore(collision.gameObject, out pObjectOut))
            {
                nextDamageSameObject.Add(new GameObjectIgnoreTime() { pObject = collision.gameObject,duration = durationForNextDame});
            }
            float pExtraDamage = 0;
            float pCurrentDamge = (DamageCaused * FactorDamage * factorDamageSelfConfig);
            for (int i  =0; i < PExtras.Count; ++i)
            {
                pExtraDamage += PExtras[i].type == DamageType.Normal ? PExtras[i].damageExtra :
                    (PExtras[i].type == DamageType.PecentHp ? (float)health.CurrentHealth * PExtras[i].damageExtra :
                    (PExtras[i].type == DamageType.PecentMaxHp ? (float)health.MaxiumHealth * PExtras[i].damageExtra : (pCurrentDamge * PExtras[i].damageExtra)));
            }
            health.Damage((int)pCurrentDamge + (int)pExtraDamage, gameObject, 0, 0);
            if (ignoreOnDamaged)
            {
                IgnoreGameObject(health.gameObject);
            }
            if (!Layers.LayerInLayerMask(collision.gameObject.layer, TakenDamageMask))
            {
                return;
            }
      
            if (_health)
            {
                _health.Damage(DamageTakenWithEveryThing, gameObject, 0, 0);
        
            }
            if (pSelf != gameObject)
            {
                Health healthChild = pSelf.GetComponent<Health>();
                if (healthChild)
                {
                    healthChild.Damage(DamageTakenWithEveryThing, pSelf, 0, 0);
                }
                else
                {
                    pSelf.gameObject.SetActive(false);
                }
            }
        }
        [HideInInspector]
        public List<GameObject> _listIgnoreObject = new List<GameObject>();
        public void IgnoreGameObject(GameObject pObject)
        {
            _listIgnoreObject.Add(pObject);
             var ignores =  GetComponentsInChildren<IgnoreObject>();
            if (ignores != null)
            {
                foreach(var pIgnore in ignores)
                {
                    if((object)pIgnore != this)
                    {
                        pIgnore.IgnoreGameObject(pObject);
                    }
                }
            }
        }
        public void ClearIgnoreList()
        {
            _listIgnoreObject.Clear();
        }
        private void Start()
        {
            
        }
        private void OnEnable()
        {
            nextDamageSameObject.Clear();
        }
        public void onRespawn()
        {
            if (damageChilds!= null)
            {
                for(int i = 0; i < damageChilds.Length; ++i)
                {
                    damageChilds[i].gameObject.SetActive(true);
                    if(damageChilds[i]._health)
                        damageChilds[i]._health.Revive();
                }
            }
        }
    }
}
