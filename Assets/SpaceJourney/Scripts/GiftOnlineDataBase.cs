using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class GiftOnlineDataBase : ScriptableObject
    {
        public string idModule;
        public ItemGiftOnlineInfo[] item;
        public string[] moduleClearID;
    }
}