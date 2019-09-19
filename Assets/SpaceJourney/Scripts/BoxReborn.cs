using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class BoxReborn : MonoBehaviour
    {
        public string itemExchange;

        public void watchReborn()
        {
            GameManager.Instance.showRewardAds(itemExchange,delegate(bool pBool){

            });
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
