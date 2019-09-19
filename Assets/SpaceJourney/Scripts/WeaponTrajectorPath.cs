using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public class WeaponTrajectorPath : MonoBehaviour
    {
        public BezierSplineRaw spline;
        [Button("Add Point")]
        public void addPoint()
        {
            spline.AddCurve();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [Button("Remove Last Point")]
        public void removePoint()
        {
            spline.deleteLast();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
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
