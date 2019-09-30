using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace EazyEngine.Tools
{
    public class TriggerEventHandle : MonoBehaviour
    {

        public LayerMask mask;
        public UnityEvent onTrigger2DEnter;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Layers.LayerInLayerMask(collision.gameObject.layer, mask))
            {
                onTrigger2DEnter.Invoke();
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
    }
}

