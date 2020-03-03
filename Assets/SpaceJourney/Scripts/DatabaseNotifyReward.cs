using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    [CreateAssetMenu(fileName = "DatabaseNotify",menuName = "EazyEngine/Space/Notification/DatabaseNotify")]
    public class DatabaseNotifyReward : EzScriptTableObject
    {
        public ItemNotifyReward[] container;

        protected static DatabaseNotifyReward _instance;
        public static DatabaseNotifyReward Instance
        {
            get
            {
                return _instance ? _instance : _instance = LoadAssets.loadAsset<DatabaseNotifyReward>("DatabaseNotify", "Variants/Database/");
            }
        }
    }
}

