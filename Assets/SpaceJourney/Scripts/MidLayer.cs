using System.Collections;
using System.Collections.Generic;
using EazyEngine.Tools;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class MidLayer : PersistentSingleton<MidLayer>
    {
        public BoxDailyGift boxDailyGift;

        public UIElement boxLucky;
        public UIElement boxShop;
        public UIElement BoxGiftOnline;
        public UIElement BoxRank;
        public void showBoxDailyGift()
        {
            boxLucky.show();
        }

        public void showBoxLucky()
        {
            boxLucky.show();
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

