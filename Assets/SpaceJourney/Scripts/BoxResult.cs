using EazyEngine.Timer;
using FlowCanvas;
using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EasyMobile;
using EazyEngine.Tools;

namespace EazyEngine.Space.UI
{

    public class EazyNode
    {
        public FlowScript flow;
        public MissionItemInstanced misson;
        public bool isSuccess = false;
        public System.Action<EazyNode> onFinishEvent;
        protected FlowScript currentInstance;
        public void runGraph(System.Action<EazyNode> pFinish)
        {
            onFinishEvent = pFinish;
            currentInstance = Graph.Clone<FlowScript>(flow, LevelManger.Instance.GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>().graph);
            currentInstance.StartGraph(LevelManger.Instance.GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>(), LevelManger.Instance.GetComponent<IBlackboard>(), true, onFinish);
        }
        public void  onFinish(bool pBool)
        {
            isSuccess = pBool;
            onFinishEvent(this);     
        }
      
    }
    public class BoxResult : MonoBehaviour
    {
        public UILabel stage, level,coinTaken,quantityDestroy,time,score,goldReward;
        public GameObject win, lose;
        public UIButton[] btnWins, btnLoses,btnLayerFrees;
        public BoxMissionLevel boxMission;
        public BoxExtract boxShowReward;
        public GameObject btnX2Ads;
        private List<EazyNode> pNodes = new List<EazyNode>();

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
        public void nextPlay()
        {
            if (!GameManager.Instance.isFree)
            {
                //GameManager.Instance.scehduleUI = ScheduleUIMain.GAME_IMEDIATELY;
                LevelInfoInstance pLevelInfo = GameManager.Instance.container.getLevelInfo(GameManager.Instance.Database.lastPlayStage.x + 1, GameManager.Instance.ChoosedHard);
                if (pLevelInfo.isLocked)
                {
                    GameManager.Instance.ChoosedHard = 0;
                }
                GameManager.Instance.Database.lastPlayStage = new Pos(GameManager.Instance.Database.lastPlayStage.x + 1, GameManager.Instance.ChoosedHard);
            }
          //  Home();
             for(int i = GameManager.Instance.ConfigLevel.itemUsed.Count-1; i >=0; --i)
            {
                var pItem = GameManager.Instance.ConfigLevel.itemUsed[i];
               if (!pItem.isActive)
                {
                    GameManager.Instance.ConfigLevel.itemUsed.RemoveAt(i);
                }
            }
            GameManager.Instance.ChoosedHard = 0;
            GameManager.Instance.ChoosedLevel++;
            GameManager.Instance.LoadLevel(GameManager.Instance.ChoosedLevel);
        }

        public IEnumerator delayaction(float pSec, System.Action pACtion)
        {
            yield return  new WaitForSeconds(pSec);
            pACtion();
        }

     
        public void showResult(bool pWin)
        {
            GameManager.Instance.lastResultWin = pWin ? 1 :0;
            if (GameManager.Instance.isFree)
            {
                btnX2Ads.gameObject.SetActive(false);
            }
            if (!pWin && timeShowLose == 0) {
                if (GameManager.Instance.isFree)
                {
                    timeShowLose++;
                    btnX2Ads.gameObject.SetActive(false);
                    showResult(false);
                    return;
                }
                TimeKeeper.Instance.getTimer("Global").TimScale = 0;
                timeShowLose++;
                HUDLayer.Instance.BoxReborn.show();
                GameManager.Instance.showBannerAds(true);
                return;
            }

            GameManager.Instance.showBannerAds(true);
            LevelManger.Instance.IsPlaying = false;
            GameManager.Instance.inGame = false;
            TimeKeeper.Instance.getTimer("Global").TimScale = 0;
      
            LevelManger.Instance._infoLevel.score = 0;
            if (pWin) {
           
                GameManager.Instance.wincount++;
                win.SetActive(true);
                lose.SetActive(false);
                LevelManger.Instance._infoLevel.score +=
                    (600 - (int) LevelManger.Instance.CurrentTime.TotalSeconds) * 50 +
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
                        misson = t
                    };
                    pNodes.Add(pNode);
                    pNode.runGraph(onFinishNode);
                }
                //set data extra craft reward
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
            stage.text  = GameManager.Instance.isFree ? "0" : GameManager.Instance.ChoosedLevel.ToString();
            string[] pStrs = new string[3] { "ui/normal", "ui/hard", "ui/super_hard" };
            level.text = I2.Loc.LocalizationManager.GetTranslation(pStrs[GameManager.Instance.ChoosedHard]);
            coinTaken.text = StringUtils.addDotMoney(LevelManger.Instance._infoLevel.goldTaken);
            currentCoin = LevelManger.Instance._infoLevel.goldTaken;
            boxMission.DataSource = LevelManger.Instance._infoLevel.missions.ToObservableList();  
            time.text = LevelManger.Instance.CurrentTime.ToString(@"mm\:ss");
            quantityDestroy.text =  $"{( (float)LevelManger.Instance._infoLevel.enemyKill / (float)(LevelManger.Instance._infoLevel.enemyKill + LevelManger.Instance.BornEnemy.Count))*100}%";
        
            gameObject.SetActive(true);
            StartCoroutine(delayaction(0.25f, delegate
            {
                if (extraItem != null && extraItem.Length > 0)
                {
                    int pFirstGuideBoxReward = PlayerPrefs.GetInt("FirstBoxReward", 0);
                    if (extraItem[0].item.itemID.StartsWith("Box") && (pFirstGuideBoxReward <= 3))
                    {
                        if (pFirstGuideBoxReward == 0)
                        {
                            PlayerPrefs.SetInt("FirstBoxReward", 1);
                            StartCoroutine(delayaction(1, delegate
                            {
                                EzEventManager.TriggerEvent(new GuideEvent("FirstRewardBox", null));
                            }));
                        }
                        else if (PlayerPrefs.GetInt("SecondBox", 0) == 0)
                        {
                            // PlayerPrefs.SetInt("FirstBoxReward",4);
                            PlayerPrefs.SetInt("SecondBox", 1);
                        }
                    }
                }
            }));
        
            if (!GameManager.Instance.isFree)
            {
                adsCourountine =  StartCoroutine(delayaction(1.5f,delegate{
                    if (SceneManager.Instance.currentScene.StartsWith("Zone"))
                    {
                        GameManager.Instance.showInterstitialAds();
                    }
                }));
            }

            if (pWin)
            {
                if (!GameManager.Instance.isFree)
                {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent($"Win_{GameManager.Instance.ChoosedLevel}_Mode_{GameManager.Instance.ChoosedHard}");
                    var pdrop =
                        GameDatabase.Instance.dropMonyeconfig[GameManager.Instance.ChoosedLevel - 1][
                            GameManager.Instance.ChoosedHard];
                    int pStarNotEngough =
                        pdrop.requireStar - GameManager.Instance.Database.getComonItem("Star").Quantity;
                    float percent = 1 - (pStarNotEngough <= 0
                                        ? 0
                                        : (pStarNotEngough * 0.2f > 0.6f ? 0.6f : pStarNotEngough * 0.2f));
                    // update coin
                    GameManager.Instance.Database.getComonItem("Coin").Quantity +=
                        (int) (LevelManger.Instance._infoLevel.goldTaken * percent);
                    //unlock high mode
                    if (GameManager.Instance.ChoosedHard < 2)
                    {
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
                        GameManager.Instance.CurrentLevelUnlock++;
                        GameManager.Instance.Database.lastPlayStage =
                            new Pos(GameManager.Instance.Database.lastPlayStage.x, 0);
                       // GameManager.Instance.ChoosedLevel++;
                    }

                    //save game
                    GameManager.Instance.SaveGame();
                    GameManager.Instance.SaveLevel();
                }
            }
            else if (!GameManager.Instance.isFree)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent($"Lose_{GameManager.Instance.ChoosedLevel}_Mode_{GameManager.Instance.ChoosedHard}_Time_{LevelManger.Instance.CurrentTime.TotalSeconds}");
                //lose game
                var pdrop = GameDatabase.Instance.dropMonyeconfig[GameManager.Instance.ChoosedLevel - 1][GameManager.Instance.ChoosedHard];
                int pStarNotEngough = pdrop.requireStar - GameManager.Instance.Database.getComonItem("Star").Quantity;
                float percent = 1 - (pStarNotEngough <= 0 ? 0 : (pStarNotEngough * 0.2f > 0.6f ? 0.6f : pStarNotEngough * 0.2f));
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
        }
        protected float currentCoin;
        public void setGold(float pCoin)
        {
            currentCoin = pCoin;
            goldReward.text = StringUtils.addDotMoney((int)pCoin)+ " +";
        }
        

        public void watch()
        {
            if (GameManager.Instance.isFree) return;
            //stop quang cao rac khi da an quang cao x2
            if (adsCourountine != null)
            {
                StopCoroutine(adsCourountine);
            }

            GameManager.Instance.showRewardAds("WatchX2WinGame", delegate(bool pSucess)
            {
                if (pSucess)
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
                    if (pCoin != null)
                    {
                        pCoin.quantity += pQuantityReward;
                    }
                    else
                    {
                        pCoin = new BaseItemGameInstanced()
                        {
                            item = pItem.item,
                            quantity = pQuantityReward
                        };
                        listExtr.Add(pCoin);
                    }
                    boxShowReward.DataSource = listExtr.ToObservableList();
                    pItem.Quantity += pQuantityReward;
                    currentCoin = 0;
                    int pTo = (int) (LevelManger.Instance._infoLevel.goldTaken * percent );
                    DOTween.To(() => currentCoin, setGold, pTo, 0.5f);
                    GameManager.Instance.SaveGame();
                }
            });
        }

        public int getScore()
        {
            return scoreCount;
        }
        public void setScore(int pScore)
        {
            scoreCount = pScore;
            this.score.text =StringUtils.addDotMoney(pScore);
        }

        public void onFinishNode(EazyNode pNode)
        {
            pNodes.Remove(pNode);
            if (pNode.isSuccess)
            {
   
                //mission complete 
                if (pNode.misson.process != 1)
                {
                    GameManager.Instance.Database.getComonItem("Star").Quantity++;
                   for(int i  = 0; i < pNode.misson.rewards.Length; ++i)
                    {
                      var pStorage =  GameManager.Instance.Database.getComonItem(pNode.misson.rewards[i].item);
                        pStorage.Quantity += pNode.misson.rewards[i].quantity;
                        if (pStorage.item.ItemID != "Coin")
                        {
                           var pExist = listExtr.Find(x => x.item.itemID == pStorage.item.ItemID);
                            if (pExist == null)
                            {
                                listExtr.Add(new BaseItemGameInstanced()
                                {
                                    item = pStorage.item,
                                    quantity = pNode.misson.rewards[i].quantity
                                });
                            }
                            else
                            {
                                pExist.quantity += pNode.misson.rewards[i].quantity;
                            }
                            extraItem = listExtr.ToArray();
                        }
                    }
                }
                pNode.misson.process = 1;
            }
            if(pNodes.Count == 0)
            {
                boxMission.DataSource = LevelManger.Instance._infoLevel.missions.ToObservableList();
                boxMission.reloadData();
                for(int i = 0; i < boxMission.items.Count; ++i)
                {
                    Sequence pSeq = DOTween.Sequence();
                    pSeq.AppendInterval(1.2f+ (i * 0.2f));
                    pSeq.Append(boxMission.items[i].transform.DOScale(1.1f, 0.25f));
                    pSeq.Append(boxMission.items[i].transform.DOScale(1, 0.25f));
                    pSeq.Play();
                }
                Sequence pSeq1 = DOTween.Sequence();
                pSeq1.AppendInterval(1.6f);
                pSeq1.Append(DOTween.To(getScore, setScore, LevelManger.Instance._infoLevel.score, 0.5f).From(0));
                pSeq1.Play();
                Sequence pSeq2 = DOTween.Sequence();
                int pGold = 0;
                pSeq2.AppendInterval(1.6f);
                pSeq2.Append(DOTween.To(()=> pGold, x => {
                    pGold = x;
                    coinTaken.text = x.ToString();
                }, LevelManger.Instance._infoLevel.goldTaken, 0.5f).From(0));
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
        }

        public void Replay()
        {
            if (GameManager.Instance.isFree)
            {
                Home();
                return;
            }
            TimeKeeper.Instance.getTimer("Global").TimScale = 1;
            LevelManger.InstanceRaw = null;
            //  GameManager.Instance.pla
            GameManager.Instance.LoadLevel(GameManager.Instance.ChoosedLevel);      
  
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
