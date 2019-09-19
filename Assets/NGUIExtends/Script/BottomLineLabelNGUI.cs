using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomLineLabelNGUI : MonoBehaviour
{
    [SerializeField]
    UILabel _labelMain = null;
    UILabel _labelSelf;
    string oldLabel;

    public UILabel LabelSelf
    {
        get
        {
            return _labelSelf ? _labelSelf : _labelSelf = GetComponent<UILabel>();
        }

        set
        {
            _labelSelf = value;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    [ContextMenu("Resort")]
    public void executeResizeWidth()
    {
        if (_labelMain != null && _labelMain.text != oldLabel)
        {

            LabelSelf.text = "";
            do
            {
                LabelSelf.text += "_";
            } while (NGUIMath.CalculateRelativeWidgetBounds(LabelSelf.transform).size.x < NGUIMath.CalculateRelativeWidgetBounds(_labelMain.transform).size.x);
            oldLabel = _labelMain.text;
        }
    }

    // Update is called once per frame
    void Update()
    {
  
    }
}