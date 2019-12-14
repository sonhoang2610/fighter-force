using System.Collections;
using System.Collections.Generic;
using EazyEngine.Tools;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class BoxAbilityInfo : BaseBox<AbilityItem,AbilityConfig>
    {
        private void OnEnable()
        {
            attachMent.SendMessage("Reposition", SendMessageOptions.DontRequireReceiver);
        }
        public override void setDataItem(AbilityConfig pData, AbilityItem pItem)
        {
            base.setDataItem(pData, pItem);
            pItem.name = "a" + pItem.Index;
            //if (!pData.isDisplay)
            //{
            //    StartCoroutine(delayDeactive(pItem));
            //}
        }

        public IEnumerator delayDeactive(AbilityItem pItem)
        {
            yield return new WaitForSeconds(0.1f);
            pItem.gameObject.SetActive(false);
            attachMent.SendMessage("Reposition", SendMessageOptions.DontRequireReceiver);
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
