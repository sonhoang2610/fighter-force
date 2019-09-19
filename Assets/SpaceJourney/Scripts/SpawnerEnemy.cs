using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space {
    public class SpawnerEnemy : MonoBehaviour {

        ObjectPooler _pooler;

        public ObjectPooler Pooler
        {
            get
            {
                return _pooler ? _pooler : _pooler = GetComponent<ObjectPooler>();
            }
        }
        
        public void spawn()
        {
            GameObject pGameObject = Pooler.GetPooledGameObject();
            pGameObject.transform.position = transform.position;
            pGameObject.SetActive(true);
            IRespawn[] pRespawns = pGameObject.GetComponents<IRespawn>();
            foreach (IRespawn res in pRespawns)
            {
                res.onRespawn();
            }
        }

        
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}
