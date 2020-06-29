using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    [RequireComponent(typeof(Health))]
    public class DopItem : MonoBehaviour,IRespawn
    {
        protected Health _health;
        public List<PickAbleItem> itemDropOnDeath = new List<PickAbleItem>();

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.onDeath.AddListener( dropItem);
        }

        public void dropItem()
        {
            for(int i = 0; i < itemDropOnDeath.Count; ++i)
            {
                if (itemDropOnDeath[i].idItem == "Booster")
                {
                    if (LevelManger.Instance.players[0].handleWeapon.CacheLevelBooster >= 5)
                    {
                        continue;
                    }
                }
                if (itemDropOnDeath[i].idItem == "Booster6")
                {
                    if (LevelManger.Instance.players[0].handleWeapon.IsSuper)
                    {
                        continue;
                    }
                }
                if (itemDropOnDeath[i].idItem == "Shield")
                {
                    if (LevelManger.Instance.players[0]._health.invulnerable)
                    {
                        continue;
                    }
                }
                var pTimeLife = LevelManger.Instance.historyMatch.timeLifes[LevelManger.Instance.historyMatch.timeLifes.Count - 1];
                var pChar = GetComponent<Character>();
                if(itemDropOnDeath[i].idItem != "Coin")
                {
                    pTimeLife.dropItem.Add(new DropItemInfo() { nameItem = itemDropOnDeath[i].idItem, state = pChar ? pChar.CurrentLevelState : "", nameOwner = gameObject.name });
                }
                
                var pObjectItem =  ItemEnviroment.Instance.getItem(itemDropOnDeath[i].gameObject);
                pObjectItem.GetComponent<CoinEffControl>().SetInfo(transform.position);
                pObjectItem.gameObject.SetActive(true);
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

        public void onRespawn()
        {
            itemDropOnDeath.Clear();
        }
    }
}
