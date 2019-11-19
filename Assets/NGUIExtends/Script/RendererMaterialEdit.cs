using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PropertyType
{
    Float,
    Color,ColorHDR
}
[System.Serializable]
public class PropertyShader
{
    [SerializeField]
    string _nameProperty;
    [SerializeField]
    PropertyType propertyType;
    [SerializeField]
    [ShowIf("propertyType", PropertyType.Float)]
    float effectAmount;
    [SerializeField]
    [ShowIf("propertyType", PropertyType.Color)]
    Color color;
    [SerializeField]
    [ShowIf("propertyType", PropertyType.ColorHDR)]
    [ColorUsage(true,true)]
    Color colorHDR;

    public string NameProperty
    {
        get
        {
            return _nameProperty;
        }

        set
        {
            _nameProperty = value;
        }
    }

    public float EffectAmount
    {
        get
        {
            return effectAmount;
        }

        set
        {
            effectAmount = value;
        }
    }

    public Color Color { get => color; set => color = value; }
    public PropertyType PropertyType { get => propertyType; set => propertyType = value; }
    public Color ColorHDR { get => colorHDR; set => colorHDR = value; }
}
public class RendererMaterialEdit : MonoBehaviour {
    [SerializeField]
    PropertyShader[] properties;
    Renderer render;
    [SerializeField]
    bool includeChild = false;

    public Renderer Render
    {
        get
        {
            return render ? render : render = GetComponent<Renderer>();
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

    public PropertyShader[] Properties
    {
        get
        {
            return properties;
        }

        set
        {
            properties = value;
        }
    }
    public void setEffectAmount(int index, float effect)
    {
        PropertyShader[] props = Properties;
        if(index < props.Length)
        {
            props[index].EffectAmount = effect;
        }
        updateMaterial();
    }
    public void setEffectAmount(string name, float effect)
    {
        PropertyShader[] props = Properties;
        if (IncludeChild)
        {
            List<PropertyShader> arrayProp = new List<PropertyShader>();
            RendererMaterialEdit[] edits = GetComponentsInChildren<RendererMaterialEdit>();
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
                props[i].EffectAmount = effect;
            }
        }
        updateMaterial();
    }
    [ContextMenu("Reupdate")]
    public void updateMaterial()
    {
        for (int i = 0; i < Properties.Length; i++)
        {         
            Render.material.SetFloat(Properties[i].NameProperty, Properties[i].EffectAmount);
        }

    }


    private void OnValidate()
    {
        //if (Application.isPlaying)
        //{
        //    updateMaterial();
        //}
    }
    // Use this for initialization
    void Start () {
        updateMaterial();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
