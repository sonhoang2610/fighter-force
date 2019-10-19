using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

// AssetBundle cache checker & loader with caching
// worsk by loading .manifest file from server and parsing hash string from it

namespace EazyEngine.Tools
{
    public enum LoadAssetBundleStatus
    {
        NEW,
        CACHE,
        LOST_CONNECT_NOT_HAVE_CACHE
    }
    public class AssetBundleLoader 
    {
        public AssetBundle result;
        public UnityWebRequest www;
        public static Dictionary<Hash128, AssetBundle> BUNDLES = new Dictionary<Hash128, AssetBundle>();
        /// <summary>
        /// load assetbundle manifest, check hash, load actual bundle with hash parameter to use caching
        /// instantiate gameobject
        /// </summary>
        /// <param name="bundleURL">full url to assetbundle file</param>
        /// <param name="assetName">optional parameter to access specific asset from assetbundle</param>
        /// <returns></returns>
        public  IEnumerator DownloadAndCache(string bundleURL, string assetName = "",bool pNewVer = true,System.Action<LoadAssetBundleStatus,AssetBundle> resultCallBack  = null)
        {
            // Wait for the Caching system to be ready
            while (!Caching.ready)
            {
                yield return null;
            }
            // if you want to always load from server, can clear cache first
            //        Caching.CleanCache();

            // get current bundle hash from server, random value added to avoid caching
          www = UnityWebRequest.Get(bundleURL+ assetName + ".manifest?r=" + (Random.value * 9999999));

            // wait for load to finish
            yield return www.SendWebRequest();
            Hash128 hashString = (default(Hash128));// new Hash128(0, 0, 0, 0);
            // if received error, exit
            if (www.isNetworkError == true || !pNewVer)
            {
                goto  lostinternet;
            }

            // create empty hash string


            // check if received data contains 'ManifestFileVersion'
            if (www.downloadHandler.text.Contains("ManifestFileVersion"))
            {
                // extract hash string from the received data, TODO should add some error checking here
                var hashRow = www.downloadHandler.text.ToString().Split("\n".ToCharArray())[5];
                hashString = Hash128.Parse(hashRow.Split(':')[1].Trim());

                if (hashString.isValid == true)
                {
 
                    // we can check if there is cached version or not
                    if (Caching.IsVersionCached(bundleURL+assetName, hashString) == true)
                    {
                        Debug.Log("Bundle with this hash is already cached!");
                    }
                    else
                    {
                    
                        Debug.Log("No cached version founded for this hash..");
                    }
                }
                else
                {
                    // invalid loaded hash, just try loading latest bundle
                  //  Debug.LogError("Invalid hash:" + hashString);
                    yield break;
                }

            }
            else
            {
               // Debug.LogError("Manifest doesn't contain string 'ManifestFileVersion': " + bundleURL + ".manifest");
                yield break;
            }
            using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL + assetName   , hashString, 0))
            {
                yield return uwr.SendWebRequest();
             
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    resultCallBack?.Invoke(LoadAssetBundleStatus.LOST_CONNECT_NOT_HAVE_CACHE,null);
                }
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent((uwr));
                PlayerPrefs.SetString(bundleURL+assetName,hashString.ToString());
                result = bundle;
    
                www.Dispose();
                www = null;
                resultCallBack?.Invoke(LoadAssetBundleStatus.NEW,bundle);
                 yield break;      
            }
            lostinternet:
    
            string pStr = PlayerPrefs.GetString(bundleURL + assetName, "");
            hashString = Hash128.Parse(pStr);
            if (!string.IsNullOrEmpty(pStr))
            {
                using (UnityWebRequest uwr =
                    UnityWebRequestAssetBundle.GetAssetBundle(bundleURL + assetName, hashString, 0))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.isNetworkError || uwr.isHttpError)
                    {
                        resultCallBack?.Invoke(LoadAssetBundleStatus.LOST_CONNECT_NOT_HAVE_CACHE,null);
                    }
                    else
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent((uwr));
                  
                        result = bundle;
                        resultCallBack?.Invoke(LoadAssetBundleStatus.CACHE,bundle);
                        yield break;
                    }
                }
            }
           
        }
        
        
    }
}