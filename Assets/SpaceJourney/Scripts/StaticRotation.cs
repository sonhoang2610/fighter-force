using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticRotation : MonoBehaviour
{
    Vector3 cacheDetalWorldPos;
    private void Awake()
    {
        cacheDetalWorldPos = transform.position - transform.parent.position;
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
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.position = transform.parent.position + cacheDetalWorldPos;
    }
}
