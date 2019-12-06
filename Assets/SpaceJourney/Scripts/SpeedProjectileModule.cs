using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EazyEngine.Space
{
    [RequireComponent(typeof(Projectile))]
    public class SpeedProjectileModule : MonoBehaviour
    {
        protected Projectile _proj;
        public SpeedModule[] speeds;
        public Projectile proj
        {
            get
            {
                return _proj ? _proj : _proj = GetComponent<Projectile>();
            }
        }
        Sequence seq;
        protected  void OnEnable()
        {
            if (seq != null)
            {
                seq.Kill();
            }
            seq = DOTween.Sequence();
            for (int i = 0; i < speeds.Length; ++i)
            {
                var pSpeed = speeds[i];
                var pAction = DOTween.To(() => proj.CurrentSpeed, x => proj.CurrentSpeed = x, pSpeed.destiny, pSpeed.duration).SetEase(pSpeed.curve);
                if (!pSpeed.noStart)
                {
                    pAction.From(pSpeed.start);
                }
                seq.AppendInterval(pSpeed.delayStart);
                seq.Append(pAction);
            
                seq.AppendCallback(delegate
                {
                    pSpeed.onComplete.Invoke();
                });
            }
            seq.Play();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

