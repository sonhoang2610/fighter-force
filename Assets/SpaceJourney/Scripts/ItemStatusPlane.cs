using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public class StatusInfo
    {
        public string id;
        public string[] idOverride;
        public Sprite icon;
        public float Duration { get; set; }
        public float CurrentDuration { get; set; }
    }
    public class ItemStatusPlane : BaseItem<StatusInfo>
    {
        public UI2DSprite icon;
        public UI2DSprite fill;

        protected Tween tweenDuration;
        public override StatusInfo Data { get => base.Data; set {
                base.Data = value;
                if(value.Duration < 0)
                {
                    fill.gameObject.SetActive(false);
                }
                else
                {
                    fill.gameObject.SetActive(true);
                    if (tweenDuration != null && tweenDuration.IsPlaying())
                    {
                        tweenDuration.Kill();
                    }
                    fill.fillAmount = value.CurrentDuration / value.Duration;
                    tweenDuration = DOTween.To(() => fill.fillAmount, x => fill.fillAmount = x,0, value.CurrentDuration).From(value.CurrentDuration / value.Duration);
                }

                icon.sprite2D = value.icon;
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
