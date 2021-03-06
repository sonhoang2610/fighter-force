﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space.UI
{

    public struct RewardEvent
    {
        public BaseItemGameInstanced item;

        public RewardEvent(BaseItemGameInstanced pItem)
        {
            item = pItem;
        }
    }
    public class BoxShowReward : Singleton<BoxShowReward>,EzEventListener<RewardEvent>,IListenerTriggerAnimator
    {
        public UILabel nameItem;
        public GameObject attachmentModel;
        public UILabel quantity;
        public UI2DSprite icon;
        public BoxExtract boxExtract;
        public GameObject layerNormal;
        public UI2DSprite compareRender;
        public AudioClip[] sfxOpen;
        protected BaseItemGame itemPackage;
        public void OnEzEvent(RewardEvent eventType)
        {
            attachmentModel.transform.DestroyChildren();
            GameObject pObjectNew = null;
            if (eventType.item.item.model)
            {
                pObjectNew = Instantiate(eventType.item.item.model, attachmentModel.transform);
            }
            else
            {
                pObjectNew = new GameObject();
                var pSprite = pObjectNew.AddComponent<SpriteRenderer>(); pSprite.sprite = eventType.item.item.iconShop;
                pObjectNew.AddComponent<RenderQueueModifier>();
                pObjectNew.transform.parent = attachmentModel.transform;
                pObjectNew.transform.localScale = new Vector3(100, 100, 100);
                pObjectNew.transform.localPosition = Vector3.zero;
            }
            pObjectNew.SetLayerRecursively(attachmentModel.layer);
            if (compareRender)
            {
                var pRender=  pObjectNew.GetComponentInChildren<RenderQueueModifier>();
                pRender.setTarget(compareRender);
            }
            nameItem.text = eventType.item.item.displayNameItem.Value;
            quantity.text = eventType.item.quantity.ToString();
            icon.sprite2D = eventType.item.item.CateGoryIcon;
            boxExtract.gameObject.SetActive(false);
            layerNormal.SetActive(false);
            if (typeof(IExtractItem).IsAssignableFrom(eventType.item.item.GetType()) && eventType.item.Quantity == 1 && ((IExtractItem)eventType.item.item).CacheExtraItemCount() > 0)
            {
                boxExtract.gameObject.SetActive(true);
                itemPackage = eventType.item.item;
                var pAnimator = pObjectNew.GetComponentInChildren<Animator>();
                if (pAnimator)
                {
                    pAnimator.SetTrigger("Extract");
                    boxExtract.DataSource = (new BaseItemGameInstanced[] { }).ToObservableList();
                }
                else
                {
                    BaseItemGameInstanced[] pItems = ((IExtractItem)itemPackage).ExtractHere(false);
                    boxExtract.DataSource = pItems.ToObservableList();
                }
            }
            else
            {
                layerNormal.SetActive(true);
            }
        }

        private void OnEnable()
        {
            EzEventManager.AddListener(this);
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool registerListen()
        {
            throw new System.NotImplementedException();
        }

        public void TriggerFromAnimator(AnimationEvent pEvent)
        {
            if(pEvent.stringParameter == "Extract" && itemPackage)
            {
                SoundManager.Instance.PlaySound(sfxOpen[Random.Range(0, sfxOpen.Length)],Vector3.zero);
                BaseItemGameInstanced[] pItems = ((IExtractItem)itemPackage).ExtractHere(false);
                boxExtract.DataSource = pItems.ToObservableList();
            }
        }
    }
}
