/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StatsRadarChart : MonoBehaviour
{

    [SerializeField] private Material radarMaterial;
    [SerializeField] private Texture2D radarTexture2D;

    private Stats stats;
    private CanvasRenderer radarMeshCanvasRenderer;

    private void Awake()
    {
        radarMeshCanvasRenderer = transform.Find("Canvas").Find("radarMesh").GetComponent<CanvasRenderer>();
    }
    float process = 0;
    public void SetStats(Stats pStats)
    {
        if (stats != null)
        {
            process = 0;
            var pOldValue = new int[stats._Stats.Length];
            var pNewValue = new int[stats._Stats.Length];
            for (int i = 0; i < stats._Stats.Length; ++i)
            {
                pOldValue[i] = stats.GetStatAmount(i);
                pNewValue[i] = pStats.GetStatAmount(i);
            }
            DOTween.To(() => process, x => process = x, 1, 0.25f).OnUpdate(delegate ()
            {
                for (int i = 0; i < stats._Stats.Length; ++i)
                {
                    stats.SetStatAmount(i, stats.GetStatAmount(i) + (int)(process * (pNewValue[i] - pOldValue[i])));
                    pOldValue[i] = stats.GetStatAmount(i);
                    pNewValue[i] = pStats.GetStatAmount(i);
                }
                //stats.SetStatAmount(TypeStats.Attack, pAttak + (int)(process * (pAttakNew - pAttak)));
                //stats.SetStatAmount(TypeStats.SpeedAttach, pSpeed + (int)(process * (pSpeedNew - pSpeed)));
                //stats.SetStatAmount(TypeStats.Lucky, pLucky + (int)(process * (pLuckyNew - pLucky)));
                //stats.SetStatAmount(TypeStats.Defence, pDef + (int)(process * (pDefNew - pDef)));
                //stats.SetStatAmount(TypeStats.Health, pHealth + (int)(process * (pHealthNew - pHealth)));
                UpdateStatsVisual();
            });
        }
        else
        {
            this.stats = pStats;
            UpdateStatsVisual();
        }
        stats.OnStatsChanged += Stats_OnStatsChanged;
    }

    private void Stats_OnStatsChanged(object sender, System.EventArgs e)
    {
        UpdateStatsVisual();
    }

    private void UpdateStatsVisual()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[stats._Stats.Length + 1];
        Vector2[] uv = new Vector2[stats._Stats.Length + 1];
        int[] triangles = new int[3 * stats._Stats.Length];

        float angleIncrement = 360f / stats._Stats.Length;
        float radarChartSize = 30f;

        Vector3[] pVertices = new Vector3[stats._Stats.Length];
        vertices[0] = Vector3.zero;
        uv[0] = Vector2.zero;
        for (int i = 0; i < pVertices.Length; ++i)
        {
            pVertices[i] = Quaternion.Euler(0, 0, -angleIncrement * i) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(i);
            vertices[i + 1] = pVertices[i];
            uv[i + 1] = Vector2.one;
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 2) > pVertices.Length  ? 1 : (i+2);
        }
        //Vector3 attackVertex = Quaternion.Euler(0, 0, -angleIncrement * 0) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(TypeStats.Attack);
        //int attackVertexIndex = 1;
        //Vector3 defenceVertex = Quaternion.Euler(0, 0, -angleIncrement * 1) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(TypeStats.Defence);
        //int defenceVertexIndex = 2;
        //Vector3 speedVertex = Quaternion.Euler(0, 0, -angleIncrement * 2) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(TypeStats.Lucky);
        //int speedVertexIndex = 3;
        //Vector3 manaVertex = Quaternion.Euler(0, 0, -angleIncrement * 3) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(TypeStats.SpeedAttach);
        //int manaVertexIndex = 4;
        //Vector3 healthVertex = Quaternion.Euler(0, 0, -angleIncrement * 4) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(TypeStats.Health);
        //int healthVertexIndex = 5;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        radarMeshCanvasRenderer.SetMesh(mesh);
        radarMeshCanvasRenderer.SetMaterial(radarMaterial, radarTexture2D);
    }

}
