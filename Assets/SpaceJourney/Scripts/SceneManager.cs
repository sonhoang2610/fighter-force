using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.Events;
using DG.Tweening;
using EazyEngine.Space.UI;
using I2.Loc;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class EventFloat : UnityEvent<float>
    {

    }
    public class SceneManager : PersistentSingleton<SceneManager>
    {
        public UIWidget fadeLayout;
        public UI2DSprite process;
        public UILabel loadingcontent;
        public Camera camera;
        public UnityEvent onStartLoad;
        public EventFloat onLoadingScene;
        public UnityEvent onComplete;
        public bool isLocal = true;
        AsyncOperation async;
        bool isStart = false;
        public string currentScene;
        public static Dictionary<string, AssetBundle> BUNDLES = new Dictionary<string, AssetBundle>();



        public void loadScene(string pScene)
        {
            currentScene = pScene;
            //   fadeLayout.alpha = 0;
            Sequence pSeq = DOTween.Sequence();
            if (process)
            {
                process.fillAmount = 0;
            }
            pSeq.Append(DOTween.To(() => fadeLayout.alpha, a => fadeLayout.alpha = a, 1, 0.25f));
            pSeq.AppendCallback(delegate ()
            {
                onStartLoad.Invoke();
                isStart = true;

                async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(pScene);
            });

            pSeq.Play();
        }

        public void complete()
        {
        
                fadeLayout.alpha = 1;
                Sequence pSeq = DOTween.Sequence();
                pSeq.AppendInterval(0.25f);
                pSeq.Append(DOTween.To(() => fadeLayout.alpha, a => fadeLayout.alpha = a, 0, 1));
                pSeq.AppendCallback(delegate
                {
                    camera.GetComponent<CropCamera>().clearRender();
                });
                pSeq.Play();
            
      

        }
        public IEnumerator delayAction(float pDelay,System.Action pAction)
        {
            yield return new WaitForSeconds(0.1f);
            pAction();
        }
        public void loadAllGame()
        {
            if (SceneManager.Instance.isLocal)
            {
                Instantiate(Resources.Load<GameObject>("Variants/Database/GameManager"));
                StartCoroutine(delayAction(0.1f,delegate {

                    Instantiate(Resources.Load<GameObject>("Variants/prefabs/ui/HUD"),transform);
                   loadScene("SpaceJourney/Scene/variant/Main");
               }));
            }
            else
            {
                fadeLayout.alpha = 0;
                Sequence pSeq = DOTween.Sequence();
                if (process)
                {
                    process.fillAmount = 0;
                }
                pSeq.Append(DOTween.To(() => fadeLayout.alpha, a => fadeLayout.alpha = a, 1, 0.25f));
                pSeq.AppendCallback(delegate ()
                {
                    StartCoroutine(loadManager());
                });

                pSeq.Play();
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
        }

        public IEnumerator loadManager()
        {
            AssetBundleLoader loader = new AssetBundleLoader();
            string pTag = "ui/uri_assetbundle";
#if UNITY_ANDROID 
            pTag = "ui/uri_assetbundle_android";
#endif
#if UNITY_IOS
             pTag = "ui/uri_assetbundle_ios";
#endif
            yield return loader.DownloadAndCache(I2.Loc.LocalizationManager.GetTranslation(pTag), "assetmanager");
            if (loader.result != null)
            {
                var pManager = loader.result.LoadAsset<AssetbundleManager>("AssetbundleManager");
                var pCurrentVersion = PlayerPrefs.GetString("Version", "0");
                List<ModuleAssetInfo> queue = new List<ModuleAssetInfo>();

                if (pCurrentVersion != pManager.currentModule.version)
                {
                    queue.AddRange(pManager.currentModule.modules);

                    float totalSize = 0;
                    for (int j = 0; j < pManager.currentModule.modules.Length; ++j)
                    {
                        totalSize += pManager.currentModule.modules[j].sizeFile;
                    }


                    for (int i = 0; i < queue.Count; ++i)
                    {
                        queue[i].Percent = queue[i].sizeFile / totalSize;
                    }
                }
                yield return loadModules(queue);
            }
        }
        List<GameObject> objectPlanInstiate = new List<GameObject>();
        public IEnumerator loadModules(List<ModuleAssetInfo> queue)
        {
            float percent = 0;
            string pTag = "ui/uri_assetbundle";
#if UNITY_ANDROID 
            pTag = "ui/uri_assetbundle_android";
#endif
#if UNITY_IOS
             pTag = "ui/uri_assetbundle_ios";
#endif
            var pUrl = I2.Loc.LocalizationManager.GetTranslation(pTag);
            for (int i = 0; i < queue.Count; ++i)
            {
                AssetBundleLoader loader = new AssetBundleLoader();
                loadingcontent.text = "Loading Module " + queue[i].nameDisplay;

                yield return loader.DownloadAndCache(pUrl, queue[i].nameModule);
                if (loader.result != null && !BUNDLES.ContainsKey(pUrl + queue[i].nameModule))
                {
                    if (queue[i].nameModule.Contains("material"))
                    {
                        var materials = loader.result.LoadAllAssets<Material>();
                        foreach (Material m in materials)
                        {
                            var shaderName = m.shader.name;
                            var newShader = Shader.Find(shaderName);
                            if (newShader != null)
                            {
                                m.shader = newShader;
                                Debug.Log("refresh material success");
                            }
                            else
                            {
                                Debug.LogWarning("unable to refresh shader: " + shaderName + " in material " + m.name);
                            }
                        }
                    }

                    BUNDLES.Add(pUrl + queue[i].nameModule, loader.result);
                }
                percent += queue[i].Percent;
                DOTween.To(() => process.fillAmount, x => process.fillAmount = x, percent, 0.2f);
            }

            Instantiate((GameObject)BUNDLES[pUrl + "resources/scripttableobject/container"].LoadAsset("GameManager"));
            yield return new WaitForSeconds(0.25f);
            Instantiate((GameObject)BUNDLES[pUrl + "resources/prefab/ui"].LoadAsset("HUD"),transform);
            yield return new WaitForSeconds(0.25f);

            process.fillAmount = 0;
            loadingcontent.text = "Loading In Game";
            var pScenes = BUNDLES[pUrl + "resources/scenes"].GetAllScenePaths();
            foreach (var pScene in pScenes)
            {
                if (pScene.EndsWith("Main.unity"))
                {
                    loadScene(pScene);
                }
            }

        }

        int checkExistDashBoard(List<ModuleAssetInfo> pList, string pName)
        {
            for (int i = 0; i < pList.Count; ++i)
            {
                if (pList[i].nameModule == pName)
                {
                    return i;
                }
            }
            return -1;
        }

        private void Update()
        {
            if(objectPlanInstiate.Count > 0)
            {
                for(int i = objectPlanInstiate.Count-1; i >= 0; i--)
                {
                    Instantiate(objectPlanInstiate[i], transform);
                    objectPlanInstiate.RemoveAt(i);
                }
            }
            if (isStart)
            {
                if (!async.isDone)
                {
                    if (process)
                    {
                        DOTween.To(() => process.fillAmount, x => process.fillAmount = x, async.progress, 0.25f);
                    }
                }
                else
                {
                    Sequence pSeq = DOTween.Sequence();

                    pSeq.Append(DOTween.To(() => process.fillAmount, x => process.fillAmount = x, 1, 0.25f));
                    pSeq.AppendCallback(delegate
                    {

                        isStart = false;
                        complete();
                    });
                }
            }
        }
    }
}
