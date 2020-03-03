using EazyEngine.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    [System.Serializable]
    public struct LayerAssignBtnIcon
    {
        public string layerID;
        public Vector3 position;
    }
    [CreateAssetMenu(fileName = "ComboPackage", menuName = "EazyEngine/Space/ExtraPackage/ComboPackage")]
    public class ComboPackage : ItemPackage
    {
        public LayerAssignBtnIcon[] layers;
        public GameObject btnIcon;
        public GameObject banner;
        public double startTime,timeExp;
    }
}
