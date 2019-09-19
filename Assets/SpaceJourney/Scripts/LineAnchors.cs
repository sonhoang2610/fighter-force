using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EazyEngine.Space
{
    [ExecuteInEditMode]
    public class LineAnchors : MonoBehaviour
    {
        public GameObject anchor1, anchor2;
        public LineRenderer lineRender;
        private void OnEnable()
        {
            if (anchor1 && anchor2 && lineRender)
            {
                lineRender.SetPosition(0, anchor1.transform.position);
                lineRender.SetPosition(1, anchor2.transform.position);
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
          
        }
        private void LateUpdate()
        {
            if (anchor1 && anchor2 && lineRender)
            {
                lineRender.SetPosition(0, anchor1.transform.position);
                lineRender.SetPosition(1, anchor2.transform.position);
            }
        }
    }
}

