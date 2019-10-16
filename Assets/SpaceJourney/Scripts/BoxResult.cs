using EazyEngine.Timer;
using FlowCanvas;
using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EasyMobile;

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
        public UILabel stage, level,coinTaken,quantityDestroy,time,score;
        public GameObject win, lose;
        public UIButton[] btnWins, btnLoses,btnLayerFrees;
        public BoxMissionLevel boxMission;
        public GameObject btnX2Ads;
        List<EazyNode> pNodes = new List<EazyNode>();

        public void showTestWin()
        {
            gameObject.SetActive(true);
            showResult(true);
        }
        int timeShowLose = 0;

        public void nextPlay()
        {
            if (!GameManager.Instance.isFree)
            {
                GameManager.Instance.scehduleUI = ScheduleUIMain.GAME_IMEDIATELY;
                LevelInfoInstance pLevelInfo = GameManager.Instance.container.getLevelInfo(GameManager.Instance.Database.lastPlayStage.x + 1, GameManager.Instance.ChoosedHard);
                if (pLevelInfo.isLocked)
                {
                    GameManager.Instance.ChoosedHard = 0;
                }
                GameManager.Instance.Database.lastPlayStage = new Pos(GameManager.Instance.Database.lastPlayStage.x + 1, GameManager.Instance.ChoosedHard);
            }
            Home();
        }

        public IEnumerator delayaction(float pSec, System.Action pACtion)
        {
            yield return  new WaitForSeconds(pSec);
            pACtion();
        }

        protected Coroutine adsCourountine;
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
                    showResult(pWin);
                    return;
                }
                TimeKeeper.Instance.getTimer("Global").TimScale = 0;
                timeShowLose++;
                HUDLayer.Instance.BoxReborn.show();
                GameManager.Instance.showBannerAds(true);
                return;
            }
          
          
            LevelManger.Instance.IsPlaying = false;
            GameManager.Instance.inGame = false;
            TimeKeeper.Instance.getTimer("Global").TimScale = 0;
      
            LevelManger.Instance._infoLevel.score = 0;
            if (pWin) {
                GameManager.Instance.wincount++;
                win.SetActive(true);
                lose.SetActive(false);
                LevelManger.Instance._infoLevel.score += (600- (int)LevelManger.Instance.CurrentTime.TotalSeconds)*50 + LevelManger.Instance._infoLevel.goldTaken*5 + LevelManger.Instance.CurrentPlayer._health.CurrentHealth*3;       
                for (int i = 0; i < LevelManger.Instance._infoLevel.missions.Count; ++i)
                {
                    var pNode = new EazyNode()
                    {
                        flow = LevelManger.Instance._infoLevel.missions[i].mission.checkComplete,
                        misson = LevelManger.Instance._infoLevel.missions[i]
                    };
                    pNodes.Add(pNode);
                    pNode.runGraph(onFinishNode);
                }

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
            quantityDestroy.text = LevelManger.Instance._infoLevel.enemyKill.ToString();
        
            gameObject.SetActive(true);
            if ((System.DateTime.Now - GameManager.Instance.Database.firstOnline).TotalSeconds > 600)
            {
                adsCourountine =  StartCoroutine(delayaction(3,delegate{
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
                    var pdrop = GameDatabase.Instance.dropMonyeconfig[GameManager.Instance.ChoosedLevel - 1][GameManager.Instance.ChoosedHard];
                    int pStarNotEngough = pdrop.requireStar - GameManager.Instance.Database.getComonItem("Star").Quantity;
                    float percent = 1 - (pStarNotEngough <= 0 ? 0 : (pStarNotEngough * 0.2f > 0.6f ? 0.6f : pStarNotEngough * 0.2f));
                    GameManager.Instance.Database.getComonItem("Coin").Quantity +=(int)( LevelManger.Instance._infoLevel.goldTaken* percent);
                    if (GameManager.Instance.ChoosedHard < 2)
                    {
                        GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard + 1).isLocked = false;
                    }
                    var pOldLevelInfo = GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard);
                    bool newbest = pOldLevelInfo.infos.score < LevelManger.Instance._infoLevel.score;
                    GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard).infos = LevelManger.Instance._infoLevel;
                    if (newbest)
                    {
                        int pScore = 0;
                        for (int i = 0; i < GameManager.Instance.container.levels.Count; ++i)
                        {
                            pScore += GameManager.Instance.container.levels[i].infos.score;
                        }
                        Debug.Log("report_score" + pScore);
                        GameServices.ReportScore(pScore, EM_GameServicesConstants.Leaderboard_Top_Fighter);
                    }
              
                  
                    if (GameManager.Instance.ChoosedLevel == GameManager.Instance.CurrentLevelUnlock)
                    {

                        GameManager.Instance.CurrentLevelUnlock++;
                        GameManager.Instance.Database.lastPlayStage = new Pos(GameManager.Instance.Database.lastPlayStage.x, 0);
                        GameManager.Instance.ChoosedLevel++;

                    }

                    GameManager.Instance.SaveGame();
                    GameManager.Instance.SaveLevel();
                }
            }
            else if (!GameManager.Instance.isFree)
            {
                var pdrop = GameDatabase.Instance.dropMonyeconfig[GameManager.Instance.ChoosedLevel - 1][GameManager.Instance.ChoosedHard];
                int pStarNotEngough = pdrop.requireStar - GameManager.Instance.Database.getComonItem("Star").Quantity;
                float percent = 1 - (pStarNotEngough <= 0 ? 0 : (pStarNotEngough * 0.2f > 0.6f ? 0.6f : pStarNotEngough * 0.2f));
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
            coinTaken.text = StringUtils.addDotMoney((int)pCoin);
        }
        

        public void watch()
        {
            if (GameManager.Instance.isFree) return;
            if (adsCourountine != null)
            {
                StopCoroutine(adsCourountine);
            }
            

           
            GameManager.Instance.showRewardAds("WatchX2WinGame", delegate (bool pSucess)
            {
                if (pSucess)
                {
            
                    var pItem = GameManager.Instance.Database.getComonItem("Coin");
                    pItem.Quantity += LevelManger.Instance._infoLevel.goldTaken;
                    int pTo = LevelManger.Instance._infoLevel.goldTaken * 2;
                    DOTween.To(() => currentCoin, setGold, pTo, 0.5f);
                    GameManager.Instance.SaveGame();
                }
            });
        }
        protected int scoreCount;
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
                if (pNode.misson.process != 1)
                {
                    GameManager.Instance.Database.getComonItem("Star").Quantity++;
                   for(int i  = 0; i < pNode.misson.rewards.Length; ++i)
                    {
                      var pStorage =  GameManager.Instance.Database.getComonItem(pNode.misson.rewards[i].item);
                        pStorage.Quantity += pNode.misson.rewards[i].quantity;
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
            GameManager.Instance.scehduleUI = ScheduleUIMain.GAME_IMEDIATELY;
            PlayerEnviroment.clear();
            GroupManager.clearCache();
            TimeKeeper.Instance.getTimer("Global").TimScale = 1;
            LevelManger.InstanceRaw = null;
            SceneManager.Instance.loadScene("Main");
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
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
   
        }
    }
}
