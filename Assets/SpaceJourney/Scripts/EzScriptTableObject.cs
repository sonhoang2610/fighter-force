using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace EazyEngine.Tools
{

    public static class EzSerializeObjectSupport
    {
        //public static bool checkExistID(string pID,Object pObjectCheck)
        //{
        //    EzMonoBehaviorSerialize[] pAssets1 = Resources.FindObjectsOfTypeAll<EzMonoBehaviorSerialize>();
        //    for (int i = 0; i < pAssets1.Length; ++i)
        //    {
        //        if (pID == pAssets1[i].guiID && pObjectCheck != pAssets1[i] && !pAssets1[i].gameObject.activeInHierarchy && !((EzMonoBehaviorSerialize)pObjectCheck).gameObject.activeInHierarchy)
        //        {
        //            return true;
        //        }
        //    }
        //    EzScriptTableObject[] pAssets = Resources.FindObjectsOfTypeAll<EzScriptTableObject>();
        //    for (int i = 0; i < pAssets.Length; ++i)
        //    {
        //        if (pID == pAssets[i].guiID && pObjectCheck != pAssets[i])
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
    [System.Serializable]
    public class EzScriptTableObject : SerializedScriptableObject
    {
        protected override void OnBeforeSerialize()
        {
            if (Application.isPlaying) return;
            base.OnBeforeSerialize();
        }
        //[ReadOnly]
        //public string guiID;
        //private void OnEnable()
        //{
        //    if (!string.IsNullOrEmpty(guiID)) return;
        //    do
        //    {
        //        guiID = Random.Range(0, 999999999).ToString();

        //    } while (EzSerializeObjectSupport.checkExistID(guiID,this));
        //    EditorUtility.SetDirty(this);
        //    AssetDatabase.SaveAssets();
        //}


    }
}