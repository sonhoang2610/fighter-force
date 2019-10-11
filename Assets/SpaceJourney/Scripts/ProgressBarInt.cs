using System.Collections;
using System.Collections.Generic;
using EazyCustomAction;
using UnityEngine;

public class ProgressBarInt : MonoBehaviour
{

    public GameObject[] elements;

    public int Value { get; set; }

    public void setProcessInt(int pProcess)
    {
        Value = pProcess;
        for(int i = 0; i < elements.Length; ++i){
            if(pProcess > i)
            {
                if (!elements[i].activeSelf)
                {
                    elements[i].transform.localScale = new Vector3(3,3,3);
                    RootMotionController.stopAllAction(elements[i]);
                    RootMotionController.runAction(elements[i],EazyScale.to(new Vector3(1,1,1),0.3f ));
                }
             
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
