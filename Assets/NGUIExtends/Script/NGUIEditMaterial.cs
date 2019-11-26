using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyCustomAction;

public interface ISetIndex
{

}
public class NGUIEditMaterial : MonoBehaviour {
    [SerializeField]
    PropertyShader[] properties;
    UI2DSprite sprite2D;
    UITexture uitexture;
    [SerializeField]
    bool includeChild = false;
    [SerializeField]
    bool groupModule = false;
    [SerializeField]
    string[] GroupID ;
    [SerializeField]
    int defaultIndex;

    public void setGroupIndex(int index)
    {
        defaultIndex = index;
    }

    int cacheIndex = 0;
    public UI2DSprite Sprite2D
    {
        get
        {
            return sprite2D ? sprite2D : sprite2D = GetComponent<UI2DSprite>();
        }
        
    }

    public PropertyShader[] Properties
    {
        get
        {
            if (!groupModule || GroupID == null || GroupID.Length == 0 || defaultIndex >= GroupID.Length)
            {
                return properties;
            }
            else
            {
                List<PropertyShader> prop = new List<PropertyShader>();
               var pstrs =  GroupID[defaultIndex].Split('|');
                for (int i = 0;i < pstrs.Length; ++i)
                {
                    prop.Add(properties[int.Parse(pstrs[i])]);
                }
                return prop.ToArray();
            }
        }

        set
        {
            properties = value;
        }
    }

    public bool IncludeChild
    {
        get
        {
            return includeChild;
        }

        set
        {
            includeChild = value;
        }
    }

    public UITexture Uitexture
    {
        get
        {
            return uitexture ? uitexture : uitexture = GetComponent<UITexture>();
        }
        
    }

    public float EffectAmount1
    {
        get
        {
            return Properties[0].EffectAmount;
        }
        set
        {
            Properties[0].EffectAmount = value;
        }
    }

    public float EffectAmount2
    {
        get
        {
            return Properties[1].EffectAmount;
        }
        set
        {
            Properties[1].EffectAmount = value;
        }
    }

    public void setBehaviorIndex(int pIndex)
    {
        setGroupIndex(pIndex);
    }
    public void setEffectAmountColorHDR(int index, Color effect)
    {
        Properties[index].ColorHDR = effect;
    }
    public void setEffectAmountColor(int index, Color effect)
    {
        Properties[index].Color = effect;
    }
    public void setEffectAmount(int index,float effect)
    {
        Properties[index].EffectAmount = effect;
    }

    public void setEffectAmount(string name, float effect)
    {
        PropertyShader[] props = Properties;
        if (IncludeChild)
        {
            List<PropertyShader> arrayProp = new List<PropertyShader>();
            NGUIEditMaterial[] edits = GetComponentsInChildren<NGUIEditMaterial>();
            for(int i = 0; i < edits.Length; ++i)
            {
                arrayProp.addFromList(edits[i].Properties);
            }
            props = arrayProp.ToArray();
        }
   
        for(int i = 0; i < props.Length; ++i)
        {
            if(props[i].NameProperty == name)
            {
                props[i].EffectAmount = effect;
            }
        }
    }
    public void setEffectAmountColor(string name, Color effect)
    {
        PropertyShader[] props = Properties;
        if (IncludeChild)
        {
            List<PropertyShader> arrayProp = new List<PropertyShader>();
            NGUIEditMaterial[] edits = GetComponentsInChildren<NGUIEditMaterial>();
            for (int i = 0; i < edits.Length; ++i)
            {
                arrayProp.addFromList(edits[i].Properties);
            }
            props = arrayProp.ToArray();
        }

        for (int i = 0; i < props.Length; ++i)
        {
            if (props[i].NameProperty == name)
            {
                props[i].Color = effect;
            }
        }
    }

    public void setEffectAmountCache(float effect)
    {
        Properties[cacheIndex].EffectAmount = effect;
    }
    public void setEffectAmountCache(Color effect)
    {
        Properties[cacheIndex].Color = effect;
    }
    [ContextMenu("Test")]
    public void test()
    {
        Material mat = GetComponent<UI2DSprite>().material;
        for (int i = 0; i < Properties.Length; i++)
        {
            if (Sprite2D)
            {
                mat.mainTexture = Sprite2D.mainTexture;
            }
            else
            {
                mat.mainTexture = Uitexture.mainTexture;
            }
            if (Properties[i].PropertyType == PropertyType.Float)
            {
                mat.SetFloat(Properties[i].NameProperty, Properties[i].EffectAmount);
            }
            else if (Properties[i].PropertyType == PropertyType.Color)
            {
                mat.SetColor(Properties[i].NameProperty, Properties[i].Color);
            }
            else if (Properties[i].PropertyType == PropertyType.ColorHDR)
            {
                mat.SetColor(Properties[i].NameProperty, Properties[i].ColorHDR);
            }
        }
    }
    // Use this for initialization
    void Start () {
        sprite2D = Sprite2D;
        GetComponent<UIWidget>().onRender = delegate (Material mat)
       {
           for (int i = 0; i < Properties.Length; i++)
           {
               if (sprite2D)
               {
                   mat.mainTexture = Sprite2D.mainTexture;
               }
               else
               {
                   mat.mainTexture = Uitexture.mainTexture;
               }
               if (Properties[i].PropertyType == PropertyType.Float)
               {
                   mat.SetFloat(Properties[i].NameProperty, Properties[i].EffectAmount);
               }else if(Properties[i].PropertyType == PropertyType.Color)
               {
                   mat.SetColor(Properties[i].NameProperty, Properties[i].Color);
               }
               else if (Properties[i].PropertyType == PropertyType.ColorHDR)
               {
                   mat.SetColor(Properties[i].NameProperty, Properties[i].ColorHDR);
               }
            }
       };
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class NGUIPropertyMaterialAction : EazyFloatAction
{

    NGUIEditMaterial mat = null;
    public string nameProp;

    public NGUIPropertyMaterialAction()
    {
    }

    public static NGUIPropertyMaterialAction to(string nameProp, int pTo, float pUnit, bool calculByTime = true)
    {
        NGUIPropertyMaterialAction action = new NGUIPropertyMaterialAction();
        action.setTo(pTo, pUnit, calculByTime);
        action.nameProp = nameProp;
        return action;
    }

    public NGUIPropertyMaterialAction from(int pFrom)
    {
        setFrom(pFrom);
        return this;
    }
    

    public override void extendCallBack(GameObject pObject)
    {
        if (mat == null && pObject != null)
        {
            mat = pObject.GetComponent<NGUIEditMaterial>();
        }
        if (mat)
        {
            mat.setEffectAmount(nameProp, _current);
        }
    }

    public override void setUpAction(RootMotionController pRoot)
    {
        if (mat == null && pRoot != null)
        {
            mat = pRoot.GetComponent<NGUIEditMaterial>();
        }
        if (mat)
        {
            for (int i = 0; i < mat.Properties.Length; ++i)
            {
                if (mat.Properties[i].NameProperty == nameProp)
                {
                    _from = mat.Properties[i].EffectAmount;
                }
            }
        }
        base.setUpAction(pRoot);

    }
}