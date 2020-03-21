using EasyMobile;
using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using EazyEngine.Space.UI;
using System.Linq;
using ParadoxNotion;

namespace EazyEngine.Space
{
    [System.Serializable]
    public struct CoundDownEvent
    {
        public string id;
        public int time;
    }
    public struct EventResultShowPackage
    {
        public Action<bool> result;
    }
    [CreateAssetMenu(fileName = "ExtraPackageDatabase", menuName = "EazyEngine/Space/ExtraPackage/ExtraPackageDatabase")]
    public class ExtraPackageDatabase : EzScriptTableObjectSingleton<ExtraPackageDatabase>
    {
        public ComboPackage[] combos;
        [System.NonSerialized]
        public List<ComboPackage> packageRegister = new List<ComboPackage>();
        [System.NonSerialized]
        public Dictionary<string, List<LayerExtraPackage>> registerLayer = new Dictionary<string, List<LayerExtraPackage>>();
        [System.NonSerialized]
        public List<LayerExtraPackage> waitLayer = new List<LayerExtraPackage>();
        [System.NonSerialized]
        public bool isInit = false;
        [ShowInInspector]
        [InlineEditor]
        public IAPSetting iapCache;

        [ShowInInspector]
        [InlineEditor]
        public ShopDatabase shop;
        public void executePackage(ComboPackage pPack)
        {
            if (pPack.isInApp)
            {
                GameManager.Instance.showInapp(pPack.ItemID.ToLower(), delegate (bool pSuccess, IAPProduct product)
                {
                    if (product.Id == pPack.ItemID.ToLower())
                    {
                        if (pSuccess)
                        {
                            claim(pPack);
                        }
                        else
                        {
                            Debug.Log("Failed Buy");
                        }
                    }
                });
            }
            else
            {
                if(GameManager.Instance.Database.PackageInfo[pPack.ItemID].status == StatusPackage.WAIT_CLAIM)
                {
                    claim(pPack);
                }
                else
                {
                    ShopManager.Instance.showBoxShop("Crystal");
                }
              
            }
        }
        public void claim(ItemPackage pPackage)
        {
            TopLayer.Instance.boxReward.show();
            BaseItemGameInstanced[] pItems = ((IExtractItem)pPackage).ExtractHere();
            foreach (var pItemAdd in pItems)
            {
                var pCheckStorage = GameManager.Instance.Database.getComonItem(pItemAdd.item);
                pCheckStorage.Quantity += pItemAdd.Quantity;
            }
            EzEventManager.TriggerEvent(new RewardEvent(new BaseItemGameInstanced() { quantity = 1, item = pPackage }));

            var pInfoPack = GameManager.Instance.Database.PackageInfo[pPackage.ItemID];
            pInfoPack.status = StatusPackage.CLAIMED;
            GameManager.Instance.Database.PackageInfo[pPackage.ItemID] = pInfoPack;
            GameManager.Instance.SaveGame();
            removePackage(pPackage);

        }
        public void removePackage(ItemPackage pPackage)
        {
            if (((ComboPackage)pPackage).isInApp)
            {
                var pItem = shop.getInfoItem(pPackage.ItemID);
                if (pItem != null)
                {
                    pItem.isVisible = false;
                    var pShop = FindObjectOfType<LayerShop>();
                    if (pShop && pShop.gameObject.activeSelf)
                    {
                        pShop.gameObject.SetActive(false);
                        pShop.gameObject.SetActive(true);
                    }
                }

            }

            if (registerLayer.ContainsKey(pPackage.itemID))
            {
                foreach (var pLayer in registerLayer[pPackage.itemID])
                {
                    pLayer.detachObject(pPackage.ItemID);
                }
            }
            var pBanner = LayerModelBoxExtraPackage.Instance.transform.Find(pPackage.itemID);
            if (pBanner)
            {
                Destroy(pBanner.gameObject);
            }
            packageRegister.Remove((ComboPackage)pPackage);
            assignBtnLayer();
        }
        public IEnumerator initIAPProduct()
        {
            isInit = true;
            for (int i = 0; i < combos.Length; ++i)
            {
                if (GameManager.Instance.Database.PackageInfo.ContainsKey(combos[i].ItemID))
                {

                    var pPackInfo = GameManager.Instance.Database.PackageInfo[combos[i].ItemID];
                    if (pPackInfo.status == StatusPackage.START || pPackInfo.status == StatusPackage.WAIT_CLAIM)
                    {
                        loadInfoFromCombo(combos[i]);
                    }
                }
                else
                {
                    var pCombo = combos[i];
                    var pObject = Instantiate<GameObject>(combos[i].conditionAnchorObject);
                    pObject.GetComponent<IBlackboard>().SetValue("result", new EventResultShowPackage()
                    {
                        result = delegate (bool pResult)
                        {
                            if (pResult)
                            {
                               // 
                                loadInfoFromCombo(pCombo);
                                GameManager.Instance.Database.PackageInfo.Add(pCombo.ItemID, new PackageInfo()
                                {
                                    status = StatusPackage.START,
                                    timeStart = TimeExtension.ToUnixTime(System.DateTime.Now)
                                });
                            }
                            Destroy(pObject);
                        }
                    });

                }

            }
            assignBtnLayer();
            yield return null;
        }
        public void completeCombo(string pID)
        {
            var pPack = packageRegister.Find(x => x.ItemID == pID);
            if (GameManager.Instance.Database.PackageInfo.ContainsKey(pID))
            {
                var pResult = GameManager.Instance.Database.PackageInfo[pID];
                pResult.status = StatusPackage.WAIT_CLAIM;
                GameManager.Instance.Database.PackageInfo[pID] = pResult;
            }
        }
        public void loadInfoFromCombo(ComboPackage pCombo)
        {
            registerLayer.Add(pCombo.itemID, new List<LayerExtraPackage>());
            if (pCombo.isInApp)
            {
                if (iapCache == null)
                {
                    iapCache = LoadAssets.loadAssetScripTableObject<IAPSetting>("IAPSetting", "Variants/Database/", true);
                }
                if (!System.Array.Exists(iapCache.items, x => x.Id == pCombo.ItemID))
                {
                    System.Array.Resize(ref iapCache.items, iapCache.items.Length + 1);
                    iapCache.items[iapCache.items.Length - 1] = new IAPItem()
                    {
                        name = pCombo.displayNameItem.Value,
                        id = pCombo.ItemID,
                        des = pCombo.descriptionItem.Value,
                        isCustom = true,
                        price = pCombo.price.ToString() + "$"
                    };
                }
                shop = LoadAssets.LoadShop("MainShop");
                if (!System.Array.Exists(shop.items, x => x.itemSell.ItemID == pCombo.ItemID))
                {
                    System.Array.Resize(ref shop.items, shop.items.Length + 1);
                    shop.items[shop.items.Length - 1] = new ShopItemInfo()
                    {
                        itemSell = pCombo,
                        isVisible = true,
                        idCategoryInShop = "Pack"
                    };
                    var pItemExchange = LoadAssets.loadAsset<BaseItemGame>("IAP", "Variants/Database/Mission/ItemCraft/");
                    shop.items[shop.items.Length - 1].priceDefines[0].unit[0].exchangeItems = new BaseItemGameInstanced() { item = pItemExchange, quantity = 1 };
                }
            }
            packageRegister.Add(pCombo);

        }
        public void assignBtnLayer()
        {
            packageRegister.Sort((x1, x2) => x2.scoreOrder.CompareTo(x1.scoreOrder));
            for (int i = 0; i < waitLayer.Count; ++i)
            {
                var pLayer = waitLayer[i];
                foreach (var pCombo in packageRegister)
                {
                    bool pExist = System.Array.Exists(pCombo.layers, x => x.layerID == pLayer.layerID);
                    if (pExist)
                    {
                        var pLayerInfo = System.Array.Find(pCombo.layers, x => x.layerID == pLayer.layerID);
                        if (pLayer.listBtn.Count  == pLayer.maxSlot)
                        {
                            continue;
                        }

                        if (!registerLayer[pCombo.ItemID].Contains(pLayer))
                        {
                            registerLayer[pCombo.ItemID].Add(pLayer);
                            var pObject = pLayer.gameObject.AddChild(pCombo.btnIcon);
                            pObject.name = pCombo.ItemID;
                            pLayer.listBtn.Add(pObject);
                            var pBox = LayerModelBoxExtraPackage.Instance.transform.Find(pCombo.ItemID) ? LayerModelBoxExtraPackage.Instance.transform.Find(pCombo.ItemID).gameObject : null;
                            if(!pBox)pBox = LayerModelBoxExtraPackage.Instance.gameObject.AddChild(pCombo.banner);
                           var pPrices = pBox.transform.GetComponentsInChildren<UILabel>(true);
                            foreach(var pPrice in pPrices)
                            {
                               if( pPrice.name == "price")
                                {
                                    pPrice.text = pCombo.price + "$";
                                }
                            }
                            pBox.name = pCombo.ItemID;
                            pBox.GetComponent<Blackboard>().SetValue("info", pCombo);
                            pObject.GetComponent<Blackboard>().SetValue("info", pCombo);
                            pObject.GetComponentInChildren<UIButton>().onClick.Add(new EventDelegate(delegate ()
                            {
                                pBox.GetComponent<FlowCanvas.FlowScriptController>().SendEvent("Show");
                            }));
                        }

                    }
                }
                pLayer.sortPos();
            }
        }
        public void RegisterThisLayer(LayerExtraPackage pLayer)
        {
            if (!waitLayer.Contains(pLayer))
            {
                waitLayer.Add(pLayer);
            }
            assignBtnLayer();
        }
        public void UnRegisterThisLayer(LayerExtraPackage pLayer)
        {
            waitLayer.Remove(pLayer);
            for(int i = 0; i < registerLayer.Values.Count; ++i)
            {
                var pList = registerLayer.Values.ElementAt(i);
                pList.Remove(pLayer);
            }
            assignBtnLayer();
        }
        [System.NonSerialized]
        GraphOwner graph;
        public void Update()
        {
            bool dirty = false;
            for(int i = packageRegister.Count-1; i >= 0; --i)
            {
                if (GameManager.Instance.Database.PackageInfo.ContainsKey(packageRegister[i].ItemID))
                {
                  var pInfo =  GameManager.Instance.Database.PackageInfo[packageRegister[i].ItemID];
                    if (pInfo.status == StatusPackage.START)
                    {
                        var pDatTime = TimeExtension.UnixTimeStampToDateTime(pInfo.timeStart);
                        var pTiming = (System.DateTime.Now - pDatTime).TotalSeconds;
                        if (!graph)
                        {
                            graph = GameManager.Instance.GetComponent<GraphOwner>();
                        }
                        Graph.SendGlobalEvent(new EventData<CoundDownEvent>("CountDown", new CoundDownEvent()
                        {
                            id = packageRegister[i].ItemID,
                            time = (int)packageRegister[i].timeExp- (int)pTiming
                        }),null);
                        if (pTiming >= packageRegister[i].timeExp)
                        {
                            pInfo.timeEnd = TimeExtension.ToUnixTime(System.DateTime.Now);
                            pInfo.status = StatusPackage.TIMEOUT;
                            GameManager.Instance.Database.PackageInfo[packageRegister[i].ItemID] = pInfo;
                            removePackage(packageRegister[i]);
                            dirty = true;
                        }
                    }
                }
            }
            if (dirty)
            {
                GameManager.Instance.SaveGame();
            }
        }
    }
}