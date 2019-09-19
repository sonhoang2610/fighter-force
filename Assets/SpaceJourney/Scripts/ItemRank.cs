using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public class RankInfo
    {
        public string id;
        public string name;
        public Texture2D ava;
        public int index;
        public long level;
    }
    public class ItemRank : BaseItem<RankInfo>
    {
        public UILabel labelTop, labelName, labelLevel;
        public EazyFrameCache frameTop;
        public UITexture ava;
        public override RankInfo Data { get => base.Data; set {
                base.Data = value;
                labelTop.gameObject.SetActive(false);
                frameTop.gameObject.SetActive(false);
                if (value.index < 3)
                {
                    frameTop.gameObject.SetActive(true);
                    frameTop.setFrameIndex(value.index);
                }
                else
                {
                    labelTop.gameObject.SetActive(true);
                    labelTop.text = value.index.ToString();
                }
                labelName.text = value.name;
                labelLevel.text = value.level.ToString();
                ava.mainTexture = value.ava;
            } }
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

