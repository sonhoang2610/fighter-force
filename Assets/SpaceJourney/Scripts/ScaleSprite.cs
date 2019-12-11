using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScaleSprite : MonoBehaviour
{
    public Vector2 resolution;
    [ContextMenu("scale")]
    public void scaleNow()
    {
      var sprites=   GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            sprite.transform.localScale = new Vector3(resolution.x / (sprite.bounds.size.x), resolution.y /( sprite.bounds.size.y), 1);
        }
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
