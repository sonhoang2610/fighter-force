using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Space;

namespace EazyEngine.Space.UI{

	public class ShopManager : Singleton<ShopManager>
	{
		public LayerShop[] shops;
		
		public EazyGroupTabNGUI group;
        protected override void Awake()
        {
            base.Awake();
        }
        
        public void showBoxShop(string pCateGory){
			GetComponent<UIElement>().show();
			int index= 3;
			for(int i = 0 ;  i < shops.Length ; ++i){
				if(shops[i].cateGoryItemLoad.ToLower() == pCateGory.ToLower()){
					index = i;
				}
			}
			group.changeTab(index);
		}

        public void hide()
        {
            if (LevelManger.InstanceRaw)
            {
                gameObject.SetActive(false);
            }
            else
            {
                GetComponent<UIElement>().close();
            }
        }
	    // Start is called before the first frame update
	    void Start()
	    {
            StartCoroutine(init());
	    }
        IEnumerator init()
        {
            yield return new WaitForEndOfFrame();
            showBoxShop("Energy");
            EzEventManager.TriggerAssetLoaded(new TriggerLoadAsset() { name = "Main/Shop/Energy", percent = 1 });
            yield return new WaitForEndOfFrame();
            showBoxShop("Crystal");
            EzEventManager.TriggerAssetLoaded(new TriggerLoadAsset() { name = "Main/Shop/Crystal", percent = 1 });
            yield return new WaitForEndOfFrame();
            showBoxShop("Coin");
            EzEventManager.TriggerAssetLoaded(new TriggerLoadAsset() { name = "Main/Shop/Coin", percent = 1 });
            yield return new WaitForEndOfFrame();
            showBoxShop("Pack");
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(false);
            EzEventManager.TriggerAssetLoaded(new TriggerLoadAsset() { name = "Main/Shop/Pack", percent = 1 });
        }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
