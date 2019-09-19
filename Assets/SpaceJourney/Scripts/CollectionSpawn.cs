using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{

    public class SpawnItem
    {
        public string _id;
        public GameObject _object;
    }
    public class CollectionSpawn : MonoBehaviour
    {
        public SpawnItem[] items;

        public void spawn(string pString)
        {
            if (items == null) return;
            for(int i = 0; i < items.Length; ++i)
            {
                if(items[i]._id == pString)
                {

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
