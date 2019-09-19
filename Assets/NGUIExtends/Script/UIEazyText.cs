using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CharacterColor
{
    public int startIndex;
    public int numberCharecter;
    public Color color;
    public CharacterColor(int start, int count, Color color)
    {
        this.color = color;
        startIndex = start;
        numberCharecter = count;
    }
}
[ExecuteInEditMode]
public class UIEazyText : MonoBehaviour
{
    public EazyStringXml _string;
    public string[] _cacheString;
    private UILabel _label;
    public string[] _parameterFormat;
    public bool isDirty = false;
    public Color _colorDefault = Color.white;
    private List<CharacterColor> _listCharacter = new List<CharacterColor>();
    private List<string> _extrarText;
    public UILabel Label
    {
        get
        {
            return _label != null ? _label : _label = GetComponent<UILabel>();
        }
    }
    public bool LoadEazyXML
    {
        get
        {
            return _string.Xml;
        }
        set
        {
            _string.Xml = value;
        }
    }
    [SerializeField]
    bool isBreakLineReplace = false;
    public string String{
        get
        {
            return getString();
        }
        set
        {
            _string.ez_str = value;
            if (Label)
            {
                Label.text = _string.ez_str;
            }
        }
    }
    public string RealString
    {
        get
        {
            string pString = getString();
            if (_extrarText != null)
            {
                for(int i = 0; i < _extrarText.Count; ++i)
                {
                    pString += _extrarText[i];
                }
            }
            return pString;
        }       
    }
    public string getString()
    {
        string pString = _string.ez_str;
        if (_parameterFormat != null && _parameterFormat.Length > 0)
        {
            pString = string.Format(pString, _parameterFormat);
        }
        return pString;
    }
    public void setParrameters(params string[] str)
    {
        _parameterFormat = str;
        OnValidate();
    }

    public void setParrameter(string str)
    {
        if (_parameterFormat != null )
        {
            if(_parameterFormat.Length == 0)
            {
                _parameterFormat = new string[1];
            }
            _parameterFormat[0] = str;
        }
        OnValidate();
    }

    public void setParrameter(int str)
    {
        if (_parameterFormat != null)
        {
            if (_parameterFormat.Length == 0)
            {
                _parameterFormat = new string[1];
            }
            _parameterFormat[0] = str.ToString();
        }
        OnValidate();
    }


    public void addExtraText(string pString)
    {
        if(_extrarText == null)
        {
            _extrarText = new List<string>();
        }
        _extrarText.Add(pString);
    }
    public void clearExtraText()
    {
        if (_extrarText != null)
        {
            _extrarText.Clear();
        }
    }
    public void setExtraText(string pString)
    {
        clearExtraText();
        addExtraText(pString);
        Label.text = RealString;
    }

    private void OnValidate()
    {

        Label.text = getString();
        if (isBreakLineReplace)
        {
            Label.text = Label.text.Replace("/n", "\n");
        }
    }

    public void addColorCharacter(int indexStart, int pCount, Color color)
    {
        _listCharacter.Add(new CharacterColor(indexStart, pCount, color));
        updateCorlor();
    }

    public void clearColor()
    {
        _listCharacter.Clear();
    }

    Pos getNearStartCharacter(int index)
    {
        Pos pIndex;
        pIndex.x = getString().Length;
        pIndex.y = -1;
        for (int i = 0; i < _listCharacter.Count; i++)
        {
            if (index == _listCharacter[i].startIndex && pIndex.x > _listCharacter[i].startIndex)
            {
                pIndex.x = _listCharacter[i].startIndex;
                pIndex.y = i;
            }
        }
        return pIndex;
    }

    public void updateCorlor()
    {
        string mainString = getString();
        string resultString = "";
        int index = 0;
        while (index < mainString.Length)
        {

            Pos indexStart = getNearStartCharacter(index);
            if (indexStart.x < mainString.Length)
            {
                resultString += "[" + ColorUtility.ToHtmlStringRGB(_listCharacter[indexStart.y].color) + "]";
                resultString += mainString.Substring(index, _listCharacter[indexStart.y].numberCharecter);
                index += _listCharacter[indexStart.y].numberCharecter;
            }
            else
            {
                resultString += "[" + ColorUtility.ToHtmlStringRGB(_colorDefault) + "]";
                resultString += mainString.Substring(index, indexStart.x - index);
                //resultString += indexStart;
                index += indexStart.x;
            }
            resultString += "[-]";

        }
        Label.text = resultString;
       // Label.MakePixelPerfect();
    }

    public void setStringCacheIndex(int index)
    {
        _string.ez_str = _cacheString[index];
        Label.text = _string.ez_str;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
