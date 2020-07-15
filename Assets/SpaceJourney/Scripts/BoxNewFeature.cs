using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public class ConfigNewFeature
    {
        public int level;
        public I2String nameFeature;
        public GameObject iconFeature;
    }
    public class BoxNewFeature : Singleton<BoxNewFeature>
    {
        public List<ConfigNewFeature> configFeature = new List<ConfigNewFeature>();
        public UI2DSprite icon;
        public I2.Loc.Localize localize;
        public void show(int level)
        {
         
            var feature =configFeature.Find(x => x.level == level);
          
            if (feature != null)
            {
            
                if (GameManager.Instance.CheckGuide("firstFeature" + level))
                {
                    icon.transform.DestroyChildren();
                    GameManager.Instance.disableGuide("firstFeature" + level);
                    GetComponent<UIElement>().show();
                    var pObject = icon.transform.AddChild(feature.iconFeature);
                    pObject.GetComponent<RenderQueueModifier>()?.setTarget(icon);
                    localize.TermSuffix = feature.nameFeature.Value;
                    localize.OnLocalize(true);
                    pObject.transform.localScale = new Vector3(60, 60, 60);
                }
            }
            
            
        }

        public void go()
        {
            BoxResult.Instance.Home();
            GetComponent<UIElement>().close();
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
