using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Space.UI;
using EazyEngine.Timer;

namespace EazyEngine.Space
{

    public class BoxItemInGame : BaseBoxSingleton<BoxItemInGame,ItemButtonInGame, ItemGameInstanced>, EzEventListener<MessageGamePlayEvent>
    {
        public override void setDataItem(ItemGameInstanced pData, ItemButtonInGame pItem)
        {
            base.setDataItem(pData, pItem);
            if (pData.item.ItemID == "Boom")
            {
                pItem.name = "CoreItemBoomInGame";
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

        public void OnEzEvent(MessageGamePlayEvent eventType)
        {
            var pItem = PlayerPrefs.GetInt(StringKeyGuide.FirstGuideItem, 0);
            if (eventType._message == StringKeyGuide.FirstGuideItem && pItem == 0)
            {
                EzEventManager.TriggerEvent(new GuideEvent(StringKeyGuide.FirstGuideItem, delegate {
                    TimeKeeper.Instance.getTimer("Global").TimScale = 1;
                }));
                PlayerPrefs.SetInt(StringKeyGuide.FirstGuideItem, 1);
                TimeKeeper.Instance.getTimer("Global").TimScale = 0;
            }
        }
    }
}
