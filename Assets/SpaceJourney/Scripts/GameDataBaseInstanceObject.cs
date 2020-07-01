using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace EazyEngine.Space {

    [CreateAssetMenu(fileName = "DatabaseDefault",menuName = "EazyEngine/Space/DatabaseDefault")]
    public class GameDataBaseInstanceObject : SerializedScriptableObject
    {
        [OdinSerialize,System.NonSerialized]
        public GameDataBaseInstance DatabaseDefault;
    }
}
