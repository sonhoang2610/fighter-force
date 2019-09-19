using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EazyRectUI : MonoBehaviour {
    UITexture _texture;
    [SerializeField]
    Color color;

    private void OnValidate()
    {
        //Texture.color = color;
    }
    public UITexture Texture
    {
        get
        {
            return _texture ? _texture : _texture = GetComponent<UITexture>();
        }
        
    }
    private void OnEnable()
    {
      
    }
    // Use this for initialization
    void Start () {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        texture.name = "black";
        Texture.mainTexture = texture;
        Texture.color = color;
    }

    private void OnDestroy()
    {
        Texture texture = Texture.mainTexture;
        Texture.mainTexture = null;
        DestroyImmediate(texture,true);
    }

    private void OnDisable()
    {
   
    }

    // Update is called once per frame
    void Update () {
		
	}
}
