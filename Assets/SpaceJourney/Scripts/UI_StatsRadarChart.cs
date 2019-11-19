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
        radarMeshCanvasRenderer = transform.Find("radarMesh").GetComponent<CanvasRenderer>();
    }
    float process = 0;
    public void SetStats(Stats pStats)
    {
        if (stats != null)
        {
            process = 0;
           var pAttak = stats.GetStatAmount(Stats.Type.Attack);
            var pSpeed = stats.GetStatAmount(Stats.Type.SpeedAttach);
            var pLucky = stats.GetStatAmount(Stats.Type.Lucky);
            var pDef = stats.GetStatAmount(Stats.Type.Defence);
            var pHealth = stats.GetStatAmount(Stats.Type.Health);
            var pAttakNew = pStats.GetStatAmount(Stats.Type.Attack);
            var pSpeedNew = pStats.GetStatAmount(Stats.Type.SpeedAttach);
            var pLuckyNew = pStats.GetStatAmount(Stats.Type.Lucky);
            var pDefNew = pStats.GetStatAmount(Stats.Type.Defence);
            var pHealthNew = pStats.GetStatAmount(Stats.Type.Health);
            DOTween.To(() => process, x => process = x, 1,0.25f).OnUpdate(delegate() {
                stats.SetStatAmount(Stats.Type.Attack, pAttak + (int)(process*(pAttakNew-pAttak)));
                 stats.SetStatAmount(Stats.Type.SpeedAttach, pSpeed +(int)(process * (pSpeedNew - pSpeed)));
                stats.SetStatAmount(Stats.Type.Lucky, pLucky+(int)(process * (pLuckyNew - pLucky)));
                stats.SetStatAmount(Stats.Type.Defence, pDef+(int)(process * (pDefNew - pDef)));
               stats.SetStatAmount(Stats.Type.Health, pHealth+(int)(process * (pHealthNew - pHealth)));
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

        Vector3[] vertices = new Vector3[6];
        Vector2[] uv = new Vector2[6];
        int[] triangles = new int[3 * 5];

        float angleIncrement = 360f / 5;
        float radarChartSize = 30f;

        Vector3 attackVertex = Quaternion.Euler(0, 0, -angleIncrement * 0) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(Stats.Type.Attack);
        int attackVertexIndex = 1;
        Vector3 defenceVertex = Quaternion.Euler(0, 0, -angleIncrement * 1) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(Stats.Type.Defence);
        int defenceVertexIndex = 2;
        Vector3 speedVertex = Quaternion.Euler(0, 0, -angleIncrement * 2) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(Stats.Type.Lucky);
        int speedVertexIndex = 3;
        Vector3 manaVertex = Quaternion.Euler(0, 0, -angleIncrement * 3) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(Stats.Type.SpeedAttach);
        int manaVertexIndex = 4;
        Vector3 healthVertex = Quaternion.Euler(0, 0, -angleIncrement * 4) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(Stats.Type.Health);
        int healthVertexIndex = 5;

        vertices[0] = Vector3.zero;
        vertices[attackVertexIndex] = attackVertex;
        vertices[defenceVertexIndex] = defenceVertex;
        vertices[speedVertexIndex] = speedVertex;
        vertices[manaVertexIndex] = manaVertex;
        vertices[healthVertexIndex] = healthVertex;

        uv[0] = Vector2.zero;
        uv[attackVertexIndex] = Vector2.one;
        uv[defenceVertexIndex] = Vector2.one;
        uv[speedVertexIndex] = Vector2.one;
        uv[manaVertexIndex] = Vector2.one;
        uv[healthVertexIndex] = Vector2.one;

        triangles[0] = 0;
        triangles[1] = attackVertexIndex;
        triangles[2] = defenceVertexIndex;

        triangles[3] = 0;
        triangles[4] = defenceVertexIndex;
        triangles[5] = speedVertexIndex;

        triangles[6] = 0;
        triangles[7] = speedVertexIndex;
        triangles[8] = manaVertexIndex;

        triangles[9] = 0;
        triangles[10] = manaVertexIndex;
        triangles[11] = healthVertexIndex;

        triangles[12] = 0;
        triangles[13] = healthVertexIndex;
        triangles[14] = attackVertexIndex;


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        radarMeshCanvasRenderer.SetMesh(mesh);
        radarMeshCanvasRenderer.SetMaterial(radarMaterial, radarTexture2D);
    }

}
