using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FillColor : MonoBehaviour {
	[SerializeField]
	Color colorFill;
    [SerializeField]
    bool allowIgnoreSetting = false;
    protected MaterialPropertyBlock block;
	protected MeshRenderer meshRenderer;
	
	public MeshRenderer MeshRenderer{
		get{
			return meshRenderer ? meshRenderer : meshRenderer = GetComponent<MeshRenderer>();
		}
	}
    private void OnEnable()
    {
        if(block != null)
        {
            int fillAlpha = Shader.PropertyToID("_FillPhase");
            block.SetFloat(fillAlpha, 0f); // Remove the fill.
            MeshRenderer.SetPropertyBlock(block);
        }
    }
    // Awake is called when the script instance is being loaded.
    protected void Awake()
	{
        //if (GameManager.Instance.Graphic || allowIgnoreSetting)
        //{
            block = new MaterialPropertyBlock();
            MeshRenderer.SetPropertyBlock(block);
        //}
	}
	protected IEnumerator FlashRoutine(float duration)
	{
		
        // You can use these instead of strings.
		int fillAlpha = 0;
		int fillColor = 0;
		
		
		if (block != null)
		{
			fillAlpha = Shader.PropertyToID("_FillPhase");
			fillColor = Shader.PropertyToID("_FillColor");
			block.SetFloat(fillAlpha, 0.4f); // Make the fill opaque.
			
			block.SetColor(fillColor,colorFill); // Fill with white.
		}
			meshRenderer.SetPropertyBlock(block);
		yield return new WaitForSeconds(duration);
		if (block != null)
		{
			block.SetFloat(fillAlpha, 0f); // Remove the fill.
		}
        MeshRenderer.SetPropertyBlock(block);	
	}
	public void flash(float pDuration){
            StartCoroutine(FlashRoutine(pDuration));
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
