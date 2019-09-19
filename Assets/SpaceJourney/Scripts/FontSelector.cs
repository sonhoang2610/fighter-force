using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Tools
{
    [RequireComponent(typeof(UILabel))]
    public class FontSelector : MonoBehaviour
    {
        public NGUIFont[] fonts;
        private UILabel _label;
        public UILabel label
        {
            get
            {
                return _label ? _label : _label = GetComponent<UILabel>();
            }
        }
        public void setFontIndex( int index)
        {
            label.bitmapFont = fonts[index];
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
