using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxAutoRun : MonoBehaviour
{
    public UIScrollView scrollView;
    public float delta;
    protected bool mPressed;
    void OnPress(bool pressed)
    {
        mPressed = pressed;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!mPressed)
        {
            scrollView.Scroll(delta);
        }
    }
}
