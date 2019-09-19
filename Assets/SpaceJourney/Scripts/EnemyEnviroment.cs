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
        foreach(var pEnemy  in pAsset.smallEnemiesObs)
        {
            if(pEnemy.info.elements[GameManager.Instance.ChoosedHard].target == pOriginal)
            {
                pChar.setDataConfig(pEnemy.info.elements[GameManager.Instance.ChoosedHard]);
                return;
            }
        }
        foreach (var pEnemy in pAsset.mediumEnemiesObs)
        {
            if (pEnemy.info.elements[GameManager.Instance.ChoosedHard].target == pOriginal)
            {
                pChar.setDataConfig(pEnemy.info.elements[GameManager.Instance.ChoosedHard]);
                return;
            }
        }
        foreach (var pEnemy in pAsset.bosssObs)
        {
            if (pEnemy.info.elements[GameManager.Instance.ChoosedHard].target == pOriginal)
            {
                pChar.setDataConfig(pEnemy.info.elements[GameManager.Instance.ChoosedHard]);
                return;
            }
        }
        foreach (var pEnemy in pAsset.miniBosssObs)
        {
            if (pEnemy.info.elements[GameManager.Instance.ChoosedHard].target == pOriginal)
            {
                pChar.setDataConfig(pEnemy.info.elements[GameManager.Instance.ChoosedHard]);
                return;
            }
        }
    }
    public GameObject getEnemyFromPool(GameObject pObject)
    {
        return getObjectFromPool(pObject);
    }

}
