using EazyEngine.Space.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EazyEngine.Space.UI
{
    public class SpinWheel : BaseBox<ItemWheel,BaseItemGameInstancedArray>
    {
        public UnityEvent onStopRoll;
        public List<AnimationCurve> animationCurves;
        public Pos randomCountWheelRange = new Pos(1, 4);
        public Vector2 randomTimeWheel = new Vector2(1, 2);
        public bool spinning;
        private float anglePerItem;
        private int randomTime;
        public int itemNumber;

        void Start()
        {
            spinning = false;
         
        }



        public void startRoll()
        {
            anglePerItem = 360 / DataSource.Count;
            randomTime = Random.Range(randomCountWheelRange.x, randomCountWheelRange.y);
            float maxAngle = 360 * randomTime + (itemNumber * anglePerItem) -(anglePerItem/2);

            StartCoroutine(SpinTheWheel(Random.Range(randomTimeWheel.x, randomTimeWheel.y) * randomTime, maxAngle));
        }

        IEnumerator SpinTheWheel(float time, float maxAngle)
        {
            spinning = true;

            float timer = 0.0f;
            float startAngle = transform.eulerAngles.z;
            maxAngle = maxAngle - startAngle;

            int animationCurveNumber = Random.Range(0, animationCurves.Count);
            Debug.Log("Animation Curve No. : " + animationCurveNumber);

            while (timer < time)
            {
                //to calculate rotation
                float angle = maxAngle * animationCurves[animationCurveNumber].Evaluate(timer / time);
                transform.eulerAngles = new Vector3(0.0f, 0.0f, angle + startAngle);
                timer += Time.deltaTime;
                yield return 0;
            }

            transform.eulerAngles = new Vector3(0.0f, 0.0f, maxAngle + startAngle);
            spinning = false;
            onStopRoll.Invoke();
            //Debug.Log("Prize: " + prize[itemNumber]);//use prize[itemNumnber] as per requirement
        }
    }
}
