using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{


    public class CircleMovement : TimeControlBehavior, IMovementProjectile
    {
        protected Projectile _proj;
        public Projectile proj
        {
            get
            {
                return _proj ? _proj : _proj = GetComponent<Projectile>();
            }
        }
        public int piorityIndex = 0;
        public bool _isBlock = true;
        public SpeedModule[] speedModules;
        protected Vector2 direction;
        protected Vector2 cachePos, oldPos;
        protected float currentRadius;
        [ShowInInspector]
        protected float CurrentSpeed;
        protected Sequence seq;
        public bool isBlockRotation()
        {
            return false;
        }
        private void OnEnable()
        {
             seq = DOTween.Sequence();
            for(int i = 0; i < speedModules.Length; ++i)
            {
                var speedModule = speedModules[i];
                var pAction = DOTween.To(() => CurrentSpeed, x => CurrentSpeed = x, speedModule.destiny, speedModule.duration).SetEase(speedModule.curve);
                if (!speedModule.noStart)
                {
                    pAction.From(speedModule.start);
                }
                seq.AppendInterval(speedModule.delayStart);
                seq.Append(pAction);
            }

            seq.Play();
            CurrentSpeed = 0;
        }

        private void OnDisable()
        {
            seq.Kill();
        }
        public int getIndex()
        {
            return piorityIndex;
        }

        public bool isBlock()
        {
            return _isBlock;
        }

        public Vector2 Movement()
        {
            oldPos = transform.position;
            if (CurrentSpeed == 0) {
             
                return Vector2.zero;
            }
            direction = direction.Rotate(CurrentSpeed * time.deltaTime);
            var pRadius = Vector2.Distance(cachePos, transform.position);
            Vector2 pNewPos = cachePos + direction.normalized * (pRadius);
            proj.Direction = direction.normalized;
            Vector2 delta = pNewPos - oldPos;
            oldPos = pNewPos;
            return delta;
        }

        public void setDirection(Vector2 pDir)
        {
            cachePos = transform.position;
            oldPos = cachePos;
            currentRadius = 0;
            direction = pDir.normalized;
        }

        public void setSpeed(float pSpeed)
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

