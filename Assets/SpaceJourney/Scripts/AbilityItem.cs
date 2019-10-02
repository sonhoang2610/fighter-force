using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space.UI
{
    public class AbilityItem : BaseItem<AbilityConfig>
    {
        public UILabel label,current;
        public UI2DSprite process;
        public override AbilityConfig Data { get => base.Data; set {
                base.Data = value;
                label.text = value._ability.displayNameItem.Value;
                current.text = StringUtils.addDotMoney( value.CurrentUnit);
                var percent = (float)value.CurrentUnit / (float)value.limitUnit;
                DOTween.To(() => process.fillAmount, x => process.fillAmount = x, percent, 0.25f);
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
