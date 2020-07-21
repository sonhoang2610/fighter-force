using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
  
    public class CoreItem : MonoBehaviour
    {
        public CollectionPlayer players;
        protected PickAbleItem itemPickAble;
        private void OnEnable()
        {
            if (!itemPickAble)
            {
                itemPickAble = GetComponent<PickAbleItem>();
            }
            if (itemPickAble.icon && players.players.Length > 0)
            {
                //itemPickAble.icon = LevelManger.Instance.players
                var indexRandom = Random.Range(1, players.players.Length);
                if (!players.players[indexRandom].IsDestroyed())
                {
                    itemPickAble.Variables["id"] = players.players[indexRandom]._info.Info.ItemID;
                    var pCacheSize = itemPickAble.icon.sprite.bounds;
                    itemPickAble.icon.sprite = players.players[indexRandom]._info.Info.iconGame;
                    itemPickAble.icon.transform.localScale = new Vector3(itemPickAble.icon.transform.localScale.x * (pCacheSize.extents.x / itemPickAble.icon.sprite.bounds.extents.x), itemPickAble.icon.transform.localScale.y * (pCacheSize.extents.y / itemPickAble.icon.sprite.bounds.extents.y), 1);
                }
            }
        }
    }
}
