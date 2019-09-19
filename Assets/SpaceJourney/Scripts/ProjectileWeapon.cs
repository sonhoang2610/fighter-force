using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;


namespace EazyEngine.Space
{
    public class ProjectileWeapon : Weapon
    {
 
        protected ObjectPooler _pooler;
        public override void init()
        {
            base.init();
            _pooler = GetComponent<ObjectPooler>();
        }
        public override void WeaponUse()
        {
            base.WeaponUse();
            GameObject pObjectProjectile = _pooler.GetPooledGameObject();
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
            if (proj)
            {
                proj.setWeapon(this);
                proj.setOwner(Owner);
                proj.GetComponent<Health>().Revive();
                Vector3 rotate =  transform.rotation.eulerAngles;
                Vector2 directon = Vector2.up.Rotate(rotate.z);
                if (IsRandom)
                {
                    Vector2 pCenter = LevelManger.Instance.mainPlayCamera.transform.position ;
                    float pWidth = Screen.width * (LevelManger.Instance.mainPlayCamera.orthographicSize * 2) / Screen.height;
                    float pHeight = LevelManger.Instance.mainPlayCamera.orthographicSize * 2;
                    float pRandom = Random.Range(-pWidth / 2, pWidth / 2);
                    pCenter += new Vector2(pRandom, -pHeight / 2);
                    directon = (pCenter - (Vector2)transform.position).normalized;
                }
                if (TargetDirection)
                {
                
                    if (anchorRotation == null)
                    {
                        directon = (TargetDirection.transform.position - transform.position).normalized;
                        proj.setDirection(directon);
                    }
                    else
                    {
                        proj.setDirection(directon);
                    }
                }
                else
                {
                    proj.setDirection(directon);
                }
            
            }
            pObjectProjectile.gameObject.SetActive(true);
        }
        private void Update()
        {
            if (TargetDirection && anchorRotation)
            {
              //  isReady = false;
                Vector2 directon = (TargetDirection.transform.position - transform.position).normalized;
                anchorRotation.transform.rotation = Quaternion.RotateTowards(anchorRotation.transform.rotation, directon.ConvertToQuaternion(TranformExtension.FacingDirection.DOWN),360.0f*time.deltaTime);
                //var hits = Physics2D.RaycastAll(transform.position, Vector2.up.Rotate(anchorRotation.transform.rotation.eulerAngles.z), 10);
                //foreach(var hit in hits)
                //{
                //    if(hit.collider.gameObject == targetDirection)
                //    {
                //        isReady = true;
                //    }
                //}
            }
        }
    }
}
