using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class ShieldInfo
    {
        public GameObject shieldObject;
        public float duration = 10;
        [HideInInspector]
        [System.NonSerialized]
        public GameObject currentShield;
        [HideInInspector]
        [System.NonSerialized]
        public float currentDuration = 0;
    }
    public class ShieldControl : TimeControlBehavior,IRespawn
    {
        public GameObject shieldObject;
        public float duration = 10;
        protected GameObject currentShield;
        protected float currentDuration = 0;

        public ShieldInfo[] extraShield;

        public void activeShield()
        {
            if (!currentShield)
            {
                currentShield = Instantiate(shieldObject);
                currentShield.SetActive(false);
            }
            currentDuration = duration;
            currentShield.transform.position = transform.position;
            if (currentShield && !currentShield.activeSelf)
            {
                invu++;
                GUIManager.Instance.addStatus("Shield", duration);
            }
            currentShield.gameObject.SetActive(true);
            var pPos = currentShield.GetComponent<PositionConstraint>();
            pPos.SetSource(0, new ConstraintSource() { sourceTransform = transform,weight =1 });
            pPos.constraintActive = true;
         
            var pHealth = GetComponent<Health>();
            if (pHealth)
            {
                pHealth.Invulnerable = true;
            }
        }
        [ShowInInspector]
        int invu = 0;
        public void activeShieldExtra(int index,float pDuration)
        {
            var pShield = extraShield[index];
            pShield.duration = pDuration;
            if (!pShield.currentShield)
            {
                pShield.currentShield = Instantiate(pShield.shieldObject);
                pShield.currentShield.gameObject.SetActive(false);
            }
            pShield.currentDuration = pShield.duration;
            pShield.currentShield.transform.position = transform.position;
            if (pShield.currentShield && !pShield.currentShield.activeSelf)
            {
                invu++;
            }        
            pShield.currentShield.gameObject.SetActive(true);
            var pPos = pShield.currentShield.GetComponent<PositionConstraint>();
            pPos.SetSource(0, new ConstraintSource() { sourceTransform = transform, weight = 1 });
            pPos.constraintActive = true;
            var pHealth = GetComponent<Health>();
            if (pHealth)
            {
                pHealth.Invulnerable = true;
            }
        }

        public void onRespawn()
        {
            currentDuration = 0;
            if (currentShield)
            {
                currentShield.gameObject.SetActive(false);
            }
            invu = 0;
            for (int i = 0; i< extraShield.Length; ++i)
            {
                extraShield[i].currentDuration = 0;
                if (extraShield[i].currentShield)
                {
                    extraShield[i].currentShield.gameObject.SetActive(false);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            for(int i = 0;i < extraShield.Length; ++i)
            {
                var pShield = extraShield[i];
                if (pShield.currentDuration > 0)
                {
                    pShield.currentDuration -= time.deltaTime;
                    if (pShield.currentDuration <= 0)
                    {
                        pShield.currentShield.gameObject.SetActive(false);
                        invu--;
                    }
                }
            }
            if(currentDuration > 0)
            {
                currentDuration -= time.deltaTime;
                if(currentDuration<= 0)
                {
                    currentShield.gameObject.SetActive(false);
                    invu--;
                }
            }
            if(invu <= 0)
            {
                invu = 0;
                GetComponent<Health>().Invulnerable = false;
            }
        }
    }
}
