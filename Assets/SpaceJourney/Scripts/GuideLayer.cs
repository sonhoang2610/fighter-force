using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public class GuideInfo
    {
        public string triggerToExcute;
        public bool showDialog = true;
        [ShowIf("showDialog")]
        public I2String title;
        [ShowIf("showDialog")]
        public I2String content;
        public string IDButonFocus;
        public Vector3 offset;
        [ShowIf("showDialog")]
        public Vector3 boxPos = new Vector3(0, -74, 0);
        public bool blockState;
        public bool newFeature = false;
        [ShowIf("newFeature")]
        public GameObject prefabIcon;
    }

    public struct GuideEvent
    {
        public string trigger;
        public System.Action onExcute,onSkip;
        public bool overrideButton;
        public GuideEvent(string pTrigger, System.Action pExcute = null,bool pOverride = true)
        {
            trigger = pTrigger;
            onExcute = pExcute;
            overrideButton = pOverride;
            onSkip = null;
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

        protected int markDirty = 0;
        protected  Dictionary<string,GameObject> cacheButton = new Dictionary<string, GameObject>();
        public void focusButton(string pID,Vector3 pOffset,System.Action pExcute,bool pOverride)
        {
      
            if (string.IsNullOrEmpty(pID)) return;
            var pObject = GameObject.Find("Core" + pID);
            if(pObject == null)
            {
                return;
            }
            hole.gameObject.SetActive(!string.IsNullOrEmpty(pID));
            handGuide.gameObject.SetActive(!string.IsNullOrEmpty(pID));
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
                    cacheButton.Add(pID,  pNewObject);
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
                    var pass = new EventDelegate(this,nameof(passGuide));
                    pass.parameters[0].value = pID;
                    pButtonNew.onClick.Add(pass);
                }
                else
                {
                    handGuide.gameObject.SetActive(false);
                    if (pExcute != null)
                    {
                        buttonBlack.onClick.Add(new EventDelegate(delegate { pExcute(); }));
                    }
                    var pass = new EventDelegate(this,nameof(passGuide));
                    pass.parameters[0].value = pID;
                    buttonBlack.onClick.Add(pass);
           
                }
            }
           
        }
        public void showNewFeature()
        {

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
        protected string lastState;
        private void ExcuteState(GuideInfo pInfo,System.Action pExcute,bool pOverride)
        {
            if(lastState == pInfo.triggerToExcute && blackBG.gameObject.activeSelf)
            {
                return;
            }
            lastState = pInfo.triggerToExcute;
            if (!string.IsNullOrEmpty(pInfo.IDButonFocus))
            {
                var pObject = GameObject.Find("Core" + pInfo.IDButonFocus);
                if (!pObject)
                {
                    return;
                }
            }
      
            if (LayerModelBoxExtraPackage.InstanceRaw)
            {
                if (pInfo.blockState)
                {
                    for (int i = 0; i < LayerModelBoxExtraPackage.Instance.transform.childCount; ++i)
                    {
                        var pModel = LayerModelBoxExtraPackage.Instance.transform.GetChild(i).Find("model");
                        if (pModel && pModel.gameObject.activeSelf)
                        {
                            pModel.gameObject.SetActive(false);
                        }
                    }
                }
            }
            blackBG.gameObject.SetActive(pInfo.blockState);
             if (pInfo.newFeature)
            {
               var pIntro = Instantiate(pInfo.prefabIcon, transform);
                pIntro.transform.localScale = Vector3.zero;
                Sequence pSequence = DOTween.Sequence();
                pSequence.AppendInterval(0.5f);
                var pObjectDes = GameObject.Find("Core" + pInfo.IDButonFocus);
               var pRootParrent = pObjectDes.GetComponent<UIWidget>().root;
               var pDes = pRootParrent.transform.InverseTransformPoint(pObjectDes.transform.position);
               var pFinal = transform.TransformPoint(pDes);
                pSequence.Append( pIntro.transform.DOMove(pFinal, 0.5f));
                pSequence.AppendCallback(delegate { passGuide(""); });
                Sequence pSequence1 = DOTween.Sequence();
                pSequence1.Append(pIntro.transform.DOScale(2, 0.5f).SetEase(Ease.OutElastic));
                pSequence1.AppendInterval(0.5f);
                pSequence1.Append(pIntro.transform.DOScale(1, 0.5f));
                pSequence1.AppendCallback(delegate { Destroy(pIntro); });
            }
            else
            {
                if (!string.IsNullOrEmpty(pInfo.title.Value) || !string.IsNullOrEmpty(pInfo.content.Value))
                {
                    box.transform.localPosition = pInfo.boxPos;
                    UIElementManager.Instance.cachePos[box] = pInfo.boxPos;
                    box.show();
                    title.text = pInfo.title.Value;
                    content.text = pInfo.content.Value;
                }
                else
                {
                    box.close();
                }
                focusButton(pInfo.IDButonFocus, pInfo.offset, pExcute, pOverride);
            }
            markDirty++;
 
        }

        private void passGuide(string id)
        {
            markDirty--;
            if (cacheButton.ContainsKey(id))
            {
                var objectDestroy = cacheButton[id];
                cacheButton.Remove(id);
                NGUITools.Destroy(objectDestroy);
            }
            if (markDirty <= 0)
            {
                markDirty = 0;
                blackBG.gameObject.SetActive(false);
                handGuide.gameObject.SetActive(false);
                box.close();
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
