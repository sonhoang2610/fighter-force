using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public class DirectionAngular : MonoBehaviour,IMovementProjectile
    {
        public Vector2 directionTarget;
        public float overRotation;
        public float rotationSpeed;
        protected float speed;
        protected Vector2 defaultDirection;
        protected float pSide = 0;
        protected Rigidbody2D rb;
        public bool IsEnable()
        {
            return enabled;
        }
        public bool isBlockRotation()
        {
            return true;
        }
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
            var pDirTarget = directionTarget.normalized;
            pSide = Vector3.Cross(pDirTarget, transform.up.normalized).z;
            rb.angularVelocity = -rotationSpeed * (pSide);
            rb.velocity = transform.up * speed;
            return Vector2.zero;
        }

        public void setDirection(Vector2 pDir)
        {
            defaultDirection = pDir;
            rb.angularVelocity = 0;
            var pDirTarget = directionTarget.normalized;
            pSide = Vector3.Cross(pDirTarget, transform.up.normalized).z;
            directionTarget = pDirTarget.Rotate2DAround(Vector2.zero, 1, (pSide > 0 ? 1 : -1) * overRotation);


        }

        public void setSpeed(float pSpeed)
        {
            speed = pSpeed;
        }
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
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
