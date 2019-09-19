using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{
    public class BaseTableGeneric<T> : BaseTable where T : BaseItemGame
    {
        [OnValueChanged("refreshID",true)]
        [InlineEditor]
        public T[] info;
        public string[] ids;
        [ContextMenu("refresh")]
        public void refreshID()
        {
            ids = new string[info.Length];
            for (int i = 0; i < ids.Length; ++i)
            {
                if(string.IsNullOrEmpty(info[i].itemID))
                {
                    info[i].itemID = info[i].name;
                }
                ids[i] = info[i].itemID;
            }
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                UnityEditor.SerializedObject pSerialize = new UnityEditor.SerializedObject(this);
                pSerialize.ApplyModifiedProperties();
#endif
            }
        }
        public override int CountItem => info.Length;

        public override BaseItemGame getItem(string pID)
        {
            for(int i = 0; i < info.Length; ++i)
            {
                if(info[i].itemID == pID)
                {
                    return info[i];
                }
            }
            return null;
        }

        public override string[] getAllIDItems()
        {
            if(ids == null || ids.Length != info.Length)
            {
                refreshID();
            }
           return ids;
        }

        public override BaseItemGame[] getAllItem()
        {
            return info;
        }
    }

    [System.Serializable]
    public class BaseTable: EzScriptTableObject
    {
        public CategoryItem cateGory;

        public virtual int CountItem
        {
            get
            {
                return 0;
            }
        }

        public virtual BaseItemGame getItem(string pID)
        {
            return null;
        }

        public virtual string[] getAllIDItems()
        {
            return new string[0];
        }

        public virtual BaseItemGame[] getAllItem()
        {
            return null;
        }
    }
}
