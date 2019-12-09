using ParadoxNotion;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class ItemPlaneMain : BaseItem<PlaneInfoConfig>
    {
	    public GameObject lockIcon;
	    public UIWidget compareRender;
        protected Transform model;
        public override PlaneInfoConfig Data {
	        get {return base.Data;}
            set
            {
                base.Data = value;
	            if (model == null && value.info.model != null)
                {
                    model = Instantiate(value.info.model.gameObject, transform).transform;
                }
            
                 lockIcon.gameObject.SetActive(!(value.CurrentLevel > 0));
                SkeletonRendererCustomMaterials custom = model.GetComponentInChildren<SkeletonRendererCustomMaterials>();
                if (custom)
                {
                    if(custom.CustomMaterialOverrides.Count> 0)
                    {
                        SkeletonRendererCustomMaterials.AtlasMaterialOverride pAt = custom.CustomMaterialOverrides[0];
                        pAt.overrideDisabled = false;
                        custom.CustomMaterialOverrides[0] = pAt;
                        custom.SetCustomMaterialOverridesPublic();
                    }
                }
	            RenderQueueModifier render = model.GetComponentInChildren<RenderQueueModifier>();
	            if(render && compareRender){
	            	render.setTarget(compareRender); var pRender = render.GetComponent<Renderer>();
                    if (pRender)
                    {
                        pRender.sortingOrder = 0;
                    }
                    
	            }
                Invoke("Trigger", 0.1f);
            }
        }

        public void Trigger()
        {
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
