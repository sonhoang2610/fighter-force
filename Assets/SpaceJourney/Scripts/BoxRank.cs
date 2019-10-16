using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.SocialPlatforms;

namespace EazyEngine.Space.UI
{
    public class BoxRank : BaseBox<ItemRank,RankInfo>,EzEventListener<UIMessEvent>
    {
        public GameObject layerLogin;
        public ItemRank itemSelf;
        public void OnEzEvent(UIMessEvent eventType)
        {
            if(eventType.Event == "GameServiceInitialized")
            {
                layerLogin.gameObject.SetActive(false);
                Debug.Log("leadboard");
                GameServices.ShowLeaderboardUI(EM_GameServicesConstants.Leaderboard_Top_Fighter);
                GameServices.LoadLocalUserScore(EM_GameServicesConstants.Leaderboard_Top_Fighter, resultLocalScores);
                GameServices.LoadScores(EM_GameServicesConstants.Leaderboard_Top_Fighter, 0, 19, TimeScope.AllTime, UserScope.Global, resultScores);
            }
        }

        protected List<RankInfo> cacheInfo = new List<RankInfo>();
        // Start is called before the first frame update
        void Start()
        {
       
        }
        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }
        private void OnEnable()
        {
            EzEventManager.AddListener(this);
            if (GameServices.IsInitialized())
            {
                Debug.Log("leadboard");
                layerLogin.gameObject.SetActive(false);
                GameServices.ShowLeaderboardUI(EM_GameServicesConstants.Leaderboard_Top_Fighter);
                GameServices.LoadLocalUserScore(EM_GameServicesConstants.Leaderboard_Top_Fighter, resultLocalScores);
                GameServices.LoadScores(EM_GameServicesConstants.Leaderboard_Top_Fighter,0,19,TimeScope.AllTime,UserScope.Global, resultScores);
            }
            else
            {
                layerLogin.gameObject.SetActive(true);
            }
        }
        public void resultLocalScores(string pName, IScore scores)
        {
            var pInfo = new RankInfo() { index = scores.rank, level = scores.value, id = scores.userID,name = GameServices.LocalUser.userName,ava = GameServices.LocalUser.image };
            
            itemSelf.Data = pInfo;
        }

        public void resultScores(string pName ,IScore[] scores)
        {
            Debug.Log("scoreleng" + scores.Length );
            cacheInfo = new List<RankInfo>();
            var ids = new List<string>();
            for (int i = 0; i < scores.Length; ++i)
            {
                ids.Add(scores[i].userID);
                cacheInfo.Add(new RankInfo() { index = i, level = scores[i].value, id = scores[i].userID });
            }
            GameServices.LoadUsers(ids.ToArray(), resultProfile);
        }
        public void resultProfile(IUserProfile[] propfiles)
        {
            Debug.Log("propleng" + propfiles.Length );
            for(int i = 0; i< propfiles.Length; ++i)
            {
               for(int j  = 0; j < cacheInfo.Count; ++j)
                {
                    if (cacheInfo[j].id == propfiles[i].id) {
                        cacheInfo[j].ava = propfiles[i].image;
                        cacheInfo[j].name = propfiles[i].userName;
                    }
                }
            }
            DataSource = cacheInfo.ToObservableList();
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

