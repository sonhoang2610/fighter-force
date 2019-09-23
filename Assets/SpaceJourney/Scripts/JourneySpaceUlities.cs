using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Space;
using NodeCanvas.Framework;
using System.Linq;

namespace EazyEngine.Tools.Space
{
    public static class JourneySpaceUlities 
    {
        public static void PassVariables(Blackboard pBlackBoardOriginal,Blackboard pBlackBoardClone)
        {
            for(int i = 0; i < pBlackBoardOriginal.variables.Count; ++i)
            {
                if (!pBlackBoardClone.variables.ContainsKey(pBlackBoardOriginal.variables.ElementAt(i).Key))
                {
                    pBlackBoardClone.AddVariable(pBlackBoardOriginal.variables.ElementAt(i).Key, pBlackBoardOriginal.variables.ElementAt(i).Value);
                }
            }
        }
        public static Vector2[] posArray(Vector2 sizeDomain,Vector2 cellgrid, int pQuantity)
        {
            int pCol =(int)( sizeDomain.x / cellgrid.x);
            int pRow = (int)(sizeDomain.y / cellgrid.y);
            List<Vector2> pAll = new List<Vector2>();
            for(int i = 0; i < pCol; ++i)
            {
                for(int j  = 0; j < pRow; ++j)
                {
                    pAll.Add(new Vector2(i, j));
                }
            }
            while(pAll.Count > pQuantity)
            {
                pAll.RemoveAt(Random.Range(0, pAll.Count));
            }
            for(int i= 0; i < pAll.Count; ++i)
            {
                Vector2 pCell = cellgrid / 2;
                pAll[i] *= cellgrid + new Vector2(cellgrid.x / 2, 0) + new Vector2( cellgrid.x/2,0)*((int)pAll[i].y%1);
               // pAll[i] += new Vector2(Random.Range(-pCell.x / 2, pCell.x/2), Random.Range(-pCell.y / 2, pCell.y/2));
            }
            return pAll.ToArray();
        }
        public static Rect Rect(this Camera pCam,float pScale = 1.0f)
        {
            float pWidth = 10.8f;// Screen.width * (pCam.orthographicSize * 2) / Screen.height;
            float pHeight = 19.2f;// LevelManger.Instance.mainPlayCamera.orthographicSize * 2;
            return new Rect((Vector2)pCam.transform.position - (new Vector2(pWidth, pHeight))* pScale / 2, new Vector2(pWidth, pHeight) * pScale);
        }
        public static int getRowCountTriangle(int pTotalQuantity)
        {
            int pRowCount = 0;
            int pLimitCol = 1;
            int counterCol = 0;
            while(pTotalQuantity > 0)
            {
                counterCol++;
                pTotalQuantity--;
                if(counterCol >= pLimitCol)
                {
                    pRowCount++;
                    counterCol = 0;
                    pLimitCol++;
                }
            }
            return pRowCount;
        }

        public static bool checkDistance(List<Vector2[]> pVecs,Vector2 pVec,float pDistance)
        {
            for(int i = 0; i < pVecs.Count; ++i)
            {
                for(int j  = 0; j < pVecs[i].Length; ++j)
                {
                    if(Vector2.Distance( pVecs[i][j], pVec) <= pDistance)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static Vector2[][] getPointFormat(FormationInfo pInfo,int pQuantityElement)
        {
            List<Vector2[]> arrayResult = new List<Vector2[]>();
            int pTotalQuantity = pQuantityElement;
            int counterCol = 0,counterRow = 0;
            int limitCol = 1;
            int limitRow = 1;
            if(pInfo.typeFomation == Formations.DomainRandom)
            {

                limitCol =(int)( pInfo.sizeDomain.x / pInfo.distanceEmiter.x);
                limitRow = (int)(pInfo.sizeDomain.y / pInfo.distanceEmiter.y);
            }
            List<Vector2> currentRow = null;
            List<Vector2> listCell = new List<Vector2>();
            //int pMaxRow = getRowCountTriangle(pQuantityElement);
            for (int i = 0; i < pQuantityElement; ++i)
            {
                if (pInfo.typeFomation == Formations.None)
                {
                    arrayResult.Add(new Vector2[] { Vector2.zero });
                }
                if (pInfo.typeFomation == Formations.DomainRandom)
                {
                    Vector2 pRandomVec = new Vector2(Random.Range(0, limitCol) , Random.Range(0, limitRow) );
                    int pRandomIndex = 0;
                    while (listCell.Contains(pRandomVec) && pRandomIndex < 100)
                    {
                        pRandomVec = new Vector2(Random.Range(0, limitCol), Random.Range(0, limitRow) );
                        pRandomIndex++;
                    }
                    listCell.Add(pRandomVec);
                    arrayResult.Add(new Vector2[] { new Vector2(- pInfo.distanceEmiter.x*(float)limitCol/2 + (pRandomVec.x +0.5f)*pInfo.distanceEmiter.x, (pInfo.distanceEmiter.y) * (float)limitRow / 2 - (pRandomVec.y + 0.5f) * pInfo.distanceEmiter.y) });
                }
                if (pInfo.typeFomation == Formations.Grid)
                {

                    if (currentRow == null)
                    {
                        currentRow = new List<Vector2>();
                        arrayResult.Add(currentRow.ToArray());
                    }
                    Vector2 pStartPos =  - new Vector2((pInfo.maxCollum - 1) * pInfo.distanceEmiter.x / 2, 0);
                    if (!pInfo.AxisX)
                    {
                        currentRow.Add(new Vector2((i % pInfo.maxCollum) * pInfo.distanceEmiter.x + pStartPos.x, pStartPos.y - pInfo.distanceEmiter.y * (i / pInfo.maxCollum)));
                    }
                    else
                    {
                        currentRow.Add(new Vector2( pStartPos.y - pInfo.distanceEmiter.y * (i / pInfo.maxCollum), (i % pInfo.maxCollum) * pInfo.distanceEmiter.x + pStartPos.x));
                    }
                    counterCol++;
                    arrayResult[arrayResult.Count - 1] = currentRow.ToArray();
                    if ((i+1) / pInfo.maxCollum != counterRow)
                    {
                     
                        currentRow = null;
                        counterCol = 0;
                    }

                }
                if (pInfo.typeFomation == Formations.Circle)
                {
                    Vector2 pCenter =Vector2.down * pInfo.radius;
                    arrayResult.Add(new Vector2[] { pCenter.Rotate2DAround(pCenter, pInfo.radius, (360.0f / (float)pQuantityElement) * i) });
                }
                if (pInfo.typeFomation == Formations.Triangle)
                {
                    if (currentRow == null )
                    {
                        currentRow = new List<Vector2>();
                        arrayResult.Add(currentRow.ToArray());
                    }
                    Vector2 pCenter = new Vector2(0, 0);
                    pCenter += new Vector2(-(limitCol-1) * pInfo.distanceEmiter.x / 2 + counterCol * pInfo.distanceEmiter.x, -counterRow * pInfo.distanceEmiter.y);

                    currentRow.Add( pCenter);
                    counterCol++;
                
                    arrayResult[arrayResult.Count-1] = (currentRow.ToArray());
                    if (counterCol >= limitCol)
                    {
                        counterRow++;
                        counterCol = 0;
                        limitCol += 1;
                        currentRow = null;
                    }
              
                }
                if (pInfo.typeFomation == Formations.Diamond)
                {
                    Vector2 pCenter = Vector2.down * pInfo.radius;
                    arrayResult.Add(new Vector2[] { pCenter.Rotate2DAround(pCenter, pInfo.radius, (360.0f / (float)pQuantityElement) * i) });

                }
            }
          
            return arrayResult.ToArray();
        }
   
        public static GameObject[][] changeFormatArray(this GameObject[] pArrayObject, Vector2[][] pArrayPos,bool ismanual = false)
        {
            int pCounter = 0;
            List<GameObject[]> pArrayObjectResult = new List<GameObject[]>();
            for (int i = 0; i < pArrayPos.Length; ++i)
            {
                GameObject[] pObjets = new GameObject[pArrayPos[i].Length];
                for (int j = 0; j < pArrayPos[i].Length; ++j)
                {
                    pObjets[j] = pArrayObject[pCounter];
                    if (!ismanual)
                    {
                        pObjets[j].transform.localPosition = pArrayPos[i][j];
                    }
                     pCounter++;
                }
                pArrayObjectResult.Add(pObjets);
            }
            return pArrayObjectResult.ToArray();
        }
    }
}
