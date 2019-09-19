using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public class BaseEnviroment<TElement,TSelft> : MonoBehaviour where TElement : BaseElementEnviroment<TSelft, TElement> where  TSelft : BaseEnviroment<TElement, TSelft>
    {
        protected List<TElement> elements = new List<TElement>();
        public virtual void registerElement(TElement pElement)
        {
            elements.Add(pElement);
        }

        public virtual void unregisterElement(TElement pElement)
        {
            elements.Remove(pElement);
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
