﻿using ParadoxNotion;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EazyEngine.Space.UI
{
    public class ItemPlaneMain : BaseItem<PlaneInfoConfig>
    {
	    public GameObject lockIcon;
	    public UIWidget compareRender;
        [System.NonSerialized]
        public GameObject attachMentObject;

        public UnityEvent onLoadModelDone;
        public Animator AnimatorModel
        {
            get
            {
                return model ? model.GetComponentInChildren<Animator>() : null;
            }
          
        }
        protected Transform model;
        public override PlaneInfoConfig Data {
	        get {return base.Data;}
            set
            {
                base.Data = value;
                if (attachMentObject == null) {
                    attachMentObject = gameObject.AddWidget<UIWidget>().gameObject;
                }
                if (model == null && value.info.Model != null)
                {
                 
                    model = Instantiate(value.info.Model.gameObject, attachMentObject.transform).transform;
                    model.gameObject.SetActive(false);
                }
            
                 lockIcon.gameObject.SetActive(!(value.CurrentLevel > 0));
                if (model)
                {
                    SkeletonRendererCustomMaterials custom = model.GetComponentInChildren<SkeletonRendererCustomMaterials>();
                    if (custom)
                    {
                        if (custom.CustomMaterialOverrides.Count > 0)
                        {
                            SkeletonRendererCustomMaterials.AtlasMaterialOverride pAt = custom.CustomMaterialOverrides[0];
                            pAt.overrideDisabled = false;
                            custom.CustomMaterialOverrides[0] = pAt;
                            custom.SetCustomMaterialOverridesPublic();
                        }
                    }
                    RenderQueueModifier render = model.GetComponentInChildren<RenderQueueModifier>();
                    if (render && compareRender)
                    {
                        render.setTarget(compareRender); var pRender = render.GetComponent<Renderer>();
                        if (pRender)
                        {
                            pRender.sortingOrder = 0;
                        }

                    }
                    if (!model.gameObject.activeSelf)
                    {
                        model.gameObject.SetActive(true);
                    }
                    Invoke("Trigger", 0.1f);
                 
                }
                else
                if(!string.IsNullOrEmpty(Data.Info.modelRef.runtimeKey))
                {
                    var pAsync = Data.Info.modelRef.loadAssetAsync<GameObject>();
                    pAsync.completed += delegate (AsyncOperation a){
                        if(a.GetType() == typeof(ResourceRequest))
                        {
                            value.info.Model = (GameObject)((ResourceRequest)a).asset;
                            Data = Data;
                        }
                        onLoadModelDone?.Invoke();
                    };
                }
            }
        }

        public void Trigger()
        {
            onLoadModelDone?.Invoke();
            var pModel = model.Find("model");
            if (pModel)
            {
                var pFlow = pModel. GetComponent<FlowCanvas.FlowScriptController>();
                if (pFlow)
                {
                    pFlow.SendEvent(new EventData<bool>("Enable", Data.CurrentLevel > 0), pFlow);
                }
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
    }
}
