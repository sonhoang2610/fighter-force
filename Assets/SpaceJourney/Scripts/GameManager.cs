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
using FlowCanvas.Nodes;
using Spine.Unity;
using Spine.Unity.Modules;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

public static class LoadAssets
{
    public static Dictionary<string, object> cacheAssets = new Dictionary<string, object>();
    public static T[] loadAssets<T>(string pPathDefault = "") where T : UnityEngine.Object
    {
        //T[] pObjectss = Resources.FindObjectsOfTypeAll<T>();
        //if(pObjectss.Length ==0 && !string.IsNullOrEmpty(pPathDefault))
        //{
        if (SceneManager.Instance.isLocal || !Application.isPlaying)
        {
            T[] pObjectss = Resources.LoadAll<T>(pPathDefault);
            return pObjectss;
        }
        else
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
            return pAll.ToArray();
        }

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
        //if (GameManager.Instance.loadFromResources)
        //{
        //T pObjectss = Resources.Load<T>(path + pName);
        //return pObjectss;
        //foreach (var pObjects in pObjectss)
        //{
        //    if (pObjects && pObjects.name == pName)
        //    {
        //        if (cacheAssets.ContainsKey(pName))
        //        {
        //            cacheAssets[pName] = pObjects;
        //        }
        //        else
        //        {
        //            cacheAssets.Add(pName, pObjects);
        //        }
        //        return pObjects;
        //    }
        //}
        //}
        if (SceneManager.Instance.isLocal)
        {
            T pObjectss = Resources.Load<T>(path + pName);
            return pObjectss;
        }
        var pListBundle = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var pBundle in pListBundle)
        {
            var pObject = pBundle.LoadAsset<T>(pName);
            if (pObject)
            {
                return pObject;
            }
        }
        return AssetBundle.FindObjectOfType<T>();
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
        var pShops = LoadAssets.loadAssets<ShopDatabase>("Variants/Database/Shop");
        foreach (var pShop in pShops)
        {
            if (pShop.nameShop == target)
            {
                return pShop;
            }
        }
        return null;
    }
}

public static class SerializeGameDataBase
{
    public static GameDataBaseInstance CloneData(this GameDataBaseInstance pInfo)
    {
        using (var ms = new MemoryStream())
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
            // 2. Have the formatter use our surrogate selector
            formatter.SurrogateSelector = ss;
            formatter.Serialize(ms, pInfo);
            ms.Position = 0;
            return (GameDataBaseInstance)formatter.Deserialize(ms);
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
    GAME_IMEDIATELY,
    UPGRADE
}
public class GameManager : PersistentSingleton<GameManager>, EzEventListener<GameDatabaseInventoryEvent>
{

    public List<GameObject> objectExcludes;
    public prefabBulletGroup[] groupPrefabBullet;
    [System.NonSerialized]
    public ScheduleUIMain scehduleUI = ScheduleUIMain.NONE;
    [System.NonSerialized]
    public bool inGame = false;
    [System.NonSerialized]
    public int lastResultWin = -1;
	public AudioSource[] backgroundStage;
	public AudioSource[] bossStage;
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
    public string freePlaneChoose = "";
    [System.NonSerialized]
    public string freeSpPlaneChoose = "";
    protected override void Awake()
    {
        base.Awake();
        databaseGame = GameDatabase.Instance;
        Application.targetFrameRate = 60;

        // Database = Instantiate(Database);
    }
    public void initGame()
    {
        if (PlayerPrefs.GetInt("firstGame", 0) == 5)
        {
           var pSke =gameObject.AddComponent<SkeletonMecanim>();
           var pSke1 =gameObject.AddComponent<SkeletonRendererCustomMaterials>();
           pSke1.setPropertyFloat("asd",0);
           var pSke2 =gameObject.AddComponent<SkeletonUtilityBone>();
           var pSke3 =gameObject.AddComponent<SkeletonUtility>();
           var pSke4=gameObject.AddComponent<BoneFollower>();
           var pSke5=gameObject.AddComponent<SkeletonRenderSeparator>();
        }
        LoadGame();
        LoadAllLevel();
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
        if (!InAppPurchasing.IsInitialized())
        {
            InAppPurchasing.InitializePurchasing();
        }
    }
    bool first = true;
    private void LateUpdate()
    {
        if (first)
        {
            initGame();
            first = false;
        }
    }
    public GameDataBaseInstance _databaseDefault;
    [Sirenix.OdinInspector.ReadOnly]
    public GameDataBaseInstance _databaseInstanced;
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
    private void OnDestroy()
    {
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
                    var pData = formatter.Deserialize(file);
                    _databaseInstanced = (GameDataBaseInstance)pData;
                    _databaseInstanced.ExtraInfo();
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
        else
        {
	        _databaseInstanced = _databaseDefault.CloneData();
	        _databaseInstanced.ExtraInfo();
            SaveGame();
        }

    }
    public List<TimeCountDown> timer = new List<TimeCountDown>();



    public void LoadAllLevel()
    {
        container = LoadFile<LevelContainer>("level_container.dat");
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
        MissionSerialize missionInfo = new MissionSerialize();
        ss.AddSurrogate(typeof(MissionItem),
                        new StreamingContext(StreamingContextStates.All),
                        missionInfo);
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
            formatter.Serialize(file, pData);
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
    public T LoadFile<T>(string fileName) where T : class, new()
    {
        FileStream file = null;
        T data = null;
        string destination = Application.persistentDataPath + "/" + fileName;
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
                MissionSerialize missionInfo = new MissionSerialize();
                ss.AddSurrogate(typeof(MissionItem),
                                new StreamingContext(StreamingContextStates.All),
                                missionInfo);
                ItemGameSerialize iteomGameInfo = new ItemGameSerialize();
                ss.AddSurrogate(typeof(ItemGame),
                                new StreamingContext(StreamingContextStates.All),
                                iteomGameInfo);
                PackageInfoSerialize pack = new PackageInfoSerialize();
                ss.AddSurrogate(typeof(ItemPackage),
                                new StreamingContext(StreamingContextStates.All),
                                pack);
                formatter.SurrogateSelector = ss;
                try
                {
                    data = (T)formatter.Deserialize(file);
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
        else
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
       
        //  Database.selectedMainPlane = 6;
        if (!GameManager.Instance.isFree) {
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

        var pInfo = container.getLevelInfo(pIndex, 0).infos;
        pInfo.InputConfig = ConfigLevel;
        isPlaying = true;
        for (int i = 0; i < ConfigLevel.itemUsed.Count; ++i)
        {
            var pItem = GameManager.Instance.Database.getComonItem(ConfigLevel.itemUsed[i].itemID);
            pItem.Quantity--;
        }
        SaveGame();
 
        TopLayer.Instance.block.gameObject.SetActive(true);
        StartCoroutine(delayAction(0.75f, delegate
        {
            TopLayer.Instance.inGame(true);
            SceneManager.Instance.loadScene(GameDatabase.Instance.LevelScene(pIndex));
        }));

    }

    IEnumerator delayAction(float pDelay, System.Action action)
    {
        yield return new WaitForSeconds(pDelay);
        action();
    }
    private void Update()
    {
        if (first) return;
        if(delaySave> 0)
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

            if (timer[i].tartget == null) { timer.RemoveAt(i); continue; }
            var pSec = (System.DateTime.Now - timer[i].lastimeWheelFree).TotalSeconds;

            var pItem = (BaseItemGame)timer[i].tartget;
            foreach (var pLabel in timer[i].LabelTimer)
            {
                pLabel.text = System.TimeSpan.FromSeconds(pItem.limitModule.timeToRestore - pSec).ToString(@"hh\:mm\:ss");
            }
            if (pSec >= pItem.limitModule.timeToRestore)
            {
                GameManager.Instance.Database.reupdateTimerCount();
            }
        }
        if (GameManager.Instance.Database != null)
        {
            giftOnlineModule.onlineTime += Time.deltaTime;
        }
        for (int i = timeoutAds.Count - 1; i >= 0; --i)
        {
            if (Advertising.IsRewardedAdReady(timeoutAds[i].placeMent))
            {
                Advertising.ShowRewardedAd(timeoutAds[i].placeMent);
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
                    if (rewardAds.ContainsKey(timeoutAds[i].placeMent.Name))
                    {
                        rewardAds[timeoutAds[i].placeMent.Name](false);
                        rewardAds.Remove(timeoutAds[i].placeMent.Name);
                    }
                }
            }
        }
    }



    public bool addTimer(TimeCountDown pTime)
    {
        if (!timer.Contains(pTime))
        {
            timer.Add(pTime);
            return true;
        }
        return false;
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
                if (pMission.process == 1)
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
        var pIAPSetting = LoadAssets.loadAsset<IAPSetting>("IAPSetting", "Variants/Database/");
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
    void OnEnable()
    {

        InAppPurchasing.PurchaseCompleted += PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed += PurchaseFailedHandler;
        InAppPurchasing.InitializeSucceeded += initIAPSuccess;
        InAppPurchasing.InitializeFailed += initFailed;
        GameServices.UserLoginSucceeded += OnUserLoginSucceeded;
        GameServices.UserLoginFailed += OnUserLoginFailed;
        Advertising.RewardedAdCompleted += onRewardedAdComplete;
        Advertising.RewardedAdSkipped += onRewardedAdSkiped;
        EzEventManager.AddListener(this);
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
        SaveGame();
        InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;
        GameServices.UserLoginSucceeded -= OnUserLoginSucceeded;
        GameServices.UserLoginFailed -= OnUserLoginFailed;
        InAppPurchasing.InitializeSucceeded -= initIAPSuccess;
        InAppPurchasing.InitializeFailed -= initFailed;
        EzEventManager.RemoveListener(this);
    }
    
    
    void onRewardedAdComplete(RewardedAdNetwork pNetWork, AdPlacement placement)
    {
        if (rewardAds.ContainsKey(placement.Name))
        {
            rewardAds[placement.Name](true);
            rewardAds.Remove(placement.Name);
        }
    }
    void onRewardedAdSkiped(RewardedAdNetwork pNetWork, AdPlacement placement)
    {
        if (rewardAds.ContainsKey(placement.Name))
        {
            rewardAds[placement.Name](false);
            rewardAds.Remove(placement.Name);
        }
    }
    // Event handlers
    void OnUserLoginSucceeded()
    {
        EzEventManager.TriggerEvent(new UIMessEvent("GameServiceInitialized"));
    }

    void OnUserLoginFailed()
    {
        Debug.Log("User login failed.");
    }
    // Successful purchase handler
    void PurchaseCompletedHandler(IAPProduct product)
    {
        PlayerPrefs.SetInt("Purchase",1);
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

    public void pruchasing(string pID)
    {
        InAppPurchasing.Purchase(pID);
    }

    private void Start()
    {
        if (!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }
        bool isInitialized = InAppPurchasing.IsInitialized();
        if (!isInitialized)
        {
            InAppPurchasing.InitializePurchasing();
        }
        if (!GameServices.IsInitialized())
        {
            GameServices.ManagedInit();
        }

    }

    public void showBannerAds(bool pBool)
    {
        int purchase = PlayerPrefs.GetInt("Purchase", 0);
        if (purchase != 0) return;
        if (pBool)
        {
            Advertising.ShowBannerAd(BannerAdPosition.Bottom, BannerAdSize.Banner);
        }
        else
        {
            Advertising.HideBannerAd();
        }
    }
    public Dictionary<string, System.Action<bool>> rewardAds = new Dictionary<string, System.Action<bool>>();
    public struct CountdownAds
    {
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
    public void showRewardAds(string pID, System.Action<bool> onResult)
    {
        var placement = AdPlacement.PlacementWithName(pID);
        if (!rewardAds.ContainsKey(pID))
        {
            rewardAds.Add(pID, onResult);
        }
        else
        {
            rewardAds[pID] += onResult;
        }
        if (Advertising.IsRewardedAdReady(placement))
        {
            Advertising.ShowRewardedAd(placement);
        }
        else
        {
            Advertising.LoadRewardedAd(placement);
            if (indexOfads(placement) < 0)
            {
                timeoutAds.Add(new CountdownAds() { placeMent = placement, currentTime = 5 });
            }
        }

    }



    public void showInterstitialAds()
    {
        int purchase = PlayerPrefs.GetInt("Purchase", 0);
        if (purchase == 0)
        {
            Advertising.ShowInterstitialAd();
        }
 
    }

    public void OnEzEvent(GameDatabaseInventoryEvent eventType)
    {
        if (eventType.item.item.limitModule != null && eventType.item.item.limitModule.isRestoreAble)
        {
            bool dirty = false;
            if (eventType.item.changeQuantity < 0)
            {
                bool pCounting = GameManager.Instance.Database.checkTimerCountdownResotreModule(eventType.item.item);
                if (!pCounting)
                {
                    GameManager.Instance.Database.timers.Add(new TimeCountDown() { key = "MainInventory/" + eventType.item.item.itemID, lastimeWheelFree = System.DateTime.Now });
                    dirty = true;
                }
            }
            if (eventType.item.quantity >= eventType.item.item.limitModule.limitInInventory)
            {
                for (int i = GameManager.Instance.Database.timers.Count - 1; i >= 0; --i)
                {
                    var pTime = GameManager.Instance.Database.timers[i];
                    if (pTime.key == "MainInventory/" + eventType.item.item.itemID)
                    {
                        foreach (var pLabel in pTime.LabelTimer)
                        {
                            pLabel.gameObject.SetActive(false);
                        }
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
