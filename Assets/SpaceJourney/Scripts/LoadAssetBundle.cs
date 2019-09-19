using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class LoadAssetBundle : MonoBehaviour
{
    public string uri;
    public string[] listAsset;
    // Use this for initialization
    void Start()
    {
        //var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "scene.pharaon"));
        //AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "assets.pharaon"));
        //if (myLoadedAssetBundle == null)
        //{
        //    Debug.Log("Failed to load AssetBundle!");
        //    return;
        //}
        StartCoroutine(InstantiateObject());
    }
    private IEnumerator InstantiateObject()
    {
        for (int i = 0; i < listAsset.Length; i++) {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri + listAsset[i], 0);
            yield return request.SendWebRequest();
            DownloadHandlerAssetBundle.GetContent(request);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
