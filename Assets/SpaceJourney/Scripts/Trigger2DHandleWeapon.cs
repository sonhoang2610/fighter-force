using EazyEngine.Space;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2DHandleWeapon : MonoBehaviour
{
    public string compareStr;
    public CharacterHandleWeapon handle;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.StartsWith(compareStr))
        {
            Debug.Log("trigger enter" + collision.name);
            handle.triggerChangeWeapon(collision.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.StartsWith(compareStr))
        {
            Debug.Log("trigger exits" + collision.name);
            handle.triggerChangeWeapon(collision.name + "off");
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
