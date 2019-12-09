using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EasyMobile;
using Random = UnityEngine.Random;
using EazyEngine.Audio;

namespace EazyEngine.Space.UI
{
    public class LayerUpgrade : Singleton<LayerUpgrade>,EzEventListener<UIMessEvent>
    {
        public string targetShop;
	    public string targetShopUpgradeSkill; 
        [SerializeField]
	    protected BoxInfoPlane boxInfo;

        public BoxChoosePlane boxplanes;
	    public GameObject effectUpgrade,effectBaokich;
        public GameObject textBaokick;
        public UIButton btnSelect;
        public AudioGroupSelector sfxUpgradePlane = AudioGroupConstrant.UpgradePlane,sfxUpgradeSkill = AudioGroupConstrant.UpgradeSkill;
        protected PlaneInfoConfig selectedPlane;
        protected SkillInfoInstanced choosedSkill;
        
        public void setDataMainPlane(PlaneInfoConfig pInfo)
        {
            selectedPlane = pInfo;
            var pShopPlane = LoadAssets.LoadShop(targetShop);
            var pItem = pShopPlane.getInfoItem(selectedPlane.info.ItemID);
            pInfo.LimitUpgrade = pItem.limitUpgrade;
	        boxInfo.Data = pInfo;
            if(pInfo.info.categoryItem == CategoryItem.SP_PLANE)
            {
                GameManager.Instance.freeSpPlaneChoose = pInfo.info.ItemID;
                if (pInfo.info.itemID == GameManager.Instance.Database.SelectedSupportPlane1)
                {
                    btnSelect.isEnabled = false;
                }
                else
                {
                    btnSelect.isEnabled = true;
                }
            }
            else
            {
                GameManager.Instance.freePlaneChoose = pInfo.info.ItemID;
                if(pInfo.info.itemID == GameManager.Instance.Database.SelectedMainPlane)
                {
                    btnSelect.isEnabled = false;
                }
                else
                {
                    btnSelect.isEnabled = true;
                }
            }
        }

        public void selectMainPlane()
        {
            var pInfo = selectedPlane;
            if (pInfo.info.categoryItem == CategoryItem.SP_PLANE)
            {
                GameManager.Instance.Database.SelectedSupportPlane1 = GameManager.Instance.freeSpPlaneChoose;
                GameManager.Instance.Database.SelectedSupportPlane2 = GameManager.Instance.freeSpPlaneChoose;
            }
            else
            {
                GameManager.Instance.Database.SelectedMainPlane = GameManager.Instance.freePlaneChoose;
            }
            btnSelect.isEnabled = false;
        }

        public void chooseSkill(object pSkill)
        {
            choosedSkill = (SkillInfoInstanced) pSkill;
        }
        public void upgradeSkill()
        {
           
            var pShopSkill = LoadAssets.LoadShop(targetShopUpgradeSkill);
            var price =  pShopSkill.getInfoItem(choosedSkill.info.Info.ItemID).getPriceFirstItem(choosedSkill.currentLevel);
            var pExist = GameManager.Instance.Database.getComonItem(price.item);
            if(pExist.quantity >= price.quantity)
            {
                SoundManager.Instance.PlaySound(sfxUpgradeSkill, Vector3.zero);
                pExist.Quantity -= price.quantity;
                if (selectedPlane.UpgradeSkill.ContainsKey(choosedSkill.info.Info.ItemID))
                {
                    selectedPlane.UpgradeSkill[choosedSkill.info.Info.ItemID]++;
                }
                else
                {
                    selectedPlane.UpgradeSkill.Add(choosedSkill.info.Info.ItemID, 1);
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
            var prices = pShopSkill.getInfoItem(choosedSkill.info.Info.ItemID).getPriceSecondItems(choosedSkill.currentLevel);
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
                        bool showButton = shop.getInfoItem(pExist1.item.ItemID) != null;
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
                SoundManager.Instance.PlaySound(sfxUpgradeSkill, Vector3.zero);
                pExist.Quantity -= price.quantity;
                if (selectedPlane.UpgradeSkill.ContainsKey(choosedSkill.info.Info.ItemID))
                {
                    selectedPlane.UpgradeSkill[choosedSkill.info.Info.ItemID]++;
                }
                else
                {
                    selectedPlane.UpgradeSkill.Add(choosedSkill.info.Info.ItemID, 1);
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
            var price = pShopPlane.getInfoItem(selectedPlane.info.ItemID).getPriceFirstItems(selectedPlane.CurrentLevel+1);
            var pExist1 = GameManager.Instance.Database.getComonItem(price[0].item);
            if (price[0].item.categoryItem ==  CategoryItem.IAP)
            {
                var shop = LoadAssets.loadAsset<IAPSetting>("IAPSetting", "Variants/Database/");
                IAPItem pIAPItem = shop.getInfo(price[0].item.ItemID);
                GameManager.Instance.showInapp(pIAPItem.Id.ToLower(), delegate (bool pSuccess, IAPProduct product)
                {
                    if (product.Id == pIAPItem.Id.ToLower())
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
                                    if (!selectedPlane.upgradeExtraAbility.ContainsKey(pAbility._ability.ItemID))
                                    {
                                        selectedPlane.upgradeExtraAbility.Add(pAbility._ability.ItemID, new int[0]);
                                    }
                                    List<int> pListInt = new List<int>();
                                    pListInt.AddRange(selectedPlane.upgradeExtraAbility[pAbility._ability.ItemID]);
                                    if (!pListInt.Contains(selectedPlane.CurrentLevel))
                                        pListInt.Add(selectedPlane.CurrentLevel);
                                    selectedPlane.upgradeExtraAbility[pAbility._ability.ItemID] = pListInt.ToArray();
                                }
                            }

                            GameManager.Instance.Database.collectionDailyInfo.upgradeMainCount++;
                            GameManager.Instance.Database.collectionDailyInfo.upgradeSpCount++;
                            SoundManager.Instance.PlaySound(sfxUpgradePlane, Vector3.zero);
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
                    textBaokick.gameObject.SetActive(false);
                    textBaokick.gameObject.SetActive(true);
                    for(int i = 0; i < selectedPlane.info.currentAbility.Count; ++i)
                    {
                        var pAbility = selectedPlane.info.currentAbility[i];
                        if (!selectedPlane.upgradeExtraAbility.ContainsKey(pAbility._ability.ItemID))
                        {
                            selectedPlane.upgradeExtraAbility.Add(pAbility._ability.ItemID, new int[0]);
                        }
                        List<int> pListInt = new List<int>();
                        pListInt.AddRange(selectedPlane.upgradeExtraAbility[pAbility._ability.ItemID]);
                        if (!pListInt.Contains(selectedPlane.CurrentLevel))
                            pListInt.Add(selectedPlane.CurrentLevel);
                        selectedPlane.upgradeExtraAbility[pAbility._ability.ItemID] = pListInt.ToArray();
                    }
                }
                StartCoroutine(delayAction(0.02f, delegate
                {
                    int pStepGame = PlayerPrefs.GetInt("firstGame", 0);
                    if (pStepGame >= 3 && pStepGame < 5)
                    {
                        EzEventManager.TriggerEvent(new GuideEvent("FirstUpgrade" + (pStepGame - 2).ToString()));
                        pStepGame++;
                        PlayerPrefs.SetInt("firstGame", pStepGame);
                    }
                     else if (pStepGame == 5)
                    {
                        EzEventManager.TriggerEvent(new GuideEvent("FirstUpgradeSuccess"));
                        pStepGame++;
                        PlayerPrefs.SetInt("firstGame", pStepGame);
                    }
                }));
                GameManager.Instance.Database.collectionDailyInfo.upgradeMainCount++;
                GameManager.Instance.Database.collectionDailyInfo.upgradeSpCount++;
                SoundManager.Instance.PlaySound(sfxUpgradePlane, Vector3.zero);
                selectedPlane.CurrentLevel++;
                selectedPlane.ExtraInfo();
                effectUpgrade.gameObject.SetActive(true);
                SoundManager.Instance.PlaySound(sfxUpgradePlane, Vector3.zero);
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
                        if (pExist1.item.categoryItem == CategoryItem.CRAFT)
                        {
                            TopLayer.Instance.boxLucky.show();
                        }
                        else
                        {
                            ShopManager.Instance.showBoxShop(pExist1.item.categoryItem.ToString());
                        }
	             
                        HUDLayer.Instance.BoxDialog.close();
                    });
                }else if(pExist2 != null)
                {
                    HUDLayer.Instance.showDialogNotEnoughMoney(pExist2.item.displayNameItem.Value, delegate
                    {
                        if (pExist2.item.categoryItem == CategoryItem.CRAFT)
                        {
                            TopLayer.Instance.boxLucky.show();
                        }
                        else
                        {
                            ShopManager.Instance.showBoxShop(pExist2.item.categoryItem.ToString());
                        }
                        HUDLayer.Instance.BoxDialog.close();
                    });
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

        public void upgradePlane1()
        {

            var pShopPlane = LoadAssets.LoadShop(targetShop);
            var price = pShopPlane.getInfoItem(selectedPlane.info.ItemID).getPriceSecondItems(selectedPlane.CurrentLevel + 1);
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
                    textBaokick.gameObject.SetActive(false);
                    textBaokick.gameObject.SetActive(true);
                    for (int i = 0; i < selectedPlane.info.currentAbility.Count; ++i)
                    {
                        var pAbility = selectedPlane.info.currentAbility[i];
                        if (!selectedPlane.upgradeExtraAbility.ContainsKey(pAbility._ability.ItemID))
                        {
                            selectedPlane.upgradeExtraAbility.Add(pAbility._ability.ItemID, new int[0]);
                        }
                        List<int> pListInt = new List<int>();
                        pListInt.AddRange(selectedPlane.upgradeExtraAbility[pAbility._ability.ItemID]);
                        if (!pListInt.Contains(selectedPlane.CurrentLevel))
                            pListInt.Add(selectedPlane.CurrentLevel);
                        selectedPlane.upgradeExtraAbility[pAbility._ability.ItemID] = pListInt.ToArray();
                    }
                }
                GameManager.Instance.Database.collectionDailyInfo.upgradeMainCount++;
                GameManager.Instance.Database.collectionDailyInfo.upgradeSpCount++;
                SoundManager.Instance.PlaySound(sfxUpgradePlane, Vector3.zero);
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
                        if (pExist1.item.categoryItem == CategoryItem.CRAFT)
                        {
                            TopLayer.Instance.boxLucky.show();
                        }
                        else
                        {
                            ShopManager.Instance.showBoxShop(pExist1.item.categoryItem.ToString());
                        }

                        HUDLayer.Instance.BoxDialog.close();
                    });
                }
                else if (pExist2 != null)
                {
                    HUDLayer.Instance.showDialogNotEnoughMoney(pExist2.item.displayNameItem.Value, delegate
                    {
                        if (pExist2.item.categoryItem == CategoryItem.CRAFT)
                        {
                            TopLayer.Instance.boxLucky.show();
                        }
                        else
                        {
                            ShopManager.Instance.showBoxShop(pExist2.item.categoryItem.ToString());
                        }
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

        public void OnEzEvent(UIMessEvent eventType)
        {
            if(eventType.Event == "ChangeTabSp")
            {
                GetComponent<EazyGroupTabNGUI>().changeTab(1);
            }
        }
    }
}
