using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
    [RequireComponent(typeof(CharacterHandleWeapon))]
    public class WeaponOverride : TimeControlBehavior,IRespawn
    {
        public WeaponInstanceInfo[] weaponOverrides;
        protected float currentTimeOverride = 0;
        CharacterHandleWeapon _handleWeapon;


        public void overrideWeapon(float pTime)
        {
            currentTimeOverride = pTime;
            _handleWeapon.setOverrideWeapon(weaponOverrides);
        }
        public void onRespawn()
        {
            currentTimeOverride = 0;
        }

        private void Awake()
        {
            _handleWeapon = GetComponent<CharacterHandleWeapon>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(currentTimeOverride > 0)
            {
                currentTimeOverride -= time.deltaTime;
                if(currentTimeOverride <= 0)
                {
                 //   _handleWeapon.rollBackSavedWeapon();
                }
            }
        }
    }
}
