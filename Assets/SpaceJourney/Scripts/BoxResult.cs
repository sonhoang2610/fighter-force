﻿using EazyEngine.Timer;
using FlowCanvas;
using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EasyMobile;
using EazyEngine.Tools;
using NodeCanvas.BehaviourTrees;

namespace EazyEngine.Space.UI
{

    public class EazyNode
    {
        public FlowScript flow;
        public MissionItemInstanced misson;
        public bool isSuccess = false;
        public System.Action<EazyNode> onFinishEvent;
        protected FlowScript currentInstance;
        public GraphOwner owner;
        public void runGraph(System.Action<EazyNode> pFinish, string pVarFlow = "")
        {
            onFinishEvent = pFinish;
            currentInstance = Graph.Clone<FlowScript>(flow, owner.graph);
            var pBlackBoard = owner.GetComponent<IBlackboard>();
            if (!string.IsNullOrEmpty(pVarFlow))
            {
                pBlackBoard.AddVariable(pVarFlow, currentInstance);
            }
            currentInstance.StartGraph(owner, pBlackBoard, true, onFinish);
        }
        public void onFinish(bool pBool)
        {
            isSuccess = pBool;
            onFinishEvent(this);
        }

    }
    public class BoxResult : MonoBehaviour
    {
        public UILabel stage, level, coinTaken, quantityDestroy, time, score, goldReward;
        public GameObject win, lose;
        public UIButton[] btnWins, btnLoses, btnLayerFrees;
        public BoxMissionLevel boxMission;
        public BoxExtract boxShowReward;
        public GameObject btnX2Ads;
        public GameObject layerRewardRandom;
        public ItemPackage[] itemBoxRewardRandom;
        public Animator boxRewardRandom;
        public UIButton btnOpenBoxRewardRandom;
        public GameObject block;
        private List<EazyNode> pNodes = new List<EazyNode>();

        static BoxResult _instance;

        public void showTestWin()
        {
            gameObject.SetActive(true);
            showResult(true);
        }

        private int timeShowLose = 0;
        private BaseItemGameInstanced[] extraItem;
        protected List<BaseItemGameInstanced> listExtr = new List<BaseItemGameInstanced>();
        private Coroutine adsCourountine;
        private int scoreCount;
        protected bool isNext = false;
        public void nextPlay()
        {
            if (!GameManager.Instance.isFree)
            {
                LevelInfoInstance pLevelInfo = GameManager.Instance.container.getLevelInfo(GameManager.Instance.Database.lastPlayStage.x + 1, GameManager.Instance.ChoosedHard);
                var levelConfig = System.Array.Find(GameDatabase.Instance.LevelConfigScene, x => x.level == GameManager.Instance.Database.lastPlayStage.x + 1);
                if (levelConfig != null && levelConfig.requrireStarToUnlock > 0 && GameManager.Instance.Database.getComonItem("Star").Quantity < levelConfig.requrireStarToUnlock)
                {
                    HUDLayer.Instance.showDialog(I2.Loc.LocalizationManager.GetTranslation("ui/notice"), I2.Loc.LocalizationManager.GetTranslation("text/not_enough_star"),null,null, false);
                    return;
                }
                if (pLevelInfo.isLocked)    
                {
                    GameManager.Instance.ChoosedHard = 0;
                }
                if (!isNext)
                {
                    GameManager.Instance.Database.lastPlayStage = new Pos(GameManager.Instance.Database.lastPlayStage.x + 1, GameManager.Instance.ChoosedHard);
                    GameManager.Instance.ChoosedLevel++;
                    isNext = true;
                }
            }
            //  Home();     
            GameManager.Instance.scehduleUI = ScheduleUIMain.NONE;
            MidLayer.Instance.boxPrepare.show();
        }

        public IEnumerator delayaction(float pSec, System.Action pACtion)
        {
            yield return new WaitForSeconds(pSec);
            pACtion();
        }
        protected Vector3 cachePosBoxReward, cacheScaleBoxReward;
        protected bool isInit = false;
        protected bool isUnlock = false;
        private void Awake()
        {
            Instance = this;
            cachePosBoxReward = boxRewardRandom.transform.localPosition;
            cacheScaleBoxReward = boxRewardRandom.transform.localScale;
            isInit = true;
        }
        
        public void showResult(bool pWin)
        {
            GameManager.Instance.lastResultWin = pWin ? 1 : 0;
            if (GameManager.Instance.isFree)
            {
                btnX2Ads.gameObject.SetActive(false);
            }
            if (!pWin && timeShowLose == 0)
            {
                if (GameManager.Instance.isFree)
                {
                    timeShowLose++;
                    btnX2Ads.gameObject.SetActive(false);
                    showResult(false);
                    return;
                }
                TimeKeeper.Instance.getTimer("Global").TimScale = 0;
                HUDLayer.Instance._onSkipReborn = delegate() {
                    timeShowLose++;
                };
                HUDLayer.Instance.BoxReborn.show();
                GameManager.Instance.showBannerAds(true);
                return;
            }
        
            if (GameManager.Instance.ChoosedLevel >= 5 && !GameManager.Instance.isFree)
            {
                int pShow = PlayerPrefs.GetInt("ShowBoxRewardRandom", 0);
                if (pShow == 0)
                {
                    if (!isInit)
                    {
                        Awake();
                    }
                    layerRewardRandom.gameObject.SetActive(true);
                    btnOpenBoxRewardRandom.gameObject.SetActive(true);
                    boxRewardRandom.transform.localPosition = cachePosBoxReward;
                    boxRewardRandom.transform.localScale = cacheScaleBoxReward;
                    if (pWin)
                    {
                        PlayerPrefs.SetInt("ShowBoxRewardRandom", Random.Range(0, 3));
                    }
                    else
                    {
                        PlayerPrefs.SetInt("ShowBoxRewardRandom", Random.Range(2, 4));
                    }
                }
                else
                {
                    PlayerPrefs.SetInt("ShowBoxRewardRandom", pShow - 1);
                }
            }
            else
            {
                layerRewardRandom.gameObject.SetActive(false);
            }
            GameManager.Instance.showBannerAds(true);
            LevelManger.Instance.IsPlaying = false;
            GameManager.Instance.inGame = false;
            TimeKeeper.Instance.getTimer("Global").TimScale = 0;

            LevelManger.Instance._infoLevel.score = 0;
            if (pWin)
            {

                GameManager.Instance.Wincount++;
                win.SetActive(true);
                lose.SetActive(false);
                LevelManger.Instance._infoLevel.score +=
                    (600 - (int)LevelManger.Instance.CurrentTime.TotalSeconds) * 50 +
                    LevelManger.Instance._infoLevel.goldTaken * 5 +
                    LevelManger.Instance.CurrentPlayer._health.CurrentHealth * 3;

                #region  check mission
                // item craft extra refresh
                extraItem = null;
                listExtr.Clear();
                foreach (var t in LevelManger.Instance._infoLevel.missions)
                {
                    var pNode = new EazyNode()
                    {
                        flow = t.mission.checkComplete,
                        misson = t,
                        owner = LevelManger.Instance.GetComponent<BehaviourTreeOwner>()
                    };
                    pNodes.Add(pNode);
                    pNode.runGraph(onFinishNode);
                }
                if (extraItem != null && extraItem.Length > 0)
                {
                    boxShowReward.DataSource = extraItem.ToObservableList();
                }
                else
                {
                    boxShowReward.DataSource = new ObservableList<BaseItemGameInstanced>();
                }
                #endregion


            }
            else
            {
                win.SetActive(false);
                lose.SetActive(true);
                GameManager.Instance.SaveLevel();
            }
            stage.text = GameManager.Instance.isFree ? "0" : GameManager.Instance.ChoosedLevel.ToString();
            string[] pStrs = new string[3] { "ui/normal", "ui/hard", "ui/super_hard" };
            level.text = I2.Loc.LocalizationManager.GetTranslation(pStrs[GameManager.Instance.ChoosedHard]);
            float percent = 1;
            if (GameManager.Instance.ChoosedLevel >= 1)
            {
                var pdrop =
                    GameDatabase.Instance.dropMonyeconfig[GameManager.Instance.ChoosedLevel - 1][
                        GameManager.Instance.ChoosedHard];
                int pStarNotEngough =
                    pdrop.requireStar - GameManager.Instance.Database.getComonItem("Star").Quantity;
                percent = 1 - (pStarNotEngough <= 0
                                   ? 0
                                   : (pStarNotEngough * 0.2f > 0.6f ? 0.6f : pStarNotEngough * 0.2f));
            }
            coinTaken.text = StringUtils.addDotMoney((int)(LevelManger.Instance._infoLevel.goldTaken* percent));
            currentCoin = LevelManger.Instance._infoLevel.goldTaken;
            boxMission.DataSource = LevelManger.Instance._infoLevel.missions.ToObservableList();
            time.text = LevelManger.Instance.CurrentTime.ToString(@"mm\:ss");
            quantityDestroy.text = $"{((float)LevelManger.Instance._infoLevel.enemyKill / (float)(LevelManger.Instance._infoLevel.enemyKill + LevelManger.Instance.BornEnemy.Count)) * 100}%";

            gameObject.SetActive(true);
            block.gameObject.SetActive(true);
            StartCoroutine(delayaction(1, delegate
            {
                block.gameObject.SetActive(false);
            }));
            StartCoroutine(delayaction(0.5f, delegate
            {
                bool showGuide = false;
                if (extraItem != null && extraItem.Length > 0)
                {
                    int pFirstGuideBoxReward = PlayerPrefs.GetInt("FirstBoxReward", 0);
                    if (extraItem[0].item.itemID.StartsWith("Box") && (pFirstGuideBoxReward <= 3))
                    {
                        if (pFirstGuideBoxReward == 0)
                        {
                            PlayerPrefs.SetInt("FirstBoxReward", 1);
                            showGuide = true;
                            StartCoroutine(delayaction(1, delegate
                            {
                                
                                EzEventManager.TriggerEvent(new GuideEvent("FirstRewardBox", delegate
                                {
                                    BoxNewFeature.Instance.show(GameManager.Instance.CurrentLevelUnlock);
                                }));
                            }));
                        }
                        else if (PlayerPrefs.GetInt("SecondBox", 0) == 0)
                        {
                            // PlayerPrefs.SetInt("FirstBoxReward",4);
                            PlayerPrefs.SetInt("SecondBox", 1);
                        }

                    }

                }
                if (!showGuide)
                {
                    BoxNewFeature.Instance.show(GameManager.Instance.CurrentLevelUnlock);
                }
                
            }));

            if (!GameManager.Instance.isFree  && (GameManager.Instance.ChoosedLevel > 3 || GameManager.Instance.ChoosedHard != 0))
            {
                adsCourountine = StartCoroutine(delayaction(1.5f, delegate
                {
                    if (SceneManager.Instance.currentScene.StartsWith("Zone"))
                    {
                        GameManager.Instance.showInterstitialAds();
                    }
                }));
            }
            LevelManger.Instance.historyMatch.resultGame = pWin ? "Win" : "Lose";
            var pTimeLife = LevelManger.Instance.historyMatch.timeLifes[LevelManger.Instance.historyMatch.timeLifes.Count - 1];
            pTimeLife.timeEnd = (int)LevelManger.Instance.CurrentTime.TotalSeconds;
            LevelManger.Instance.historyMatch.matchID = LevelManger.Instance.startMatchInfo.matchID;
            var pHistoryString = JsonUtility.ToJson(LevelManger.Instance.historyMatch);
            EazyAnalyticTool.LogEvent("EndGame","history",pHistoryString);
            if (pWin)
            {
            
                if (!GameManager.Instance.isFree)
                {
                    
                    // update coin
                    GameManager.Instance.Database.getComonItem("Coin").Quantity +=
                        (int)(LevelManger.Instance._infoLevel.goldTaken * percent);
                    //unlock high mode
                    if (GameManager.Instance.ChoosedHard < 2)
                    {
                        var pLevelInfoNow = GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel,
                              GameManager.Instance.ChoosedHard);
                        if (!pLevelInfoNow.isPassed)
                        {
                            GameManager.Instance.Database.collectionInfo.PassedLevelMode(GameManager.Instance.ChoosedHard);
                            GameManager.Instance.Database.collectionDailyInfo.PassedLevelMode(GameManager.Instance.ChoosedHard);
                        }
                        pLevelInfoNow.isPassed = true;
                        var pOldPass = GameManager.Instance.ChoosedHard == 0 ? GameManager.Instance.Database.collectionInfo.passNormal : (GameManager.Instance.ChoosedHard == 1 ? GameManager.Instance.Database.collectionInfo.passHard : GameManager.Instance.Database.collectionInfo.passSuperHard);
                        GameManager.Instance.Database.collectionInfo.singletonRecalculate();
                        var pNewPass = GameManager.Instance.ChoosedHard == 0 ? GameManager.Instance.Database.collectionInfo.passNormal : (GameManager.Instance.ChoosedHard == 1 ? GameManager.Instance.Database.collectionInfo.passHard : GameManager.Instance.Database.collectionInfo.passSuperHard);
                        if (GameManager.Instance.ChoosedHard == 0)
                        {
                            GameManager.Instance.Database.collectionDailyInfo.passNormal += pOldPass - pNewPass;
                        }
                        else if (GameManager.Instance.ChoosedHard == 1)
                        {
                            GameManager.Instance.Database.collectionDailyInfo.passHard += pOldPass - pNewPass;
                        }
                        else if (GameManager.Instance.ChoosedHard == 2)
                        {
                            GameManager.Instance.Database.collectionDailyInfo.passSuperHard += pOldPass - pNewPass;
                        }

                        GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel,
                            GameManager.Instance.ChoosedHard + 1).isLocked = false;
                    }

                    //report new score
                    var pOldLevelInfo = GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel,
                        GameManager.Instance.ChoosedHard);
                    bool newbest = pOldLevelInfo.infos.score < LevelManger.Instance._infoLevel.score;
                    GameManager.Instance.container
                            .getLevelInfo(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard).infos =
                        LevelManger.Instance._infoLevel;
                    if (newbest)
                    {
                        int pScore = 0;
                        for (int i = 0; i < GameManager.Instance.container.levels.Count; ++i)
                        {
                            pScore += GameManager.Instance.container.levels[i].infos.score;
                        }

                        GameServices.ReportScore(pScore, EM_GameServicesConstants.Leaderboard_Top_Fighter);
                    }

                    //unlock level
                    if (GameManager.Instance.ChoosedLevel == GameManager.Instance.CurrentLevelUnlock)
                    {
                        var levelConfig = System.Array.Find(GameDatabase.Instance.LevelConfigScene, x => x.level == GameManager.Instance.CurrentLevelUnlock + 1);
                        if (levelConfig == null || levelConfig.requrireStarToUnlock <= GameManager.Instance.Database.getComonItem("Star").Quantity)
                        {
                            GameManager.Instance.CurrentLevelUnlock++;
                        }
                        isUnlock = true;
                        // GameManager.Instance.ChoosedLevel++;
                    }

                    //save game
                    GameManager.Instance.SaveGame();
                    GameManager.Instance.SaveLevel();
                }
            }
            else if (!GameManager.Instance.isFree)
            {
              
                //lose game
                //update money
                GameManager.Instance.Database.getComonItem("Coin").Quantity += (int)(LevelManger.Instance._infoLevel.goldTaken * percent);
                GameManager.Instance.SaveGame();
            }

            foreach (var t in btnWins)
            {
                t.gameObject.SetActive(!GameManager.Instance.isFree);
            }

            foreach (var t in btnLoses)
            {
                t.gameObject.SetActive(!GameManager.Instance.isFree);
            }
            foreach (var t in btnLayerFrees)
            {
                t.gameObject.SetActive(GameManager.Instance.isFree);
            }
            EzEventManager.TriggerEvent(new MessageGamePlayEvent("MissionDirty"));
        }
        protected float currentCoin;

        public static BoxResult Instance { get => _instance; set => _instance = value; }

        public void setGold(float pCoin)
        {
            currentCoin = pCoin;
            goldReward.text = StringUtils.addDotMoney((int)pCoin) + " +";
        }


        public void watch()
        {
            if (GameManager.Instance.isFree) return;
            //stop quang cao rac khi da an quang cao x2
            if (adsCourountine != null)
            {
                StopCoroutine(adsCourountine);
            }

            GameManager.Instance.showRewardAds("WatchX2WinGame", delegate (ResultStatusAds pSucess)
            {
                if (pSucess == ResultStatusAds.Success)
                {
                    #region tinh percent nhan dc trong map
                    btnX2Ads.gameObject.SetActive(false);
                    var pdrop =
                        GameDatabase.Instance.dropMonyeconfig[GameManager.Instance.ChoosedLevel - 1][
                            GameManager.Instance.ChoosedHard];
                    int pStarNotEngough =
                        pdrop.requireStar - GameManager.Instance.Database.getComonItem("Star").Quantity;
                    float percent = 1 - (pStarNotEngough <= 0
                                        ? 0
                                        : (pStarNotEngough * 0.2f > 0.6f ? 0.6f : pStarNotEngough * 0.2f));

                    #endregion
                    int pQuantityReward = (int)(LevelManger.Instance._infoLevel.goldTaken * percent);
                    //  goldReward.gameObject.SetActive(true);
                    var pItem = GameManager.Instance.Database.getComonItem("Coin");
                    var pCoin = listExtr.Find(x => x.item.itemID == "Coin");
                    pCoin = new BaseItemGameInstanced()
                    {
                        item = pItem.item,
                        quantity = pQuantityReward
                    };
                    listExtr.Add(pCoin);
                    boxShowReward.DataSource = listExtr.ToObservableList();
                    pItem.Quantity += pQuantityReward;
                    currentCoin = 0;
                    int pTo = (int)(LevelManger.Instance._infoLevel.goldTaken * percent);
                    DOTween.To(() => currentCoin, setGold, pTo, 0.5f);
                    GameManager.Instance.SaveGame();
                }
            },PositionADS.X2Reward);
        }
        public void watchRewardRandom()
        {
            if (GameManager.Instance.isFree) return;
            //stop quang cao rac khi da an quang cao x2
            if (adsCourountine != null)
            {
                StopCoroutine(adsCourountine);
            }
            btnOpenBoxRewardRandom.gameObject.SetActive(false);
            GameManager.Instance.showRewardAds("WatchX2WinGame", delegate (ResultStatusAds pSucess)
            {
                if (pSucess == ResultStatusAds.Success)
                {

                    boxRewardRandom.SetTrigger("Open");
                    boxRewardRandom.transform.localPosition = Vector3.zero;
                    boxRewardRandom.transform.localScale *= 1.5f;
                }
                else
                {
                    btnOpenBoxRewardRandom.gameObject.SetActive(true);
                }
            },PositionADS.OpenBoxInGame);
        }

        public void claimRewardRandom()
        {
            BaseItemGameInstanced[] pItems = null;
            pItems = itemBoxRewardRandom[GameManager.Instance.ChoosedHard == 0 ? ((GameManager.Instance.ChoosedLevel / 5) - 1) : (GameManager.Instance.ChoosedLevel / 5)].ExtractHere();
            foreach (var pItemAdd in pItems)
            {
                var pCheckStorage = GameManager.Instance.Database.getComonItem(pItemAdd.item);
                pCheckStorage.Quantity += pItemAdd.Quantity;
            }
            listExtr.AddRange(pItems);
            boxShowReward.DataSource = listExtr.ToObservableList();
            GameManager.Instance.SaveGame();
        }

        public void openDone()
        {
            boxRewardRandom.gameObject.SetActive(false);
            layerRewardRandom.gameObject.SetActive(false);
        }
        public int getScore()
        {
            return scoreCount;
        }
        public void setScore(int pScore)
        {
            scoreCount = pScore;
            this.score.text = StringUtils.addDotMoney(pScore);
        }

        public void onFinishNode(EazyNode pNode)
        {
            pNodes.Remove(pNode);
            if (pNode.isSuccess)
            {

                //mission complete 
                if (pNode.misson.Process != 1)
                {
                    GameManager.Instance.Database.getComonItem("Star").Quantity++;
                    for (int i = 0; i < pNode.misson.rewards.Length; ++i)
                    {
                        var pStorage = GameManager.Instance.Database.getComonItem(pNode.misson.rewards[i].item);
                        pStorage.Quantity += pNode.misson.rewards[i].Quantity;
                        if (pStorage.item.ItemID != "Coin")
                        {
                            var pExist = listExtr.Find(x => x.item.itemID == pStorage.item.ItemID);
                            if (pExist == null)
                            {
                                listExtr.Add(new BaseItemGameInstanced()
                                {
                                    item = pStorage.item,
                                    quantity = pNode.misson.rewards[i].Quantity
                                });
                            }
                            else
                            {
                                pExist.quantity += pNode.misson.rewards[i].Quantity;
                            }
                            extraItem = listExtr.ToArray();
                        }
                    }
                }
                pNode.misson.Process = 1;
            }
            if (pNodes.Count == 0)
            {
                float percent = 1;
                if (GameManager.Instance.ChoosedLevel >= 1)
                {
                    var pdrop =
                        GameDatabase.Instance.dropMonyeconfig[GameManager.Instance.ChoosedLevel - 1][
                            GameManager.Instance.ChoosedHard];
                    int pStarNotEngough =
                        pdrop.requireStar - GameManager.Instance.Database.getComonItem("Star").Quantity;
                     percent = 1 - (pStarNotEngough <= 0
                                        ? 0
                                        : (pStarNotEngough * 0.2f > 0.6f ? 0.6f : pStarNotEngough * 0.2f));
                }
                boxMission.DataSource = LevelManger.Instance._infoLevel.missions.ToObservableList();
                boxMission.reloadData();
                for (int i = 0; i < boxMission.items.Count; ++i)
                {
                    Sequence pSeq = DOTween.Sequence();
                    pSeq.AppendInterval(1.2f + (i * 0.2f));
                    pSeq.Append(boxMission.items[i].transform.DOScale(1.1f, 0.25f));
                    pSeq.Append(boxMission.items[i].transform.DOScale(1, 0.25f));
                    pSeq.Play();
                }
                Sequence pSeq1 = DOTween.Sequence();
                pSeq1.AppendInterval(1.6f);
                pSeq1.Append(DOTween.To(getScore, setScore, LevelManger.Instance._infoLevel.score, 0.5f).From(0));
                pSeq1.Play();
                Sequence pSeq2 = DOTween.Sequence();
                float pGold = 0;
                pSeq2.AppendInterval(1.6f);
                pSeq2.Append(DOTween.To(() => pGold, x =>
                {
                    pGold = x;
                    coinTaken.text = StringUtils.addDotMoney((int) x);
                }, LevelManger.Instance._infoLevel.goldTaken*percent, 0.5f).From(0));
                pSeq2.Play();

            }

        }
        private void OnDisable()
        {
            GroupManager.clearCache();
            PlayerEnviroment.clear();   
            TimeKeeper.Instance.getTimer("Global").TimScale = 1;
            GameManager.Instance.showBannerAds(false);
        }
        public void Home()
        {
            GroupManager.clearCache();
            PlayerEnviroment.clear();
            TimeKeeper.Instance.getTimer("Global").TimScale = 1;
            LevelManger.InstanceRaw = null;
            SceneManager.Instance.loadScene("Main");
            GameManager.Instance.scehduleUI = ScheduleUIMain.NONE;
            if (isUnlock && !GameManager.Instance.isFree)
            {
                GameManager.Instance.Database.lastPlayStage = new Pos(GameManager.Instance.Database.lastPlayStage.x + 1, 0);
            }
        }

        public void Replay()
        {
            if (GameManager.Instance.isFree)
            {
                Home();
                return;
            }
            GameManager.Instance.scehduleUI = ScheduleUIMain.REPLAY;
            MidLayer.Instance.boxPrepare.show();
            //TimeKeeper.Instance.getTimer("Global").TimScale = 1;
            //LevelManger.InstanceRaw = null;
            ////  GameManager.Instance.pla
            //GameManager.Instance.LoadLevel(GameManager.Instance.ChoosedLevel);      

            // SceneManager.Instance.loadScene("Main");
        }

        public void Upgrade()
        {
            if (GameManager.Instance.isFree)
            {
                Home();
                return;
            }
            GameManager.Instance.scehduleUI = ScheduleUIMain.UPGRADE;
            PlayerEnviroment.clear();
            GroupManager.clearCache();
            TimeKeeper.Instance.getTimer("Global").TimScale = 1;
            LevelManger.InstanceRaw = null;
            SceneManager.Instance.loadScene("Main");
        }
        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}
