using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
    [System.Serializable]
    public class WeaponIntanceAttachMent
    {
        public GameObject attachMent;
        public Weapon weapon;
    }
    public class WeaponGroup : Weapon
    {
        public WeaponIntanceAttachMent[] dataWeapons;
        public List<Weapon> weapons = new List<Weapon>();
        public override int FixDamage { get => base.FixDamage; set {
                for (int i = 0; i < weapons.Count; ++i)
                {
                    weapons[i].FixDamage = value;
                }
                base.FixDamage = value;
            } }
        public override void setData(WeaponInstancedConfig pData)
        {
            base.setData(pData);
            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].extraDamage.Add(pData.extraDamge);
            }
        }
        public override bool IsFirstActive { get => base.IsFirstActive;
            set
            {
                base.IsFirstActive = value;
                for (int i = 0; i < weapons.Count; ++i)
                {
                    weapons[i].IsFirstActive = value;
                }
            }
        }
        public override bool ReActive 
        {
            get => base.ReActive;
            set
            {
                base.ReActive = value;
                for (int i = 0; i < weapons.Count; ++i)
                {
                    weapons[i].ReActive = value;
                }
            }
        }
        public override void AnimUse()
        {
            base.AnimUse();
            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].AnimUse();
            }
        }
        public override void onTrigger(string pTrigger)
        {
            base.onTrigger(pTrigger);
            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].onTrigger(pTrigger);
            }
        }
        public override void changeState(WeaponState nextState)
        {
            base.changeState(nextState);
            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].changeState(nextState);
            }
        }
        public override int getConsumeShoot()
        {
            int pCount = 0;
            for (int i = 0; i < weapons.Count; ++i)
            {
                pCount += weapons[i].getConsumeShoot();
            }
            return pCount;
        }
        public override void WeaponUse()
        {
            base.WeaponUse();
            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].WeaponUse();
            }
        }
        public override void WeaponStart()
        {
            base.WeaponUse();
            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].WeaponStart();
            }
        }
        public override void WeaponIdle()
        {
            base.WeaponUse();
            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].WeaponIdle();
            }
        }
        public override int currentTargetCount()
        {
            int pCount = 0;
            for (int i = 0; i < weapons.Count; ++i)
            {
                pCount += weapons[i].currentTargetCount();
            }
            return pCount;
        }
        public override int countTargetNeeded()
        {
            int pCount = 0;
            for (int i = 0; i < weapons.Count; ++i)
            {
                pCount += weapons[i].countTargetNeeded();
            }
            return pCount;
        }
        public override void addTargetDirection(GameObject pObject)
        {
            base.addTargetDirection(pObject);
            for(int i = 0; i < weapons.Count; ++i)
            {
                if(weapons[i].currentTargetCount() < weapons[i].countTargetNeeded())
                {
                    weapons[i].addTargetDirection(pObject);
                    return;
                }
            }
        }
        public override void InputStop()
        {
            base.InputStop();
            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].InputStop();
            }
        }
        public override void init()
        {
            base.init();
            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].init();
                weapons[i].WeaponParrent = this;
                weapons[i].BlockState = true;
                weapons[i].initDone();
            }
            for(int i = 0; i < dataWeapons.Length; ++i)
            {
                var pweaon = Instantiate<Weapon>(dataWeapons[i].weapon, dataWeapons[i].attachMent.transform);
                weapons.Add(pweaon);
                pweaon.init();
                pweaon.WeaponParrent = this;
                pweaon.BlockState = true;
                pweaon.initDone();
            }
        }
        
    }
}
