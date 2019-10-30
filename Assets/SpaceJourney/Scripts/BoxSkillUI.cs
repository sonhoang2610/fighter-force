using System.Collections;
using System.Collections.Generic;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public struct TradeItem
    {
        public UI2DSprite exchangeIcon;
        public UILabel quantity;


        private BaseItemGameInstanced data;

        public BaseItemGameInstanced Data { get => data; set => data = value; }

        public void setData(BaseItemGameInstanced pItem)
        {
            data = pItem;
            quantity.text = pItem.quantity.ToString();
            exchangeIcon.sprite2D = pItem.item.iconGame;
        }
    }
    [System.Serializable]
    public class TradeItemGroup 
    {
        public TradeItem[] items;
    }
    public class BoxSkillUI : BaseBox<ItemSkillUI,SkillInfoInstanced>
    {
       

        public EazyGroupTabNGUI group;
        public UILabel desChoosedSkill;
        public UILabel labelRequireLevel;
        public UIProgressBar progress;
        public bool ableUpgrade = false;
        [ShowIf("ableUpgrade")]
        public string targetShop;
        [ShowIf("ableUpgrade")]
        public List<TradeItemGroup> infoTrade;
        private int indexChoosed = 0;
        protected bool initData = false;
        public GameObject  layerAbleUpgradeSkill;
        public GameObject  layerLimitSkill;


        protected PlaneInfoConfig owner;
        public void setOwner(PlaneInfoConfig pInfo)
        {
            owner = pInfo;
        }
        public void chooseIndex(int index)
        {
            IndexChoosed = 0;
            if (index >= DataSource.Count) return;
            var pShop = LoadAssets.LoadShop(targetShop);
            var pItemShop = pShop.getInfoItem(DataSource[index].info.info.ItemID);
            DataSource[index].info.LimitUpgrade = pItemShop.limitUpgrade;
            desChoosedSkill.text = DataSource[index].info.Info.Desc;
            layerAbleUpgradeSkill.gameObject.SetActive(true);
	        labelRequireLevel.gameObject.SetActive(false);
            if (DataSource[index].info.CurrentLevelPlaneOwner <= 0 || DataSource[index].info.CurrentLevelSkill <= 0)
            {
                layerAbleUpgradeSkill.gameObject.SetActive(false);
                if(DataSource[index].info.CurrentLevelPlaneOwner < owner.info.skills[index].requireLevelUnlock)
                {
                    labelRequireLevel.gameObject.SetActive(true);
                    labelRequireLevel.text = string.Format(I2.Loc.LocalizationManager.GetTranslation("text/require_skill"), owner.info.skills[index].requireLevelUnlock);
                }
            }
            
            layerLimitSkill.gameObject.SetActive(true);
            if (layerAbleUpgradeSkill)
            {
                if (DataSource[index].currentLevel >= DataSource[index].info.LimitUpgrade)
                {
                    layerAbleUpgradeSkill.gameObject.SetActive(false);
                }
                else
                {
                    layerLimitSkill.gameObject.SetActive(false);
                }
            }
            progress.value = (float)DataSource[index].currentLevel / DataSource[index].info.LimitUpgrade;
            //infoTrade.setDa
  
      
            var pItem = pShop.getInfoItem(DataSource[index].info.Info.ItemID);
            var paymentInfos = pItem.getPrice(DataSource[index].currentLevel);
            for (int i = 0; i < infoTrade.Count; ++i)
            {
                for (int j = 0; j < infoTrade[i].items.Length; ++j)
                {
                    infoTrade[i].items[j].quantity.gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < paymentInfos.Length; ++i)
            {
                for (int j = 0; j < paymentInfos[i].Length; ++j)
                {
                    infoTrade[i].items[j].quantity.gameObject.SetActive(true);
                    infoTrade[i].items[j].quantity.text =( paymentInfos[i][j].isRequire ? I2.Loc.LocalizationManager.GetTranslation("text/require") : "" ) + " "+ StringUtils.addDotMoney( paymentInfos[i][j].quantity);
                    infoTrade[i].items[j].exchangeIcon.sprite2D = paymentInfos[i][j].item.iconShop;
                }
            }
        }
        bool planReload = false;
        public override ObservableList<SkillInfoInstanced> DataSource { get => base.DataSource; set {
                base.DataSource = value;
                if (group)
                {
                    group.GroupTab.Clear();
                    for (int i = 0; i < items.Count; ++i)
                    {
                        group.GroupTab.Add(items[i].GetComponent<EazyTabNGUI>());
                    }
                    if (gameObject.activeInHierarchy)
                    {
                        StartCoroutine(delayReloadTab(IndexChoosed));
                    }
                    else
                    {
                        planReload = true;
                    }
                }
            }
        }

        public int IndexChoosed { get => indexChoosed; set => indexChoosed = value; }

        private void OnEnable()
        {
            if (planReload)
            {
                StartCoroutine(delayReloadTab(IndexChoosed));
                planReload = false;
            }
        
        }
        public IEnumerator delayReloadTab(int pIndex)
        {
            yield return new WaitForSeconds(0.1f);
            group.reloadTabs();
            EventDelegate.Execute(group.GroupTab[pIndex].Button.onClick);
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
