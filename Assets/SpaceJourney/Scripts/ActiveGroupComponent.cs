using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Tools
{
    [System.Serializable]
    public struct ComponentGroup
    {
        public string id;
        public Collider2D[] components;
    }
    public class ActiveGroupComponent : MonoBehaviour
    {
        public ComponentGroup[] groups;
        public void enableComponents(string pID,bool pBool)
        {
            foreach (var pComponent in groups)
            {
                if (pComponent.id == pID)
                {
                    foreach (var pcompoent in pComponent.components)
                    {
                        pcompoent.enabled = pBool;
                    }
                }
            }
        }
        public void enableComponents(string pID)
        {
            foreach(var pComponent in groups)
            {
                if(pComponent.id == pID)
                {
                    foreach(var pcompoent in pComponent.components)
                    {
                        pcompoent.enabled = true;
                    }
                }
            }
        }
        public void disableComponents(string pID)
        {
            foreach (var pComponent in groups)
            {
                if (pComponent.id == pID)
                {
                    foreach (var pcompoent in pComponent.components)
                    {
                        pcompoent.enabled = false;
                    }
                }
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

