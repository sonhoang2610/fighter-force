using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace EazyEngine.Space
{
    public class ProjectileTween : ProjectileMultipeWeapon
    {
        public bool playOnStart = false;
        protected DOTweenAnimation[] tweens;
        private void Start()
        {
            //tweens = new DOTweenAnimation[AttachMentPosStart.Length];
            //for (int i = 0; i < AttachMentPosStart.Length; ++i)
            //{
            //    tweens[i] = AttachMentPosStart[i].GetComponent<DOTweenAnimation>();
            //    if (tweens[i] != null)
            //    {
            //        tweens[i].DOPlay();
            //    }
            //}
        }

        public override void WeaponUse()
        {
            base.WeaponUse();
            if (!playOnStart)
            {
                //for (int i = 0; i < AttachMentPosStart.Length; ++i)
                //{
                //    if (tweens[i] != null)
                //    {
                //        tweens[i].DOPlay();
                //    }
                //}
            }
        }

        public override void WeaponIdle()
        {
            base.WeaponIdle();
            if (!playOnStart)
            {
                //for (int i = 0; i < AttachMentPosStart.Length; ++i)
                //{
                //    if (tweens[i] != null)
                //    {
                //       tweens[i].DOPause();
                //    }
                //}
            }
        }
    }
}
