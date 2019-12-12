using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class BoxExtract : BaseBox<ItemInventorySlot, BaseItemGameInstanced>
    {
        public bool effectOnShow = true;
        public override bool isItemEffect()
        {
            return effectOnShow;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
