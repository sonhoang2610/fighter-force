using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public class ComplexLaserWeapon : WeaponGroup
    {
        public LaserWeapon parent;
        public LaserWeapon[] childs;

        public override void WeaponStart()
        {
            if (IsFirstActive || ReActive)
            {
                if (needTargetToFire)
                {
                    if (!Radar)
                    {
                        Radar = GetComponent<WeaponFindTarget>();
                    }
                    Radar.RequestTarget(this);
                }
                Vector3 rotate = transform.rotation.eulerAngles;
                Vector2 directon = Vector2.up.Rotate(rotate.z).normalized;
                Vector3 disteny = (Vector3)directon.normalized * 30 + transform.position;
                Vector2 vuonggoc = Vector2.Perpendicular(directon).normalized;
                if (TargetDirection)
                {

                    Vector2 pMain1 = vuonggoc * 10 + (Vector2)TargetDirection.transform.position;
                    Vector2 pMain2 = -vuonggoc * 10 + (Vector2)TargetDirection.transform.position;
                    Vector2 pTarget = Vector2.zero;

                    bool pInterSec = MathExtends.LineIntersection(transform.position, disteny, pMain1, pMain2, ref pTarget);
                    if (pInterSec)
                    {
                        parent.maxLength = Vector2.Distance(transform.position, pTarget);
                        for (int i = 0; i < childs.Length; ++i)
                        {
                            childs[i].transform.position = pTarget;
                        }
                    }
                }
            }
            base.WeaponStart();
       
        }

        public override void WeaponIdle()
        {
            base.WeaponIdle();
            for (int i = 0; i < childs.Length; ++i)
            {
                childs[i].pauseWeapon(false);
            }

            parent.maxLength = 30;
        }

        public void onShootingParrent(int pLevel,bool isHitting)
        {
            for (int i = 0; i < childs.Length; ++i)
            {
                childs[i].pauseWeapon(isHitting);
            }
        }
        private void Awake()
        {
            parent.onShootingLevel += onShootingParrent;
        }
        //public override void LaserWeaponUse()
        //{
        //    Vector3 rotate = transform.rotation.eulerAngles;
        //    Vector2 directon = Vector2.up.Rotate(rotate.z).normalized;
        //    Vector3 disteny = (Vector3)directon.normalized * 30 + transform.position;
        //    Vector2 vuonggoc = Vector2.Perpendicular(directon).normalized;
        //    if (TargetDirection)
        //    {
        //        Vector2 pMain1 = vuonggoc * 10 + (Vector2)TargetDirection.transform.position;
        //        Vector2 pMain2 = -vuonggoc * 10 + (Vector2)TargetDirection.transform.position;
        //        Vector2 pTarget = Vector2.zero;
        //        bool pInterSec = MathExtends.LineIntersection(transform.position, disteny, pMain1, pMain2, ref pTarget);
        //        if (pInterSec)
        //        {
        //            lineLaser.gameObject.SetActive(true);
        //            startPoint.gameObject.SetActive(true);
        //            startPoint.transform.localPosition = Vector3.zero;
        //            endPoint.transform.localPosition = transform.InverseTransformPoint(pTarget);
        //            lineLaser.SetPositions(new Vector3[] { Vector3.zero, pTarget });
        //            for(int i = 0; i < childs.Length; ++i)
        //            {
        //                childs[i].transform.position = endPoint.transform.position;
        //            }
        //        }
        //    }
        //}
    }
}
