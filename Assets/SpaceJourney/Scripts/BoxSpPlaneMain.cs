using System.Collections;
using System.Collections.Generic;
using EazyEngine.Tools;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class BoxSpPlaneMain : BoxBasePlane
    {
        public int indexBox = 0;

        public void refreshData()
        {
            List<PlaneInfoConfig> pInfoIntanceds = new List<PlaneInfoConfig>();
            for (int i = 0; i < GameManager.Instance.Database.spPlanes.Count; ++i)
            {
                if (GameManager.Instance.Database.spPlanes[i].CurrentLevel > 0)
                {

                    pInfoIntanceds.Add(GameManager.Instance.Database.spPlanes[i]);
                }
            }
            onLoadingAsync.RemoveAllListeners();
            onLoadingAsync.AddListener(delegate (float perent) {
                EzEventManager.TriggerAssetLoaded(new TriggerLoadAsset() { name = "Main/MainScene/SpPlane", percent = perent });
            });
            DataSource = pInfoIntanceds.ToObservableList();
        }
        protected override void Start()
        {
            refreshData();
            if (DataSource != null && DataSource.Count > 0)
            {
           
                for (int i = 0; i < DataSource.Count; ++i)
                {
                    if (DataSource[i].Info.ItemID == GameManager.Instance.Database.SelectedSupportPlane1)
                    {
                        currentPage = i;
                    }
                }
                Invoke(nameof(updatePage), 0.25f);
            }
            base.Start();
            MainScene.Instance.addSelectedBoxPlane(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (DataSource != null && DataSource.Count > 0)
            {
                refreshData();
                for (int i = 0; i < DataSource.Count; ++i)
                {
                    if (DataSource[i].Info.ItemID == GameManager.Instance.Database.SelectedSupportPlane1)
                    {
                        currentPage = i;
                    }
                }
                Invoke(nameof(updatePage), 0.25f);
            }

        }
        public override void setDataItem(PlaneInfoConfig pData, ItemPlaneMain pItem)
        {
            base.setDataItem(pData, pItem);
            pItem.transform.SetSiblingIndex(pItem.Index);
            pItem.attachMentObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            pItem.onLoadModelDone.RemoveListener(delayClip);
            pItem.onLoadModelDone.AddListener(delayClip);
        }
        public int refStencil;
        public override ObservableList<PlaneInfoConfig> DataSource { get => base.DataSource; set { base.DataSource = value;
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
            } }
        public int sortPlane(PlaneInfoConfig pInfo1, PlaneInfoConfig pInfo2)
        {
            return pInfo2.CurrentLevel.CompareTo(pInfo1.CurrentLevel);
        }
        public void delayClip()
        {
            StartCoroutine(delayClip1());
        }
        public IEnumerator delayClip1()
        {
            yield return new WaitForSeconds(0.25f);
            var renders = GetComponentsInChildren<RendererMaterialEdit>();
            foreach (var render in renders)
            {
                render.setEffectAmount(0, refStencil);
            }
        }
        public override void updatePage()
        {
            if (AssetLoaderManager.Instance.getJob("Main/MainScene/SpPlane").CurrentPercent < 1) return;
            if (Item.Count <= 0) return;
            base.updatePage();
            GameManager.Instance.Database.SelectedSupportPlane1 = DataSource[currentPage].Info.ItemID;
            GameManager.Instance.freeSpPlaneChoose = DataSource[currentPage].Info.ItemID;
            foreach (var item in items)
            {
                var render = item.GetComponentInChildren<RendererMaterialEdit>(true);
                if (render)
                {
                    render.setEffectAmount(0, refStencil);
                }
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
