using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using EazyReflectionSupport;
using UnityEngine.Animations;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class ObjectGroupAttachMent
    {
        public ObjectPooler pooler;
        public Vector2 randomOffset = Vector2.zero;
        public bool isShuffe = false;
        public GameObject[] attachMentPosStart;
    }

    public class ProjectileMultipeWeapon : Weapon
    {
        protected ObjectPooler _pooler;
        [HideInEditorMode]
        [ShowInInspector]
        protected List<ObjectGroupAttachMent> cacchePosAttachment = new List<ObjectGroupAttachMent>();
        [OnValueChanged("dirty")]
        public int defaultIndex = -1;
        [SerializeField]
       
        protected ObjectGroupAttachMent[] attachMentPosStartNew;
        [SerializeField]
        protected int countBulletPerAttachment = 1;
        [SerializeField]
        [ShowIf("@countBulletPerAttachment > 1")]
        protected float delayEachProjecttile = 0;
        [SerializeField]
        [ShowIf("@countBulletPerAttachment > 1")]
        protected float randomDelayEachProjTo = 0;
        [SerializeField]
        [ShowIf("@countBulletPerAttachment > 1")]
        protected bool posStartAtAttachMent = false;
        [SerializeField]
        protected bool autoRotateIfHaveTarget = false;
        protected Dictionary<GameObject, GameObject> prepareBullet = new Dictionary<GameObject, GameObject>();
        public bool isLocal = false;
        public WeaponTrajectorPath trajectorPath;
        public UnityEventGameObject onFireAttachMent;
        public UnityEvent2GameObject onFire;
        [HideInInspector]
        public UnityEvent onUseEvent;
        [HideInInspector]
        public UnityEvent onPrepareBullet;

        public override void init()
        {
            base.init();
            _pooler = GetComponent<ObjectPooler>();
        }
        public void dirty()
        {
            cacchePosAttachment.Clear();
        }
        public override int countTargetNeeded()
        {
            return Mathf.Max( cacchePosAttachment.Count,1);
        }
        protected override void LateUpdate()
        {
            base.LateUpdate();
            for(int i = holdBullet.Count -1; i >= 0; --i)
            {
                if (!holdBullet[i].gameObject.activeSelf)
                {
                    holdBullet.RemoveAt(i);
                }
            }
        }

        public override void addDamageExtra(DamageExtra[] extra, string pStr = "")
        {
            base.addDamageExtra(extra, pStr);
            for(int i = 0; i < holdBullet.Count; ++i)
            {
                List<DamageExtra> pExtrs = new List<DamageExtra>();
                pExtrs.AddRange(holdBullet[i].CacheExtras);
                pExtrs.AddRange(extra);
                holdBullet[i].setDamage(FixDamage, FactorDamage, pExtrs.ToArray());
            }
        }
        //private void Update()
        //{
        //    if (TargetDirection && anchorRotation)
        //    {
        //        //  isReady = false;
        //        Vector2 directon = (TargetDirection.transform.position - transform.position).normalized;
        //        float pAngle = Vector2.SignedAngle(defaultFace, directon);
        //        //  pAngle = -pAngle;
        //        if (anchorRotation.transform.localScale.x < 0)
        //        {
        //            pAngle = -pAngle;
        //        }
        //        var pRotation = Quaternion.Euler(0, 0, pAngle);
        //        var pDestiny = Quaternion.Inverse(anchorRotation.transform.parent.rotation) * pRotation;
        //        float from = (anchorRotation.transform.localRotation.eulerAngles.z);
        //        float to = (pDestiny.eulerAngles.z);
        //        anchorRotation.transform.localRotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(from, to, time.deltaTime * rotationSpeedd));
        //    }
        //}
        protected List<ObjectGroupAttachMent> AttachMentPosStart
        {

            get
            {
                if (cacchePosAttachment.Count == 0)
                {
                    if (defaultIndex == -1)
                    {
                        defaultIndex = 0;
                    }
                     if (AttachMentPosStartNew.Length > 0)
                    {
                        cacchePosAttachment.Add( AttachMentPosStartNew[defaultIndex]);
                    }
                }
                return cacchePosAttachment;

            }
        }
        public List<GameObject> AttachMentGameObject
        {

            get
            {
                var pArray = new List<GameObject>();
                for(int i = 0; i < AttachMentPosStart.Count; ++i)
                {
                    pArray.AddRange(AttachMentPosStart[i].attachMentPosStart);
                }
                return pArray;

            }
        }

        public ObjectGroupAttachMent[] AttachMentPosStartNew { get => attachMentPosStartNew; set => attachMentPosStartNew = value; }

        public void setAttachMentIndex(int index)
        {
            StopAllCoroutines();
            cacchePosAttachment.Clear();
           cacchePosAttachment.Add(AttachMentPosStartNew[index]);
        }
        public void setAttachMentIndexString(string pStr)
        {
            StopAllCoroutines();
            cacchePosAttachment.Clear();
            var pStrs = pStr.Split('|');
            for (int i = 0; i < pStrs.Length; ++i)
            {
                int a = -1;
                if (int.TryParse(pStrs[i], out a))
                {
                    addAttachMentIndex(a);
                }
            }
        }
        public void addAttachMentIndex(int index)
        {
            if (cacchePosAttachment.Contains(AttachMentPosStartNew[index])) return;
            cacchePosAttachment.Add(AttachMentPosStartNew[index]);
        }
        public void addAttachMentIndexString(string pStr)
        {
            var pStrs = pStr.Split('|');
            for (int i = 0; i < pStrs.Length; ++i)
            {
                int a = -1;
                if (int.TryParse(pStrs[i], out a) )
                {
                    addAttachMentIndex(a);
                }
            }
        }
        public override int getConsumeShoot()
        {
            int totalCount = 0;
            for (int j = 0; j < AttachMentPosStart.Count; ++j)
            {
                totalCount += AttachMentPosStart[j].attachMentPosStart.Length*countBulletPerAttachment;
            }
            return totalCount;
        }

        public override void setTimeLifeProj(float pTimeLife)
        {
            base.setTimeLifeProj(pTimeLife);
            foreach(var pBullet in holdBullet)
            {
                pBullet.LifeTime = pTimeLife;
            }
        }

        [System.NonSerialized]
        [ShowInInspector]
        [HideInEditorMode]
        public List<Projectile> holdBullet = new List<Projectile>();
        public override void WeaponUse()
        {
            onUseEvent.Invoke();
            base.WeaponUse();
            if (AttachMentPosStart == null) return;
            float currentDelay = 0;
            for (int j = 0; j < AttachMentPosStart.Count; ++j)
            {
               
                GameObject[] attachMentPosStart = AttachMentPosStart[j].attachMentPosStart;
                if (AttachMentPosStart[j].isShuffe)
                {
                    List<GameObject> pAttachs = new List<GameObject>();
                    pAttachs.AddRange(attachMentPosStart);
                    pAttachs.Shuffle();
                    attachMentPosStart = pAttachs.ToArray();
                }
            
                int pJ = j;
                for (int i = 0; i < attachMentPosStart.Length; ++i)
                {
                    int pI = i;
                    int index = currentIndexBullet;
                    System.Action pAction = delegate
                     {
                         ObjectPooler pPool = _pooler;
                         if (AttachMentPosStart[pJ].pooler)
                         {
                             pPool = AttachMentPosStart[pJ].pooler;
                         }
                         for (int g = 0; g < countBulletPerAttachment; g++)
                         {

                             GameObject pObjectProjectile = pPool.GetPooledGameObject();
                             if (isLocal)
                             {
                                 pObjectProjectile.transform.parent = transform;
                             }
                             pObjectProjectile.SetActive(false);
                             pObjectProjectile.transform.position = transform.position;
                             var ignores = pObjectProjectile.GetComponents<IgnoreObject>();
                             if (ignores != null)
                             {
                                 foreach (var pIgnore in ignores)
                                 {
                                     pIgnore.IgnoreObjects = IgnoreObjects;
                                 }
                             }

                             Projectile proj = pObjectProjectile.GetComponent<Projectile>();
                        
                             holdBullet.Add(proj);
                             if (proj)
                             {
                                 proj.GetComponent<Health>().Revive(false);
                                 if (data != null)
                                 {
                                     foreach (var pBulletInfo in data.bullets)
                                     {
                                         if (pBulletInfo.prefab == pPool.GetLastOriginal())
                                         {
                                             proj.setData(pBulletInfo);
                                         }
                                     }
                                 }

                                 if (Owner)
                                 {
                                     proj.transform.localScale =
                                         pPool.GetLastOriginal().transform.localScale * Owner.transform.localScale.x;
                                 }
                                 proj.LifeTime = !forceTimeLife ? proj.LifeTime : timelife;
                                 proj.setWeapon(this);
                                 proj.setOwner(Owner);
                                 proj.time._groupName = time._groupName;
                                 proj.setDamage(FixDamage, FactorDamage, extraDamage.ToArray());
                                 Vector3 rotate = attachMentPosStart[pI].transform.rotation.eulerAngles;
                                 Vector2 directon = Vector2.up.Rotate(rotate.z);
                                 if (TargetDirection != null && !forceDirectionFollowGun)
                                 {
                                     directon = TargetDirection.transform.position - transform.position;
                                     directon = directon.normalized;
                                 }
                                 proj.transform.position = attachMentPosStart[pI].transform.position;
                                 proj.setDirection(directon);
                                 if (TargetDirection)
                                 {
                                     proj.setTarget(TargetDirection);
                                     proj.target = TargetDirection.transform.position;
                                 }
                                 if (trajectorPath)
                                 {
                                     var pSpline = trajectorPath.spline.DeepClone();

                                     pSpline.inverseTransform(transform);
                                     //pSpline.setDelta(transform.position);
                                     //pSpline.rotation(directon);
                                     proj.setSplineMove(pSpline);
                                 }
                             }
                             var pEffectInfo = getParticle(i);
                             proj.setIndex(index);
                             if (g == 0)
                             {
                                 Vector3 rotate = attachMentPosStart[pI].transform.rotation.eulerAngles;
                                 Vector2 directon = Vector2.up.Rotate(rotate.z);
                                 pObjectProjectile.gameObject.SetActive(true);
                         
                                 if (pEffectInfo.particleEffect)
                                 {
                                     float pScale = pEffectInfo.scale;
                                     if (pScale == 0)
                                     {
                                         pScale = 1;
                                     }
                                     GameObject pEffect = null;
                                     pEffect = ParticleEnviroment.Instance.createEffect(pEffectInfo.particleEffect, attachMentPosStart[pI].transform.position, pEffectInfo.orderlayer);
                                     pEffect.transform.RotationDirect2D(directon, TranformExtension.FacingDirection.DOWN);
                                     pEffect.transform.localScale = new Vector3(1,1,1)* pScale;
                                     pEffect.GetComponent<ParticleSystem>().Play();
                                     if (pEffectInfo.isLocal)
                                     {
                                       var pSources =  pEffect.GetComponent<PositionConstraint>().GetSource(0);
                                         pSources.sourceTransform = transform;
                                         pEffect.GetComponent<PositionConstraint>().SetSource(0, pSources);
                                     }
                                       
                                 }
                               
                                 onFireAttachMent.Invoke(attachMentPosStart[pI]);
                                 onFire.Invoke(attachMentPosStart[pI], proj.gameObject);
                             }
                             else
                             {
                                 pObjectProjectile.name = "[block]" + pObjectProjectile.name;
                                 StartCoroutine(delayActive(g * (randomDelayEachProjTo == 0 ? delayEachProjecttile : Random.Range(delayEachProjecttile, randomDelayEachProjTo)) * FactorSpeed, pObjectProjectile, attachMentPosStart[pI],index));
                             }
                         }
                     };
                    float pDelay = Random.Range(AttachMentPosStart[pJ].randomOffset.x, AttachMentPosStart[pJ].randomOffset.y);
                    if(i != 0 || j != 0)
                        currentDelay += pDelay*FactorSpeed;
                    if(currentDelay != 0)
                    {
                        StartCoroutine(delayAction(currentDelay, pAction));
                    }
                    else
                    {
                        pAction();
                    }
                    currentIndexBullet++;
                }
            }
        }
        public IEnumerator delayAction(float pDelay, System.Action pAction)
        {
            yield return new WaitForSeconds(pDelay);
            pAction();
        }
        public EffectInfo getParticle(int index)
        {
            var particleInfo = particleEffects.Length > 0? particleEffects[particleEffects.Length - 1] : new EffectInfo();
            if (index < particleEffects.Length)
            {
                particleInfo = particleEffects[index];
            }
            if (particleEffects.Length == 0 || (index != 0 && particleInfo.onePerUse))
            {
                return new EffectInfo();
            }
            if(index >= particleEffects.Length)
            {
               return  particleEffects[particleEffects.Length - 1];
            }
            return particleEffects[index];
        }
        protected IEnumerator delayActive(float pDelay,GameObject pObject,GameObject pAttachMent,int index )
        {
            yield return new WaitForSeconds(pDelay);
            onFireAttachMent.Invoke(pAttachMent);
            if (posStartAtAttachMent)
            {
                pObject.transform.position = pAttachMent.transform.position;
            }
            if (pObject.name.StartsWith("[block]"))
            {
                pObject.name = pObject.name.Remove(0, 7);
            }
            pObject.SetActive(true);
           var proj = pObject.GetComponent<Projectile>();
            onFire.Invoke(pAttachMent, pObject);
            if(proj)
            proj.onIndexBullet.Invoke(index);
        }
        public override void obtainBulletRelaoding()
        {
            onPrepareBullet.Invoke();
            base.obtainBulletRelaoding();
        }
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;
            for (int j = 0; j < AttachMentPosStart.Count; ++j)
            {
                for (int i = 0; i < AttachMentPosStart[j].attachMentPosStart.Length; ++i)
                {
                    Vector2 pDirection = Vector2.up.Rotate(AttachMentPosStart[j].attachMentPosStart[i].transform.rotation.eulerAngles.z);
                    MMDebug.DrawGizmoArrow(AttachMentPosStart[j].attachMentPosStart[i].transform.position, pDirection.normalized * 3, Color.yellow);
                }
            }
        }
    }
}
