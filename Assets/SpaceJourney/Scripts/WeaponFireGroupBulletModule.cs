using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public interface IWeaponModule
    {
        void startFire(ObjectGroupAttachMent currentAttack,int indexAttachMent);
    }
    public class WeaponFireGroupBulletModule : MonoBehaviour, IWeaponModule
    {
        public void startFire(ObjectGroupAttachMent currentAttack, int indexAttachMent)
        {
            
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
