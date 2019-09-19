using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EazyLoadingText : MonoBehaviour {
    UIEazyText text;
    public int countDot=0;
    public float duration=0;
    float currentTimer=0;
    int currentDot = 0;

    public UIEazyText EazyText
    {
        get
        {
            return text ? text : text = GetComponent<UIEazyText>();
        }
        
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(countDot > 0 && duration > 0 && EazyText)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= duration)
            {
                string dot = "";
          
                for (int i = 0; i < currentDot; ++i)
                {
                    dot += ".";
                }
                currentDot++;
                EazyText.setExtraText(dot);
                if (currentDot > countDot)
                {
                    currentDot = 0;
                }
                currentTimer = 0;
            }
        }
	}
}
