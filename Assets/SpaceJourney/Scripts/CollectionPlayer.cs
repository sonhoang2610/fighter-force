using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    [CreateAssetMenu(fileName = "CollectionPlayer",menuName = "EazyEngine/Space/CollectionPlayer")]
    public class CollectionPlayer : ScriptableObject
    {
        public Character[] players;
    }
}
