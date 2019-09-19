using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space {
    public class FollowerMainPlayer : MonoBehaviour
    {
        [SerializeField]
        protected float SpeedFollow = 5;
        [SerializeField]
        protected float MaxDuration = 0.5f;
        [SerializeField]
        protected Vector2 offsetSupportPlane = new Vector2(0, 0);

        public Vector2 OffsetSupportPlane
        {
            get
            {
                return offsetSupportPlane;
            }

            set
            {
                offsetSupportPlane = value;
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }
        bool isBooster = false;
        float pSpeedBooster = 0;
        // Update is called once per frame
        void Update()
        {
            if (LevelManger.InstanceRaw &&  LevelManger.Instance.CurrentPlayer)
            {
                Vector2 pTarget = LevelManger.Instance.CurrentPlayer.transform.localPosition + (Vector3) offsetSupportPlane ;
                float pDistance = Vector2.Distance(pTarget, transform.localPosition);
                if (!isBooster)
                {
                     pSpeedBooster = SpeedFollow;
                    if (pDistance / SpeedFollow > MaxDuration)
                    {
                        pSpeedBooster = pDistance / MaxDuration;
                        isBooster = true;
                    }
                }
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, pTarget, Time.deltaTime * pSpeedBooster);
                if(pDistance <= Time.deltaTime * pSpeedBooster)
                {
                    isBooster = false;
                    transform.localPosition = pTarget;
                }
            }
    }
    }
}
