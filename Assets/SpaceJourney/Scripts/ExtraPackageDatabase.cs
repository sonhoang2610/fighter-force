using EasyMobile;
using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace EazyEngine.Space
{

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
        public void initIAPProduct()
        {
            for(int i = 0; i < combos.Length; ++i)
            {
                if((System.DateTime.Now - TimeExtension.UnixTimeStampToDateTime(combos[i].startTime)).TotalSeconds < combos[i].timeExp)
                {
                    loadInfoFromCombo(combos[i]);
                }
            }
        }

        public void loadInfoFromCombo(ComboPackage pCombo)
        {
            registerLayer.Add(pCombo.itemID, new List<LayerExtraPackage>());
            var hashset = new HashSet<ProductDefinition>();
            hashset.Add(new ProductDefinition(pCombo.ItemID.ToLower(), ProductType.NonConsumable));
            InAppPurchasing.StoreController.FetchAdditionalProducts(hashset, () =>
            {
                packageRegister.Add(pCombo);
                for(int i = 0; i < waitLayer.Count; ++i)
                {
                    var pLayer = waitLayer[i];
                    bool pExist = System.Array.Exists(pCombo.layers, x => x.layerID == pLayer.layerID);
                    if (pExist)
                    {
                        var pLayerInfo = System.Array.Find(pCombo.layers, x => x.layerID == pLayer.layerID);
                        if (!registerLayer[pCombo.ItemID].Contains(pLayer))
                        {
                            registerLayer[pCombo.ItemID].Add(pLayer);
                            var pObject = pLayer.gameObject.AddChild(pCombo.btnIcon);
                            pObject.transform.localPosition = pLayerInfo.position;
                        }

                    }
                }
              
            }, (a) =>
            {
                Debug.Log("load Package Extra error" + pCombo.itemID);
            });
        }

        public void RegisterThisLayer(LayerExtraPackage pLayer)
        {
            if (!waitLayer.Contains(pLayer))
            {
                waitLayer.Add(pLayer);
            }
            foreach (var pPackage in packageRegister)
            {
                 bool pExist = System.Array.Exists(pPackage.layers, x => x.layerID == pLayer.layerID);
                if (pExist)
                {
                    var pLayerInfo =  System.Array.Find(pPackage.layers, x => x.layerID == pLayer.layerID);
                    if (!registerLayer[pPackage.ItemID].Contains(pLayer))
                    {
                        registerLayer[pPackage.ItemID].Add(pLayer);
                        var pObject = pLayer.gameObject.AddChild(pPackage.btnIcon);
                        pObject.transform.localPosition = pLayerInfo.position;
                    }
                  
                }
            }
        }

        public void UnRegisterThisLayer(LayerExtraPackage pLayer)
        {
            waitLayer.Remove(pLayer);
        }


    }
}