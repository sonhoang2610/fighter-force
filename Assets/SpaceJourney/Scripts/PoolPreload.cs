using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;

public enum PoolType
{
    Normal,
    Particle,
    Item,
    Enemy,
    Common,
}
[System.Serializable]
public struct PoolElement
{
    public PoolType poolType;
    public GameObject objectToPool;
    public int count;
}
public class PoolPreload : MonoBehaviour
{
    public PoolElement[] pools;
    private void Awake()
    {
        for(int i = 0; i < pools.Length; ++i)
        {
            if(pools[i].poolType == PoolType.Particle)
            {
                ParticleEnviroment.Instance.getObjectFromPoolWithNumberPreload( pools[i].objectToPool,pools[i].count);
            }
            if (pools[i].poolType == PoolType.Common)
            {
                PoolManagerComon.Instance.getObjectFromPoolWithNumberPreload(pools[i].objectToPool, pools[i].count);
            }
            if (pools[i].poolType == PoolType.Enemy)
            {
                EnemyEnviroment.Instance.getObjectFromPoolWithNumberPreload(pools[i].objectToPool, pools[i].count);
            }
            if (pools[i].poolType == PoolType.Item)
            {
                ItemEnviroment.Instance.getObjectFromPoolWithNumberPreload(pools[i].objectToPool, pools[i].count);
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
