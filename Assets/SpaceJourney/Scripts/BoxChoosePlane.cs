using System.Collections;
using System.Collections.Generic;
using EazyEngine.Tools;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class BoxChoosePlane : BaseBox<ItemBtnChoosePlane,PlaneInfoConfig>
    {
        public EazyGroupTabNGUI group;
        public GameObject attachMentModel;
        public UIWidget compareRender;
        public UIButton btnNext, btnPrevious;

        protected int currentPage = 0;

        public void updatePage()
        {
            btnNext.isEnabled = true;
            btnPrevious .isEnabled = true;
            if (currentPage >= DataSource.Count-1) {
                currentPage = DataSource.Count - 1;
                btnNext.isEnabled = false;
            }
            if (currentPage <=0 )
            {
                currentPage =  0;
                btnPrevious.isEnabled = false;
            }
            group.changeTab(currentPage);
      
        }

        public void nextPage()
        {
            currentPage++;
            updatePage();
        }

        public void previousPage()
        {
            currentPage--;
            updatePage();
        }

        protected Dictionary<string, GameObject> cacheModel = new Dictionary<string, GameObject>();
        public override ObservableList<PlaneInfoConfig> DataSource { get => base.DataSource;
            set {
                bool isChange = false;
                if(value != this.DataSource)
                {
                    isChange = true;
                }
                group.GroupTab.Clear();
                for (int i = 0; i < group.GroupLayer.Count; ++i)
                {
                    group.GroupLayer[i].gameObject.SetActive(false);
                }
                group.GroupLayer.Clear();
      
                base.DataSource = value;
                if (isChange) {
                    for (int i = 0; i < items.Count; ++i)
                    {
                        group.GroupTab.Add(items[i].GetComponent<EazyTabNGUI>());
                        GameObject pObjectNew = null;
                        if (cacheModel.ContainsKey(items[i].Data.Info.ItemID))
                        {
                            pObjectNew = cacheModel[items[i].Data.Info.ItemID];
                        }
                        else
                        {
                            pObjectNew = new GameObject();
                            pObjectNew.transform.parent = attachMentModel.transform;
                            pObjectNew.transform.localScale = new Vector3(1, 1, 1);
                            pObjectNew.transform.localPosition = Vector3.zero;

                            if (!string.IsNullOrEmpty(items[i].Data.Info.modelRef.runtimeKey))
                            {
                                var pAsync = items[i].Data.Info.modelRef.loadAssetAsync<GameObject>();
                                pAsync.completed += delegate (AsyncOperation a)
                                {
                                    if (a.GetType() == typeof(ResourceRequest))
                                    {
                                        Instantiate((GameObject)((ResourceRequest)a).asset, pObjectNew.transform);
                                        pObjectNew.SetLayerRecursively(attachMentModel.layer);
                                        pObjectNew.GetComponentInChildren<RenderQueueModifier>(true)?.setTarget(compareRender);
                                    }
                                };
                            }
                            else
                            {
                                Instantiate(items[i].Data.Info.Model, pObjectNew.transform);
                            }
                            cacheModel.Add(items[i].Data.Info.ItemID, pObjectNew);

                        }
             
                
                        pObjectNew.SetLayerRecursively(attachMentModel.layer);
                        pObjectNew.GetComponentInChildren<RenderQueueModifier>(true)?.setTarget(compareRender);
                        group.GroupLayer.Add(pObjectNew.transform);
                    }
              
              
                }
                group.reloadTabs();
                for (int i = 0; i < DataSource.Count; ++i)
                {
                    if ((currentIndexTab == 0 && DataSource[i].Info.ItemID == GameManager.Instance.Database.SelectedMainPlane) || (currentIndexTab == 1 && DataSource[i].Info.ItemID == GameManager.Instance.Database.SelectedSupportPlane1))
                    {
                        currentPage = i;
                    }
                }
                updatePage();

            }
        }
        public int findIndex<T>(List<T> pList,BaseItemGame pItem) where T : PlaneInfoConfig
        {
            for(int i = 0; i < pList.Count; ++i)
            {
                if(pList[i].Info == pItem)
                {
                    return i;
                }
            }
            return -1;
        }

        protected int currentIndexTab = 0;
        public void reloadIndex(int pIndex)
        {
            currentIndexTab = pIndex;
            List<PlaneInfoConfig> pInfos = new List<PlaneInfoConfig>();
        
            if (pIndex == 0)
            {
               var pItems = GameDatabase.Instance.getAllItem(CategoryItem.PLANE);
                var pItemExists = GameManager.Instance.Database.planes;
                foreach (var pItem in pItems)
                {
                    int index = -1;
                    if ((index = findIndex(pItemExists, pItem)) >=0)
                    {
                        pInfos.Add(pItemExists[index]);
                    }
                    else
                    {
                        var pInfo =   new PlaneInfoConfig() {info = (PlaneInfo)pItem, currentLevel = 0 };
                        pInfo.ExtraInfo();
                        GameManager.Instance.Database.planes.Add(pInfo);
                        pInfos.Add(pInfo);
                    }
                }
                pInfos.Sort(sortPlane);
                DataSource = pInfos.ToObservableList();
            }
            else
            {
                var pItems = GameDatabase.Instance.getAllItem(CategoryItem.SP_PLANE);
                var pItemExists = GameManager.Instance.Database.spPlanes;
                foreach (var pItem in pItems)
                {
                    int index = -1;
                    if ((index = findIndex(pItemExists, pItem)) >= 0)
                    {
                        pInfos.Add(pItemExists[index]);
                    }
                    else
                    {
                        var pInfo = new SupportPlaneInfoConfig() { info = (PlaneInfo)pItem, currentLevel = 0 };
                        pInfo.ExtraInfo();
                        GameManager.Instance.Database.spPlanes.Add(pInfo);
                        pInfos.Add(pInfo);
                    }
                }
                pInfos.Sort(sortPlane);
                DataSource = pInfos.ToObservableList();
            }
        }
        public int sortPlane(PlaneInfoConfig a, PlaneInfoConfig b)
        {
            if(a.CurrentLevel == b.CurrentLevel)
            {
               return a.Info.RankPlane.CompareTo(b.Info.RankPlane);
            }
            return b.CurrentLevel.CompareTo(a.CurrentLevel);
        } 
        public void choosedPlane(int index)
        {

            if (index >= group.GroupLayer.Count) return;
            currentPage = index;
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
            //for(int i  = 0; i < onDataAction.Count; ++i)
            //{
            //    onDataAction[i].parameters[0].value = DataSource[index];
            //}
            //EventDelegate.Execute(onDataAction);
            LayerUpgrade.Instance.setDataMainPlane(group.GroupTab[index].GetComponent<ItemBtnChoosePlane>().Data);
            var pAni = group.GroupLayer[index].GetComponentInChildren<Animator>();
            if (!pAni) return;
            pAni.SetTrigger("Preview");
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
