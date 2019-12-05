using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public class MovingFormatingBulletElement : MonoBehaviour, IMovementProjectile
    {
        public GameObject leader;
        public Vector2 deltaOffset;
        public float speed = 5;
        public bool Horizontal = true;
        public bool Vertical = true;
        public bool isBlockRotation()
        {
            return false;
        }
        public int getIndex()
        {
            return 0;
        }

        public bool isBlock()
        {
            return false;
        }

        public Vector2 Movement()
        {
            return new Vector2(0, 0);
        }

        public void setDirection(Vector2 pDir)
        {
          
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
            if (leader)
            {
                Vector2 destiny = (Vector2)leader.transform.position + deltaOffset;
                destiny = destiny.Rotate(leader.transform.eulerAngles.z);
            }

        }
    }
}
