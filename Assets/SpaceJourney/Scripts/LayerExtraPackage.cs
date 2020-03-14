using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace EazyEngine.Space
{
    public class LayerExtraPackage : SerializedMonoBehaviour
    {
        public string layerID;
        public int maxSlot;
        public Dictionary<int, GameObject[]> slots = new Dictionary<int, GameObject[]>();
        public List<GameObject> listBtn = new List<GameObject>();
        public void sortPos()
        {
            GameObject[] pArrayPos = null;
            for (int i = 0; i < slots.Count; ++i)
            {
                if(listBtn.Count <= slots.Keys.ElementAt(i))
                {
                    pArrayPos = slots.Values.ElementAt(i);
                    break;
                }
            }
            if(pArrayPos != null)
            {
                for(int i = 0; i < listBtn.Count; ++i)
                {
                    listBtn[i].transform.position = pArrayPos[i].transform.position;
                    listBtn[i].transform.localScale = pArrayPos[i].transform.localScale;
                }
            }
        }

        public void detachObject(string pString)
        {
            for(int i = listBtn.Count-1; i >= 0; --i)
            {
                if(listBtn[i].name == pString)
                {
                    Destroy(listBtn[i].gameObject);
                    listBtn.RemoveAt(i);
                  
                }
            }
            sortPos();
        }
        private void OnEnable()
        {
            ExtraPackageDatabase.Instance.RegisterThisLayer(this);
        }
        private void OnDisable()
        {
            ExtraPackageDatabase.Instance.UnRegisterThisLayer(this);
        }
    }
}
