using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaParrent : MonoBehaviour
{
    [SerializeField]
    protected float currentAlpha =1;
    public float A
    {
        set
        {
            var pSprites = GetComponentsInChildren<SpriteRenderer>();
            foreach(var pSprite in pSprites)
            {
                Color color = pSprite.color;
                color.a = value;
                pSprite.color = color;
            }
            currentAlpha = value;
        }
        get
        {
            return currentAlpha;
        }
    }
        
    private void OnValidate()
    {
        A = currentAlpha;
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
