using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space {
    public class CameraShakeInvoke : MonoBehaviour
    {
        public void shake()
        {
            CameraShake.Instance.shakeDuration = 0.2f;
        }
    }
}
