using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {

        string assetBundleDirectory = "Assets/StreamingAssets~/PC";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Assets/Build AssetBundles Android")]
    static void BuildAllAssetBundlesAndroid()
    {

        string assetBundleDirectory = "Assets/StreamingAssets~/Android";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    [MenuItem("Assets/Build AssetBundles IOS")]
    static void BuildAllAssetBundlesIOS()
    {

        string assetBundleDirectory = "Assets/StreamingAssets~/IOS";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.iOS);
    }

    [MenuItem("Assets/Build AssetBundles Webgl")]
    static void BuildAllAssetBundlesWebgl()
    {

        string assetBundleDirectory = "Assets/StreamingAssets~/WebGL";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.WebGL);
    }

    [MenuItem("Assets/LinkedPrefab/Excute")]
    static void LinkPrefab()
    {

        GameObject pObject = Selection.activeGameObject;
        GameObject pNewObject =(GameObject) PrefabUtility.InstantiatePrefab(pObject);
        string path = AssetDatabase.GetAssetOrScenePath(pObject);
        path = path.Replace(".prefab", "[Child].prefab");

       var pObjects =  FindAllPrefabInstances(pObject);
        Dictionary<GameObject, PropertyModification[]> modifis = new Dictionary<GameObject, PropertyModification[]>();
        foreach(var pObjectRef in pObjects)
        {
            PropertyModification[] props = PrefabUtility.GetPropertyModifications(pObjectRef);
            if(!modifis.ContainsKey(pObjectRef))
            {
                modifis.Add(pObjectRef, props);
            }
            else
            {
                modifis[pObjectRef] = props;
            }
        }


       //   var scenePath = SceneManager.GetActiveScene().path;
       // SearchTarget search = new SearchTarget(pObject, scenePath);
       // var files = FindDependencies.InScenePro(search);
       // var pResults = files
       //.OrderBy(t => t.LabelContent.text, StringComparer.Ordinal)
       //.ToArray();
        PrefabUtility.SaveAsPrefabAsset(pNewObject, path);
        foreach (var pObjectRef in pObjects)
        {
            PrefabUtility.SetPropertyModifications(pObjectRef, modifis[pObjectRef]);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    static List<GameObject> FindAllPrefabInstances(UnityEngine.Object myPrefab)
    {
        var scenePath = SceneManager.GetActiveScene().path;
        List<GameObject> result = new List<GameObject>();
        GameObject[] allObjects = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject GO in allObjects)
        {
            if (GO.scene.path != scenePath) continue;
            if (EditorUtility.GetPrefabType(GO) == PrefabType.PrefabInstance)
            {
                UnityEngine.Object GO_prefab = EditorUtility.GetPrefabParent(GO);
                if (myPrefab == GO_prefab)
                    result.Add(GO);
            }
        }
        return result;
    }
}