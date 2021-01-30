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

    public override void onNewCreateObject(GameObject pObject, GameObject pOriginal)
    {
        base.onNewCreateObject(pObject, pOriginal);
        var pChar = pObject.GetComponent<Character>();
        pChar.originalPreb = pOriginal;
        var pAsset = LoadAssets.loadAsset<EazySpaceConfigDatabase>("EnemyConfig", "Variants/Database/");
        int[] scoreSmall = new int[] { 50, 100, 180 };
        int[] scoremedium = new int[] { 200, 350, 600 };
        int[] scoreBoss = new int[] { 10000, 18000, 30000 };
        if (LevelManger.InstanceRaw && LevelManger.Instance.IsMatching)
        {
            Debug.Log($"NotEnough_{ pOriginal.name }_Level_{GameManager.Instance.ChoosedLevel}_Mode_{GameManager.Instance.ChoosedLevel}_State_{LevelStateManager.currentState}");
            Firebase.Analytics.FirebaseAnalytics.LogEvent($"NotEnough_{ pOriginal.name }_Level_{GameManager.Instance.ChoosedLevel}_Mode_{GameManager.Instance.ChoosedLevel}_State_{LevelStateManager.currentState}");
        }
        string pKey = "";
        foreach (var pEnemy in pAsset.smallEnemiesObs)
        {
            if (pOriginal.tryGetRuntimeKey(out pKey))
            {
                if (pEnemy.info.elements[GameManager.Instance.ChoosedHard].targetRef.runtimeKey == pKey)
                {
                    pObject.transform.localScale = pOriginal.transform.localScale * 0.9f;
                    var pInfo = pEnemy.info.elements[GameManager.Instance.ChoosedHard];
                    pInfo.score = scoreSmall[GameManager.Instance.ChoosedHard];
                    pChar.setDataConfig(pInfo);
                    pChar.EnemyType = EazyEngine.Space.EnemyType.SMALL;
                    return;
                }
            }
        }
        foreach (var pEnemy in pAsset.mediumEnemiesObs)
        {
            if (pOriginal.tryGetRuntimeKey(out pKey))
            {
                if (pEnemy.info.elements[GameManager.Instance.ChoosedHard].targetRef.runtimeKey == pKey)
                {
                    if (!pOriginal.name.ToLower().Contains("tower"))
                    {
                        pObject.transform.localScale = pOriginal.transform.localScale * 0.8f;
                    }
                    else
                    {
                        pObject.transform.localScale = pOriginal.transform.localScale * 0.9f;
                    }
                    var pInfo = pEnemy.info.elements[GameManager.Instance.ChoosedHard];
                    pInfo.score = scoremedium[GameManager.Instance.ChoosedHard];
                    pChar.setDataConfig(pInfo);
                    pChar.EnemyType = EazyEngine.Space.EnemyType.MEDIUM;
                    return;
                }
            }
        }
        foreach (var pEnemy in pAsset.bosssObs)
        {
            if (pOriginal.tryGetRuntimeKey(out pKey))
            {
                if (pEnemy.info.elements[GameManager.Instance.ChoosedHard].targetRef.runtimeKey == pKey)
                {
                    var pInfo = pEnemy.info.elements[GameManager.Instance.ChoosedHard];
                    pInfo.score = scoreBoss[GameManager.Instance.ChoosedHard];
                    pChar.setDataConfig(pInfo);
                    pChar.EnemyType = EazyEngine.Space.EnemyType.BOSS;
                    return;
                }
            }
        }
        foreach (var pEnemy in pAsset.miniBosssObs)
        {
            if (pOriginal.tryGetRuntimeKey(out pKey))
            {
                if (pEnemy.info.elements[GameManager.Instance.ChoosedHard].targetRef.runtimeKey == pKey)
                {
                    var pInfo = pEnemy.info.elements[GameManager.Instance.ChoosedHard];
                    pInfo.score = scoreBoss[GameManager.Instance.ChoosedHard];
                    pChar.setDataConfig(pInfo);
                    pChar.EnemyType = EazyEngine.Space.EnemyType.MINIBOSS;
                    return;
                }
            }
        }
  
        if (!pOriginal.tryGetRuntimeKey(out pKey))
        {
            Debug.Log($"NotFindData_{ pOriginal.name }_Level_{GameManager.Instance.ChoosedLevel}_Mode_{GameManager.Instance.ChoosedLevel}_State_{LevelStateManager.currentState}");
            Firebase.Analytics.FirebaseAnalytics.LogEvent($"NotFindData_{ pOriginal.name }_Level_{GameManager.Instance.ChoosedLevel}_Mode_{GameManager.Instance.ChoosedLevel}_State_{LevelStateManager.currentState}");
        }

    }
    public GameObject getEnemyFromPool(GameObject pObject)
    {
        return getObjectFromPool(pObject);
    }
    [Button("Resolved Preload")]
    public void findNonExistPreload()
    {
        var pState = transform.GetComponentInParent<LevelStateManager>();
        if (pState)
        {
            for (int i = 0; i < pState.states.Length; ++i)
            {
                for (int j = 0; j < pState.states[i].formatInfo.prefabEnemies.Length; ++j)
                {
                    bool exist = false;
                    for (int g = 0; g < _storage.Count; g++)
                    {
                        if (_storage[pState.states[i].formatInfo.prefabEnemies[j]] != null)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        getObjectFromPoolZero(pState.states[i].formatInfo.prefabEnemies[j]);
                    }
                }
            }
            for (int g = 0; g < _storage.Count; g++)
            {
                bool exist = false;
                for (int i = 0; i < pState.states.Length; i++)
                {
                    for (int j = 0; j < pState.states[i].formatInfo.prefabEnemies.Length; ++j)
                    {
                        if (_storage.ElementAt(g) == pState.states[i].formatInfo.prefabEnemies[j])
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (exist)
                    {
                        break;
                    }
                }
                if (!exist)
                {
                    _storage[_storage.ElementAt(g)].countPreload = -1;
                }
            }
        }
    }
    public void getObjectFromPoolZero(GameObject pObject)
    {
        if (_storage.ContainsKey(pObject) && _storage[pObject].pooler == null)
        {
            _storage.Remove(pObject);
        }
        if (!_storage.ContainsKey(pObject))
        {
            GameObject pObjectNew = new GameObject();
            pObjectNew.name = "[Pool]" + pObject.name;
            pObjectNew.transform.parent = transform;
            pObjectNew.transform.localPosition = Vector3.zero;
            var pooler = pObjectNew.AddComponent<SimpleObjectPooler>();
            pooler.onNewGameObjectCreated = (onNewCreateObject);
            pooler.GameObjectToPool = pObject;
            pooler.PoolSize = 0;
            pooler.FillObjectPool();
            _storage.Add(pObject, new PrefabInfoMain());
            _storage[pObject].pooler = pooler;
        }

    }
}
