using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;


namespace EazyEngine.Space.UI
{
    public class BoxInfoPlane : BaseItem<PlaneInfoConfig>, EzEventListener<UIMessEvent>
    {
        public string targetShop;
        public string targetShopUpgradeSkill;
        public UILabel namePlane;
        public BoxAbilityInfo boxAbility;
        public BoxSkillUI boxSkill;
        public UILabel des;
        public UIScrollView scroll;
        public ItemInventorySlotRequire slot;
        public UILabel[] priceLabels;
        public UILabel priceLabelsOneWay;
        public UILabel labelRequire;
	    public GameObject attachMentSorting;
        public GameObject boxRank;
        public GameObject layerAbleUpgradePlane;
        public GameObject layerLimitSkillPlane;
        public GameObject layerOneWay, layerTwoWay;
        public void OnEzEvent(UIMessEvent eventType)
        {
            if (eventType.Event.StartsWith("ChangeLanguage"))
            {
                Data = Data;
            }
        }
        
        public override PlaneInfoConfig Data { get {return base.Data;}
            set {
                bool ischange = false;
                if (base.Data.info == null || value == null || value.info.ItemID != base.Data.info.ItemID)
                {
                    ischange = true;
                }
                base.Data = value;
                if (boxRank)
                {
                    boxRank.gameObject.SetActive((value.CurrentLevel > 0));
                    boxRank.GetComponentInChildren<EazyFrameCache>().setFrameIndex(value.Rank);
                }
                if (layerAbleUpgradePlane)
                {
                    layerAbleUpgradePlane.gameObject.SetActive(true);
                    layerLimitSkillPlane.gameObject.SetActive(true);
                    if (value.currentLevel >= value.LimitUpgrade)
                    {
                        layerAbleUpgradePlane.gameObject.SetActive(false);
                    }
                    else
                    {
                        layerLimitSkillPlane.gameObject.SetActive(false);
                    }
                }
              
                namePlane.text = value.Info.displayNameItem.Value + " (" +(  value.CurrentLevel.ToString()) + ")";
                boxAbility.DataSource = value.Info.currentAbility.ToObservableList();
                des.text = value.Info.Desc;
 
                boxSkill.targetShop = targetShopUpgradeSkill;
                List<SkillInfoInstanced> pDatas = new List<SkillInfoInstanced>();
                for(int i = 0; i < value.Info.skills.Count; ++i)
                {
                    value.Info.skills[i].Info.VariableDict = value.Info.skills[i].Info.VariableDict.MergeLeft(value.Info.skills[i].skillBlackBoard);
                    if (value.UpgradeSkill.ContainsKey(value.Info.skills[i].Info.ItemID))
                    {
                        pDatas.Add(new SkillInfoInstanced()
                        {
                            currentLevel = value.UpgradeSkill[value.Info.skills[i].Info.ItemID] == 0 ? (value.Info.skills[i].isEnabled ? 1 : 0) : value.UpgradeSkill[value.Info.skills[i].Info.ItemID],
                            info = value.Info.skills[i],
                        });
                    }
                    else
                    {
                        SkillInfoInstanced pSkill = new SkillInfoInstanced()
                        {
                            currentLevel = value.Info.skills[i].isEnabled ? 1 : 0,
                            info = value.Info.skills[i],
                        };
                        pDatas.Add(pSkill);
                        value.UpgradeSkill.Add(value.Info.skills[i].Info.ItemID, value.Info.skills[i].isEnabled ? 1 : 0);
                    }
                }
                if (ischange)
                {
                    boxSkill.IndexChoosed = 0;
                }
                else
                {
                    boxSkill.IndexChoosed = indexSkillChoosed;
                }
                boxSkill.setOwner(value);


                boxSkill.DataSource = pDatas.ToObservableList();
	            if(attachMentSorting){
		            attachMentSorting.SendMessage("Reposition",SendMessageOptions.DontRequireReceiver);
	            }
                scroll.ResetPosition();
                if (!string.IsNullOrEmpty(targetShop))
                {
                    var pShop = LoadAssets.LoadShop(targetShop);
                 
                    var pItem = pShop.getInfoItem(value.info.ItemID);
                    var paymentInfos = pItem.getPrice(value.CurrentLevel + 1);
                    layerOneWay.gameObject.SetActive(false);
                    layerTwoWay.gameObject.SetActive(false);
                    if (paymentInfos.Length > 1)
                    {
                        layerTwoWay.gameObject.SetActive(true);
                        for (int i = 0; i < priceLabels.Length; ++i)
                        {
                            priceLabels[i].text = StringUtils.addDotMoney(paymentInfos[i][0].quantity);
                        }
                    }
                    else if (paymentInfos.Length == 1)
                    {
                        layerOneWay.gameObject.SetActive(true);
                        if (paymentInfos[0][0].item.categoryItem == CategoryItem.IAP)
                        {
                            var shop = LoadAssets.loadAsset<IAPSetting>("IAPSetting", "Variants/Database/");
                            priceLabelsOneWay.text = shop.getInfo(paymentInfos[0][0].item.ItemID).Price;
                        }
                        else
                        {
                            priceLabelsOneWay.text = StringUtils.addDotMoney( paymentInfos[0][0].quantity);
                        }
                    
                    }
             
                    bool pUprank = false;
                    int requireCraft = 0;
                    if(paymentInfos[0].Length > 1)
                    {
                        pUprank = true;
                        requireCraft = paymentInfos[0][1].quantity;

                    }
                    if (slot)
                    {
                        //int lastCount = 0;
                        //BaseItemGame itemToload = null;
                        //for (int i = value.info.configRank.Length - 1; i >= 0; --i)
                        //{
                        //    if (value.CurrentLevel <= value.info.configRank[i].levelUpRank)
                        //    {
                        //        lastCount = value.info.configRank[i].condition.quantityRequire;
                        //        itemToload = value.info.configRank[i].condition.craftItem;
                        //    }
                        //}
                        var pItemInfo = GameManager.Instance.Database.getComonItem(value.info.conditionUnlock.craftItem);
                        int lastRequire = 0;
                        for(int j = 0; j < value.info.configRank.Length ; ++j)
                        {
                            if(value.CurrentLevel <= value.Info.configRank[j].levelUpRank )
                            {
                                var pPayment = pItem.getPrice( value.Info.configRank[j].levelUpRank +1);
                                if(pPayment[0].Length > 1)
                                {
                                    lastRequire = pPayment[0][1].Quantity;
                                }
                             
                                break;
                            }
                    
                        }
                        if (value.CurrentLevel == 0)
                        {
                            lastRequire = value.info.conditionUnlock.quantityRequire;
                        }
                        slot.Data = new ItemRequireInfo() { item = value.info.conditionUnlock.craftItem, quantity = pItemInfo.Quantity, quantityRequire = lastRequire };
                    }
                    isInit = true;
                    // for(int i = 0; i < value.info.configRank.Length; ++i)
                    //{
                    //    if(value.CurrentLevel == value.info.configRank[i].levelUpRank )
                    //    {
                    //        pUprank = true;
                    //        requireCraft = value.info.configRank[i].condition.quantityRequire; 
                    //        break;
                    //    }
                    //}
                    // if (value.CurrentLevel == 0)
                    //{
                    //    pUprank = true;
                    //    requireCraft = value.info.conditionUnlock.quantityRequire;
                    //}
                    if (pUprank)
                    {
                       
                        labelRequire.gameObject.SetActive(true);
                        labelRequire.text = string.Format( I2.Loc.LocalizationManager.GetTranslation("text/require_more_puzzle"),requireCraft);
                    }
                    else
                    {
                        labelRequire.gameObject.SetActive(false);
                    }
                }
            }
        }
        protected int indexSkillChoosed = 0;
        protected bool isInit = false;
        public void chooseIndexskill(int index)
        {
            indexSkillChoosed = index;
        }

        private void OnEnable()
        {
            if(Data != null && isInit)
            {
                Data = Data;
            }
            EzEventManager.AddListener(this);
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
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
