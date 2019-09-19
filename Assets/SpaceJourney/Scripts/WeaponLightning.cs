using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;
using EazyEngine.Tools.Space;

namespace EazyEngine.Space
{

    public class WeaponLightning : Weapon
    {

        public LightningBoltTransformTrackerScript lightningTracker;
        public LightningBoltPrefabScript lightning;
        public int dameTaken = 0;
        // public float durationApear = 0.1f;
        protected GameObject anchor, start, end;
        private void Awake()
        {
            anchor = new GameObject();
            anchor.transform.parent = transform;
            anchor.transform.localPosition = Vector3.zero;
            anchor.transform.localScale = new Vector3(1, 1, 1);
            lightning.transform.parent = anchor.transform;
            start = new GameObject();
            start.transform.parent = anchor.transform;
            start.transform.localScale = new Vector3(1, 1, 1);
            end = new GameObject();
            end.transform.parent = anchor.transform;
            end.transform.localScale = new Vector3(1, 1, 1);
            start.transform.localPosition = Vector3.zero;
            lightningTracker.StartTarget = start.transform;
            lightningTracker.EndTarget = end.transform;
            lightning.transform.localScale = new Vector3(1, 1, 1);

        }
        
        public override void WeaponUse()
        {
            base.WeaponUse();
            if (TargetDirection && TargetDirection.gameObject.activeSelf )
            {

                lightning.Source = start;
                lightning.Destination = end;
                if (lightning.ManualMode)
                {
                    lightning.onCompleteAll = onHandlerLightning;
                    lightning.Trigger();
                }
                else
                {
                    lightning.onCompleteAll = null;
                    var pHeath = TargetDirection.GetComponent<Health>();
                    float pExtraDamage = 0;
                    float pCurrentDamge = (FixDamage * FactorDamage );
                    var PExtras = extraDamage.ToArray();
                    for (int i = 0; i < PExtras.Length; ++i)
                    {
                        pExtraDamage += PExtras[i].type == DamageType.Normal ? PExtras[i].damageExtra :
                            (PExtras[i].type == DamageType.PecentHp ? (float)pHeath.CurrentHealth * PExtras[i].damageExtra :
                            (PExtras[i].type == DamageType.PecentMaxHp ? (float)pHeath.MaxiumHealth * PExtras[i].damageExtra : (pCurrentDamge * PExtras[i].damageExtra)));
                    }
                    pHeath.Damage((int)pCurrentDamge + (int)pExtraDamage, Owner ? Owner.gameObject : null, 0, 0);
                    if (Owner)
                    {
                        Owner._health.Damage(dameTaken, Owner ? Owner.gameObject : null, 0, 0);
                    }
                }

             }
        }
     
        public void onHandlerLightning()
        {
            if (TargetDirection)
            {
                var pHeath = TargetDirection.GetComponent<Health>();
                float pExtraDamage = 0;
                float pCurrentDamge = (FixDamage * FactorDamage);
                var PExtras = extraDamage.ToArray();
                for (int i = 0; i < PExtras.Length; ++i)
                {
                    pExtraDamage += PExtras[i].type == DamageType.Normal ? PExtras[i].damageExtra :
                        (PExtras[i].type == DamageType.PecentHp ? (float)pHeath.CurrentHealth * PExtras[i].damageExtra :
                        (PExtras[i].type == DamageType.PecentMaxHp ? (float)pHeath.MaxiumHealth * PExtras[i].damageExtra : (pCurrentDamge * PExtras[i].damageExtra)));
                }
                pHeath.Damage((int)pCurrentDamge + (int)pExtraDamage, Owner ? Owner.gameObject : null, 0, 0);
            }
           
            if (Owner)
            {
                Owner._health.Damage(dameTaken, Owner ? Owner.gameObject : null, 0, 0);
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
            if (TargetDirection)
            {
                end.transform.position = TargetDirection.transform.position;
                //  anchor.transform.RotationDirect2D((targetDirection.transform.position - anchor.transform.position),TranformExtension.FacingDirection.DOWN);
            }
        }
    }
}
