using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EazyEngine.Space.UI
{
    public class BoxDialog : UIElement
    {

        public UILabel content;
        public UILabel title;
        public UIButton btn1, btn2;
        public UnityAction onButton1, onButton2;

        public string Title
        {
            set
            {
                title.text = value;
            }
        }

        public string Content
        {
            set
            {
                content.text = value;
            }
        }
        public void onButtonFunc1()
        {
            if(onButton1 == null)
            {
                close();
            }
            else
            {
                onButton1();
            }
        }
        public void onButtonFunc2()
        {
            if (onButton2 == null)
            {
                close();
            }
            else
            {
                onButton2();
            }
        }

        public BoxDialog disableButton(bool pBool)
        {
            btn1.gameObject.SetActive(!pBool);
            btn2.gameObject.SetActive(!pBool);
            return this;
        }

        public void setButton1Info(ButtonInfo pInfo)
        {
            if(pInfo == null)
            {
                onButton1 = null;
                return;
            }
            onButton1 = pInfo.action;
            btn1.GetComponentInChildren<UILabel>().text = pInfo.Value;
        }

        public void setButton2Info(ButtonInfo pInfo)
        {
            if (pInfo == null)
            {
                onButton2 = null;
                return;
            }
            onButton2 = pInfo.action;
            btn2.GetComponentInChildren<UILabel>().text = pInfo.Value;
        }
        private void Awake()
        {
            btn1.onClick.Add(new EventDelegate(onButtonFunc1));
            btn2.onClick.Add(new EventDelegate(onButtonFunc2));
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
