using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class BezierEnviroment : MonoBehaviour {
    [SerializeField]
    [HideInInspector]
    bool isFirstTime = false;
    private void Awake()
    {
        if (!isFirstTime)
        {
                RectTransform rect = gameObject.GetComponent<RectTransform>();
            //    //BezierSpline spline = gameObject.GetComponent<BezierSpline>();
            //    if (rect == null)
            //    {
            //        rect = gameObject.AddComponent<RectTransform>();
            //    }
            //    //if(spline == null)
            //    //{
            //    //    spline = gameObject.AddComponent<BezierSpline>();
            //    //}
                rect.sizeDelta = new Vector2(Camera.main.orthographicSize, Camera.main.orthographicSize);
               isFirstTime = true;
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
