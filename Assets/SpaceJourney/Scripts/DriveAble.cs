using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyCustomAction;


namespace EazyEngine.Space
{
    public class DriveAble : BaseElementEnviroment<DriveBulletToTarget,DriveAble>,IRespawn
    {
        public Projectile prop;
        public System.Action _onDeath;
        private Vector2[] trajectors;

        public Vector2[] Trajectors { get => trajectors; set {
                trajectors = value;
            }
        }

        public void onRespawn()
        {
            RootMotionController.stopAllAction(gameObject);
            Trajectors = null;
            prop.IgnoreMove = false;
        }
        Sequences _seq;
        public void onStartDrive(Vector2[] poss,Vector2 lastDirection)
        {
           
            List<EazyAction> actions = new List<EazyAction>();
           for (int i = 0; i < poss.Length; ++i)
            {
                actions.Add(EazyMove.to(poss[i], prop.Speed,false));
                if(i == 0)
                {
                   // actions.Add(DelayTime.create(0.25f));
                }
            }
          //  actions.Add(DelayTime.create(0.25f));
            actions.Add(CallFunc.create(delegate
            {
                prop.IgnoreMove = false;
                prop.setDirection(lastDirection);
            }));
            _seq = Sequences.create(actions.ToArray());
            RootMotionController.runAction(gameObject, _seq);
        }
        private void Awake()
        {
            prop = GetComponent<Projectile>();
            GetComponent<Health>().onDeath.AddListener( onDeath);
        }
        public void onDeath()
        {
            if(_onDeath!= null)
            {
                _onDeath();
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //if (isMoving)
            //{
            //    if (Trajectors != null)
            //    {
            //        for(int i = 0; i < Trajectors.Length; ++i)
            //        {
                        
            //        }
            //    }
            //}
        }
    }
}

