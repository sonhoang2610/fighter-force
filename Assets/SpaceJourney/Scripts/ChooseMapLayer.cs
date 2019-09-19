using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EazyEngine.Space.UI
{
    public class ChooseMapLayer : BaseBox<ChooseMapBtn,ChooseMapInfo>
    {
        public GameObject lineDefault;
        public EazyGroupTabNGUI group;
        public GameObject select;

        public void chooseMap(int index)
        {
            GameManager.Instance.ChoosedLevel = index +1;
            GameManager.Instance.ChoosedHard = 0;
            select.transform.parent = Item[DataSource[index]].transform;
            select.transform.localPosition = Vector3.zero;
           // MainScene.Instance.preparePlay();
        }
        public override void Awake()
        {
            base.Awake();

            var PosDatabase = LoadAssets.loadAsset<PositionDatabase>("PositionChooseMap", "Variants/Database/");
            for (int i = 0; i < attachMent.transform.childCount; ++i)
            {
                attachMent.transform.GetChild(i).gameObject.SetActive(false);
            }
            List<ChooseMapInfo> pDatas = new List<ChooseMapInfo>();
            for (int i = 0; i < PosDatabase.pos.Length; ++i)
            {
                List<int> pStar = new List<int>();
                for (int j = 0; j < 3; j++)
                {
                    var pMissions = GameManager.Instance.container.getLevelInfo(i + 1, j).infos.Missions;
                    int pCount = 0;
                    foreach (var pMission in pMissions)
                    {
                        if (pMission.process >= 1)
                        {
                            pCount++;
                        }
                    }
                    pStar.Add(pCount);
                }

                pDatas.Add(new ChooseMapInfo() { pos = PosDatabase.pos[i].pos, star = pStar.ToArray(),isBoss = PosDatabase.pos[i].isBoss });
            }
            DataSource = pDatas.ToObservableList();
            group.GroupTab.Clear();
            for (int i = 0; i < DataSource.Count; ++i)
            {
                if (i < DataSource.Count - 1)
                {
                    GameObject pLine = Instantiate(lineDefault, attachMent.transform);
                    pLine.SetActive(true);
                    var pLineRender = pLine.GetComponent<UI2DSprite>();
                    var pDistance = Vector2.Distance(items[i].transform.localPosition, items[i + 1].transform.localPosition);
                    pLineRender.transform.localPosition = items[i].transform.localPosition;
                    //Vector3 pScale = pLineRender.transform.localScale;
                    // pScale.x = pDistance / 1024.0f;
                    pLineRender.width = (int)pDistance;
                    // pLineRender.transform.transform.localScale = pScale;
                    pLineRender.transform.RotationDirect2D(items[i + 1].transform.localPosition - items[i].transform.localPosition);
                    if(GameManager.Instance.Database.currentUnlockLevel -1 <= i)
                    {
                        pLineRender.GetComponent<EazyFrameCache>().setFrameIndex(1);
                    }
                }
                group.GroupTab.Add(items[i].GetComponent<EazyTabNGUI>());
                group.reloadTabs();
                if (GameManager.Instance.Database.lastPlayLevel == 0)
                {
                    GameManager.Instance.Database.lastPlayLevel = 1;
                }
                group.changeTab(GameManager.Instance.Database.lastPlayLevel - 1);

            }
            //GetComponentInChildren<UIScrollView>().ResetPosition();

        }
        //[ContextMenu("line")]
        //public void lineInstant()
        //{
        //    for (int i = 0; i < DataSource.Count; ++i)
        //    {
        //        if (i < DataSource.Count - 1)
        //        {
        //            GameObject pLine = Instantiate(lineDefault, attachMent.transform);
        //            pLine.SetActive(true);
        //            var pLineRender = pLine.GetComponent<UI2DSprite>();
        //            var pDistance = Vector2.Distance(items[i].transform.localPosition, items[i + 1].transform.localPosition);
        //            pLineRender.transform.localPosition = items[i].transform.localPosition;
        //            //Vector3 pScale = pLineRender.transform.localScale;
        //            // pScale.x = pDistance / 1024.0f;
        //            StartCoroutine(delaySetLinewidth(pLineRender, (int)pDistance));
        //         //   pLineRender.width = (int)pDistance;
        //            // pLineRender.transform.transform.localScale = pScale;
        //            pLineRender.transform.RotationDirect2D(items[i + 1].transform.localPosition - items[i].transform.localPosition);
        //        }
        //        //group.GroupTab.Add(items[i].GetComponent<EazyTabNGUI>());
        //        //group.reloadTabs();
        //        //if (GameManager.Instance.Database.lastPlayLevel == 0)
        //        //{
        //        //    GameManager.Instance.Database.lastPlayLevel = 1;
        //        //}
        //        //group.changeTab(GameManager.Instance.Database.lastPlayLevel - 1);

        //    }
        //}

        //public IEnumerator delaySetLinewidth(UI2DSprite pLine,int pWidth)
        //{
        //    yield return new WaitForSeconds(1);
        //    pLine.width = (int)pWidth;
        //}
        bool firstLateUpdate = true;
        UICenterOnChild cacheCenter;
        private void LateUpdate()
        {
 
            if (!firstLateUpdate) return;
            if(cacheCenter == null)
            {
                cacheCenter = GetComponentInChildren<UICenterOnChild>();
            }
            if (cacheCenter)
            {
                GetComponentInChildren<UICenterOnChild>().onFinished = delegate
                {
                    GetComponentInChildren<UICenterOnChild>().enabled = false;
                };
                if(GameManager.Instance.Database.lastPlayLevel == 0)
                {
                    GameManager.Instance.Database.lastPlayLevel = 1;
                }
                if (GameManager.Instance.Database.lastPlayLevel > 5)
                {
                    GetComponentInChildren<UICenterOnChild>().CenterOn(items[GameManager.Instance.Database.lastPlayLevel - 1].transform);
                }
                firstLateUpdate = false;
            }
        }
#if UNITY_EDITOR
        [ContextMenu("Export")]
        public void ExportPos()
        {
            var PosDatabase = LoadAssets.loadAsset<PositionDatabase>("PositionChooseMap", "Variants/Database/");

            List<ChooseMapInfoElement> poss = new List<ChooseMapInfoElement>();
            for(int i = 0; i < attachMent.transform.childCount; ++i)
            {
                poss.Add( new ChooseMapInfoElement() { pos = attachMent.transform.GetChild(i).localPosition,isBoss = attachMent.transform.GetChild(i).GetComponent<ChooseMapBtn>().isBoos } );
            }
            PosDatabase.pos = poss.ToArray();

            EditorUtility.SetDirty(PosDatabase);
            AssetDatabase.SaveAssets();

        }
#endif
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void btnBack()
        {
            MainScene.Instance.onBack();
        }
    }
}
