using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EazyEngine.Tools;
using NodeCanvas.Framework;
using EazyEngine.Tools.Space;

namespace EazyEngine.Space.UI
{
    public class BoxBasePlane : BaseBox<ItemPlaneMain, PlaneInfoConfig>,EzEventListener<UIMessEvent>, EzEventListener<TriggerLoadAsset>
    {
        public string assetsIDListen;
        public bool selectOnStart = false;
        public UIButton btnNext, btnPrevious;
        public GameObject layerButton;
        public UnityEventInt onChoosePlaneIndex;
        public UnityEvent onSelected;
        public UnityEvent onDeselected;
        protected int currentPage = 0;

        protected virtual void OnEnable()
        {
            EzEventManager.AddListener<UIMessEvent>(this);
            EzEventManager.AddListener<TriggerLoadAsset>(this);
        }

         protected virtual  void OnDisable()
        {
            EzEventManager.RemoveListener<UIMessEvent>(this);
            EzEventManager.RemoveListener<TriggerLoadAsset>(this);
        }
        protected virtual void Start()
        {
            if (selectOnStart)
            {
                MainScene.Instance.addSelectedBoxPlane(this);
            }
        }
        protected int callFps = 0;
        public virtual void nextPage()
        {
            currentPage++;
            callFps++;
            SceneManager.Instance.markDirtySlowFps();
            updatePage();
        }


        public virtual void previousPage()
        {
            currentPage--;
            callFps++;
            SceneManager.Instance.markDirtySlowFps();
            updatePage();
        }
        public void onFinishFocus()
        {
            for(int i = 0; i < DataSource.Count; ++i)
            {
                if(i != currentPage)
                {
                    if (Item[DataSource[i]].gameObject.activeSelf)
                    {
                        Item[DataSource[i]].gameObject.SetActive(false);
                    }
                }
            }
            for(int i = 0; i < callFps; ++i)
            {
                SceneManager.Instance.removeDirtySlowFps();
            }
            callFps = 0;
        }
        public virtual void updatePage()
        {
            btnNext.isEnabled = true;
            btnPrevious.isEnabled = true;
            if (currentPage >= DataSource.Count - 1)
            {
                currentPage = DataSource.Count - 1;
                btnNext.isEnabled = false;
            }
            if (currentPage <= 0)
            {
                currentPage = 0;
                btnPrevious.isEnabled = false;
            }
            var pCenter = GetComponent<UICenterOnChild>();
            if(!pCenter)
            {
                pCenter = GetComponentInChildren<UICenterOnChild>();
              
            }
            pCenter.onFinished -= onFinishFocus;
            pCenter.onFinished += onFinishFocus;
            pCenter.CenterOn(Item[DataSource[currentPage]].transform);
            if (isSelected)
            {
                Item[DataSource[currentPage]].onExecute();
            }
            Item[DataSource[currentPage]].gameObject.SetActive(true);
            Item[DataSource[currentPage]].AnimatorModel?.triggerAnimator("Default");
            Item[DataSource[currentPage]].AnimatorModel?.triggerAnimator("Preview");
            onChoosePlaneIndex.Invoke(currentPage);

        }
        protected bool isSelected = false;
        public virtual void selected(bool pBool)
        {
            isSelected = pBool;
            if (layerButton)
            {
               // layerButton.gameObject.SetActive(pBool);
            }
            if (pBool)
            {
                onSelected.Invoke();
        
            }
            else
            {
                onDeselected.Invoke();
            }
            updatePage();
        }

        public void OnEzEvent(UIMessEvent eventType)
        {
            if (eventType.Event.StartsWith("ChangeLanguage"))
            {
                reloadData();
            }
        }

        public void OnEzEvent(TriggerLoadAsset eventType)
        {
            if(eventType.name == assetsIDListen && AssetLoaderManager.Instance.getJob(assetsIDListen).CurrentPercent >= 1)
            {
                Invoke("updatePage", 0.1f);
            }
        }
    }
    public class BoxPlaneMain : BoxBasePlane
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            if (DataSource!= null && DataSource.Count > 0)
            {
                refreshData();
                for (int i = 0; i < DataSource.Count; ++i)
                {
                    if (DataSource[i].Info.ItemID == GameManager.Instance.Database.SelectedMainPlane)
                    {
                        currentPage = i;
                    }
                }
                Invoke("updatePage", 0.1f);
            }
        
        }
        public override void setDataItem(PlaneInfoConfig pData, ItemPlaneMain pItem)
        {
            base.setDataItem(pData, pItem);
            pItem.transform.SetSiblingIndex(pItem.Index);
        }
        public void refreshData()
        {
            PlaneInfo[] pInfos = (PlaneInfo[])GameDatabase.Instance.getAllItem(CategoryItem.PLANE);
            List<PlaneInfoConfig> pInfoIntanceds = new List<PlaneInfoConfig>();
            for (int i = 0; i < pInfos.Length; ++i)
            {
                var pInfoConfig = GameManager.Instance.Database.getPlane(pInfos[i].ItemID);
                if (pInfoConfig == null)
                {
                    pInfoConfig = new PlaneInfoConfig() { Info = pInfos[i], currentLevel = 0 };
                    pInfoConfig.ExtraInfo();
                }
                if (pInfoConfig != null/* && pInfoConfig.currentLevel > 0*/)
                {
    
                    pInfoIntanceds.Add(pInfoConfig);
                }
            }
            pInfoIntanceds.Sort(sortPlane);
            onLoadingAsync.RemoveAllListeners();
            onLoadingAsync.AddListener(delegate (float perent) {
                EzEventManager.TriggerAssetLoaded(new TriggerLoadAsset() { name = "Main/MainScene/MainPlane", percent = perent });
            });
            DataSource = pInfoIntanceds.ToObservableList();
        }
        // Start is called before the first frame update
        protected override void Start()
        {
            // ShopDatabase.
            refreshData();
            for (int i = 0; i < DataSource.Count; ++i) {
                if(DataSource[i].Info.ItemID == GameManager.Instance.Database.SelectedMainPlane)
                {
                    currentPage = i;
                }
            }
            Invoke("updatePage", 0.1f);
            base.Start();
        }
        public override ObservableList<PlaneInfoConfig> DataSource
        {
            get => base.DataSource;
            set
            {
                base.DataSource = value;
                if (DataSource != null && DataSource.Count > 0)
                {
                    for (int i = 0; i < DataSource.Count; ++i)
                    {
                        if (DataSource[i].Info.ItemID == GameManager.Instance.Database.SelectedMainPlane)
                        {
                            currentPage = i;
                        }
                    }
                    Invoke("updatePage", 0.1f);
                }
            }
        }
        public int sortPlane(PlaneInfoConfig a, PlaneInfoConfig b)
        {
            if (a.CurrentLevel == b.CurrentLevel)
            {
                return a.Info.RankPlane.CompareTo(b.Info.RankPlane);
            }
            return b.CurrentLevel.CompareTo(a.CurrentLevel);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public override void updatePage()
        {
            if (AssetLoaderManager.Instance.getJob("Main/MainScene/MainPlane").CurrentPercent < 1) return;
            if (Item.Count <= 0) return;
            base.updatePage();
            GameManager.Instance.Database.SelectedMainPlane = DataSource[currentPage].Info.ItemID;
            GameManager.Instance.freePlaneChoose = DataSource[currentPage].Info.ItemID;
        }
    }
}
