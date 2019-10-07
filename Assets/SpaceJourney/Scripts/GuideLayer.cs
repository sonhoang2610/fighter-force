using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;


namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public class GuideInfo
    {
        public string triggerToExcute;
        public I2String title;
        public I2String content;
        public string IDButonFocus;
        public bool blockState;
    }

    public struct GuideEvent
    {
        public string trigger;
        public System.Action onExcute;
        public bool overrideButton;
        public GuideEvent(string pTrigger, System.Action pExcute = null,bool pOverride = true)
        {
            trigger = pTrigger;
            onExcute = pExcute;
            overrideButton = pOverride;
        }
    }
    public class GuideLayer : Singleton<GuideLayer>, EzEventListener<GuideEvent>
    {

        public GuideInfo[] containerGuide;
        public SpriteRenderer blackBG;
        public GameObject hole;
        public UILabel title;
        public UILabel content;
        public UIElement box;
        public GameObject handGuide;
        public void focusButton(string pID,System.Action pExcute,bool pOverride)
        {
            hole.gameObject.SetActive(!string.IsNullOrEmpty(pID));
            handGuide.gameObject.SetActive(!string.IsNullOrEmpty(pID));
            if (string.IsNullOrEmpty(pID)) return;
            var pObject = GameObject.Find("Core" + pID);
            if (pObject != null)
            {
                var pRoot = pObject.GetComponentInParent<UIRoot>();
                if (pRoot)
                {
                    var pRootHole = hole.GetComponentInParent<UIRoot>();
                    Vector2 posWorld =
                        pRootHole.transform.TransformPoint(
                            pRoot.transform.InverseTransformPoint(pObject.transform.position));
                    hole.transform.localPosition = hole.transform.parent.InverseTransformPoint(posWorld);
                    handGuide.transform.position = hole.transform.position;
                }
              
                var pButton = pObject.GetComponent<UIButton>();
                if (pButton)
                {
                    var pCollider = pButton.GetComponent<BoxCollider>();
                    var pNewObject = new GameObject();
                    pNewObject.transform.parent = transform;
                    pNewObject.transform.localScale = new Vector3(1,1,1);
                    pNewObject.transform.position = hole.transform.position;
                   
                    pNewObject.AddComponent<BoxCollider>(pCollider);
                    pNewObject.AddComponent<UIWidget>();
                    var pButtonNew = pNewObject.AddComponent<UIButton>(pButton);
                    if (pExcute != null)
                    {
                        if (pOverride)
                        {
                            pButtonNew.onClick.Clear();
                        }

                        pButtonNew.onClick.Add(new EventDelegate(delegate { pExcute();} ));
                    }
                    pButtonNew.onClick.Add(new EventDelegate(passGuide));
                }
            }
        }

        public void OnEzEvent(GuideEvent eventType)
        {
            for (var i = 0; i < containerGuide.Length; ++i)
            {
                if (containerGuide[i].triggerToExcute == eventType.trigger)
                {
                    ExcuteState(containerGuide[i],eventType.onExcute,eventType.overrideButton);
                    return;
                }
            }
        }

        private void ExcuteState(GuideInfo pInfo,System.Action pExcute,bool pOverride)
        {
            blackBG.gameObject.SetActive(pInfo.blockState);
     
            box.show();
            title.text = pInfo.title.Value;
            content.text = pInfo.content.Value;
            focusButton(pInfo.IDButonFocus,pExcute,pOverride);
        }

        private void passGuide()
        {
            blackBG.gameObject.SetActive(false);
            handGuide.gameObject.SetActive(false);
            box.close();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            EzEventManager.AddListener(this);
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }
    }

}
