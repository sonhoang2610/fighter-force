using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public class TriangleMovement : TimeControlBehavior,IMovementProjectile
    {
        public float Speed = 7;
        public Vector2 axis = Vector2.down;
        protected float currentPos;
        protected Vector2 cachePos, oldPos,dir;
        public int getIndex()
        {
            return 0;
        }

        public bool isBlock()
        {
            return true;
        }

        public Vector2 Movement()
        {
            currentPos += time.deltaTime*Speed;
            Vector2 pDir = axis.normalized * currentPos;
            Vector2 perpendicular = Vector2.Perpendicular(axis);
            if (Vector2.Angle(perpendicular, dir) >= 90)
            {
                perpendicular *= -1;
            }
            Vector2 point = pDir + perpendicular * Mathf.Tan(Vector2.Angle(axis.normalized, dir.normalized) * Mathf.Deg2Rad) * currentPos;
            Vector2 newPoint = point + cachePos;
            Vector2 delta = newPoint - oldPos;
            oldPos = newPoint;
            return delta;
        }

        public void setDirection(Vector2 pDir)
        {
            cachePos = transform.position;
            oldPos = cachePos;
            dir = pDir;
            if (Vector2.Angle(axis, pDir) > 90)
            {
                axis = -axis;
            }
            axis = axis.normalized;
        }
        public void setSpeed(float pSpeed)
        {
            Speed = pSpeed;
        }
        private void OnDisable()
        {
            currentPos = 0;
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

