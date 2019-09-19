using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIEditBox : UIElement {

    public UILabel _labelText;
    public EazyStringXml _hint;
    public EazyStringXml _text;
    private UIInput _input;

    public UIInput Input
    {
        get
        {
            return _input == null ? _input = GetComponent<UIInput>() : _input;
        }

        //set
        //{
        //    _input = value;
        //}
    }
   
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        DoUpdate();
	}
    
    public void setHint(string str,bool pXml)
    {
        _hint.Xml = pXml;
        _hint.ez_str = str;
        GetComponent<UIInput>().defaultText = _hint.ez_str;
    }

    public void setHint(string str)
    {
        GetComponent<UIInput>().defaultText = str;
    }

    public void setText(string str)
    {
        GetComponent<UIInput>().value = str;
    }
    private void OnValidate()
    {
        GetComponent<UIInput>().defaultText = _hint.ez_str;
        GetComponent<UIInput>().value = _text.ez_str;
    }

    //protected override void DoUpdate()
    //{
    //    if (!Application.isPlaying)
    //    {
    //        if (_labelText)
    //        {
    //            _labelText.text = _hint.ez_str;
    //        }
    //    }
    //}
}
