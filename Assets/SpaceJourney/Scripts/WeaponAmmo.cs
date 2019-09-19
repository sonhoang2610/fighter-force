using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public enum ConditionRestoreAmmo
    {
        SmallerStorage,
        Zero,
    }
    public class WeaponAmmo : TimeControlBehavior,IRespawn
    {
        public int initStorage = 100;
        [HideInEditorMode]
        public int storageBullet = 0;
  
        public bool isRandomTimeRestore = false;
        [HideIf("isRandomTimeRestore")]
        public float timeToRestore = 0.1f;
        [ShowIf("isRandomTimeRestore")]
        public Vector2 randomTimeRestore = new Vector2(0.1f, 0.2f);
        public int quantityRestoreEachTime = 1;
        public int limitRestore = 999999;
        public ConditionRestoreAmmo conditonRestore = ConditionRestoreAmmo.SmallerStorage;
        protected float currentRestoreTime = 0;
        protected float RestoreTimeReal = 0;
        private int totalRestore = 0;
        [HideInInspector]
        public Weapon _weapon;
        public UnityEngine.Events.UnityEvent onEmptyStorage;
        public UnityEngine.Events.UnityEvent onStartRestore;
        public UnityEngine.Events.UnityEvent onNotEnough;
        public float TimeToRestore { get { return RestoreTimeReal; } set => timeToRestore = value; }

        public int TotalRestore { get => totalRestore; set => totalRestore = value; }

        bool idle = true;
        private void Awake()
        {
            storageBullet = initStorage;
            _weapon = GetComponent<Weapon>();
            RestoreTimeReal = isRandomTimeRestore ? Random.Range(randomTimeRestore.x,randomTimeRestore.y) : timeToRestore;
        }
        public bool EnoughToFire(int pComsume)  
        {
            if (storageBullet < pComsume)
            {
                onNotEnough.Invoke();   
            }
            return storageBullet >= pComsume;
        }

        public void changeStorage(int pStep)
        {
            storageBullet += pStep;
            if(storageBullet <= 0) {
                onEmptyStorage.Invoke();
            }
        }

        private void LateUpdate()
        {
            if(TotalRestore >= limitRestore && limitRestore != 999999)
            {
                return;
            }
            if((storageBullet < initStorage && conditonRestore == ConditionRestoreAmmo.SmallerStorage)  || (storageBullet <= 0))
            {
                if (idle)
                {
                    onStartRestore.Invoke();
                    RestoreTimeReal = isRandomTimeRestore ? Random.Range(randomTimeRestore.x, randomTimeRestore.y) : timeToRestore;
                }
                currentRestoreTime += time.deltaTime * _weapon.FactorSpeed* _weapon.FactorSpeedWeapon;
                if(currentRestoreTime >= TimeToRestore)
                {
                    currentRestoreTime = 0;
                    TotalRestore += quantityRestoreEachTime;
                    storageBullet+=  quantityRestoreEachTime;
                    _weapon.onRestoreAmmo(quantityRestoreEachTime);
                }
                idle = false;
            }
            else
            {
                idle = true;
            }
        }

        public void onRespawn()
        {
            storageBullet = initStorage;
            TotalRestore = 0;
            idle = true;
            currentRestoreTime = 0;
        }
    }
}
