using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace EazyEngine.Space
{
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
        [HorizontalGroup("1"), PropertyOrder(0)]
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
            return valueExtra != null ? valueExtra.ToString() : "";
        }

    }
    [System.Serializable]
    public class DamageExtraVariants : List<DamageExtraVariant>, ILevelSetter
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
            for (int i = 0; i < this.Count; ++i)
            {
                pArray.Add(this[i].toNormal());
            }
            return pArray.ToArray();
        }
    }
    [System.Serializable]
    public class DamageGivenEventUnity : UnityEvent<DamageGivenEvent>
    {

    }
    public class DamageOnTouch : TimeControlBehavior, IRespawn, IgnoreObject
    {
        public LayerMask TargetMaskLayer;
        public bool ignoreOnDamaged = false;
        public bool autoGetOwnerMainPlane = false;
        public float limitDamage = 1.5f;
        public int DamageCaused = 10;
        public float factorDamageDecreaseSameObjects = 0;
        public float factorMinDamageDecrease = 1;
        public GameObject damagedEffect;
        public float durationForNextDame = 0.1f;
        public LayerMask TakenDamageMask = ~0;
        public int DamageTakenWithEveryThing = 0;
        public DamageGivenEventUnity onDamageAnother;
        [HideInInspector]
        public DamageOnTouch parentDamage;
        [HideInInspector]
        public Health _health;
        protected Collider2D _collider;
        public DamageOnTouch[] damageChilds;

        [SerializeField]
        [HideInEditorMode]
        protected float factorDamage = 1;


        [HideInEditorMode]
        protected List<GameObjectIgnoreTime> nextDamageSameObject = new List<GameObjectIgnoreTime>();
        public bool getObjectIgnore(GameObject pObjectCompare, out int pObjectIndex)
        {
            for (int i = 0; i < NextDamageSameObject.Count; ++i)
            {
                if (NextDamageSameObject[i].pObject == pObjectCompare)
                {
                    pObjectIndex = i;
                    if (NextDamageSameObject[i].duration <= 0)
                    {
                        return false;
                    }
                    return true;
                }
            }

            pObjectIndex = -1;
            return false;
        }
        [System.Serializable]
        public struct GameObjectIgnoreTime
        {
            public GameObject pObject;
            public float duration;
            public int indexDamaged;
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
        public DamageExtra[] ExtraDamge
        {
            get => extraDamge; set
            {
                tableExtraDamage.Clear();
                extraDamge = value;
            }
        }
        protected Dictionary<string, int> tableExtraDamage = new Dictionary<string, int>();
        public void addExtraDamge(DamageExtra pDamage, string pID)
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
        public List<DamageExtra> PExtras
        {
            get
            {
                if (pExtras == null) pExtras = new List<DamageExtra>();
                pExtras.Clear();
                int pSelfExtraLength = extraDamageSelf != null ? extraDamageSelf.Length : 0;
                int pExtraLength = ExtraDamge != null ? ExtraDamge.Length : 0;
                if (pExtras.Count != pSelfExtraLength + pExtraLength)
                {
                    if (extraDamageSelf != null)
                    {
                        pExtras.AddRange(extraDamageSelf);
                    }
                    if (ExtraDamge != null)
                    {
                        pExtras.AddRange(ExtraDamge);
                    }

                }
                return pExtras;
            }
            set => pExtras = value;
        }

        public int DamageCausedProp
        {
            get
            {
                if (autoGetOwnerMainPlane)
                {
                    if (LevelManger.Instance.CurrentPlayer )
                    {
                        {
                            return (int)LevelManger.Instance.CurrentPlayer.handleWeapon.FixDamage;
                        }
                    }
                  
                }
                return DamageCaused;
            }
            set
            {

                DamageCaused = value;

            }
        }

        protected List<GameObjectIgnoreTime> NextDamageSameObject { get => NextDamageSameObject1; set => NextDamageSameObject1 = value; }
        public List<GameObjectIgnoreTime> NextDamageSameObject1 { get => nextDamageSameObject; set => nextDamageSameObject = value; }

        private void Update()
        {
            for (int i = NextDamageSameObject.Count - 1; i >= 0; --i)
            {
                GameObjectIgnoreTime pObject = NextDamageSameObject[i];
                if (pObject.duration > 0)
                {
                    pObject.duration -= time.deltaTime;
                    NextDamageSameObject[i] = pObject;
                }
            }
        }
        protected virtual void Awake()
        {
            _health = GetComponent<Health>();
            for (int i = 0; i < damageChilds.Length; ++i)
            {
                damageChilds[i].parentDamage = this;
            }
            _collider = GetComponent<Collider2D>();
        }
        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            int pObjectOut;
            if (getObjectIgnore(collision.gameObject, out pObjectOut)) return;
            if (!_collider || !_collider.isTrigger || !_collider.enabled) return;
            OnEazyTriggerEnter2D(gameObject, collision);
        }
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            int pObjectOut;
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
            Collider(pObject, collision);
        }
        List<DamageExtra> pExtras = new List<DamageExtra>();
        protected void Collider(GameObject pSelf, Collider2D collision)
        {

            if (!Layers.LayerInLayerMask(collision.gameObject.layer, TargetMaskLayer))
            {
                return;
            }
            ConnectLayerTrigger pConnect = null;
            if (LevelManger.InstanceRaw && (pConnect = LevelManger.Instance.checkAssignedObject(collision.gameObject)) != null)
            {
                if (Layers.LayerInLayerMask(gameObject.layer, pConnect.layer))
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
            int pObjectOut = -1;
            int indexDamagedSamObject = 0;
            if (durationForNextDame > 0 && !getObjectIgnore(collision.gameObject, out pObjectOut))
            {
                if (pObjectOut < 0)
                {
                    NextDamageSameObject.Add(new GameObjectIgnoreTime() { pObject = collision.gameObject, duration = durationForNextDame, indexDamaged = 1 });
                }
                else
                {
                    var pObject = NextDamageSameObject[pObjectOut];
                    indexDamagedSamObject = pObject.indexDamaged;
                    pObject.indexDamaged++;
                    pObject.duration = durationForNextDame;
                    NextDamageSameObject[pObjectOut] = pObject;
                }

            }
            float pExtraDamage = 0;
            float pCurrentDamge = (DamageCausedProp * FactorDamage * factorDamageSelfConfig);
            for (int i = 0; i < PExtras.Count; ++i)
            {
                var pDamageExtraLocal = PExtras[i].type == DamageType.Normal ? PExtras[i].damageExtra :
                    (PExtras[i].type == DamageType.PecentHp ? (float)health.CurrentHealth * PExtras[i].damageExtra / 100.0f :
                    (PExtras[i].type == DamageType.PecentMaxHp ? (float)health.MaxiumHealth * PExtras[i].damageExtra : (pCurrentDamge * PExtras[i].damageExtra / 100.0f)));
                if (PExtras[i].type == DamageType.PecentHp || PExtras[i].type == DamageType.PecentMaxHp)
                {
                    if (limitDamage > 0)
                    {
                        if (pDamageExtraLocal > DamageCausedProp * limitDamage)
                        {
                            pDamageExtraLocal = DamageCausedProp * limitDamage;
                        }
                    }
                }
                pExtraDamage += pDamageExtraLocal;
            }

            float pDecrease = 1 - indexDamagedSamObject * factorDamageDecreaseSameObjects;
            if (pDecrease < factorMinDamageDecrease)
            {
                pDecrease = factorMinDamageDecrease;
            }
            var previousHealth = health.CurrentHealth;
            var pDamageCause = (int)((pCurrentDamge + pExtraDamage) * pDecrease*((pDecrease<1) ? 0.5f : 1));
            health.Damage((int)((pCurrentDamge + pExtraDamage) * pDecrease), gameObject, 0, 0);
            onDamageAnother?.Invoke(new DamageGivenEvent(health.gameObject, gameObject, health.CurrentHealth, pDamageCause, previousHealth));
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
        [HideInEditorMode]
        public List<GameObject> _listIgnoreObject = new List<GameObject>();
        public void IgnoreGameObject(GameObject pObject)
        {
            _listIgnoreObject.Add(pObject);
            var ignores = GetComponentsInChildren<IgnoreObject>();
            if (ignores != null)
            {
                foreach (var pIgnore in ignores)
                {
                    if ((object)pIgnore != this)
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
            NextDamageSameObject.Clear();
        }
        public void onRespawn()
        {
            NextDamageSameObject.Clear();
            if (damageChilds != null)
            {
                for (int i = 0; i < damageChilds.Length; ++i)
                {
                    damageChilds[i].gameObject.SetActive(true);
                    if (damageChilds[i]._health)
                        damageChilds[i]._health.Revive();
                }
            }
        }
    }
}
