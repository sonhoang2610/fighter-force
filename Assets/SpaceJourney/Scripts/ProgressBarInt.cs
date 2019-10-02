using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarInt : MonoBehaviour
{

    public GameObject[] elements;


    public void setProcessInt(int pProcess)
    {
        for(int i = 0; i < elements.Length; ++i){
            if(pProcess > i)
            {
                elements[i].gameObject.SetActive(true);
            }
            else
            {
                elements[i].gameObject.SetActive(false);
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
