using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;
using EazyEngine.Tools.Space;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class UnityEventLightningInfo : UnityEvent<LightningInfo>
    {

    }
    public class LightningInfo
    {
        public GameObject start, end;
        public LightningBoltPrefabScript lightning;
        public GameObject target;
        public GameObject anchor;
        public GameObject startTarget;
        public LightningBoltTransformTrackerScript tracker;
        public void init()
        {
            lightning.onCompleteAll = OnComplete;
        }
        public void OnComplete()
        {
            onComplete.Invoke(this);
        }
        public UnityEventLightningInfo onComplete = new UnityEventLightningInfo();
    }
    public class WeaponLightning : Weapon
    {
        public LightningBoltTransformTrackerScript tracker;
        public LightningBoltPrefabScript lightning;
        public int dameTaken = 0;
        [SerializeField]
        private int countTarget = 1;
        // public float durationApear = 0.1f;
        [ShowInInspector]
        protected List<LightningInfo> anchorLightning = new List<LightningInfo>();

        public int CountTarget { get => countTarget; set {
                bool change = false;
                if(value != countTarget)
                {
                    change = true;
                }
                countTarget = value;
               if(change) initLightning();
            } }
        public void initLightning()
        {
            lightning.gameObject.SetActive(true);
            tracker.gameObject.SetActive(true);
            for (int i = 0; i < Mathf.Max( CountTarget, anchorLightning.Count); ++i)
            {
                if(i >= anchorLightning.Count)
                {
                    GameObject anchor = new GameObject();
                    anchor.transform.parent = transform;
                    anchor.transform.localPosition = Vector3.zero;
                    anchor.transform.localScale = new Vector3(1, 1, 1);
                    LightningBoltTransformTrackerScript pTracker = Instantiate<LightningBoltTransformTrackerScript>(tracker, anchor.transform);
                    LightningBoltPrefabScript pLightning = Instantiate<LightningBoltPrefabScript>(lightning, anchor.transform);
                    pLightning.transform.parent = anchor.transform;
                    GameObject start = new GameObject();
                    start.transform.parent = anchor.transform;
                    start.transform.localScale = new Vector3(1, 1, 1);
                    GameObject end = new GameObject();
                    end.transform.parent = anchor.transform;
                    end.transform.localScale = new Vector3(1, 1, 1);
                    start.transform.localPosition = Vector3.zero;
                    //   lightningTracker.StartTarget = start.transform;
                    //  lightningTracker.EndTarget = end.transform;
                    pLightning.transform.localScale = new Vector3(1, 1, 1);
                    anchorLightning.Add(new LightningInfo()
                    {
                        anchor = anchor,
                        end = end,
                        start = start,
                        lightning = pLightning,
                        tracker = pTracker
                    });
                    anchorLightning[anchorLightning.Count - 1].tracker.StartTarget = anchorLightning[anchorLightning.Count - 1].start.transform;
                    anchorLightning[anchorLightning.Count - 1].tracker.EndTarget = anchorLightning[anchorLightning.Count - 1].end.transform;
                    anchorLightning[anchorLightning.Count - 1].tracker.LightningScript = anchorLightning[anchorLightning.Count - 1].lightning;
                    anchorLightning[anchorLightning.Count - 1].onComplete.AddListener(onHandlerLightning);
                    pLightning.Destination = end;
                    pLightning.Source = start;
                    pLightning.Camera = Camera.main;
                    pLightning.Trigger();
                }    
                if(i >= CountTarget)
                {
                    anchorLightning[i].target = null;
                }
            }
            lightning.gameObject.SetActive(false);
            tracker.gameObject.SetActive(false);
        }
        private void Awake()
        {
            initLightning();
      

        }
        public override void WeaponUse()
        {
            base.WeaponUse();
     
            if (TargetDirection && TargetDirection.gameObject.activeSelf)
            {
                anchorLightning[0].target = TargetDirection;
                anchorLightning[0].startTarget = gameObject;
                Vector2 pos = TargetDirection.transform.position;
                GameObject oldTarget = TargetDirection;
                List<GameObject> foundObject = new List<GameObject>();
                foundObject.Add(TargetDirection);
                for (int i = 0; i < CountTarget-1; ++i)
                {
                    var pFound = Radar.findTargetFromPos(pos, 5,true);
                    System.Array.Sort(pFound, Radar.sortDistance);
                    if (pFound.Length > 0)
                    {
                        GameObject pTarget = null;
                        foreach(var pChild in pFound)
                        {
                            if (!foundObject.Contains(pChild._obect))
                            {
                                pTarget = pChild._obect;
                            }
                        }
                        if (pTarget)
                        {
                            foundObject.Add(pTarget);
                            anchorLightning[i + 1].target = pTarget;
                            anchorLightning[i + 1].startTarget = oldTarget;
                            oldTarget = anchorLightning[i + 1].target;
                        }
                        else
                        {
                            anchorLightning[i + 1].target = null;
                        }
                   
                    }
                    else
                    {
                        anchorLightning[i+1].target = null;
                    }                   
                }
                for (int j = 0; j < CountTarget; ++j){
                    if (!anchorLightning[j].target) break;
                    var pLightning = anchorLightning[j].lightning;
                    anchorLightning[j].start.transform.position = anchorLightning[j].startTarget.transform.position;
                    anchorLightning[j].end.transform.position = anchorLightning[j].target.transform.position;
                    pLightning.Source = anchorLightning[j].start;
                    pLightning.Destination = anchorLightning[j].end;
                    if (pLightning.ManualMode)
                    {
                        anchorLightning[j].init();
                        pLightning.Trigger(0.2f);
                    }
                    else
                    {
                        pLightning.onCompleteAll = null;
                        var pHeath = anchorLightning[j].target.GetComponent<Health>();
                        if (pHeath)
                        {
                            float pExtraDamage = 0;
                            float pCurrentDamge = (FixDamage * FactorDamage);
                            var PExtras = extraDamage.ToArray();
                            for (int i = 0; i < PExtras.Length; ++i)
                            {
                                pExtraDamage += PExtras[i].type == DamageType.Normal ? PExtras[i].damageExtra :
                                    (PExtras[i].type == DamageType.PecentHp ? (float)pHeath.CurrentHealth * PExtras[i].damageExtra/100f :
                                    (PExtras[i].type == DamageType.PecentMaxHp ? (float)pHeath.MaxiumHealth * PExtras[i].damageExtra / 100f : (pCurrentDamge * PExtras[i].damageExtra / 100f)));
                            }
                            pHeath.Damage((int)pCurrentDamge + (int)pExtraDamage, Owner ? Owner.gameObject : null, 0, 0);
                            if (Owner)
                            {
                                Owner._health.Damage(dameTaken, Owner ? Owner.gameObject : null, 0, 0);
                            }
                        }
                    }
                }
            }
        }

        public void onHandlerLightning(LightningInfo pInfo)
        {
            if (pInfo.target && pInfo.target.activeSelf)
            {
                var pHeath = pInfo.target.GetComponent<Health>();
                if (pHeath && pHeath.CurrentHealth > 0)
                {
                    float pExtraDamage = 0;
                    float pCurrentDamge = (FixDamage * FactorDamage);
                    var PExtras = extraDamage.ToArray();
                    for (int i = 0; i < PExtras.Length; ++i)
                    {
                        pExtraDamage += PExtras[i].type == DamageType.Normal ? PExtras[i].damageExtra  :
                            (PExtras[i].type == DamageType.PecentHp ? (float)pHeath.CurrentHealth * PExtras[i].damageExtra / 100f :
                            (PExtras[i].type == DamageType.PecentMaxHp ? (float)pHeath.MaxiumHealth * PExtras[i].damageExtra / 100f : (pCurrentDamge * PExtras[i].damageExtra / 100f)));
                    }
                    pHeath.Damage((int)pCurrentDamge + (int)pExtraDamage, Owner ? Owner.gameObject : null, 0, 0);
                    if (Owner && Owner._health)
                    {
                        Owner._health.Damage(dameTaken, Owner ? Owner.gameObject : null, 0, 0);
                    }
                }
            }

     
        }
        IEnumerator disableLightning()
        {
            yield return new WaitForSeconds(0.1f);

            // lightning.Destination = null;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (transform.lossyScale.x < 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            for (int i = 0; i < CountTarget; ++i)
            {
                if (i < anchorLightning.Count && anchorLightning[i].target != null)
                {
                    anchorLightning[i].start.transform.position = anchorLightning[i].startTarget.transform.position;
                    anchorLightning[i].end.transform.position = anchorLightning[i].target.transform.position;
                }
            }
  
        }
    }
}
