using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NGUIClipingParticle : MonoBehaviour
{
    public UIPanel target;
    static int[] ClipRange = null;
    public ParticleSystemRenderer[] system;

    public ParticleSystemRenderer[] System { get { return system; }  }

    private void Awake()
    {

        if (ClipRange == null)
        {
            ClipRange = new int[]
            {
                Shader.PropertyToID("_ClipRanges"),
                Shader.PropertyToID("_ClipRange1"),
                Shader.PropertyToID("_ClipRange2"),
                Shader.PropertyToID("_ClipRange4"),
            };
        }
        
    }
    public void clip()
    {
        if(target != null)
        {
            if (ClipRange == null)
            {
                ClipRange = new int[]
                {
                Shader.PropertyToID("_ClipRanges"),
                Shader.PropertyToID("_ClipRange1"),
                Shader.PropertyToID("_ClipRange2"),
                Shader.PropertyToID("_ClipRange4"),
                };
            }
            
            Vector4 cr = target.drawCallClipRange;
            SetClipping(0, cr, target.clipSoftness, 0);
        }
    }

    void SetClipping(int index, Vector4 cr, Vector2 soft, float angle)
    {
        if (System == null) return;
        angle *= -Mathf.Deg2Rad;

        Vector2 sharpness = new Vector2(1000.0f, 1000.0f);
        if (soft.x > 0f) sharpness.x = cr.z / soft.x;
        if (soft.y > 0f) sharpness.y = cr.w / soft.y;
        for(int i = 0;i < System.Length; ++i)
        {
            Material pMat = System[i].sharedMaterial;
            if (Application.isPlaying)
            {
                pMat = System[i].material;
            }
            Vector2 pos = target.transform.TransformPoint(new Vector2(cr.x, cr.y));
            var pCr = new Vector4(pos.x, pos.y, cr.z * target.transform.lossyScale.x, cr.w * target.transform.lossyScale.y);
            pMat.SetVector(ClipRange[0], pCr);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        clip();
    }
}
