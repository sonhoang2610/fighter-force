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
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
