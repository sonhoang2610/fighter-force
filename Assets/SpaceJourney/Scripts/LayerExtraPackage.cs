using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
    public class LayerExtraPackage : MonoBehaviour
    {


        public string layerID;

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
