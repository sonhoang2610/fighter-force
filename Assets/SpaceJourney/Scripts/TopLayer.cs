﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space.UI
{

    public class TopLayer : PersistentSingleton<TopLayer>
    {
        public UIElement boxsetting;
        public UIElement boxReward,boxShop,boxLucky;
        public GameObject hudItemLayer;
        public GameObject block;
        private GameObject loadingAds;

        public GameObject LoadingAds { get {
                if(loadingAds == null)
                {
                    loadingAds = SceneManager.Instance.loadingAds;
                }
                return loadingAds;
                    } set => loadingAds = value; }

        public void inGame(bool pBool)
        {
            if (pBool)
            {
                
                block.gameObject.SetActive(false);
            }
            hudItemLayer.gameObject.SetActive(!pBool);
      
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
