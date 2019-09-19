using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
    [System.Serializable]
    public class ListGameObject 
    {
        public List<GameObject> objects;
    }
    public class DelayActive : MonoBehaviour
    {
        public float delay = 0.2f;
        public ListGameObject[] groups;

        public IEnumerator activeAfter(float pDelay,int index)
        {
            yield return new WaitForSeconds(pDelay);
            foreach(var pObject in groups[index].objects)
            {
                pObject.SetActive(true);
            }
        }
        private void OnEnable()
        {
            for(int i = 0; i < groups.Length; ++i)
            {
                StartCoroutine(activeAfter(i * delay, i));
            }
        }

        private void OnDisable()
        {
            for (int index = 0; index < groups.Length; ++index)
            {
                foreach (var pObject in groups[index].objects)
                {
                    pObject.SetActive(false);
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
