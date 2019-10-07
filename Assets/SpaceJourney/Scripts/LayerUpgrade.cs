using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EasyMobile;
using Random = UnityEngine.Random;

namespace EazyEngine.Space.UI
{
    public class LayerUpgrade : Singleton<LayerUpgrade>
    {
        public string targetShop;
	    public string targetShopUpgradeSkill; 
        [SerializeField]
	    protected BoxInfoPlane boxInfo;

        public BoxChoosePlane boxplanes;
	    public GameObject effectUpgrade,effectBaokich;
        protected PlaneInfoConfig selectedPlane;
        protected SkillInfoInstanced choosedSkill;
        
        public void setDataMainPlane(PlaneInfoConfig pInfo)
        {
            selectedPlane = pInfo;
            var pShopPlane = LoadAssets.LoadShop(targetShop);
            var pItem = pShopPlane.getInfoItem(selectedPlane.info.itemID);
            pInfo.LimitUpgrade = pItem.limitUpgrade;
	        boxInfo.Data = pInfo;
            if(pInfo.info.categoryItem == CategoryItem.SP_PLANE)
            {
                GameManager.Instance.freeSpPlaneChoose = pInfo.info.itemID;
            }
            else
            {
                GameManager.Instance.freePlaneChoose = pInfo.info.itemID;
            }
        }

        public void chooseSkill(object pSkill)
        {
            choosedSkill = (SkillInfoInstanced) pSkill;
        }
        public void upgradeSkill()
        {
           
            var pShopSkill = LoadAssets.LoadShop(targetShopUpgradeSkill);
            var price =  pShopSkill.getInfoItem(choosedSkill.info.Info.itemID).getPriceFirstItem(choosedSkill.currentLevel);
            var pExist = GameManager.Instance.Database.getComonItem(price.item);
            if(pExist.quantity >= price.quantity)
            {
                pExist.Quantity -= price.quantity;
                if (selectedPlane.UpgradeSkill.ContainsKey(choosedSkill.info.Info.itemID))
                {
                    selectedPlane.UpgradeSkill[choosedSkill.info.Info.itemID]++;
                }
                else
                {
                    selectedPlane.UpgradeSkill.Add(choosedSkill.info.Info.itemID, 1);
                }
            }
            else
            {
                HUDLayer.Instance.showDialogNotEnoughMoney(pExist.item.displayNameItem.Value, delegate
                {
	                ShopManager.Instance.showBoxShop(pExist.item.categoryItem.ToString());
                    HUDLayer.Instance.BoxDialog.close();
                });
            }
            GameManager.Instance.SaveGame();
            boxInfo.Data = boxInfo.Data;
        }
        public void upgradeSkill1()
        {
           
            var pShopSkill = LoadAssets.LoadShop(targetShopUpgradeSkill);
            var prices = pShopSkill.getInfoItem(choosedSkill.info.Info.itemID).getPriceSecondItems(choosedSkill.currentLevel);
            var price = prices[0];
            var pExist = GameManager.Instance.Database.getComonItem(price.item);
            if (pExist.quantity >= price.quantity)
            {
                if(prices.Length > 1)
                {
                    var pExist1 = GameManager.Instance.Database.getComonItem(prices[1].item);
                    if (pExist1.quantity >= prices[1].quantity)
                    {
                        if (!prices[1].isRequire)
                        {
                            pExist1.Quantity -= prices[1].quantity;
                        }
                    }
                    else
                    {
                        var shop =  LoadAssets.LoadShop("MainShop");
                        bool showButton = shop.getInfoItem(pExist1.item.itemID) != null;
                        if (showButton)
                        {
                            HUDLayer.Instance.showDialogNotEnoughMoney(pExist1.item.displayNameItem.Value, delegate
                            {
                                ShopManager.Instance.showBoxShop(pExist1.item.categoryItem.ToString());
                                HUDLayer.Instance.BoxDialog.close();
                            }, null);
                        }
                        else
                        {
                            HUDLayer.Instance.showDialogNotEnoughCantBuy(pExist1.item.displayNameItem.Value);
                        }
                 
                        return;
                    }
                }
              
                pExist.Quantity -= price.quantity;
                if (selectedPlane.UpgradeSkill.ContainsKey(choosedSkill.info.Info.itemID))
                {
                    selectedPlane.UpgradeSkill[choosedSkill.info.Info.itemID]++;
                }
                else
                {
                    selectedPlane.UpgradeSkill.Add(choosedSkill.info.Info.itemID, 1);
                }
            }
            else
            {
                HUDLayer.Instance.showDialogNotEnoughMoney(pExist.item.displayNameItem.Value, delegate
                {
	                ShopManager.Instance.showBoxShop(pExist.item.categoryItem.ToString());
                    HUDLayer.Instance.BoxDialog.close();
                });
            }
            GameManager.Instance.SaveGame();
            boxInfo.Data = boxInfo.Data;
        }

        public void upgradePlane()
        {
 
            var pShopPlane = LoadAssets.LoadShop(targetShop);
            var price = pShopPlane.getInfoItem(selectedPlane.info.itemID).getPriceFirstItems(selectedPlane.CurrentLevel+1);
            var pExist1 = GameManager.Instance.Database.getComonItem(price[0].item);
            if (price[0].item.categoryItem ==  CategoryItem.IAP)
            {
                var shop = LoadAssets.loadAsset<IAPSetting>("IAPSetting", "Variants/Database/");
                GameManager.Instance.showInapp(shop.getInfo(price[0].item.itemID).Id, delegate (bool pSuccess, IAPProduct product)
                {
                    if (product.Name == selectedPlane.info.itemID.ToLower())
                    {
                        if (pSuccess)
                        {
                            if (Random.Range(0, 100) < selectedPlane.info.upgradeRateCriticalConfig)
                            {
                                effectBaokich.gameObject.SetActive(true);
                                effectBaokich.GetComponent<ParticleSystem>().Play();
                                for (int i = 0; i < selectedPlane.info.currentAbility.Count; ++i)
                                {
                                    var pAbility = selectedPlane.info.currentAbility[i];
                                    if (!selectedPlane.upgradeExtraAbility.ContainsKey(pAbility._ability.itemID))
                                    {
                                        selectedPlane.upgradeExtraAbility.Add(pAbility._ability.itemID, new int[0]);
                                    }
                                    List<int> pListInt = new List<int>();
                                    pListInt.AddRange(selectedPlane.upgradeExtraAbility[pAbility._ability.itemID]);
                                    if (!pListInt.Contains(selectedPlane.CurrentLevel))
                                        pListInt.Add(selectedPlane.CurrentLevel);
                                    selectedPlane.upgradeExtraAbility[pAbility._ability.itemID] = pListInt.ToArray();
                                }
                            }

                            if (PlayerPrefs.GetInt("firstGame", 0) == 3)
                            {
                                EzEventManager.TriggerEvent(new GuideEvent("FirstUpgradeSuccess"));
                                PlayerPrefs.SetInt("firstGame",4);
                            }
                       
                            selectedPlane.CurrentLevel++;
                            selectedPlane.ExtraInfo();
                            effectUpgrade.gameObject.SetActive(true);
                            effectUpgrade.GetComponent<ParticleSystem>().Play();
                            GameManager.Instance.SaveGame();
                            boxInfo.Data = boxInfo.Data;
                            return;
                        }
                    }
                });
                return;
            }
  
            var pExist2 = price.Length == 2 ? GameManager.Instance.Database.getComonItem(price[1].item) : null;
            if (pExist1.quantity >= price[0].quantity)
            {
              
                if (pExist2 != null && pExist2.Quantity >= price[1].Quantity)
                {
                    pExist2.Quantity -= price[1].quantity;
       
                }
                else if(pExist2 != null)
                {
                    goto notenough;
                }
                pExist1.Quantity -= price[0].quantity;
                if (Random.Range(0,100) < selectedPlane.info.upgradeRateCriticalConfig)
                {
                    effectBaokich.gameObject.SetActive(true);
                    effectBaokich.GetComponent<ParticleSystem>().Play();
                    for(int i = 0; i < selectedPlane.info.currentAbility.Count; ++i)
                    {
                        var pAbility = selectedPlane.info.currentAbility[i];
                        if (!selectedPlane.upgradeExtraAbility.ContainsKey(pAbility._ability.itemID))
                        {
                            selectedPlane.upgradeExtraAbility.Add(pAbility._ability.itemID, new int[0]);
                        }
                        List<int> pListInt = new List<int>();
                        pListInt.AddRange(selectedPlane.upgradeExtraAbility[pAbility._ability.itemID]);
                        if (!pListInt.Contains(selectedPlane.CurrentLevel))
                            pListInt.Add(selectedPlane.CurrentLevel);
                        selectedPlane.upgradeExtraAbility[pAbility._ability.itemID] = pListInt.ToArray();
                    }
                }
	            selectedPlane.CurrentLevel++;
                selectedPlane.ExtraInfo();
                effectUpgrade.gameObject.SetActive(true);
	            effectUpgrade.GetComponent<ParticleSystem>().Play();
                GameManager.Instance.SaveGame();
                boxplanes.reloadData();
                boxInfo.Data = boxInfo.Data;
                return;
            }
           
           
                notenough:
                if (pExist1.quantity < price[0].quantity)
                {
                    HUDLayer.Instance.showDialogNotEnoughMoney(pExist1.item.displayNameItem.Value, delegate
                    {
	                    ShopManager.Instance.showBoxShop(pExist1.item.categoryItem.ToString());
                        HUDLayer.Instance.BoxDialog.close();
                    });
                }else if(pExist2 != null)
                {
                    HUDLayer.Instance.showDialogNotEnoughMoney(pExist2.item.displayNameItem.Value, delegate
                    {
	                    ShopManager.Instance.showBoxShop(pExist2.item.categoryItem.ToString());
                        HUDLayer.Instance.BoxDialog.close();
                    });
                }
            
  
        }

        private void OnEnable()
        {
      
        }

        public void upgradePlane1()
        {

            var pShopPlane = LoadAssets.LoadShop(targetShop);
            var price = pShopPlane.getInfoItem(selectedPlane.info.itemID).getPriceSecondItems(selectedPlane.CurrentLevel + 1);
            var pExist1 = GameManager.Instance.Database.getComonItem(price[0].item);
            var pExist2 = price.Length == 2 ? GameManager.Instance.Database.getComonItem(price[1].item) : null;
            if (pExist1.quantity >= price[0].quantity )
            {
         
                if (pExist2 != null && pExist2.Quantity >= price[1].Quantity)
                {
                    pExist2.Quantity -= price[1].quantity;
         
                }
                else if (pExist2 != null)
                {
                    goto notenough;
                }
                pExist1.Quantity -= price[0].quantity;
                if (Random.Range(0, 100) < selectedPlane.info.upgradeRateCriticalConfig*1.5f)
                {
                    effectBaokich.gameObject.SetActive(true);
                    effectBaokich.GetComponent<ParticleSystem>().Play();
                    for (int i = 0; i < selectedPlane.info.currentAbility.Count; ++i)
                    {
                        var pAbility = selectedPlane.info.currentAbility[i];
                        if (!selectedPlane.upgradeExtraAbility.ContainsKey(pAbility._ability.itemID))
                        {
                            selectedPlane.upgradeExtraAbility.Add(pAbility._ability.itemID, new int[0]);
                        }
                        List<int> pListInt = new List<int>();
                        pListInt.AddRange(selectedPlane.upgradeExtraAbility[pAbility._ability.itemID]);
                        if (!pListInt.Contains(selectedPlane.CurrentLevel))
                            pListInt.Add(selectedPlane.CurrentLevel);
                        selectedPlane.upgradeExtraAbility[pAbility._ability.itemID] = pListInt.ToArray();
                    }
                }
                selectedPlane.CurrentLevel++;
                selectedPlane.ExtraInfo();
                effectUpgrade.gameObject.SetActive(true);
                effectUpgrade.GetComponent<ParticleSystem>().Play();
                GameManager.Instance.SaveGame();
                boxplanes.reloadData();
                boxInfo.Data = boxInfo.Data;
                return;
            }
            notenough:
            if (pExist1.quantity < price[0].quantity)
                {
                    HUDLayer.Instance.showDialogNotEnoughMoney(pExist1.item.displayNameItem.Value, delegate
                    {
	                    ShopManager.Instance.showBoxShop(pExist1.item.categoryItem.ToString());
                        HUDLayer.Instance.BoxDialog.close();
                    });
                }
                else if (pExist2 != null)
                {
                    HUDLayer.Instance.showDialogNotEnoughMoney(pExist2.item.displayNameItem.Value, delegate
                    {
	                    ShopManager.Instance.showBoxShop(pExist2.item.categoryItem.ToString());
                        HUDLayer.Instance.BoxDialog.close();
                    });
                }
            
        }

        private IEnumerator delayAction(float pDelay, System.Action pAction)
        {
            yield return new WaitForSeconds(pDelay);
            pAction();
        }
        public void checkGuide()
        {
            StartCoroutine(delayAction(0.1f, delegate
            {
                int pStepGame = PlayerPrefs.GetInt("firstGame", 0);
                if (pStepGame == 2)
                {
                    PlayerPrefs.SetInt("firstGame", 3);
                    EzEventManager.TriggerEvent(new GuideEvent("FirstUpgrade"));
                }
            }));

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
