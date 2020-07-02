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
        private UIWidget cacheItemBoom;
        public override void setDataItem(ItemGameInstanced pData, ItemButtonInGame pItem)
        {
            base.setDataItem(pData, pItem);
            if (pData.item.ItemID == "Boom")
            {
                pItem.name = "CoreItemBoomInGame";
              
                if (PlayerPrefs.GetInt(StringKeyGuide.FirstGuideItem, 0) == 0 && GameManager.Instance.ChoosedLevel ==  2 && GameManager.Instance.ChoosedHard == 0)
                {
                    cacheItemBoom = pItem.GetComponent<UIWidget>();
                    cacheItemBoom.alpha = 0;
                }
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
            if (eventType._message == StringKeyGuide.FirstGuideItem && pItem == 0 )
            {
                if (cacheItemBoom)
                {
                    cacheItemBoom.alpha = 1;
                }
                InputManager.Instance.BlockTouch = true;
                EzEventManager.TriggerEvent(new GuideEvent(StringKeyGuide.FirstGuideItem, delegate {
                    TimeKeeper.Instance.getTimer("Global").TimScale = 1;
                    InputManager.Instance.BlockTouch = false;
             
                },false));
                PlayerPrefs.SetInt(StringKeyGuide.FirstGuideItem,1);
                TimeKeeper.Instance.getTimer("Global").TimScale = 0;
            }
        }
    }
}
