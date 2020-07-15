using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.Events;
using DG.Tweening;
using EasyMobile;
using EazyEngine.Space.UI;
using I2.Loc;
using Firebase;
using Firebase.Analytics;
using System.Threading.Tasks;
using MK.Glow.Legacy;
using Facebook.Unity;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class EventFloat : UnityEvent<float>
    {

    }

    public enum StateLoadingGame
    {
        PoolFirst,
        PoolAfter
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
        public UIElement boxlostConnection;
        public GameObject loadingAds;
        public int loadObjectPerFrame = 2;
        [SerializeField]
        private Vector2 resolutionDefault = new Vector2(1080, 1920);
        AsyncOperation async;
        protected ResourceRequest reuestGameManager = null, requestHUD = null;
        protected AsyncOperation requestState = null;
        protected List<GameObject> loadObjectAsync = new List<GameObject>();
        bool isStart = false;
        [System.NonSerialized]
        public string currentScene = "Home";
        [System.NonSerialized]
        public string previousScene = "Home";
        public static Dictionary<string, AssetBundle> BUNDLES = new Dictionary<string, AssetBundle>();
        private bool loadState = false;
        [System.NonSerialized]
        public List<SimpleObjectPooler> poolRegisterLoading = new List<SimpleObjectPooler>();
        protected int nextIndexFirstPool;
        [System.NonSerialized]
        public StateLoadingGame stateLoading;
        IEnumerator preloadPool()
        {
            while (nextIndexFirstPool < poolRegisterLoading.Count)
            {
                yield return new WaitForEndOfFrame();
                poolRegisterLoading[nextIndexFirstPool].FillObjectNow();
                loadingDirty(StateLoadingGame.PoolFirst);
                nextIndexFirstPool++;
            }
        }
        public void loadScene(string pScene)
        {
            if (pScene != currentScene)
            {
                previousScene = currentScene;
            }
            currentScene = pScene;
            SoundManager.Instance.StopAllCoroutines();
            //   fadeLayout.alpha = 0;
            isLoading = true;
            if (currentScene.Contains("Zone") && !LoadState)
            {
                DOTween.To(() => fadeLayout.alpha, a => fadeLayout.alpha = a, 1, 0.25f);
                LoadState = true;
                string stringState = GameManager.Instance.isFree ? "Statesfree" : "States" + GameManager.Instance.ChoosedLevel + "_" + GameManager.Instance.ChoosedHard;
                requestState = LoadAssets.loadAssetAsync<GameObject>(stringState, "Variants/States/");
            }
            else
            {
                Sequence pSeq = DOTween.Sequence();
                if (process && process.fillAmount >= 1)
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

        }

        protected float blockScene = 0, maxBlock = 0;
        protected Tween tween = null;
        [System.NonSerialized]
        public bool isLoading = false;
        public void addloading(int pQuantity)
        {
            blockScene += pQuantity;
            maxBlock = blockScene;
        }

        public void loadingDirty(StateLoadingGame pState)
        {
            blockScene--;
            if (tween != null)
            {
                tween.Kill();
                tween = null;
            }
            if (pState == StateLoadingGame.PoolFirst)
            {
                tween = DOTween.To(() => process.fillAmount, x => process.fillAmount = x, (1 - (blockScene / maxBlock)) * 0.2f + 0.6f, 0.25f);
                if (blockScene <= 0)
                {
                    for (int i = 0; i < poolRegisterLoading.Count; ++i)
                    {
                        poolRegisterLoading[i].FillRemain();
                        StartCoroutine(poolRegisterLoading[i].delayCheckSpawnPool());
                    }
                }
                stateLoading = StateLoadingGame.PoolAfter;
            }
            else if (pState == StateLoadingGame.PoolAfter)
            {
                tween = DOTween.To(() => process.fillAmount, x => process.fillAmount = x, (1 - (blockScene / maxBlock)) * 0.2f + 0.8f, 0.25f);
                if (blockScene == 0)
                {
                    complete();
                }
            }

        }

        protected Coroutine corountineFirstPool = null;
        private void AuthCallback(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                Debug.Log(aToken.UserId);
                // Print current access token's granted permissions
                foreach (string perm in aToken.Permissions)
                {
                    Debug.Log(perm);
                }
            }
            else
            {
                Debug.Log("User cancelled login");
            }
        }
        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
                if (!FB.IsLoggedIn)
                {
                    var perms = new List<string>() { "public_profile", "email" };
                    FB.LogInWithReadPermissions(perms, AuthCallback);
                }
       
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {

        }
        public void complete()
        {

            fadeLayout.alpha = 1;
            if (blockScene <= 0)
            {
                isLoading = false;
                if (currentScene.Contains("Zone"))
                {
                    if (LevelManger.InstanceRaw)
                    {
                        LevelManger.Instance.StartGame();
                    }
                }
                Sequence pSeq = DOTween.Sequence();
                if (currentScene.Contains("Zone"))
                {
                    pSeq.AppendInterval(0.25f);
                    pSeq.Append(DOTween.To(() => fadeLayout.alpha, a => fadeLayout.alpha = a, 0, 1));
                }
                pSeq.AppendCallback(delegate
                {
                    camera.GetComponent<CropCamera>().clearRender();
                    SoundManager.Instance.cleanAudio();
                    if (currentScene.Contains("Main"))
                    {
                        if (dirtyBloomMK <= 0)
                        {
                            if (mk)
                            {
                                mk.enabled = false;
                                mk.GetComponent<Camera>().allowHDR = false;
                            }
                        }
#if !UNITY_STANDALONE 
                        if (!RuntimeManager.IsInitialized())
                        {
                            if (PlayerPrefs.GetInt("firstGameFireBase", 0) == 1)
                            {
                                PlayerPrefs.SetInt("firstGameFireBase", 2);
                                EazyAnalyticTool.LogEvent("InitFirstGameDone");
                            }
                            EazyAnalyticTool.LogEvent("InitGameDone");
                            RuntimeManager.Init();
                        }
                        bool isInitialized = InAppPurchasing.IsInitialized();
                        if (!isInitialized)
                        {
                            InAppPurchasing.InitializePurchasing();
                        }
               
                        //if (!FB.IsInitialized)
                        //{
                        //    // Initialize the Facebook SDK
                        //    FB.Init(InitCallback, OnHideUnity);
                        //}
                        //else
                        //{
                        //    // Already initialized, signal an app activation App Event
                        //    FB.ActivateApp();
                        //    if (!FB.IsLoggedIn)
                        //    {
                        //        var perms = new List<string>() { "public_profile", "email" };
                        //        FB.LogInWithReadPermissions(perms, AuthCallback);
                        //    }
                        //}
                        //if (!AudienceNetwork.AudienceNetworkAds.IsInitialized())
                        //{
                        //    AudienceNetwork.AudienceNetworkAds.Initialize();
                        //}
#endif
                    }
                    else
                    {
                        EazyAnalyticTool.LogEvent("LoadLevelComplete");
                        EzEventManager.TriggerEvent(new MessageGamePlayEvent("LoadLevelComplete"));
                    }
                });
                 if (currentScene.Contains("Main"))
                {
                    pSeq.AppendInterval(0.25f);
                    pSeq.Append(DOTween.To(() => fadeLayout.alpha, a => fadeLayout.alpha = a, 0, 1));
                    pSeq.AppendCallback(delegate { Debug.Log("end load main"); });
                }
                pSeq.Play();
            }
            else if (corountineFirstPool == null)
            {
                nextIndexFirstPool = 0;
                stateLoading = StateLoadingGame.PoolFirst;
                corountineFirstPool = StartCoroutine(preloadPool());
            }


        }
        public IEnumerator delayAction(float pDelay, System.Action pAction)
        {
            yield return new WaitForSeconds(pDelay);
            pAction();
        }
        public void loadAllGame()
        {
            if (SceneManager.Instance.isLocal)
            {
                //   DOTween.To(() => fadeLayout.alpha, a => fadeLayout.alpha = a, 1, 0.25f);


                Sequence pSeq = DOTween.Sequence();
                if (process)
                {
                    process.fillAmount = 0;
                }
                pSeq.Append(DOTween.To(() => fadeLayout.alpha, a => fadeLayout.alpha = a, 1, 0.25f));
                pSeq.AppendCallback(delegate ()
                {
                    reuestGameManager = Resources.LoadAsync<GameObject>("Variants/Database/GameManager");
                });

                pSeq.Play();
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
                    StartCoroutine(loadManager(true));
                });

                pSeq.Play();
            }
        }
        protected int dirtySlowFps = 0;
        [System.NonSerialized]
        public List<MKGlow> _mk = new List<MKGlow>();
        public MKGlow mk
        {
            get
            {
                return (_mk.Count == 0  || _mk[_mk.Count - 1].IsDestroyed() ) ? null : _mk[_mk.Count - 1];
            }
        }
        
        public void addMK(MKGlow pMK)
        {
            foreach( var ppMK in _mk)
            {
                ppMK.enabled = false;
            }
            _mk.Add(pMK);
            updateMK();
        }
        public void removeMK(MKGlow pMK)
        {
            _mk.Remove(pMK);
            updateMK();
        }
        public void markDirtySlowFps()
        {
            dirtySlowFps++;
            if (dirtySlowFps >= 1)
            {
                Application.targetFrameRate = 60;
                Debug.Log("FPS" + Application.targetFrameRate);
            }
        }
        public void removeDirtySlowFps()
        {
            dirtySlowFps--;
            if (dirtySlowFps <= 0)
            {
                // Application.targetFrameRate = 30;
                Debug.Log("FPS" + Application.targetFrameRate);
            }
        }
        protected int dirtyBloomMK = 0;
        public void updateMK()
        {
            if (mk == null) return;
            mk.enabled = dirtyBloomMK > 0 && MK.Glow.Compatibility.IsSupported;
            mk.GetComponent<Camera>().allowHDR = dirtyBloomMK > 0 && MK.Glow.Compatibility.IsSupported;
        }
        public void markDirtyBloomMK()
        {
            dirtyBloomMK++;
            if (dirtyBloomMK >= 1)
            {
                if (mk)
                {
                    mk.enabled = true;
                    mk.GetComponent<Camera>().allowHDR = true;
                }
            }
        }
        public void removeDirtyBloomMK()
        {
            dirtyBloomMK--;
            if (dirtyBloomMK <= 0)
            {
                if (mk)
                {
                    mk.enabled = false;
                    mk.GetComponent<Camera>().allowHDR = false;
                }
            }
        }

      
        protected override void Awake()
        {
         
            base.Awake();
            PlayerPrefs.SetString("ItemUsed", "");
            EazyAnalyticTool.Init(delegate {
            });
            
            FB.Init();
            MK.Glow.Resources.LoadResourcesAsset();
#if UNITY_IOS
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
#if !UNITY_STANDALONE 
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                if(PlayerPrefs.GetInt("firstGameFireBase", 0) == 0)
                {
                    PlayerPrefs.SetInt("firstGameFireBase", 1);
                    EazyAnalyticTool.LogEvent("InitFirstGame");
                }
                EazyAnalyticTool.LogEvent("InitGame");
            });
#endif
            Application.targetFrameRate = 60;
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            if (GameManager._instance)
            {
                if (!GameManager._instance.IsDestroyed())
                {
                    DestroyImmediate(GameManager._instance);
                }
                GameManager._instance = null;
            }
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        protected Coroutine corountineNotice;
        protected string lastAssetLoaded;
        public IEnumerator loadManager(bool pNewVer)
        {
            AssetBundleLoader loader = new AssetBundleLoader();
            string pTag = "ui/uri_assetbundle";
#if UNITY_ANDROID 
            pTag = "ui/uri_assetbundle_android";
#endif
#if UNITY_IOS && !UNITY_EDITOR
             pTag = "ui/uri_assetbundle_ios";
#endif
            LoadAssetBundleStatus status = LoadAssetBundleStatus.NEW;
            if (corountineNotice != null)
            {
                StopCoroutine(corountineNotice);
            }
            //30s check show box lost connection
            corountineNotice = StartCoroutine(delayAction(5, delegate
            {
                boxlostConnection.show();
            }));
            lastAssetLoaded = I2.Loc.LocalizationManager.GetTranslation(pTag) + "assetmanager";
            yield return loader.DownloadAndCache(I2.Loc.LocalizationManager.GetTranslation(pTag), "assetmanager", pNewVer, (LoadAssetBundleStatus pNew, AssetBundle pBundle) =>
                  {
                      status = pNew;
                  });
            if (status == LoadAssetBundleStatus.LOST_CONNECT_NOT_HAVE_CACHE)
            {
                boxlostConnection.show();
                yield break;
            }
            else
            {
                if (boxlostConnection.gameObject.activeSelf)
                {
                    boxlostConnection.close();
                }
            }
            if (corountineNotice != null)
            {
                StopCoroutine(corountineNotice);
                corountineNotice = null;
            }
            if (loader.result != null)
            {
                var pManager = loader.result.LoadAsset<AssetbundleManager>("AssetbundleManager");
                if (float.Parse(pManager.currentModule.version) > float.Parse(Application.version) && status == LoadAssetBundleStatus.NEW)
                {
                    loader.result.Unload(true);
                    yield return loadManager(false);
                    yield break;
                }
                List<ModuleAssetInfo> queue = new List<ModuleAssetInfo>();


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

                yield return loadModules(queue, status == LoadAssetBundleStatus.NEW);
            }
        }

        public void closeBoxLostConnect()
        {
            boxlostConnection.close();
            if (corountineNotice != null)
            {
                StopCoroutine(corountineNotice);
            }

            corountineNotice = StartCoroutine(delayAction(10, delegate { boxlostConnection.show(); }));
        }
        public void refreshload()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            StopAllCoroutines();
            if (process)
            {
                process.fillAmount = 0;
            }
            StartCoroutine(loadManager(true));
        }

        public IEnumerator tryReconect(float pSec, string pUrl, string pAsset, System.Action<LoadAssetBundleStatus, AssetBundle> pOnResult)
        {
            yield return new WaitForSeconds(pSec);
            AssetBundleLoader loader = new AssetBundleLoader();
            LoadAssetBundleStatus pStatusSub = LoadAssetBundleStatus.NEW;
            yield return loader.DownloadAndCache(pUrl, pAsset, true, (LoadAssetBundleStatus pStatus, AssetBundle pBundle) =>
               {
                   pStatusSub = pStatus;
                   pOnResult?.Invoke(pStatus, pBundle);
               });
            if (pStatusSub != LoadAssetBundleStatus.LOST_CONNECT_NOT_HAVE_CACHE)
            {
                yield break;
            }
            yield return tryReconect(pSec, pUrl, pAsset, pOnResult);
        }
        List<GameObject> objectPlanInstiate = new List<GameObject>();
        public IEnumerator loadModules(List<ModuleAssetInfo> queue, bool pNew)
        {
            float percent = 0;
            string pTag = "ui/uri_assetbundle";
#if UNITY_ANDROID 
            pTag = "ui/uri_assetbundle_android";
#endif
#if UNITY_IOS && !UNITY_EDITOR
             pTag = "ui/uri_assetbundle_ios";
#endif
            var pUrl = I2.Loc.LocalizationManager.GetTranslation(pTag);
            int pCountDownload = 0;
            for (int i = 0; i < queue.Count; ++i)
            {
                if (queue[i].disableDownload) continue;
                pCountDownload++;
                AssetBundleLoader loader = new AssetBundleLoader();
                loadingcontent.text = "Loading Module " + queue[i].nameDisplay;
                if (corountineNotice != null)
                {
                    StopCoroutine(corountineNotice);
                }

                //30s check show box lost connection
                corountineNotice = StartCoroutine(delayAction(10, delegate
                {
                    boxlostConnection.show();
                }));
                LoadAssetBundleStatus status = LoadAssetBundleStatus.NEW;
                lastAssetLoaded = pUrl + queue[i].nameModule;
                var pLoader = loader;
                var pModelAssetInfo = queue[i];
                StartCoroutine(loader.DownloadAndCache(pUrl, queue[i].nameModule, pNew, (LoadAssetBundleStatus pStatus, AssetBundle pBundle) =>
                 {
                     pCountDownload--;
                     //status = pStatus;
                     if (pStatus == LoadAssetBundleStatus.LOST_CONNECT_NOT_HAVE_CACHE)
                     {
                         boxlostConnection.show();
                     }

                     if (pLoader.result != null && !BUNDLES.ContainsKey(pUrl + pModelAssetInfo.nameModule))
                     {
                         BUNDLES.Add(pUrl + pModelAssetInfo.nameModule, pLoader.result);
                     }
                     percent += pModelAssetInfo.Percent;
                     DOTween.To(() => process.fillAmount, x => process.fillAmount = x, percent, 0.2f);
                 }));

            }
            while (pCountDownload > 0)
            {
                yield return new WaitForEndOfFrame();
            }
            if (corountineNotice != null)
            {
                StopCoroutine(corountineNotice);
                corountineNotice = null;
            }
            yield return new WaitForSeconds(0.25f);
            if (BUNDLES.ContainsKey(pUrl + "resources/scripttableobject/container"))
            {
                Instantiate((GameObject)BUNDLES[pUrl + "resources/scripttableobject/container"].LoadAsset("GameManager"));
            }
            else
            {
                reuestGameManager = Resources.LoadAsync<GameObject>("Variants/Database/GameManager");
            }

            yield return new WaitForSeconds(0.25f);
            if (BUNDLES.ContainsKey(pUrl + "resources/prefab/ui"))
            {
                Instantiate((GameObject)BUNDLES[pUrl + "resources/prefab/ui"].LoadAsset("HUD"), transform);
            }
            else
            {
                requestHUD = Resources.LoadAsync<GameObject>("Variants/prefabs/ui/HUD");
            }
            yield return new WaitForSeconds(0.25f);

            process.fillAmount = 0;
            loadingcontent.text = "Loading In Game";
            if (BUNDLES.ContainsKey(pUrl + "resources/scenes"))
            {
                var pScenes = BUNDLES[pUrl + "resources/scenes"].GetAllScenePaths();
                foreach (var pScene in pScenes)
                {
                    if (pScene.EndsWith("Main.unity"))
                    {
                        loadScene(pScene);
                    }
                }
            }
            else
            {

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
        public GameObject cacheStatePreload;
        public void ResetGame()
        {
            poolRegisterLoading.Clear();
            corountineFirstPool = null;
            LoadState = false;
            stateLoading = StateLoadingGame.PoolFirst;
        }
        public bool LoadState { get => loadState; set => loadState = value; }
        public Vector2 ResolutionDefault { get => resolutionDefault; set => resolutionDefault = value; }
        protected AssetLoaderManager ManagerAsset { get => managerAsset ? managerAsset : managerAsset = GetComponent<AssetLoaderManager>(); set => managerAsset = value; }

        private AssetLoaderManager managerAsset;
        private void Update()
        {
            if (objectPlanInstiate.Count > 0)
            {
                for (int i = objectPlanInstiate.Count - 1; i >= 0; i--)
                {
                    Instantiate(objectPlanInstiate[i], transform);
                    objectPlanInstiate.RemoveAt(i);
                }
            }
            if (requestState != null)
            {
                if (requestState.isDone)
                {
                    var pTypeAsync = requestState.GetType();
                    cacheStatePreload = pTypeAsync == typeof(AssetBundleRequest) ? (GameObject)((AssetBundleRequest)requestState).asset : (GameObject)((ResourceRequest)requestState).asset;
                    //var pState = (GameObject)requestState.asset;
                    //List<GameObject> pLoadObjects = new List<GameObject>();
                    //var pPools = pState.GetComponentsInChildren<IPool>();
                    //foreach (var pPool in pPools)
                    //{
                    //    pPool.GameObjectPools(pLoadObjects);
                    //}
                    //foreach (var pGameObject in pLoadObjects)
                    //{
                    //    Instantiate(pGameObject);
                    // }
                    requestState = null;
                    loadScene(currentScene);
                }
                else
                {
                    DOTween.To(() => process.fillAmount, x => process.fillAmount = x, requestState.progress * 0.2f, 0.25f);
                }
            }
            if (requestHUD != null)
            {
                if (requestHUD.isDone)
                {

                    Instantiate((GameObject)requestHUD.asset, transform);
                    requestHUD = null;
                    loadScene("SpaceJourney/Scene/variant/Main");
                }
                else
                {
                    DOTween.To(() => process.fillAmount, x => process.fillAmount = x, 0.2f + requestHUD.progress * 0.2f, 0.25f);
                }
            }
            if (reuestGameManager != null)
            {
                if (reuestGameManager.isDone)
                {

                    Instantiate((GameObject)reuestGameManager.asset);
                    reuestGameManager = null;
                    requestHUD = Resources.LoadAsync<GameObject>("Variants/prefabs/ui/HUD");
                }
                else
                {
                    DOTween.To(() => process.fillAmount, x => process.fillAmount = x, reuestGameManager.progress * 0.2f, 0.25f);
                }
            }
            if (isStart)
            {
                float pAnchor = currentScene.Contains("Zone") ? 0.4f : 0.1f;
                float pFrom = currentScene.Contains("Zone") ? 0.2f : 0.4f;
                if (!async.isDone)
                {
                    if (process)
                    {
                        DOTween.To(() => process.fillAmount, x => process.fillAmount = x, pFrom + async.progress * pAnchor, 0.25f);
                    }
                }
                else
                {
                    if (currentScene.Contains("Zone")) {
                        Sequence pSeq = DOTween.Sequence();

                        pSeq.Append(DOTween.To(() => process.fillAmount, x => process.fillAmount = x, pFrom + pAnchor, 0.25f));
                        pSeq.AppendCallback(delegate
                        {

                            isStart = false;
                            complete();
                        });
                    }
                    else
                    {
                        var percentJob = ManagerAsset.getPercentJob("Main");
                        var percent = ManagerAsset.getPercentJob("Main") * 0.5f;
                        if(percentJob >= 1)
                        {
                            Sequence pSeq = DOTween.Sequence();

                            pSeq.Append(DOTween.To(() => process.fillAmount, x => process.fillAmount = x, 1, 0.25f));
                            pSeq.AppendCallback(delegate
                            {

                                isStart = false;
                                complete();
                            });
                        }
                        else
                        {
                            DOTween.To(() => process.fillAmount, x => process.fillAmount = x, 0.5f + percent, 0.25f);
                        }
                       
                    }
                
                }
            }
        }
    }
}
