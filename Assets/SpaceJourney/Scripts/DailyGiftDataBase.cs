using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class DailyGiftDataBase : ScriptableObject
    {
        public string idModule;
        [ListDrawerSettings(CustomAddFunction = "AddValueToitem")]
        public List<ItemDailyGiftInfo> item;
        public string[] moduleClearID;
        private void AddValueToitem()
        {
#if UNITY_EDITOR
            UnityEditor.SerializedObject pSe = new UnityEditor.SerializedObject(this);
            pSe.Update();
            item.Add(new ItemDailyGiftInfo());
            item[item.Count - 1].day = item.Count;
            if(item.Count > 1)
            {
                item[item.Count - 1].mainData = new BaseItemGameInstanced();
                item[item.Count - 1].mainData.item = item[item.Count - 2].mainData.item;
            }
            pSe.ApplyModifiedProperties();
#endif
        }
    }
}