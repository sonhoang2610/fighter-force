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
        public Vector3 offset;
        public Vector3 boxPos = new Vector3(0, -74, 0);
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
        
        protected  GameObject cacheButton;
        public void focusButton(string pID,Vector3 pOffset,System.Action pExcute,bool pOverride)
        {
            hole.gameObject.SetActive(!string.IsNullOrEmpty(pID));
            handGuide.gameObject.SetActive(!string.IsNullOrEmpty(pID));
            if (string.IsNullOrEmpty(pID)) return;
            var pObject = GameObject.Find("Core" + pID);
            var buttonBlack = transform.Find("bg").GetComponent<UIButton>();
            if (pObject != null)
            {
                var pRoot = pObject.GetComponentInParent<UIRoot>();
                if (pRoot)
                {
                    var pRootHole = hole.GetComponentInParent<UIRoot>();
                    Vector2 posWorld =
                        pRootHole.transform.TransformPoint(
                            pRoot.transform.InverseTransformPoint(pObject.transform.position));
                    hole.transform.localPosition = hole.transform.parent.InverseTransformPoint(posWorld) + pOffset;
                    handGuide.transform.position = hole.transform.position;
                }
              
                var pButton = pObject.GetComponent<UIButton>();
                buttonBlack.onClick.Clear();
                if (pButton)
                {
                    var pCollider = pButton.GetComponent<BoxCollider>();
                    var pNewObject = new GameObject();
                    pNewObject.transform.parent = transform;
                    pNewObject.transform.localScale = new Vector3(1,1,1);
                    pNewObject.transform.position = hole.transform.position;
                    cacheButton = pNewObject;
                    var pCollider1 = pNewObject.AddComponent<BoxCollider>(pCollider);
                    pCollider1.center = pNewObject.transform.TransformPoint(  hole.transform.position);
                    var pWidget = pNewObject.AddComponent<UIWidget>();
                    pWidget.depth = 10;
                    var pButtonNew = pNewObject.AddComponent<UIButton>(pButton);
                    pButtonNew.onClick = new List<EventDelegate>();
                    for (int j = 0; j < pButton.onClick.Count; ++j)
                    {
                        pButtonNew.onClick.Add(pButton.onClick[j]);
                    }
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
                else
                {
                    handGuide.gameObject.SetActive(false);
                    if (pExcute != null)
                    {
                        buttonBlack.onClick.Add(new EventDelegate(delegate { pExcute(); }));
                    }
                    buttonBlack.onClick.Add(new EventDelegate(passGuide));
           
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
            box.transform.localPosition = pInfo.boxPos;
            UIElementManager.Instance.cachePos[box] = pInfo.boxPos;
            box.show();
            title.text = pInfo.title.Value;
            content.text = pInfo.content.Value;
            focusButton(pInfo.IDButonFocus,pInfo.offset,pExcute,pOverride);
        }

        private void passGuide()
        {
            if (cacheButton)
            {
                NGUITools.Destroy(cacheButton);
            }
            
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
