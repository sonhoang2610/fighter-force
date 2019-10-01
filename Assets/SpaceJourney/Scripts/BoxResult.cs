using EazyEngine.Timer;
using FlowCanvas;
using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        public BoxMissionLevel boxMission;
        List<EazyNode> pNodes = new List<EazyNode>();

        public void showTestWin()
        {
            gameObject.SetActive(true);
            showResult(true);
        }
        int timeShowLose = 0;

        public void nextPlay()
        {
            GameManager.Instance.scehduleUI = ScheduleUIMain.GAME_IMEDIATELY;
            LevelInfoInstance pLevelInfo = GameManager.Instance.container.getLevelInfo(GameManager.Instance.Database.lastPlayStage.x + 1, GameManager.Instance.ChoosedHard);
            if (pLevelInfo.isLocked)
            {
                GameManager.Instance.ChoosedHard = 0;
            }
            GameManager.Instance.Database.lastPlayStage = new Pos(GameManager.Instance.Database.lastPlayStage.x + 1, GameManager.Instance.ChoosedHard);
            Home();
        }   
        public void showResult(bool pWin)
        {
          
            if (!pWin && timeShowLose == 0) {
                TimeKeeper.Instance.getTimer("Global").TimScale = 0;
                timeShowLose++;
                HUDLayer.Instance.BoxReborn.show();
                GameManager.Instance.showBannerAds(true);
                return;
            }
            GameManager.Instance.showInterstitialAds();
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
            stage.text = GameManager.Instance.ChoosedLevel.ToString();
            string[] pStrs = new string[3] { "ui/normal", "ui/hard", "ui/super_hard" };
            level.text = I2.Loc.LocalizationManager.GetTranslation(pStrs[GameManager.Instance.ChoosedHard]);
            coinTaken.text = StringUtils.addDotMoney(LevelManger.Instance._infoLevel.goldTaken);
            boxMission.DataSource = LevelManger.Instance._infoLevel.missions.ToObservableList();  
            time.text = LevelManger.Instance.CurrentTime.ToString(@"mm\:ss");
            quantityDestroy.text = LevelManger.Instance._infoLevel.enemyKill.ToString();
        
            gameObject.SetActive(true);
            if (pWin)
            {
                if (!GameManager.Instance.isFree)
                {
                    GameManager.Instance.Database.getComonItem("Coin").Quantity += LevelManger.Instance._infoLevel.goldTaken;
                    if (GameManager.Instance.ChoosedHard < 2)
                    {
                        GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard + 1).isLocked = false;
                    }
                    GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, GameManager.Instance.ChoosedHard).infos = LevelManger.Instance._infoLevel;
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
        }

        public void watch()
        {
            GameManager.Instance.showRewardAds("WatchX2WinGame", delegate (bool pSucess)
            {
                if (pSucess)
                {
                    
                    var pItem = GameManager.Instance.Database.getComonItem("Coin");
                    pItem.Quantity += LevelManger.Instance._infoLevel.goldTaken;
                    GameManager.Instance.SaveGame();
                }
                Debug.Log("Watach X2 " + pSucess.ToString());
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
            this.score.text = pScore.ToString();
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
            TimeKeeper.Instance.getTimer("Global").TimScale = 1;
            GameManager.Instance.showBannerAds(false);
        }
        public void Home()
        {
            PlayerEnviroment.clear();
            TimeKeeper.Instance.getTimer("Global").TimScale = 1;
            LevelManger.InstanceRaw = null;
            SceneManager.Instance.loadScene("Main");
        }

        public void Replay()
        {
            GameManager.Instance.scehduleUI = ScheduleUIMain.GAME_IMEDIATELY;
            PlayerEnviroment.clear();
            TimeKeeper.Instance.getTimer("Global").TimScale = 1;
            LevelManger.InstanceRaw = null;
            SceneManager.Instance.loadScene("Main");
        }

        public void Upgrade()
        {
            GameManager.Instance.scehduleUI = ScheduleUIMain.UPGRADE;
            PlayerEnviroment.clear();
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
