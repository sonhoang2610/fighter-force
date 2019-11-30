using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.Events;

namespace EazyEngine.Space.UI
{
    public interface IConvertBaseItemGameInstanced<T> where T : BaseItemGameInstanced
    {
        T convertFromBaseItemGameInstanced(BaseItemGameInstanced pObject);
    }
    [System.Serializable]
    public class GameDatabaseInventoryEventUnity : UnityEvent<GameDatabaseInventoryEvent>
    {

    }
    public class ItemInventorySlotGeneric<T> : BaseItem<T>, EzEventListener<GameDatabaseInventoryEvent> where T : BaseItemGameInstanced, new()
    {
        public UI2DSprite icon;
        public UI2DSprite iconCateGory;
        public UILabel quantity;
        public UIButton btnAddMore;
        public GameObject[] behaviorScores;
        public GameObject model;
        public bool EffectOnShow = false;
        public bool autoSelfLoad = true;
        public bool isHome = false;
        public GameDatabaseInventoryEventUnity onChange;
        public UnityEvent onEffectShow;
        public ParticleHomissile particleWhenClaim;

        [ShowIf("autoSelfLoad")]
        public BaseItemGame itemToLoad;

        public List<EventDelegate> onAddMore = new List<EventDelegate>();
        protected bool isInit = false;

   
        public override T Data
        {
	        get {return base.Data;}
	        set
            {
           
             
                base.Data = value;
                icon.sprite2D = value.item.CateGoryIcon;
                quantity.text = StringUtils.addDotMoney(value.quantity);
                if (iconCateGory)
                {
                    if (value.item.categoryItem == CategoryItem.CRAFT)
                    {
                        icon.sprite2D = value.item.iconShop;
                      //  iconCateGory.gameObject.SetActive(true);
                        iconCateGory.sprite2D = value.item.CateGoryIcon;
                    }
                    else
                    {
                        iconCateGory.gameObject.SetActive(false);
                    }
                }
                calculateScore();
                isInit = true;
            }
        }
        public void setBehaviorScore(int pIndex)
        {
            foreach(var pBehavior in behaviorScores)
            {
                var monos = pBehavior.GetComponents<MonoBehaviour>();
                foreach(var mono in monos)
                {
                   var pMethod = mono.GetType().GetMethod("setBehaviorIndex");
                    if(pMethod != null)
                    {
                        pMethod.Invoke(mono,new object[] { pIndex });
                    }
                }
            }
        }

        public void calculateScore()
        {
            var pScore = Data.item.score * Data.quantity;
            if(pScore < 29999)
            {
                setBehaviorScore(0);
            }else if( pScore < 59999)
            {
                setBehaviorScore(1);
            }
            else if (pScore < 99999)
            {
                setBehaviorScore(2);
            }
            else if (pScore < 200000)
            {
                setBehaviorScore(3);
            }
            else
            {
                setBehaviorScore(4);
            }
        }
        public void clickMore()
	    {
		    ShopManager.Instance.showBoxShop(Data.item.categoryItem.ToString());
            EventDelegate.Execute(onAddMore);
        }

        public void OnEzEvent(GameDatabaseInventoryEvent eventType)
        {
            if (eventType.item.item == itemToLoad && isInit)
            {
           
                //quantity.text = eventType.item.quantity.ToString();
                if(onChange != null)
                {
                    onChange.Invoke(eventType);
                }
          
                reloadData();

            }
        }
        public virtual void reloadData()
        {
            Data = Data;
        }
       

        // Start is called before the first frame update
         protected virtual void Start()
        {
            //if (btnAddMore)
            //{
            //    btnAddMore.onClick.Clear();
            //    btnAddMore.onClick.AddRange(onAddMore);
            //}
            if (autoSelfLoad && itemToLoad)
            {
                var pDataItem = GameManager.Instance.Database.getComonItem(itemToLoad);
                if (pDataItem == null)
                {
                    pDataItem = new BaseItemGameInstanced();
                    pDataItem.quantity = 0;
                    pDataItem.item = itemToLoad;

                }
                T pData = new T();
                if (typeof(IConvertBaseItemGameInstanced<T>).IsAssignableFrom(pData.GetType()))
                {
                    Data = ((IConvertBaseItemGameInstanced<T>)pData).convertFromBaseItemGameInstanced(pDataItem);
                }else if(typeof(T) == typeof(BaseItemGameInstanced))
                {
                    Data = (T)pDataItem;
                }
            }
        }

       

         // Update is called once per frame
        void Update()
        {

        }

        protected virtual void OnEnable()
        {
            if (autoSelfLoad && itemToLoad)
            {
                var pDataItem = GameManager.Instance.Database.getComonItem(itemToLoad);
                if (pDataItem == null)
                {
                    pDataItem = new BaseItemGameInstanced();
                    pDataItem.quantity = 0;
                    pDataItem.item = itemToLoad;

                }
                T pData = new T();
                if (typeof(IConvertBaseItemGameInstanced<T>).IsAssignableFrom(pData.GetType()))
                {
                    Data = ((IConvertBaseItemGameInstanced<T>)pData).convertFromBaseItemGameInstanced(pDataItem);
                }else if(typeof(T) == typeof(BaseItemGameInstanced))
                {
                    Data = (T)pDataItem;
                }
            }
            if (autoSelfLoad)
            {
                EzEventManager.AddListener(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (autoSelfLoad)
            {
                EzEventManager.RemoveListener(this);
            }
        }
        protected Tween cacheTween;
        public Vector2 offsetParticleClaim = Vector2.zero;
        public override void show()
        {
            base.show();
            if (EffectOnShow && Dirty)
            {

                //var widget = GetComponent<UIWidget>();
                //widget.alpha = 0;
                if (cacheTween != null)
                {
                    cacheTween.Kill();
                }
                model.transform.localScale = Vector3.zero;
                var pFade = model.transform.DOScale(1, 0.5f).SetEase(Ease.OutElastic);
                Sequence pSequence = DOTween.Sequence();
                pSequence.AppendInterval(Index * 0.2f);
                pSequence.AppendCallback(delegate
                {
                    showEffect();
                    onEffectShow.Invoke();
                });
                pSequence.Append(pFade);
                pSequence.Play();
                cacheTween = pSequence;
            }
        }

        public void showEffect()
        {
            if (particleWhenClaim)
            {

                var pFinds = FindObjectsOfType<ItemInventorySlot>();
                GameObject pTarget = null;
                GameObject pTargetPlanB = null;
                foreach (var pFind in pFinds)
                {
                    if (Data != null && Data.item != null && pFind.Data != null && pFind.Data.item != null && Data.item.ItemID == pFind.Data.item.itemID && pFind.isHome)
                    {
                        pTarget = pFind.gameObject;
                        break;
                    }
                    if (Data != null && Data.item != null && pFind.Data != null && pFind.Data.item != null && pFind.Data.item.ItemID == "Coin" && pFind.isHome)
                    {
                        pTargetPlanB = pFind.gameObject;
                    }
                }
                if(pTarget == null)
                {
                    pTarget = pTargetPlanB;
                }
                if (pTarget != null)
                {

                    var pRootParentTarget = pTarget.GetComponentInParent<UIRoot>();
                    //  particleWhenClaim.GetComponent<ParticleSystemEm>
                    var pParticle = Instantiate(particleWhenClaim, pRootParentTarget.transform);
                    pParticle.gameObject.SetActive(false);
                    pParticle.gameObject.SetLayerRecursively(pRootParentTarget.gameObject.layer);
                    var pRootParent = transform.GetComponentInParent<UIRoot>();
                    pParticle.transform.localPosition = pRootParent.transform.InverseTransformPoint(transform.position);
                    pParticle.Target = pTarget.transform;
                    pParticle.GetComponent<ParticleSystemRenderer>().material.mainTexture = Data.item.iconShop.texture;
                    if(Data.quantity <= 10)
                    {
                       var e = pParticle.GetComponent<ParticleSystem>().emission;
                        e.SetBursts(new ParticleSystem.Burst[] {
                            new ParticleSystem.Burst(0, Data.quantity <= 3 ? Data.quantity : 3)
                        });
                        var pMainModule = pParticle.GetComponent<ParticleSystem>().main;
                        var pSize = pMainModule.startSize;
                        pSize.constantMin = 1.2f;
                        pSize.constantMax = 1.2f;
                        pMainModule.startSize = pSize;
                    }
                    pParticle.gameObject.SetActive(true);
                }
            }
        }
        public override int Index { get => base.Index; set {
                name = value.ToString("D2");
                base.Index = value;
            }
        }
    }
    public class ItemInventorySlot : ItemInventorySlotGeneric<BaseItemGameInstanced>
    {
    }
}
