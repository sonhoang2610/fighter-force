using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Timer;
using EasyMobile;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace EazyEngine.Space.UI
{
    [System.Serializable]
    public class SerizalizeDataGame
    {
        public GameDataBaseInstance gameinfo;
        public LevelContainer levelInfo;
        public Dictionary<string, int> playerPrefInt = new Dictionary<string, int>();
        public Dictionary<string, string> playerPrefstring = new Dictionary<string, string>();
    }
    public class BoxSetting : BaseBox<ItemLanguage, string>
    {
        public CustomCenterChild center;
        public ToogleSlider slideSound, slideVibrate;
        public EazyGroupTabNGUI groupSide, groupControl;
        public UILabel statussave;
        public UIButton btnSave, btnLoad,btnLogOut;
        private SavedGame mySavedGame;

        protected bool haveData;

        public void logout()
        {
            GameServices.SignOut();
            EzEventManager.TriggerEvent(new UIMessEvent("GameServiceLogOut"));
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
        void FetchSavedGames()
        {
            GameServices.SavedGames.FetchAllSavedGames(
                (SavedGame[] games, string error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        statussave.text = "Load data successfully...";
                        Debug.Log("Fetched saved games successfully! Got " + games.Length + " saved games.");
                        if(games.Length > 0)
                        {
                            haveData = true;
                        }

                        btnLoad.isEnabled = true;
                        // Here you can show a UI to display these saved games to the user...
                    }
                    else
                    {
                        statussave.gameObject.SetActive(false);
                        btnLoad.isEnabled = false;
                        Debug.Log("Fetching saved games failed with error " + error);
                    }
                }
            );
        }
        public static string[] keyInts = new string[] { "Vibrate", "Handle", "Control", "prompt_sign_in", "firstGame", "firstGameFireBase", "FirstBoxReward", "FirstOpenGoogle", "Purchase" };
        public static string[] keyStrings = new string[] { "an_isUnitySDK" };
        // Open a saved game with manual conflict resolution
        void OpenSavedGame()
        {
            // Open a saved game named "My_Saved_Game" and resolve any outstanding conflicts manually using
            // the specified resolution function.
            GameServices.SavedGames.OpenWithAutomaticConflictResolution(
                    "My_Saved_Game",
                    OpenSavedGameCallback
            );
        }
        SavedGameConflictResolutionStrategy MyConflictResolutionFunction(SavedGame baseGame, byte[] baseData,
                                                                SavedGame remoteGame, byte[] remoteData)
        {
            {
                // Perform whatever required calculation, comparison, etc. on the two versions
                // and their associated data to help you decide which version should be chosen.

                // After determining the canonical version, use the return value to indicate your choice
                return SavedGameConflictResolutionStrategy.UseBase;        // use the base version

                // If you want to select the remote version instead, just change it to
                // return SavedGameConflictResolutionStrategy.UseRemote;
            }
        }
        public void saveGame()
        {
            Dictionary<string, int> pInts = new Dictionary<string, int>();
            for (int i = 0; i < keyInts.Length; ++i)
            {
                if (PlayerPrefs.HasKey(keyInts[i]))
                {
                    pInts.Add(keyInts[i], PlayerPrefs.GetInt(keyInts[i]));
                }
            }
            Dictionary<string, string> pStr = new Dictionary<string, string>();
            for (int i = 0; i < keyStrings.Length; ++i)
            {
                if (PlayerPrefs.HasKey(keyStrings[i]))
                {
                    pStr.Add(keyStrings[i], PlayerPrefs.GetString(keyStrings[i]));
                }
            }
            SerizalizeDataGame pData = new SerizalizeDataGame()
            {
                gameinfo = GameManager.Instance.Database,
                levelInfo = GameManager.Instance.container,
                playerPrefInt = pInts,
                playerPrefstring = pStr,
            };
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = null;
            pData.EzSerializeData(stream, ref formatter);
            WriteSavedGame(mySavedGame, stream.GetBuffer());
        }
        public void LoadGame()
        {
            ReadSavedGame(mySavedGame);
        }
        // Open saved game callback
        void OpenSavedGameCallback(SavedGame savedGame, string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                Debug.Log("Saved game opened successfully!");
                mySavedGame = savedGame;
                btnSave.isEnabled = true;
                statussave.gameObject.SetActive(true);
                statussave.text = "Loading data...";
                FetchSavedGames();
                // keep a reference for later operations      
            }
            else
            {
                Debug.Log("Open saved game failed with error: " + error);
            }
        }
        void WriteSavedGame(SavedGame savedGame, byte[] data)
        {
            statussave.gameObject.SetActive(true);
      
            if (savedGame.IsOpen)
            {
                // The saved game is open and ready for writing
                // Prepare the updated metadata of the saved game
                SavedGameInfoUpdate.Builder builder = new SavedGameInfoUpdate.Builder();
                builder.WithUpdatedDescription("New_Description");
                builder.WithUpdatedPlayedTime(TimeSpan.FromMinutes(30));    // update the played time to 30 minutes
                SavedGameInfoUpdate infoUpdate = builder.Build();
            
                GameServices.SavedGames.WriteSavedGameData(
                    savedGame,
                    data,
                    infoUpdate,    // update saved game properties
                    (SavedGame updatedSavedGame, string error) =>
                    {
                        if (string.IsNullOrEmpty(error))
                        {
                            Debug.Log("Saved game data has been written successfully!");
                        }
                        else
                        {
                            Debug.Log("Writing saved game data failed with error: " + error);
                        }
                    }
                );
                statussave.text = "Save succesfully";
            }
            else
            {
                // The saved game is not open. You can optionally open it here and repeat the process.
                Debug.Log("You must open the saved game before writing to it.");
                statussave.text = "Save failed";
            }
            StopAllCoroutines();
            StartCoroutine(GameManager.Instance.delayAction(5, delegate
            {
                statussave.gameObject.SetActive(false);
            }));
        }
        void ReadSavedGame(SavedGame savedGame)
        {
            statussave.gameObject.SetActive(true);
            statussave.text = haveData  ? "Loading..." : "No data exist to load";
            StopAllCoroutines();
            StartCoroutine(GameManager.Instance.delayAction(5, delegate
            {
                statussave.gameObject.SetActive(false);
            }));
            if (!haveData) return;
            if (savedGame.IsOpen)
            {
                // The saved game is open and ready for reading
                GameServices.SavedGames.ReadSavedGameData(
                    savedGame,
                    (SavedGame game, byte[] data, string error) =>
                    {
                        if (string.IsNullOrEmpty(error))
                        {
                            Debug.Log("Saved game data has been retrieved successfully!");
                            // Here you can process the data as you wish.
                            if (data.Length > 0)
                            {
                                MemoryStream stream = new MemoryStream(data);
                                BinaryFormatter formatter = null;
                                var pDataGame = stream.EzDeSerializeData<SerizalizeDataGame>(ref formatter);
                                GameManager.Instance._databaseInstanced = pDataGame.gameinfo;
                                GameManager.Instance.container = pDataGame.levelInfo;
                                if (pDataGame.playerPrefInt != null)
                                {
                                    for (int i = 0; i < pDataGame.playerPrefInt.Count; ++i)
                                    {
                                        PlayerPrefs.SetInt(pDataGame.playerPrefInt.Keys.ElementAt(i), pDataGame.playerPrefInt.Values.ElementAt(i));
                                    }
                                }
                                if (pDataGame.playerPrefstring != null) { 
                                    for (int i = 0; i < pDataGame.playerPrefstring.Count; ++i)
                                    {
                                        PlayerPrefs.SetString(pDataGame.playerPrefstring.Keys.ElementAt(i), pDataGame.playerPrefstring.Values.ElementAt(i));
                                    }
                                }
                                GameManager.Instance.SaveGame();
                                GameManager.Instance.SaveLevel();
                                SceneManager.Instance.loadScene("Main");
                                // Data processing
                            }
                            else
                            {
                                Debug.Log("The saved game has no data!");
                            }
                        }
                        else
                        {
                            Debug.Log("Reading saved game data failed with error: " + error);
                        }
                    }
                );
            }
            else
            {
                // The saved game is not open. You can optionally open it here and repeat the process.
                Debug.Log("You must open the saved game before reading its data.");
            }
     
        }


        protected bool init = false;
        public void centerLanguage(GameObject pObject)
        {
            if (init)
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    if (items[i].gameObject == pObject)
                    {
                        EzEventManager.TriggerEvent(new UIMessEvent("ChangeLanguage" + items[i].Data));
                        I2.Loc.LocalizationManager.CurrentLanguage = items[i].Data;
                    }
                }
            }
        }
        public void pause(bool pBool)
        {
            if (LevelManger.InstanceRaw == null) return;
            LevelManger.Instance.IsMatching = !pBool;
            TimeKeeper.Instance.getTimer("Global").TimScale = pBool ? 0 : 1;
        }

        public void replay()
        {
            if (GameManager.Instance.isFree)
            {
                Home();
                return;
            }
            GameManager.Instance.scehduleUI = ScheduleUIMain.REPLAY;
            MidLayer.Instance.boxPrepare.show();
            //GameManager.Instance.scehduleUI = ScheduleUIMain.GAME_IMEDIATELY;
            //PlayerEnviroment.clear();
            //GroupManager.clearCache();
            //Time.timeScale = 1;
            //LevelManger.InstanceRaw = null;
            //SceneManager.Instance.loadScene("Main");
        }

        public void Home()
        {
            HUDLayer.Instance.showDialogTag("ui/notice", "ui/alert_leave", new ButtonInfo()
            {
                str = "ui/yes",
                isTag = true,
                action = delegate
                {
                    PlayerEnviroment.clear();
                    GroupManager.clearCache();
                    Time.timeScale = 1;
                    GameManager.Instance.inGame = false;
                    LevelManger.InstanceRaw = null;
                    SceneManager.Instance.loadScene("Main");
                    HUDLayer.Instance.BoxDialog.close();
                }
            }, new ButtonInfo() { str = "ui/no", isTag = true, action = null });

        }
        public void turnSound(bool pBool)
        {
            if (init)
            {
                SoundManager.Instance.SfxOn = pBool;
            }
        }

        public void turnSound()
        {
            turnSound(!SoundManager.Instance.SfxOn);
        }
        void OnUserLoginSucceeded()
        {
            if (btnLogOut && GameServices.IsInitialized() && GameServices.LocalUser != null && !string.IsNullOrEmpty(GameServices.LocalUser.userName))
            {
                btnLogOut.gameObject.SetActive(true);
            }
            if (mySavedGame == null || !mySavedGame.IsOpen)
            {
                if (statussave)
                {
                    statussave.gameObject.SetActive(true);
                    btnSave.isEnabled = false;
                    btnLoad.isEnabled = false;
                    statussave.text = "Login GameService before use Save Game to Cloud";
                    GameManager.Instance.showBannerAds(true);
                    if (GameServices.IsInitialized() && GameServices.LocalUser != null && !string.IsNullOrEmpty(GameServices.LocalUser.userName))
                    {
                        OpenSavedGame();
                    }
                }
            }
        }

        void OnUserLoginFailed()
        {
           
        }
        private void OnEnable()
        {
            GameServices.UserLoginSucceeded += OnUserLoginSucceeded;
            if (statussave)
            {
                statussave.gameObject.SetActive(true);
                btnSave.isEnabled = false;
                btnLoad.isEnabled = false;
                statussave.text = "Login GameService before use Save Game to Cloud";
                GameManager.Instance.showBannerAds(true);
                if (GameServices.IsInitialized() && GameServices.LocalUser != null && !string.IsNullOrEmpty(GameServices.LocalUser.userName))
                {
                    OpenSavedGame();
                }
            }
            if (btnLogOut)
            {
                btnLogOut.gameObject.SetActive(false);
            }
            if (btnLogOut && GameServices.IsInitialized() && GameServices.LocalUser != null && !string.IsNullOrEmpty(GameServices.LocalUser.userName))
            {
                btnLogOut.gameObject.SetActive(true);
            }
        }
        private void OnDisable()
        {
            GameServices.UserLoginSucceeded -= OnUserLoginSucceeded;
            if (GameManager.Instance.IsDestroyed()) return;
            GameManager.Instance.showBannerAds(false);
        }
        public void turnVibrate(bool pBool)
        {
            //SoundManager.Instance.vib = pBool;
            if (init)
            {
                PlayerPrefs.SetInt("Vibrate", pBool ? 1 : 0);
            }
        }
        public void turnVibrate()
        {
            turnVibrate(PlayerPrefs.GetInt("Vibrate", 1) == 1);
        }
        public void chooseSide(int index)
        {
            if (init)
            {
                EzEventManager.TriggerEvent(new UIMessEvent("ChangeSide" + (index == 0 ? "Left" : "Right")));
                PlayerPrefs.SetInt("Handle", index);
            }
        }

        public void chooseControl(int index)
        {
            if (init)
            {
                EzEventManager.TriggerEvent(new UIMessEvent("Control" + index));
                PlayerPrefs.SetInt("Control", index);
            }
        }
        public void reload()
        {
            if (slideSound.Value != SoundManager.Instance.SfxOn) { slideSound.turnValue(); }
            groupControl.changeTab(PlayerPrefs.GetInt("Control", 1));
            groupSide.changeTab(PlayerPrefs.GetInt("Handle", 0));
            if (slideVibrate.Value != (PlayerPrefs.GetInt("Vibrate", 1) == 1)) { slideVibrate.turnValue(); }
            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i].Data == I2.Loc.LocalizationManager.CurrentLanguage)
                {
                    center.setCurrentPage(i);
                }
            }
            init = true;
        }
        // Start is called before the first frame update
        void Start()
        {

        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
