using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Jobs;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
    public class PoolableObject : TimeControlBehavior
    {
        public UnityEvent onEndLifeTime;
        public float lifeTime = 3;
        [ValidateInput("checkRandomLifeTime","Greater than life Time")]
        public float lifeTimeRandomTo = 0;
#if UNITY_EDITOR
        public bool checkRandomLifeTime(float value)
        {
            if (lifeTimeRandomTo != 0 && lifeTimeRandomTo < lifeTime)
            {
                return false;
            }
            return true;
        }
#endif
        [SerializeField]
        protected bool ignoreLiveTime = false;
        [System.NonSerialized]
        public  float delayLifeTime = 0;
        protected virtual void OnEnable()
        {
            delayLifeTime = lifeTimeRandomTo != 0 ? Random.Range(lifeTime, lifeTimeRandomTo) : lifeTime;
        }

        protected virtual void OnDisable()
        {
            delayLifeTime = 0;
            if (name.StartsWith("[block]"))
            {
                name = name.Remove(0, 7);
            }
        }

        public virtual void KillObject()
        {
            onEndLifeTime.Invoke();
            gameObject.SetActive(false);
        }
        
        protected virtual void Update()
        {
            if (delayLifeTime > 0 && !ignoreLiveTime)
            {
                delayLifeTime -= time.deltaTime;
                if (delayLifeTime <= 0)
                {
                    KillObject();
                }
            }
        }
    }
}
