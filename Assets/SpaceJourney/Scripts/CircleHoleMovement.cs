using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{

    public interface IMovementProjectile
    {
        void setDirection(Vector2 pDir);
        void setSpeed(float pSpeed);
        Vector2 Movement();
        bool isBlock();
        bool isBlockRotation();
        int getIndex();

        bool IsEnable();
    }
    public class CircleHoleMovement : TimeControlBehavior, IMovementProjectile
    {

        public float SpeedRad = 120;
        public float SpeedSpread = 5;
        public int piorityIndex = 0;
        public bool _isBlock = true;

        protected Vector2 direction;
        protected Vector2 cachePos,oldPos;
        protected float currentRadius;
        public int getIndex()
        {
            return piorityIndex;
        }

        public bool isBlock()
        {
            return _isBlock;
        }

        public bool isBlockRotation()
        {
            return false;
        }

        public bool IsEnable()
        {
            return enabled;
        }

        public Vector2 Movement()
        {
            direction = direction. Rotate(SpeedRad * time.deltaTime);
            currentRadius += time.deltaTime * SpeedSpread;
            Vector2 pNewPos = cachePos + direction.normalized * currentRadius;
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

