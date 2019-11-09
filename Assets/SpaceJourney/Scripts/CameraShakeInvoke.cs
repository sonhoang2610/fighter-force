using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space {
    public class CameraShakeInvoke : MonoBehaviour
    {
        public void shake()
        {
            CameraShake.Instance.shakeAmount = 0.25f;
            CameraShake.Instance.shakeDuration = 0.2f;
        }
        public void shakeDuration(float pDurtion)
        {
            CameraShake.Instance.shakeAmount = 0.1f;
            CameraShake.Instance.shakeDuration = pDurtion;
        }
    }
}
