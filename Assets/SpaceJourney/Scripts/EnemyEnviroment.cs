using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Space;
using Sirenix.OdinInspector;
using System.Linq;
using EazyEngine.Space;

public class EnemyEnviroment : PoolManagerGeneric<EnemyEnviroment>
{
    // [ShowInInspector,HideInEditorMode]

    public override void onNewCreateObject(GameObject pObject,GameObject pOriginal)
    {
        base.onNewCreateObject(pObject, pOriginal);
        var pChar =   pObject.GetComponent<Character>();
        pChar.originalPreb = pOriginal;
        var pAsset = LoadAssets.loadAsset<EazySpaceConfigDatabase>("EnemyConfig", "Variants/Database/");
        int[] scoreSmall = new int[] { 50,100,180};
        int[] scoremedium = new int[] {200,350,600 };
        int[] scoreBoss = new int[] {10000,18000,30000 };
        foreach (var pEnemy  in pAsset.smallEnemiesObs)
        {
            if(pEnemy.info.elements[GameManager.Instance.ChoosedHard].target == pOriginal)
            {
                var pInfo = pEnemy.info.elements[GameManager.Instance.ChoosedHard];
                pInfo.score = scoreSmall[GameManager.Instance.ChoosedHard];
                pChar.setDataConfig(pInfo);
                return;
            }
        }
        foreach (var pEnemy in pAsset.mediumEnemiesObs)
        {
            if (pEnemy.info.elements[GameManager.Instance.ChoosedHard].target == pOriginal)
            {
                var pInfo = pEnemy.info.elements[GameManager.Instance.ChoosedHard];
                pInfo.score = scoremedium[GameManager.Instance.ChoosedHard];
                pChar.setDataConfig(pInfo);
                return;
            }
        }
        foreach (var pEnemy in pAsset.bosssObs)
        {
            if (pEnemy.info.elements[GameManager.Instance.ChoosedHard].target == pOriginal)
            {
                var pInfo = pEnemy.info.elements[GameManager.Instance.ChoosedHard];
                pInfo.score = scoreBoss[GameManager.Instance.ChoosedHard];
                pChar.setDataConfig(pInfo);
                return;
            }
        }
        foreach (var pEnemy in pAsset.miniBosssObs)
        {
            if (pEnemy.info.elements[GameManager.Instance.ChoosedHard].target == pOriginal)
            {
                var pInfo = pEnemy.info.elements[GameManager.Instance.ChoosedHard];
                pInfo.score = scoreBoss[GameManager.Instance.ChoosedHard];
                pChar.setDataConfig(pInfo);
                return;
            }
        }
    }
    public GameObject getEnemyFromPool(GameObject pObject)
    {
        return getObjectFromPool(pObject);
    }

}
