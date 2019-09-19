using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;
using EazyEngine.Tools;

namespace EazyEngine.Space {

    public class DamageElectric : TimeControlBehavior
    {
        public LayerMask targetDamage;
        public int damamagePerSec = 500;
        [HideInInspector]
        public LightningBoltPrefabScript line;

        private void Awake()
        {
            line = gameObject.GetComponent<LightningBoltPrefabScript>();
        }
        

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!line.Destination || !line.Source || !line.Destination.activeSelf || !line.Source.activeSelf) return;
            var hit = MMDebug.RayCast(line.Source.transform.position, (line.Destination.transform.position - line.Source.transform.position).normalized, Vector3.Distance(line.Destination.transform.position, line.Source.transform.position), targetDamage, Color.red, true);
            if (hit)
            {
               var health = hit.collider.gameObject.GetComponent<Health>();
                if (health)
                {
                    health.Damage((int)(damamagePerSec * time.deltaTime), gameObject, 0, 0);
                }
            }
        }
    }
}