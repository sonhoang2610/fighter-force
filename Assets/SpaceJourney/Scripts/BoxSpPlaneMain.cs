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
            PlaneInfo[] pInfos = (PlaneInfo[])GameDatabase.Instance.getAllItem(CategoryItem.SP_PLANE);
            List<PlaneInfoConfig> pInfoIntanceds = new List<PlaneInfoConfig>();
            for (int i = 0; i < GameManager.Instance.Database.spPlanes.Count; ++i)
            {
                if (GameManager.Instance.Database.spPlanes[i].CurrentLevel > 0)
                {
                    pInfoIntanceds.Add(GameManager.Instance.Database.spPlanes[i]);
                }
            }
            DataSource = pInfoIntanceds.ToObservableList();
        }
        protected override void Start()
        {

            refreshData();
            for (int i = 0; i < DataSource.Count; ++i)
            {
                if (DataSource[i].Info.ItemID == GameManager.Instance.Database.SelectedSupportPlane1)
                {
                    currentPage = i;
                }
            }
            Invoke("updatePage", 0.1f);
            base.Start();
        }

        private void OnEnable()
        {
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
                Invoke("updatePage", 0.1f);
            }

        }
        
        public int refStencil;
        public override ObservableList<PlaneInfoConfig> DataSource { get => base.DataSource; set { base.DataSource = value;
                StartCoroutine(delayClip());
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
        public IEnumerator delayClip()
        {
            yield return new WaitForSeconds(0.1f);
            var renders = GetComponentsInChildren<RendererMaterialEdit>();
            foreach (var render in renders)
            {
                render.setEffectAmount(0, refStencil);
            }
        }

        public override void updatePage()
        {
            base.updatePage();
            GameManager.Instance.Database.SelectedSupportPlane1 = DataSource[currentPage].Info.ItemID;
            GameManager.Instance.freeSpPlaneChoose = DataSource[currentPage].Info.ItemID;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
