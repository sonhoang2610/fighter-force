using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using EazyEngine.Space;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using EasyMobile;
using UnityEngine.Purchasing;
using EazyEngine.Space.UI;
using System.Linq;
using EazyEngine.Tools.Space;
using Firebase;
using Firebase.Analytics;
using FlowCanvas.Nodes;
using Spine.Unity;
using Spine.Unity.Modules;
using System;
using UnityEngine.Networking;
using EazyEngine.Timer;
using EazyEngine.Audio;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
public enum PositionADS
{
    Reborn,
    OpenBox,
    OpenBoxInGame,
    Luckywheel,
    Shop,
    DaiLy,
    X2Reward
}

public static class StringKeyGuide
{
    public static string OpenPrepare = "FirstOpenPrepare";


    public static string FirstGuideItem = "FirstGuideItem";
    public static string FirstGuideSkill = "FirstGuideSkill";
    public static string StartGameNow = "StartGameNow";
}
public static class LoadAssets
{
    public static Dictionary<string, object> cacheAssets = new Dictionary<string, object>();
    public static T[] loadAssets<T>(string pPathDefault = "") where T : UnityEngine.Object
    {
        if (!SceneManager.Instance.isLocal && Application.isPlaying)
        {
            var pListBundle = AssetBundle.GetAllLoadedAssetBundles();
            List<T> pAll = new List<T>();
            foreach (var pBundle in pListBundle)
            {
                var pObject = pBundle.LoadAllAssets<T>();
                if (pObject != null)
                {
                    pAll.AddRange(pObject);
                }
            }
            if (pAll.Count > 0)
            {
                return pAll.ToArray();
            }
        }
        T[] pObjectss = Resources.LoadAll<T>(pPathDefault);
        return pObjectss;

    }
    public static AsyncOperation loadAssetAsync<T>(string pName, string path = "") where T : UnityEngine.Object
    {
        if (!SceneManager.Instance.isLocal)
        {
            var pListBundle = AssetBundle.GetAllLoadedAssetBundles();
            foreach (var pBundle in pListBundle)
            {
                var pNames = pBundle.GetAllAssetNames();
                foreach (var pLocalName in pNames)
                {
                    if (pLocalName.Contains(pName.ToLower()))
                    {
                        return pBundle.LoadAssetAsync<T>(pLocalName);
                    }
                }
            }
        }
        return Resources.LoadAsync<T>(path + pName);
    }
    public static bool tryGetRuntimeKey(this GameObject pAssetObject, out string pKey)
    {
        pKey = "";
        var pAddress = (pAssetObject).GetComponent<AdressableObject>();
        if (pAddress)
        {
            pKey = pAddress.uniqueID;
        }

        return !string.IsNullOrEmpty(pKey);
    }
    public static AsyncOperation loadAssetAsync<T>(this AssetSelectorRef pObjectRef) where T : UnityEngine.Object
    {
        return Resources.LoadAsync<T>(AddressableDatabase.Instance.loadPath(pObjectRef.runtimeKey));
    }

    public static T loadAsset<T>(string pName, string path = "") where T : UnityEngine.Object
    {
        if (cacheAssets.ContainsKey(pName))
        {
            if (cacheAssets[pName] != null)
            {
                if (cacheAssets[pName].GetType() == typeof(T))
                {
                    return (T)cacheAssets[pName];
                }
            }
            else
            {
                cacheAssets.Remove(pName);
            }
        }
        if (!SceneManager.Instance.isLocal && Application.isPlaying)
        {
            var pListBundle = AssetBundle.GetAllLoadedAssetBundles();
            foreach (var pBundle in pListBundle)
            {
                var pObject = pBundle.LoadAsset<T>(pName);
                if (pObject)
                {
                    cacheAssets.Add(pName, pObject);
                    return pObject;
                }
            }
            var pObjectFind = AssetBundle.FindObjectOfType<T>();
            if (pObjectFind)
            {
                cacheAssets.Add(pName, pObjectFind);
                return pObjectFind;
            }
        }

        T pObjectss = Resources.Load<T>(path + pName);
        cacheAssets.Add(pName, pObjectss);
        return pObjectss;

    }
    public static T loadAssetScripTableObject<T>(string pName, string path = "", bool isClone = false) where T : UnityEngine.ScriptableObject
    {
        if (cacheAssets.ContainsKey(pName))
        {
            if (cacheAssets[pName] != null)
            {
                if (cacheAssets[pName].GetType() == typeof(T))
                {
                    return (T)cacheAssets[pName];
                }
            }
            else
            {
                cacheAssets.Remove(pName);
            }
        }
        if (!SceneManager.Instance.isLocal && Application.isPlaying)
        {
            var pListBundle = AssetBundle.GetAllLoadedAssetBundles();
            foreach (var pBundle in pListBundle)
            {
                var pObject = pBundle.LoadAsset<T>(pName);
                if (pObject)
                {
                    if (isClone)
                    {
                        pObject = ScriptableObject.Instantiate<T>(pObject);

                    }
                    cacheAssets.Add(pName, pObject);
                    return pObject;
                }
            }
            var pObjectFind = AssetBundle.FindObjectOfType<T>();
            if (pObjectFind)
            {
                if (isClone)
                {
                    pObjectFind = ScriptableObject.Instantiate<T>(pObjectFind);

                }
                cacheAssets.Add(pName, pObjectFind);
                return pObjectFind;
            }
        }

        T pObjectss = Resources.Load<T>(path + pName);
        if (isClone)
        {
            pObjectss = ScriptableObject.Instantiate<T>(pObjectss);

        }
        cacheAssets.Add(pName, pObjectss);
        return pObjectss;

    }
    public static T loadAssetPath<T>(string pName) where T : UnityEngine.Object
    {
        if (cacheAssets.ContainsKey(pName))
        {
            if (cacheAssets[pName] != null)
            {
                if (cacheAssets[pName].GetType() == typeof(T))
                {
                    return (T)cacheAssets[pName];
                }
            }
            else
            {
                cacheAssets.Remove(pName);
            }
        }
        if (SceneManager.Instance.isLocal)
        {
            T pObjectss = Resources.Load<T>(pName);
            if (pObjectss != null)
            {
                if (cacheAssets.ContainsKey(pName))
                {
                    cacheAssets[pName] = pObjectss;
                }
                else
                {
                    cacheAssets.Add(pName, pObjectss);
                }
                return pObjectss;
            }
        }
        else
        {

        }
        return null;
    }

    public static ShopDatabase LoadShop(string target)
    {
        if (cacheAssets.ContainsKey(target))
        {
            return (ShopDatabase)cacheAssets[target];
        }
        var pShops = LoadAssets.loadAssets<ShopDatabase>("Variants/Database/Shop");

        foreach (var pShop in pShops)
        {
            if (!cacheAssets.ContainsKey(pShop.nameShop))
            {
                var pShopClone = ScriptableObject.Instantiate<ShopDatabase>(pShop);
                cacheAssets.Add(pShop.nameShop, pShopClone);
                if (pShopClone.nameShop == target)
                {
                    return pShopClone;
                }
            }
        }
        return null;
    }
}

public static class SerializeGameDataBase
{
    public static void EzSerializeData<T>(this T pInfo, Stream ms, ref BinaryFormatter formatter)
    {
        if (formatter == null)
        {
            formatter = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();



            AbilitySerialize ability = new AbilitySerialize();
            ss.AddSurrogate(typeof(AbilityInfo),
                            new StreamingContext(StreamingContextStates.All),
                            ability);
            PlaneInfoSerialize planeinfo = new PlaneInfoSerialize();
            ss.AddSurrogate(typeof(PlaneInfo),
                            new StreamingContext(StreamingContextStates.All),
                            planeinfo);
            BaseItemSerialize baseItem = new BaseItemSerialize();
            ss.AddSurrogate(typeof(BaseItemGame),
                            new StreamingContext(StreamingContextStates.All),
                            baseItem);
            MissionSerialize missionInfo = new MissionSerialize();
            ss.AddSurrogate(typeof(MissionItem),
                            new StreamingContext(StreamingContextStates.All),
                            missionInfo);
            SkillInfoSerialize skillInfo = new SkillInfoSerialize();
            ss.AddSurrogate(typeof(SkillInfo),
                            new StreamingContext(StreamingContextStates.All), skillInfo);
            GameDatabaseSerialize gameinfo = new GameDatabaseSerialize();
            ss.AddSurrogate(typeof(GameDatabase),
                            new StreamingContext(StreamingContextStates.All),
                            gameinfo);
            CharacterSerialize charInfo = new CharacterSerialize();
            ss.AddSurrogate(typeof(Character),
                            new StreamingContext(StreamingContextStates.All),
                            charInfo);
            SpriteSerialize spriteInfo = new SpriteSerialize();
            ss.AddSurrogate(typeof(Sprite),
                            new StreamingContext(StreamingContextStates.All),
                            spriteInfo);
            ItemGameSerialize iteomGameInfo = new ItemGameSerialize();
            ss.AddSurrogate(typeof(ItemGame),
                            new StreamingContext(StreamingContextStates.All),
                            iteomGameInfo);
            PackageInfoSerialize pack = new PackageInfoSerialize();
            ss.AddSurrogate(typeof(ItemPackage),
                            new StreamingContext(StreamingContextStates.All),
                            pack);
#if UNITY_EDITOR
            AudioSerialize audio = new AudioSerialize();
            ss.AddSurrogate(typeof(AudioClip),
                            new StreamingContext(StreamingContextStates.All),
                            audio);
#endif
            formatter.SurrogateSelector = ss;
        }
        // 2. Have the formatter use our surrogate selector

        formatter.Serialize(ms, pInfo);
    }
    public static T EzDeSerializeData<T>(this Stream ms, ref BinaryFormatter formatter)
    {
        if (formatter == null)
        {
            formatter = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();

            AbilitySerialize ability = new AbilitySerialize();
            ss.AddSurrogate(typeof(AbilityInfo),
                            new StreamingContext(StreamingContextStates.All),
                            ability);
            PlaneInfoSerialize planeinfo = new PlaneInfoSerialize();
            ss.AddSurrogate(typeof(PlaneInfo),
                            new StreamingContext(StreamingContextStates.All),
                            planeinfo);
            BaseItemSerialize baseItem = new BaseItemSerialize();
            ss.AddSurrogate(typeof(BaseItemGame),
                            new StreamingContext(StreamingContextStates.All),
                            baseItem);
            MissionSerialize missionInfo = new MissionSerialize();
            ss.AddSurrogate(typeof(MissionItem),
                            new StreamingContext(StreamingContextStates.All),
                            missionInfo);
            SkillInfoSerialize skillInfo = new SkillInfoSerialize();
            ss.AddSurrogate(typeof(SkillInfo),
                            new StreamingContext(StreamingContextStates.All), skillInfo);
            GameDatabaseSerialize gameinfo = new GameDatabaseSerialize();
            ss.AddSurrogate(typeof(GameDatabase),
                            new StreamingContext(StreamingContextStates.All),
                            gameinfo);
            CharacterSerialize charInfo = new CharacterSerialize();
            ss.AddSurrogate(typeof(Character),
                            new StreamingContext(StreamingContextStates.All),
                            charInfo);
            SpriteSerialize spriteInfo = new SpriteSerialize();
            ss.AddSurrogate(typeof(Sprite),
                            new StreamingContext(StreamingContextStates.All),
                            spriteInfo);
            ItemGameSerialize iteomGameInfo = new ItemGameSerialize();
            ss.AddSurrogate(typeof(ItemGame),
                            new StreamingContext(StreamingContextStates.All),
                            iteomGameInfo);
            PackageInfoSerialize pack = new PackageInfoSerialize();
            ss.AddSurrogate(typeof(ItemPackage),
                            new StreamingContext(StreamingContextStates.All),
                            pack);
#if UNITY_EDITOR
            AudioSerialize audio = new AudioSerialize();
            ss.AddSurrogate(typeof(AudioClip),
                            new StreamingContext(StreamingContextStates.All),
                            audio);
#endif
            // 2. Have the formatter use our surrogate selector
            formatter.SurrogateSelector = ss;
        }
        ms.Position = 0;
        return (T)formatter.Deserialize(ms);
    }
    public static T CloneData<T>(this T pInfo)
    {
        using (var ms = new MemoryStream())
        {
            BinaryFormatter formatter = null;
            pInfo.EzSerializeData(ms, ref formatter);
            return EzDeSerializeData<T>(ms, ref formatter);
        }
    }
}

public static class GameConstraint
{

}
[System.Serializable]
public class prefabBulletGroup
{
    public List<GameObject> prefabBullet = new List<GameObject>();
}
public enum ScheduleUIMain
{
    NONE,
    REPLAY,
    UPGRADE
}

public enum TimerState
{
    Start,
    Complete,
    Running
}
public struct EventTimer
{
    public TimerState state;
    public string key;
}
public enum ResultStatusAds
{
    Success,
    Failed,
    TimeOut
}
public class GameManager : PersistentSingleton<GameManager>, EzEventListener<GameDatabaseInventoryEvent>
{
    [ContextMenu("hackk")]
    public void hackk()
    {
        Database.getComonItem("BoosterCollect").Quantity = 0;
    }
    public float frameTarget = 60;
    [System.NonSerialized]
    public List<SubAssetInfo> loadedAssets = new List<SubAssetInfo>();
    public List<AssetSelectorRef> objectRefExcludes;

    public prefabBulletGroup[] groupPrefabBullet;
    [System.NonSerialized]
    public ScheduleUIMain scehduleUI = ScheduleUIMain.NONE;
    [System.NonSerialized]
    public bool inGame = false;
    [System.NonSerialized]
    public int lastResultWin = -1;
    public AudioClip btnSfx;
    public MovingLeader leaderTemplate;



    public prefabBulletGroup getGroupPrefab(GameObject pObject)
    {
        for (int i = 0; i < groupPrefabBullet.Length; ++i)
        {
            if (groupPrefabBullet[i].prefabBullet.Contains(pObject))
            {
                return groupPrefabBullet[i];
            }
        }
        return null;
    }
    public bool loadFromResources = true;
    public bool isPlaying = false;
    [Sirenix.OdinInspector.ReadOnly]
    public LevelContainer container;

    public int CurrentLevelUnlock
    {
        get
        {
            int levelUnlock = 0;
            for (int i = 0; i < container.levels.Count; ++i)
            {
                if (container.levels[i].level > levelUnlock && !container.levels[i].isLocked)
                {
                    levelUnlock = container.levels[i].level;
                }
            }
            if (levelUnlock == 0)
            {
                for (int i = 0; i < container.levels.Count; ++i)
                {
                    if (container.levels[i].level == 1 && container.levels[i].hard == 0)
                    {
                        container.levels[i].isLocked = false;
                    }
                }
                levelUnlock = 1;
            }
            return levelUnlock;
        }
        set
        {
            if (value > CurrentLevelUnlock)
            {
                for (int i = 0; i < container.levels.Count; ++i)
                {
                    if (container.levels[i].level == value && container.levels[i].hard == 0)
                    {
                        container.levels[i].isLocked = false;
                    }
                }
            }
        }
    }
    protected int choosedLevel;
    protected int choosedHard;
    public int ChoosedLevel
    {
        get
        {
            return choosedLevel;
        }

        set
        {
            choosedLevel = value;
        }
    }
    public int ChoosedHard { get => choosedHard; set => choosedHard = value; }
    [System.NonSerialized]
    [ShowInInspector]
    [HideInEditorMode]
    public GiftOnlineModule giftOnlineModule;
    [System.NonSerialized]
    [ShowInInspector]
    [HideInEditorMode]
    public DailyGiftModule dailyGiftModule;
    [System.NonSerialized]
    [ShowInInspector]
    [HideInEditorMode]
    public GameDatabase databaseGame;
    public int wincount = 0;
    [System.NonSerialized]
    public bool isFree = false;
    [System.NonSerialized]
    public bool isGuide = true;
    [System.NonSerialized]
    public string freePlaneChoose = "";
    [System.NonSerialized]
    public string freeSpPlaneChoose = "";

    public List<GameObject> pendingObjects = new List<GameObject>();

    protected int countPending;
    protected override void Awake()
    {
        base.Awake();
        databaseGame = GameDatabase.Instance;
        Application.targetFrameRate = (int)frameTarget;
        if (_databaseInstanced == null)
        {
            LoadGame();
            LoadAllLevel();
        }
        //     StartCoroutine(delayAction(0.2f, spawnPool));
        // Database = Instantiate(Database);
    }
    //public void spawnPool()
    //{

    //    while (loadSequences.Count > 0)
    //    {

    //        if (loadSequences[loadSequences.Count - 1].count > 0)
    //        {
    //            var pool = loadSequences[loadSequences.Count - 1];
    //            pendingObject = pool.pooler.AddOneObjectToThePoolRemainTime(false);
    //            pool.count--;
    //            loadSequences[loadSequences.Count - 1] = pool;

    //            break;
    //        }
    //        else
    //        {
    //            loadSequences.RemoveAt(loadSequences.Count - 1);
    //        }
    //    }
    //    StartCoroutine(delayAction(0.2f, spawnPool));

    //}

    IEnumerator checkTimePackage()
    {
        yield return new WaitForSeconds(1);
        if (ExtraPackageDatabase.Instance != null)
        {
            ExtraPackageDatabase.Instance.Update();
        }
        yield return checkTimePackage();
        //for(int i =  0; i < ExtraPackageDatabase.Instance.packageRegister.Count; ++i)
        //{

        //}
        //System.DateTime.Now - GameManager.Instance.Database.PackageInfo[]
    }
    public void initGame()
    {
        if (GameManager.Instance.CurrentLevelUnlock >= 7)
        {
            StartCoroutine(checkTimePackage());
        }
        if (PlayerPrefs.GetInt("firstGame", 0) == 5)
        {
            var pSke = gameObject.AddComponent<SkeletonMecanim>();
            var pSke1 = gameObject.AddComponent<SkeletonRendererCustomMaterials>();
            pSke1.setPropertyFloat("asd", 0);
            var pSke2 = gameObject.AddComponent<SkeletonUtilityBone>();
            var pSke3 = gameObject.AddComponent<SkeletonUtility>();
            var pSke4 = gameObject.AddComponent<BoneFollower>();
            var pSke5 = gameObject.AddComponent<SkeletonRenderSeparator>();
        }
        if (_databaseInstanced == null)
        {
            LoadGame();
            LoadAllLevel();
        }
        if (GameManager.Instance.CurrentLevelUnlock >= 7)
        {
            StartCoroutine(ExtraPackageDatabase.Instance.initIAPProduct());
        }

        StartCoroutine(delayAction(1, delegate ()
        {
            if (!InAppPurchasing.IsInitialized())
            {
                InAppPurchasing.InitializePurchasing();
            }
        }));

        if (_databaseInstanced != null && _databaseInstanced.spPlanes.Count > 0 && _databaseInstanced.planes.Count > 0)
        {
            SaveGameCache();
        }

        reCalCulateStar();
        var currentModuleGiftOnline = GameDatabase.Instance.databaseGiftOnline;
        giftOnlineModule = Database.getModuleGiftOnline(currentModuleGiftOnline.idModule);
        if (giftOnlineModule == null)
        {
            giftOnlineModule = new GiftOnlineModule() { calimedIndex = -1, id = currentModuleGiftOnline.idModule, onlineTime = 0 };
            Database.giftOnlineModules.Add(giftOnlineModule);
            Database.clearModules(currentModuleGiftOnline.moduleClearID);
        }

        var currentModuleDailyGift = GameDatabase.Instance.databaseDailyGift;
        dailyGiftModule = Database.getModuleGiftDaily(currentModuleDailyGift.idModule);
        if (dailyGiftModule == null)
        {
            dailyGiftModule = new DailyGiftModule() { lastDate = -1, id = currentModuleDailyGift.idModule, currentDay = -1 };
            Database.dailyGiftModules.Add(dailyGiftModule);
            Database.clearDailyModules(currentModuleDailyGift.moduleClearID);
        }
        for (int i = 0; i < GameManager.Instance.Database.timers.Count; ++i)
        {
            addTimer(GameManager.Instance.Database.timers[i]);
        }
    }
    bool first = true;
    float checkAds = 3;
    private void LateUpdate()
    {
        if (!Advertising.IsInterstitialAdReady() && checkAds <= 0)
        {
            Advertising.LoadInterstitialAd();
            checkAds = 3;
        }
        else if (Advertising.IsInterstitialAdReady())
        {
            checkAds = 0;
        }
        if (checkAds > 0)
        {
            checkAds -= Time.deltaTime;
        }

        if (first)
        {
            initGame();
            first = false;
            MissionContainer.Instance.LoadState();


        }
        for (int i = 0; i < pendingObjects.Count; ++i)
        {
            if (pendingObjects[i].activeSelf)
            {
                countPending++;
            }
        }
    }
    //[InlineEditor]
    //public GameDataBaseInstanceObject _databaseDefault;
    public GameDataBaseInstance _databaseDefault1;
    [System.NonSerialized]
    public GameDataBaseInstance _databaseInstanced = null;
    [ShowInInspector]
    public GameDataBaseInstance Database
    {
        get
        {
            if (_databaseInstanced == null)
            {
                LoadGame();
                LoadAllLevel();
            }
            return _databaseInstanced;
        }
    }

    public LevelConfig ConfigLevel { get => configLevel; set => configLevel = value; }
    public int Wincount
    {
        get => wincount; set
        {
            GameManager.Instance.Database.collectionDailyInfo.winCount += (value - wincount);
            GameManager.Instance.Database.collectionInfo.winCount += (value - wincount);
            wincount = value;
        }
    }

    protected FileStream fileSaveGame;

    protected bool saveGameAgain = false;

    protected float delaySave = 0;


    [Button("Save Game")]
    [HideInEditorMode]
    public void SaveGame()
    {
        if (delaySave > 0)
        {
            saveGameAgain = true;
            return;
        }
        else
        {
            saveGameAgain = false;
            delaySave = 0.1f;
        }
        try
        {

            string destination = Application.persistentDataPath + "/GameInfo.dat";
            if (File.Exists(destination)) fileSaveGame = File.OpenWrite(destination);
            else fileSaveGame = File.Create(destination);
            if (fileSaveGame != null)
            {
                //using (var memoryStream = new MemoryStream())
                //{
                // Serialize to memory instead of to file
                var formatter = new BinaryFormatter();
                SurrogateSelector ss = new SurrogateSelector();
                AbilitySerialize ability = new AbilitySerialize();
                ss.AddSurrogate(typeof(AbilityInfo),
                                new StreamingContext(StreamingContextStates.All),
                                ability);
                PlaneInfoSerialize planeinfo = new PlaneInfoSerialize();
                ss.AddSurrogate(typeof(PlaneInfo),
                                new StreamingContext(StreamingContextStates.All),
                                planeinfo);
                BaseItemSerialize baseItem = new BaseItemSerialize();
                ss.AddSurrogate(typeof(BaseItemGame),
                                new StreamingContext(StreamingContextStates.All),
                                baseItem);
                MissionSerialize missionInfo = new MissionSerialize();
                ss.AddSurrogate(typeof(MissionItem),
                                new StreamingContext(StreamingContextStates.All),
                                missionInfo);
                SkillInfoSerialize skillInfo = new SkillInfoSerialize();
                ss.AddSurrogate(typeof(SkillInfo),
                                new StreamingContext(StreamingContextStates.All), skillInfo);
                GameDatabaseSerialize gameinfo = new GameDatabaseSerialize();
                ss.AddSurrogate(typeof(GameDatabase),
                                new StreamingContext(StreamingContextStates.All),
                                gameinfo);
                CharacterSerialize charInfo = new CharacterSerialize();
                ss.AddSurrogate(typeof(Character),
                                new StreamingContext(StreamingContextStates.All),
                                charInfo);
                SpriteSerialize spriteInfo = new SpriteSerialize();
                ss.AddSurrogate(typeof(Sprite),
                                new StreamingContext(StreamingContextStates.All),
                                spriteInfo);
                ItemGameSerialize iteomGameInfo = new ItemGameSerialize();
                ss.AddSurrogate(typeof(ItemGame),
                                new StreamingContext(StreamingContextStates.All),
                                iteomGameInfo);
                PackageInfoSerialize pack = new PackageInfoSerialize();
                ss.AddSurrogate(typeof(ItemPackage),
                                new StreamingContext(StreamingContextStates.All),
                                pack);

                // 2. Have the formatter use our surrogate selector
                formatter.SurrogateSelector = ss;
                formatter.Serialize(fileSaveGame, _databaseInstanced);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            if (fileSaveGame != null)
            {
                fileSaveGame.Flush();
                fileSaveGame.Close();
                fileSaveGame = null;
            }
        }
    }
    public void SaveGameCache()
    {
        FileStream fileSave = null;
        try
        {
            string destination = Application.persistentDataPath + "/GameInfoCache.dat";
            if (File.Exists(destination)) fileSave = File.OpenWrite(destination);
            else fileSave = File.Create(destination);
            if (fileSave != null)
            {
                //using (var memoryStream = new MemoryStream())
                //{
                // Serialize to memory instead of to file
                var formatter = new BinaryFormatter();
                SurrogateSelector ss = new SurrogateSelector();
                AbilitySerialize ability = new AbilitySerialize();
                ss.AddSurrogate(typeof(AbilityInfo),
                                new StreamingContext(StreamingContextStates.All),
                                ability);
                PlaneInfoSerialize planeinfo = new PlaneInfoSerialize();
                ss.AddSurrogate(typeof(PlaneInfo),
                                new StreamingContext(StreamingContextStates.All),
                                planeinfo);
                BaseItemSerialize baseItem = new BaseItemSerialize();
                ss.AddSurrogate(typeof(BaseItemGame),
                                new StreamingContext(StreamingContextStates.All),
                                baseItem);
                MissionSerialize missionInfo = new MissionSerialize();
                ss.AddSurrogate(typeof(MissionItem),
                                new StreamingContext(StreamingContextStates.All),
                                missionInfo);
                SkillInfoSerialize skillInfo = new SkillInfoSerialize();
                ss.AddSurrogate(typeof(SkillInfo),
                                new StreamingContext(StreamingContextStates.All), skillInfo);
                GameDatabaseSerialize gameinfo = new GameDatabaseSerialize();
                ss.AddSurrogate(typeof(GameDatabase),
                                new StreamingContext(StreamingContextStates.All),
                                gameinfo);
                CharacterSerialize charInfo = new CharacterSerialize();
                ss.AddSurrogate(typeof(Character),
                                new StreamingContext(StreamingContextStates.All),
                                charInfo);
                SpriteSerialize spriteInfo = new SpriteSerialize();
                ss.AddSurrogate(typeof(Sprite),
                                new StreamingContext(StreamingContextStates.All),
                                spriteInfo);
                ItemGameSerialize iteomGameInfo = new ItemGameSerialize();
                ss.AddSurrogate(typeof(ItemGame),
                                new StreamingContext(StreamingContextStates.All),
                                iteomGameInfo);
                PackageInfoSerialize pack = new PackageInfoSerialize();
                ss.AddSurrogate(typeof(ItemPackage),
                                new StreamingContext(StreamingContextStates.All),
                                pack);

                // 2. Have the formatter use our surrogate selector
                formatter.SurrogateSelector = ss;
                formatter.Serialize(fileSave, _databaseInstanced);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            if (fileSave != null)
            {
                fileSave.Flush();
                fileSave.Close();
                fileSave = null;
            }
        }
    }
    private void OnDestroy()
    {
        PlayerPrefs.SetString("ItemUsed", "");
        if (fileSaveGame != null)
        {
            fileSaveGame.Close();
            fileSaveGame = null;
        }

        int pGameStep = PlayerPrefs.GetInt("firstGame", 0);
        if (pGameStep < 3 && Application.isPlaying)
        {
            PlayerPrefs.SetInt("firstGame", 3);
        }
    }
    [Button("Load Game")]
    [HideInEditorMode]
    public void LoadGame()
    {
        // UnityEditor.Undo.RecordObject(Database, "loadgame");
        FileStream file = null;
        string destination = Application.persistentDataPath + "/GameInfo.dat";
        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
            if (file != null)
            {

                var formatter = new BinaryFormatter();
                SurrogateSelector ss = new SurrogateSelector();
                AbilitySerialize ability = new AbilitySerialize();
                ss.AddSurrogate(typeof(AbilityInfo),
                                new StreamingContext(StreamingContextStates.All),
                                ability);
                PlaneInfoSerialize planeinfo = new PlaneInfoSerialize();
                ss.AddSurrogate(typeof(PlaneInfo),
                                new StreamingContext(StreamingContextStates.All),
                                planeinfo);
                BaseItemSerialize baseItem = new BaseItemSerialize();
                ss.AddSurrogate(typeof(BaseItemGame),
                                new StreamingContext(StreamingContextStates.All),
                                baseItem);
                MissionSerialize missionInfo = new MissionSerialize();
                ss.AddSurrogate(typeof(MissionItem),
                                new StreamingContext(StreamingContextStates.All),
                                missionInfo);
                SkillInfoSerialize skillInfo = new SkillInfoSerialize();
                ss.AddSurrogate(typeof(SkillInfo),
                                new StreamingContext(StreamingContextStates.All), skillInfo);
                GameDatabaseSerialize gameinfo = new GameDatabaseSerialize();
                ss.AddSurrogate(typeof(GameDatabase),
                                new StreamingContext(StreamingContextStates.All),
                                gameinfo);
                CharacterSerialize charInfo = new CharacterSerialize();
                ss.AddSurrogate(typeof(Character),
                                new StreamingContext(StreamingContextStates.All),
                                charInfo);
                SpriteSerialize spriteInfo = new SpriteSerialize();
                ss.AddSurrogate(typeof(Sprite),
                                new StreamingContext(StreamingContextStates.All),
                                spriteInfo);
                ItemGameSerialize iteomGameInfo = new ItemGameSerialize();
                ss.AddSurrogate(typeof(ItemGame),
                                new StreamingContext(StreamingContextStates.All),
                                iteomGameInfo);
                PackageInfoSerialize pack = new PackageInfoSerialize();
                ss.AddSurrogate(typeof(ItemPackage),
                                new StreamingContext(StreamingContextStates.All),
                                pack);
                // 2. Have the formatter use our surrogate selector
                formatter.SurrogateSelector = ss;
                try
                {
                    file.Seek(0, SeekOrigin.Begin);
                    var pData = formatter.Deserialize(file);
                    _databaseInstanced = (GameDataBaseInstance)pData;
                    _databaseInstanced.ExtraInfo();
                }
                catch
                {
                    file.Flush();
                    file.Close();

                    LoadGameCache();

                    string destinationClone = Application.persistentDataPath + "/GameInfo.dat";
                    if (File.Exists(destinationClone))
                    {
                        File.Delete(destinationClone);
                    }
                    SaveGame();
                }
                finally
                {
                    file.Close();
                }
                if (_databaseInstanced == null || _databaseInstanced.spPlanes.Count == 0)
                {
                    LoadGameCache();
                    string destinationClone = Application.persistentDataPath + "/GameInfo.dat";
                    if (File.Exists(destinationClone))
                    {
                        File.Delete(destinationClone);
                    }


                    if (_databaseInstanced == null || _databaseInstanced.spPlanes.Count == 0)
                    {
                        _databaseInstanced = _databaseDefault1.CloneData();
                        _databaseInstanced.ExtraInfo();
                    }
                    SaveGame();
                }

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                }
#endif
            }
        }
        else
        {
            _databaseInstanced = _databaseDefault1.CloneData();
            _databaseInstanced.ExtraInfo();
            SaveGame();
        }

    }
    public void LoadGameCache()
    {
        // UnityEditor.Undo.RecordObject(Database, "loadgame");
        FileStream file = null;
        string destination = Application.persistentDataPath + "/GameInfoCache.dat";
        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
            if (file != null)
            {

                var formatter = new BinaryFormatter();
                SurrogateSelector ss = new SurrogateSelector();
                PlaneInfoSerialize planeinfo = new PlaneInfoSerialize();
                ss.AddSurrogate(typeof(PlaneInfo),
                                new StreamingContext(StreamingContextStates.All),
                                planeinfo);
                BaseItemSerialize baseInfo = new BaseItemSerialize();
                ss.AddSurrogate(typeof(BaseItemGame),
                                new StreamingContext(StreamingContextStates.All),
                                baseInfo);
                ItemGameSerialize iteomGameInfo = new ItemGameSerialize();
                ss.AddSurrogate(typeof(ItemGame),
                                new StreamingContext(StreamingContextStates.All),
                                iteomGameInfo);
                SkillInfoSerialize skill = new SkillInfoSerialize();
                ss.AddSurrogate(typeof(SkillInfo),
                                new StreamingContext(StreamingContextStates.All),
                                skill);
                PackageInfoSerialize pack = new PackageInfoSerialize();
                ss.AddSurrogate(typeof(ItemPackage),
                                new StreamingContext(StreamingContextStates.All),
                                pack);
                // 2. Have the formatter use our surrogate selector
                formatter.SurrogateSelector = ss;
                try
                {
                    file.Seek(0, SeekOrigin.Begin);
                    var pData = formatter.Deserialize(file);
                    _databaseInstanced = (GameDataBaseInstance)pData;
                    _databaseInstanced.ExtraInfo();
                }
                catch
                {
                }
                finally
                {
                    file.Close();
                }
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                }
#endif
            }
            if (GameManager.Instance.Database.firstOnline.ToUnixTime() < TimeExtension.ToUnixTime(new DateTime(2020, 7, 5)))
            {
                PlayerPrefs.SetInt("FirstGuideItem", 1);
                PlayerPrefs.SetInt("FirstGuideSkill", 1);
                PlayerPrefs.SetInt("FirstOpenPrepare", 1);
                PlayerPrefs.SetInt("OnlineGiftFirstPress", 1);
                PlayerPrefs.SetInt("FirstPressMission", 1);
                PlayerPrefs.SetInt("FirstPressDailyGift", 1);
                PlayerPrefs.SetInt("FirstPressUpgrade", 1);
            }
        }
        else
        {
            _databaseInstanced = _databaseDefault1.CloneData();
            _databaseInstanced.ExtraInfo();
            SaveGame();
        }

    }
    public List<TimeCountDown> timer = new List<TimeCountDown>();



    public void LoadAllLevel()
    {
        container = LoadFile<LevelContainer>("level_container.dat");
        for(int i  =0; i < container.levels.Count; ++i)
        {
            if(i < CurrentLevelUnlock && container.getLevelInfo(i + 1, 0).isLocked)
            {
                container.getLevelInfo(i + 1, 0).isLocked = false;
            }
        }
    }
    public void SaveLevel()
    {
        SaveFile<LevelContainer>("level_container.dat", container);
    }

    public void SaveFile<T>(string fileName, T pDataRaw) where T : class, new()
    {
        FileStream file = null;
        string destination = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else
        {
            file = File.Create(destination);
        }
        T pData = pDataRaw;
        if (pData == null)
        {
            pData = new T();
        }

        try
        {
            BinaryFormatter formatter = null;
            pData.EzSerializeData(file, ref formatter);
        }
        catch (SerializationException e)
        {
            Debug.Log(e.Message);
            throw;
        }
        finally
        {
            file.Close();
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
    public T LoadFile<T>(string fileName, bool createNew = true) where T : class, new()
    {
        FileStream file = null;
        T data = null;
        string destination = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
            if (file != null)
            {

                try
                {
                    BinaryFormatter formatter = null;
                    data = file.EzDeSerializeData<T>(ref formatter);
                }
                catch (SerializationException e)
                {
                    Debug.Log(e.Message);
                    throw;
                }
                finally
                {
                    file.Close();
                }
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                }
#endif
            }
        }
        else if (createNew)
        {
            SaveFile<T>(fileName, data = new T());
        }
        return data;
    }
    public ItemGame[] defaultTestItem;
    LevelConfig configLevel;

    [ContextMenu("Reset Data")]
    public void resetData()
    {
        File.Delete(Application.persistentDataPath + "/" + "level_container.dat");
        File.Delete(Application.persistentDataPath + "/" + "GameInfo.dat");
        PlayerPrefs.DeleteAll();
    }
    public void LoadLevel(int pIndex)
    {

        inGame = true;
        LevelManger.InstanceRaw = null;
        GroupManager.clearCache();
        PlayerEnviroment.clear();
        TimeKeeper.Instance.getTimer("Global").TimScale = 1;
        GameManager.Instance.showBannerAds(false);
        //  Database.selectedMainPlane = 6;
        if (!GameManager.Instance.isFree)
        {
            var pEnergy = GameManager.Instance.Database.getComonItem("Energy");
            if (pEnergy.quantity <= 0)
            {
                HUDLayer.Instance.showDialogNotEnoughMoney(pEnergy.item.displayNameItem.Value, delegate
                {
                    ShopManager.Instance.showBoxShop(pEnergy.item.categoryItem.ToString());
                    HUDLayer.Instance.BoxDialog.close();
                });
                return;
            }
            else
            {
                pEnergy.Quantity--;
            }
            Database.lastPlayStage = new Pos(pIndex, GameManager.Instance.ChoosedHard);
        }

        var pInfo = container.getLevelInfo(pIndex, GameManager.Instance.ChoosedHard).infos;
        pInfo.InputConfig = ConfigLevel;
        isPlaying = true;
        string pItemUsed = "";
        for (int i = 0; i < ConfigLevel.itemUsed.Count; ++i)
        {
            pItemUsed += ConfigLevel.itemUsed[i].itemID + ",";
            var pItem = GameManager.Instance.Database.getComonItem(ConfigLevel.itemUsed[i].itemID);
            if (!((ItemGame)pItem.item).isActive)
            {
                pItem.Quantity--;
            }
        }
        PlayerPrefs.SetString("ItemUsed", pItemUsed);
        SaveGame();

        TopLayer.Instance.block.gameObject.SetActive(true);
        Physics2D.autoSimulation = false;
        Time.timeScale = 1;
        EazyAnalyticTool.LogEvent("LoadLevelStart");
        StartCoroutine(delayAction(0.75f, delegate
        {
         
            MidLayer.Instance.boxPrepare.close();
            TopLayer.Instance.inGame(true);
            SceneManager.Instance.loadScene(GameDatabase.Instance.LevelScene(pIndex));
        }));

    }
    public IEnumerator delayAction(float pDelay, System.Action action)
    {
        yield return new WaitForSeconds(pDelay);
        action();
    }
    public bool CheckGuide(string id)
    {
        var double1 = TimeExtension.ToUnixTime(System.DateTime.Now);
        var double2 = TimeExtension.ToUnixTime(new DateTime(2020,7,5));
        return PlayerPrefs.GetInt(id, 0) == 0 && double1 >= double2;
    }
    public static string convertTime(int pTime)
    {
        if (pTime > 86400 || pTime <= 0)
        {
            return "";
        }
        return System.TimeSpan.FromSeconds(pTime).ToString(@"hh\:mm\:ss");
    }
    private void Update()
    {
        if (first) return;
        if (delaySave > 0)
        {
            delaySave -= Time.deltaTime;
            if (delaySave <= 0)
            {
                if (saveGameAgain)
                {
                    SaveGame();
                }
            }
        }
        for (int i = timer.Count - 1; i >= 0; --i)
        {

            if (timer[i].length <= 0) { timer.RemoveAt(i); continue; }
            var pSec = (System.DateTime.Now - timer[i].lastimeWheelFree).TotalSeconds;

            for (int g = timer[i].LabelTimer.Count - 1; g >= 0; g--)
            {
                var pLabel = timer[i].LabelTimer[g];
                if (pLabel.IsDestroyed() || pLabel == null) { timer[i].LabelTimer.RemoveAt(g); continue; }
                pLabel.text = System.TimeSpan.FromSeconds(timer[i].length - pSec).ToString(@"hh\:mm\:ss");
            }
            if (pSec >= timer[i].length)
            {
                var pOldTimer = timer[i];
                EzEventManager.TriggerEvent(new EventTimer()
                {
                    key = timer[i].key,
                    state = TimerState.Complete
                });
                GameManager.Instance.Database.reupdateTimerCount();
                if (timer.Contains(pOldTimer))
                {
                    EzEventManager.TriggerEvent(new EventTimer()
                    {
                        key = pOldTimer.key,
                        state = TimerState.Start
                    });
                }
            }
        }
        if (GameManager.Instance.Database != null)
        {
            giftOnlineModule.onlineTime += Time.deltaTime;
        }
        for (int i = timeoutAds.Count - 1; i >= 0; --i)
        {
            if (timeoutAds[i].placeMent != null && Advertising.IsRewardedAdReady(timeoutAds[i].placeMent))
            {
                TopLayer.Instance.LoadingAds.gameObject.SetActive(false);
                Advertising.ShowRewardedAd(timeoutAds[i].placeMent);
                timeoutAds.RemoveAt(i);
                continue;
            }
            else if (timeoutAds[i].placeMent == null && Advertising.IsRewardedAdReady())
            {
                TopLayer.Instance.LoadingAds.gameObject.SetActive(false);
                Advertising.ShowRewardedAd();
                timeoutAds.RemoveAt(i);
                continue;
            }
            if (timeoutAds[i].currentTime > 0)
            {
                var ptime = timeoutAds[i];
                ptime.currentTime -= Time.deltaTime;
                timeoutAds[i] = ptime;
                if (timeoutAds[i].currentTime <= 0)
                {
                    if (timeoutAds[i].placeMent != null && rewardAds.ContainsKey(timeoutAds[i].placeMent.Name))
                    {
                        TopLayer.Instance.LoadingAds.gameObject.SetActive(false);
                        rewardAds[timeoutAds[i].placeMent.Name](ResultStatusAds.TimeOut);
                        rewardAds.Remove(timeoutAds[i].placeMent.Name);

                    }
                    else if (timeoutAds[i].placeMent == null)
                    {
                        HUDLayer.Instance.showDialog("ERROR", "Load ADS failed", new ButtonInfo()
                        {
                            str = "Retry",
                            isTag = false,
                            action = delegate
                            {
                                HUDLayer.Instance.BoxDialog.close();
                                TopLayer.Instance.LoadingAds.gameObject.SetActive(true);
                                Advertising.LoadRewardedAd();
                                if (!timeoutAds.Exists(x => x.isDefault))
                                {
                                    timeoutAds.Add(new CountdownAds() { placeMent = null, isDefault = true, currentTime = 10 });
                                }
                            }
                        }, new ButtonInfo()
                        {
                            str = "ui/no",
                            isTag = true,
                            action = delegate
                            {
                                HUDLayer.Instance.BoxDialog.close();
                                TopLayer.Instance.LoadingAds.gameObject.SetActive(false);
                                if (rewardAds.ContainsKey("DefaultADS"))
                                {
                                    TopLayer.Instance.LoadingAds.gameObject.SetActive(false);
                                    rewardAds["DefaultADS"](ResultStatusAds.TimeOut);
                                    rewardAds.Remove("DefaultADS");
                                }
                            }
                        },true, false);

                    }
                    timeoutAds.RemoveAt(i);
                }
            }
        }
    }
    public TimeCountDown addTimer(TimeCountDown pTime)
    {
        var pTimeCounting = GameManager.Instance.Database.timers.Find(x => x.key == pTime.key);
        if (pTimeCounting == null)
        {

            GameManager.Instance.Database.timers.Add(pTime);
            EzEventManager.TriggerEvent(new EventTimer()
            {
                key = pTime.key,
                state = TimerState.Start
            });
        }
        else
        {
            pTime = pTimeCounting;
        }
        if (!timer.Contains(pTime))
        {
            timer.Add(pTime);
        }
        return pTime;
    }
    public void removeTimer(TimeCountDown pTime)
    {
        timer.Remove(pTime);
    }
    public void reCalCulateStar()
    {
        int totalStar = 0;
        for (int i = 0; i < container.levels.Count; ++i)
        {
            var pLEvelInfo = container.levels[i];
            foreach (var pMission in pLEvelInfo.infos.missions)
            {
                if (pMission.Process == 1)
                {
                    totalStar++;
                }
            }
        }
        var pStar = GameManager.Instance.Database.getComonItem("Star");
        if (pStar.Quantity < totalStar)
        {
            pStar.quantity = totalStar;
        }
    }
    public void LoadScene(string pScene)
    {
        SceneManager.Instance.loadScene(pScene);
    }
    public void initFailed()
    {
        if (!InAppPurchasing.IsInitialized())
        {
            StartCoroutine(delayAction(5, delegate
            {
                InAppPurchasing.InitializePurchasing();
            }));
        }

    }
    public void initIAPSuccess()
    {
        var pIAPSetting = LoadAssets.loadAssetScripTableObject<IAPSetting>("IAPSetting", "Variants/Database/", true);
        var products = pIAPSetting.items;
        foreach (var product in products)
        {
            ProductMetadata data = InAppPurchasing.GetProductLocalizedData(product.Id.ToLower());
            {

                if (data != null)
                {

                    product.Price = data.localizedPriceString;
                    product.isLoadLocalize = true;
                }
            }
        }
    }

    public void loadAdsFailed(string pError)
    {
       if( timeoutAds.Exists(x => x.isDefault))
        {
            var pCount = timeoutAds.Find(x => x.isDefault);
            if(pCount.currentTime >= 2)
            {
                StartCoroutine(delayAction(1, delegate
                {
                    Advertising.LoadRewardedAd();
                }));
                return;
            }
        }
        timeoutAds.RemoveAll(x => x.isDefault);
        HUDLayer.Instance.showDialog("ERROR","Load ADS failed: " + pError, new ButtonInfo()
        {
            str = "Retry",
            isTag = false,
            action = delegate
            {
                HUDLayer.Instance.BoxDialog.close();
                TopLayer.Instance.LoadingAds.gameObject.SetActive(true);
                Advertising.LoadRewardedAd();
                if (!timeoutAds.Exists(x => x.isDefault))
                {
                    timeoutAds.Add(new CountdownAds() { placeMent = null, isDefault = true, currentTime = 10 });
                }
            }
        }, new ButtonInfo()
        {
            str = "ui/no",
            isTag = true,
            action = delegate
            {
                HUDLayer.Instance.BoxDialog.close();
                TopLayer.Instance.LoadingAds.gameObject.SetActive(false);
                if (rewardAds.ContainsKey("DefaultADS"))
                {
                    TopLayer.Instance.LoadingAds.gameObject.SetActive(false);
                    rewardAds["DefaultADS"](ResultStatusAds.TimeOut);
                    rewardAds.Remove("DefaultADS");
                }
            }
        },true,false);
 
    }
    void OnEnable()
    {
        Advertising.LoadRewardAdsFailed += loadAdsFailed;
        InAppPurchasing.PurchaseCompleted += PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed += PurchaseFailedHandler;
        InAppPurchasing.InitializeSucceeded += initIAPSuccess;
        InAppPurchasing.InitializeFailed += initFailed;
        GameServices.UserLoginSucceeded += OnUserLoginSucceeded;
        GameServices.UserLoginFailed += OnUserLoginFailed;
        Advertising.RewardedAdCompleted += onRewardedAdComplete;
        Advertising.RewardedAdSkipped += onRewardedAdSkiped;
        Notifications.LocalNotificationOpened += OnLocalNotificationOpened;
        Notifications.RemoteNotificationOpened += OnRemoteNotificationOpened;
        //   Notifications.PushTokenReceived += TokenRecieved;
        EzEventManager.AddListener<GameDatabaseInventoryEvent>(this);
    }
    void OnLocalNotificationOpened(EasyMobile.LocalNotification delivered)
    {
        EazyAnalyticTool.LogEvent("LocalNTF");
    }
    void OnRemoteNotificationOpened(EasyMobile.RemoteNotification delivered)
    {
        EazyAnalyticTool.LogEvent("RemoteNTF");
    }
    public void TokenRecieved(string pToken)
    {
        Debug.Log(pToken + "token notification");
    }
    public Dictionary<string, System.Action<bool, IAPProduct>> inapps = new Dictionary<string, System.Action<bool, IAPProduct>>();
    public void showInapp(string id, System.Action<bool, IAPProduct> pResult)
    {
        if (!InAppPurchasing.IsInitialized())
        {
            InAppPurchasing.InitializePurchasing();
            return;
        }
        if (!inapps.ContainsKey(id))
        {
            inapps.Add(id, pResult);
        }
        else
        {
            inapps[id] += pResult;
        }
        InAppPurchasing.PurchaseWithId(id);
    }

    // Unsubscribe when the game object is disabled
    void OnDisable()
    {
        //  
        Advertising.LoadRewardAdsFailed -= loadAdsFailed;
        InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;
        GameServices.UserLoginSucceeded -= OnUserLoginSucceeded;
        GameServices.UserLoginFailed -= OnUserLoginFailed;
        InAppPurchasing.InitializeSucceeded -= initIAPSuccess;
        InAppPurchasing.InitializeFailed -= initFailed;
        Notifications.LocalNotificationOpened -= OnLocalNotificationOpened;
        Notifications.RemoteNotificationOpened -= OnRemoteNotificationOpened;
        //  Notifications.PushTokenReceived-=TokenRecieved;
        EzEventManager.RemoveListener<GameDatabaseInventoryEvent>(this);


    }
    float TimeScaleCache = 1;
    bool isFocus = true;
    private void OnApplicationFocus(bool focus)
    {
        if (focus == isFocus) return;
        if (!focus)
        {
            EazyAnalyticTool.LogEvent("UnFocusGame");
            AudioListener.pause = true;
            TimeScaleCache = Time.timeScale;
            // Time.timeScale = 1;
            Application.targetFrameRate = 10;
            GameManager.Instance.Database.lastInGameTime = System.DateTime.Now;
            var pContainer = DatabaseNotifyReward.Instance.container;
            List<ItemNotifyReward> pNotifies = new List<ItemNotifyReward>();
            pNotifies.AddRange(pContainer);
            pNotifies.Sort((x1, x2) => x2.TimeAFK.CompareTo(x1.TimeAFK));
            for (int i = 0; i < pNotifies.Count; ++i)
            {
                NotificationContent content = PrepareNotificationContent(pNotifies[i]);
                TimeSpan delay = TimeSpan.FromSeconds(pNotifies[i].TimeAFK);
                GameManager.Instance.Database.IdNotifySchedule.Add(Notifications.ScheduleLocalNotification(delay, content));
            }
            if (_databaseInstanced != null && _databaseInstanced.spPlanes.Count > 0)
            {
                SaveGame();
            }

        }
        else
        {
            EazyAnalyticTool.LogEvent("FocusGame");
            if (Database.IdNotifySchedule.Count > 0)
            {
                foreach (var pNoti in Database.IdNotifySchedule)
                {
                    Notifications.CancelPendingLocalNotification(pNoti);
                }
                Database.IdNotifySchedule.Clear();
            }
            Application.targetFrameRate = 60;
            //Time.timeScale = TimeScaleCache;
            AudioListener.pause = false;
        }
        isFocus = focus;
    }
    void onRewardedAdComplete(RewardedAdNetwork pNetWork, AdPlacement placement)
    {
        if (placement == AdPlacement.Default)
        {
            GameManager.Instance.Database.collectionDailyInfo.watchADS++;
            GameManager.Instance.Database.collectionInfo.watchADS++;
            EzEventManager.TriggerEvent(new MessageGamePlayEvent("MissionDirty"));
            if (rewardAds.ContainsKey("DefaultADS"))
            {
                rewardAds["DefaultADS"](ResultStatusAds.Success);
                rewardAds.Remove("DefaultADS");
            }
            return;
        }
        GameManager.Instance.Database.collectionDailyInfo.watchADS++;
        GameManager.Instance.Database.collectionInfo.watchADS++;
        EzEventManager.TriggerEvent(new MessageGamePlayEvent("MissionDirty"));
        if (rewardAds.ContainsKey(placement.Name))
        {
            rewardAds[placement.Name](ResultStatusAds.Success);
            rewardAds.Remove(placement.Name);
        }
    }
    void onRewardedAdSkiped(RewardedAdNetwork pNetWork, AdPlacement placement)
    {
        if (placement == AdPlacement.Default)
        {
            if (rewardAds.ContainsKey("DefaultADS"))
            {
                rewardAds["DefaultADS"](ResultStatusAds.Failed);
                rewardAds.Remove("DefaultADS");
            }
            return;
        }
        if (rewardAds.ContainsKey(placement.Name))
        {
            rewardAds[placement.Name](ResultStatusAds.Failed);
            rewardAds.Remove(placement.Name);
        }
    }

    // Event handlers
    void OnUserLoginSucceeded()
    {
        OnApplicationFocus(true);
        EzEventManager.TriggerEvent(new UIMessEvent("GameServiceInitialized"));
    }

    void OnUserLoginFailed()
    {
        OnApplicationFocus(true);
        Debug.Log("User login failed.");
    }
    // Successful purchase handler
    void PurchaseCompletedHandler(IAPProduct product)
    {
        PlayerPrefs.SetInt("Purchase", 1);
        // Compare product name to the generated name constants to determine which product was bought
        switch (product.Name)
        {
            case EM_IAPConstants.Product_Test:
                Debug.Log("Sample_Product was purchased. The user should be granted it now.");
                break;
                // More products here...
        }
        if (inapps.ContainsKey(product.Id))
        {
            inapps[product.Id](true, product);
            inapps.Remove(product.Id);
        }
    }

    // Failed purchase handler
    void PurchaseFailedHandler(IAPProduct product)
    {
        if (inapps.ContainsKey(product.Id))
        {
            inapps[product.Id](false, product);
            inapps.Remove(product.Id);
        }
    }
    [ContextMenu("hack")]
    public void hack()
    {
        Database.getComonItem("Star").Quantity = 150;
        SaveGame();
        for(int i =0; i < 23; ++i)
        {
            container.getLevelInfo(i + 1, 0).isLocked = false;
        }
        SaveLevel();
    }

    public void pruchasing(string pID)
    {
        InAppPurchasing.Purchase(pID);
    }
    NotificationContent PrepareNotificationContent(ItemNotifyReward pItem)
    {
        NotificationContent content = new NotificationContent();

        // Provide the notification title.
        content.title = pItem.displayNameItem.Value;

        //// You can optionally provide the notification subtitle, which is visible on iOS only.
        //content.subtitle = pItem.descriptionItem.Value;

        // Provide the notification message.
        content.body = pItem.descriptionItem.Value;

        // You can optionally attach custom user information to the notification
        // in form of a key-value dictionary.
        content.userInfo = new Dictionary<string, object>();
        content.userInfo.Add("string", "OK");
        content.userInfo.Add("number", 3);
        content.userInfo.Add("bool", true);

        // You can optionally assign this notification to a category using the category ID.
        // If you don't specify any category, the default one will be used.
        // Note that it's recommended to use the category ID constants from the EM_NotificationsConstants class
        // if it has been generated before. In this example, UserCategory_notification_category_test is the
        // generated constant of the category ID "notification.category.test".
        content.categoryId = EM_NotificationsConstants.UserCategory_notification_category_callplayerback;
        // If you want to use default small icon and large icon (on Android),
        // don't set the smallIcon and largeIcon fields of the content.
        // If you want to use custom icons instead, simply specify their names here (without file extensions).
        content.largeIcon = "ic_stat_home_gift";
        content.smallIcon = "ic_game";
        return content;
    }

    private void Start()
    {


    }

    public void showBannerAds(bool pBool)
    {
        int purchase = PlayerPrefs.GetInt("Purchase", 0);
        if (purchase != 0) return;
        if (pBool)
        {
            Advertising.ShowBannerAd(BannerAdPosition.Top, BannerAdSize.SmartBanner);
        }
        else
        {
            Advertising.HideBannerAd();
        }
    }
    public Dictionary<string, System.Action<ResultStatusAds>> rewardAds = new Dictionary<string, System.Action<ResultStatusAds>>();
    public struct CountdownAds
    {
        public bool isDefault;
        public AdPlacement placeMent;
        public float currentTime;
    }
    public List<CountdownAds> timeoutAds = new List<CountdownAds>();
    int indexOfads(AdPlacement place)
    {
        for (int i = 0; i < timeoutAds.Count; ++i)
        {
            if (timeoutAds[i].placeMent.Name == place.Name)
            {
                return i;
            }
        }
        return -1;
    }
    IEnumerator checkInternetConnection(Action<bool> action)
    {
        Debug.Log("internet");
        UnityWebRequest www = new UnityWebRequest("http://google.com");
        yield return www;
        action(www.error == null);
        www.Dispose();
    }
    public void showRewardAds(string pID, System.Action<ResultStatusAds> onResult, PositionADS pos)
    {
        //#if UNITY_EDITOR
        //        onResult(ResultStatusAds.Success);
        //        return;
        //#endif
        EazyAnalyticTool.LogEvent("LoadADS", "Position", pos.ToString(),"Status","Loading","matchID",LevelManger.InstanceRaw ? LevelManger.InstanceRaw.startMatchInfo.matchID : "");
        StartCoroutine(checkInternetConnection(delegate (bool pResult)
        {
            if (pResult)
            {
                System.Action<ResultStatusAds> pResultADS = delegate (ResultStatusAds pStatus)
                {
                    EazyAnalyticTool.LogEvent("LoadADS", "Position", pos.ToString(), "Status", pStatus.ToString(), "matchID", LevelManger.InstanceRaw ? LevelManger.InstanceRaw.startMatchInfo.matchID : "");
                    onResult(pStatus);
                };
                if (Advertising.IsRewardedAdReady())
                {
                    Advertising.ShowRewardedAd();
                }
                else
                {
                    TopLayer.Instance.LoadingAds.gameObject.SetActive(true);
                    Advertising.LoadRewardedAd();
                    if (!timeoutAds.Exists(x => x.isDefault))
                    {
                        timeoutAds.Add(new CountdownAds() { placeMent = null, isDefault = true, currentTime = 10 });
                    }

                }
                if (!rewardAds.ContainsKey("DefaultADS"))
                {
                    rewardAds.Add("DefaultADS", pResultADS);
                }
                else
                {
                    rewardAds["DefaultADS"] += pResultADS;
                }
                //var placement = AdPlacement.PlacementWithName(pID);
                //if (!rewardAds.ContainsKey(pID))
                //{
                //    rewardAds.Add(pID, onResult);
                //}
                //else
                //{
                //    rewardAds[pID] += onResult;
                //}
                //if (Advertising.IsRewardedAdReady(placement))
                //{
                //    Advertising.ShowRewardedAd(placement);
                //}
                //else
                //{
                //    TopLayer.Instance.LoadingAds.gameObject.SetActive(true);
                //    Advertising.LoadRewardedAd(placement);
                //    if (indexOfads(placement) < 0)
                //    {
                //        timeoutAds.Add(new CountdownAds() { placeMent = placement, currentTime = 10 });
                //    }
                //}
            }
            else
            {
                EazyAnalyticTool.LogEvent("LoadADSConnectionFailed", "matchID", LevelManger.InstanceRaw ? LevelManger.InstanceRaw.startMatchInfo.matchID : "");
                onResult?.Invoke(ResultStatusAds.TimeOut);
            }
        }));


    }



    public void showInterstitialAds()
    {
        int purchase = PlayerPrefs.GetInt("Purchase", 0);
        if (purchase == 0)
        {
            Debug.Log("show Inter gamemanager");
            Advertising.ShowInterstitialAd();
        }

    }

    public void OnEzEvent(GameDatabaseInventoryEvent eventType)
    {
        if (eventType.item.item.GetType() == typeof(UnlockItem))
        {
            GameManager.Instance.Database.removeItem(eventType.item.item.ItemID);
            var pPlane = GameManager.Instance.Database.getPlane(((UnlockItem)eventType.item.item).planeUnlock.ItemID);
            if (pPlane == null || pPlane.currentLevel == 0)
            {
                pPlane.CurrentLevel++;
            }
            else
            {
                BaseItemGameInstanced[] pItems = ((UnlockItem)eventType.item.item).itemEqual;
                foreach (var pItemAdd in pItems)
                {
                    var pCheckStorage = GameManager.Instance.Database.getComonItem(pItemAdd.item);
                    pCheckStorage.Quantity += pItemAdd.Quantity;
                }

            }
        }
        if (eventType.item.item.limitModule != null && eventType.item.item.limitModule.isRestoreAble)
        {
            bool dirty = false;
            if (eventType.item.changeQuantity < 0)
            {
                bool pCounting = GameManager.Instance.Database.checkTimerCountdownResotreModule(eventType.item.item);
                if (!pCounting)
                {
                    addTimer(new TimeCountDown() { key = "MainInventory/" + eventType.item.item.ItemID, lastimeWheelFree = System.DateTime.Now, length = eventType.item.item.limitModule.timeToRestore });
                    dirty = true;
                }
            }
            if (eventType.item.quantity >= eventType.item.item.limitModule.limitInInventory)
            {
                for (int i = GameManager.Instance.Database.timers.Count - 1; i >= 0; --i)
                {
                    var pTime = GameManager.Instance.Database.timers[i];
                    if (pTime.key == "MainInventory/" + eventType.item.item.ItemID)
                    {
                        EzEventManager.TriggerEvent<EventTimer>(new EventTimer()
                        {
                            key = "MainInventory/" + eventType.item.item.ItemID,
                            state = TimerState.Complete
                        });
                        GameManager.Instance.Database.timers.Remove(pTime);
                        GameManager.Instance.removeTimer(pTime);
                        dirty = true;
                    }
                }
            }
            if (dirty)
            {
                GameManager.Instance.SaveGame();
            }
        }
    }


}
