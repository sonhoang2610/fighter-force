using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public class BaseElementEnviroment<TEnviroment,TSelft> : MonoBehaviour where TEnviroment : BaseEnviroment<TSelft,TEnviroment> where TSelft : BaseElementEnviroment<TEnviroment, TSelft>
    {
        protected List<TEnviroment> enviroments = new List<TEnviroment>();
        public virtual void OnEnable()
        {
            enviroments.Clear();
            TEnviroment[] pEnviromentActives = FindObjectsOfType<TEnviroment>();
            enviroments.AddRange(pEnviromentActives);
            foreach (var pEnvi in enviroments)
            {
                pEnvi.registerElement((TSelft)this);
            }
        }
        public virtual void OnDisable()
        {
            foreach (var pEnvi in enviroments)
            {
                pEnvi.unregisterElement((TSelft)this);
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
