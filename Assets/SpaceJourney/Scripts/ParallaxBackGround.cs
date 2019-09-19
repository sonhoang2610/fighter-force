using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EazyEngine.Space
{
    public static class BoundExtend
    {

        public static Bounds getBound(this Transform pObject)
        {
            if (pObject.GetComponent<SpriteRenderer>())
            {
                return pObject.GetComponent<SpriteRenderer>().bounds;
            }
            if (pObject.GetComponent<UIWidget>())
            {
                return pObject.GetComponent<UIWidget>().CalculateBounds();
            }
            return new Bounds();
        }
    }
    [System.Serializable]
    public class ParrallaxInfo
    {
        public string tag = "";
        public float Speed = 50;

        public Dictionary<Transform, float> transformRelative = new Dictionary<Transform, float>();
        [HideInInspector]
        public float currentSpeedPercent;
        [HideInInspector]
        public float currentPercent = 0.5f;
        [HideInInspector]
        public float allViewSize;
        [HideInInspector]
        public float minY, maxY;

        public void updateElementPos()
        {
            for (int i = 0; i < transformRelative.Keys.Count; ++i)
            {
                float percentOffset = transformRelative[transformRelative.Keys.ElementAt(i)] + currentPercent;
                if(percentOffset > 1)
                {
                    percentOffset -= (int)percentOffset;
                }
                else if(percentOffset < 0)
                {
                    percentOffset += 1;
                }
                var pTrans = transformRelative.Keys.ElementAt(i);
                pTrans.localPosition = new Vector3(pTrans.localPosition.x, minY + percentOffset * allViewSize, pTrans.localPosition.z);
            }
        }
    }
    public class ParallaxBackGround : MonoBehaviour
    {

        public ParrallaxInfo[] infos;
        
        private void Awake()
        {
            for (int j = 0; j < infos.Length; ++j)
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    if(transform.GetChild(i).tag == infos[j].tag)
                    {
                        if (!infos[j].transformRelative.ContainsKey(transform.GetChild(i)))
                        {
                            infos[j].transformRelative.Add(transform.GetChild(i), 0);
                        }
                    }
                }
            }
            for (int j = 0; j < infos.Length; j++)
            {
                float allView = 0;
                float maxY = 0;
                float minY = 0;
                for (int i = 0; i < infos[j].transformRelative.Keys.Count ; ++i)
                {
        
                    var pTrans = infos[j].transformRelative.Keys.ElementAt(i);// infos[j].transformRelative[infos[j].transformRelative.Keys.ElementAt(i)];
                    if (i == 0)
                    {
                        minY = pTrans.localPosition.y - pTrans.getBound().size.y/2; 
                        maxY = pTrans.localPosition.y + pTrans.getBound().size.y / 2;
                    }
                    if (pTrans.position.y + pTrans.getBound().size.y / 2 > maxY)
                    {
                        maxY = pTrans.localPosition.y + pTrans.getBound().size.y / 2;
                    }
                    if (pTrans.position.y - pTrans.getBound().size.y / 2 < minY)
                    {
                        minY = pTrans.localPosition.y - pTrans.getBound().size.y / 2;
                    }
                    allView = maxY - minY;
           
                }
                for (int i = 0; i < infos[j].transformRelative.Keys.Count; ++i)
                {
                    var pTrans = infos[j].transformRelative.Keys.ElementAt(i);
                    float pPosY = pTrans.localPosition.y / allView;
                    infos[j].transformRelative[pTrans] =  pPosY;
                }
                infos[j].currentSpeedPercent = infos[j]. Speed / allView;
                infos[j].currentPercent =  (-minY) / (maxY - minY);
                infos[j].allViewSize = allView;
                infos[j].minY = minY;
                infos[j].maxY = maxY;
            }
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            for (int j = 0; j < infos.Length; j++)
            {
                infos[j].currentPercent -= infos[j].currentSpeedPercent * Time.deltaTime;
                if (infos[j].currentPercent < 0)
                {
                    infos[j].currentPercent += 1;
                }
                else if (infos[j].currentPercent > 1)
                {
                    infos[j].currentPercent -= 1;
                }
                infos[j].updateElementPos();
            }
  
        }
    }
}
