using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

namespace EazyEngine.Tools
{
    public class ItemLanguage : BaseItem<string>
    {
        public Localize sprite;
        public override string Data { get => base.Data;
            set {
                base.Data = value;
            }
        }

        public override void onExecute()
        {
            base.onExecute();
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
