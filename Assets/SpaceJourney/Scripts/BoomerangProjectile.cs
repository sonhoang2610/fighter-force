using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class SpeedModule
    {
        public float delayStart = 0;
        public bool noStart = false;
        [HideIf("noStart")]
        public float start;
        public float destiny;
        public float duration = 2;
        public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

  
    }
    public class BoomerangProjectile : Projectile
    {
        public SpeedModule firstModule;
        public SpeedModule secondModule;
        protected override bool onSpeedConstraintFeature()
        {
            return true;
        }
        Sequence seq;
        protected override void OnEnable()
        {
           // _SpeedConstraint = true;
            base.OnEnable();
            if(seq != null)
            {
                seq.Kill();
            }
            seq = DOTween.Sequence();
            if (!firstModule.noStart)
            {
                currentSpeed = firstModule.start;
            }
            var pFirst = DOTween.To(() => currentSpeed, x => currentSpeed = x, firstModule.destiny, firstModule.duration).OnComplete(() => setDirection(Direction *= -1)).SetEase(firstModule.curve);
            seq.Append(pFirst);

            var pSecond = DOTween.To(() => currentSpeed, x => currentSpeed = x, secondModule.destiny, secondModule.duration).SetEase(secondModule.curve);
            if (!secondModule.noStart)
            {
                pSecond.From(secondModule.start);
            }
            seq.Append(pSecond);
            seq.Play();
        }


        // Start is called before the first frame update
        void Start()
        {

        }
        private void LateUpdate()
        {
            
        }
    }
}
