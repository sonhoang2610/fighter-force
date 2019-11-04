using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space.UI
{
    public class MapInfoInstanced
    {
        public int level;
        public int highScore;
        public string nameMap;
    }
    public class LayerPrepare : BaseItem<MapInfoInstanced>
    {
        public UILabel level, highScore,goldReceived;
        public UIButton btnPlay;
        public override MapInfoInstanced Data { get => base.Data; set {
                base.Data = value;
                level.text = "[FFDC00]MAP " + value.level.ToString()+":[-]" + value.nameMap;
                highScore.text = StringUtils.addDotMoney( value.highScore);
            }
        }
        private void OnEnable()
        {
           // showInfo(GameManager.Instance.ChoosedLevel,0);
        }
        public void showInfo(int pLevel,int pHard)
        {
            var pdrop = GameDatabase.Instance.dropMonyeconfig[pLevel - 1][pHard];
            int pStarNotEngough = pdrop.requireStar - GameManager.Instance.Database.getComonItem("Star").Quantity;
            float percent = 1 - (pStarNotEngough <= 0 ? 0 : (pStarNotEngough * 0.2f > 0.6f ? 0.6f : pStarNotEngough * 0.2f));
            if(pStarNotEngough <= 0)
            {
                goldReceived.gameObject.SetActive(false);
            }
            else
            {
                goldReceived.gameObject.SetActive(true);
            }
            goldReceived.text = string.Format(I2.Loc.LocalizationManager.GetTranslation("ui/gold_received"), percent*100, pdrop.requireStar).ToUpper();
            var levelinfo = GameManager.Instance.container.getLevelInfo(pLevel, pHard);
            var mapInfo = GameDatabase.Instance.getMapInfo(pLevel);
            Data = new MapInfoInstanced() { highScore = levelinfo.infos.score, level = pLevel, nameMap = mapInfo.mapName.Value };
        }

        public void chooseHardMode(int index)
        {
           var pLevelInfo =  GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, index);
            if(index <2)
            {
               var pLevelInfoOld = GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, index+1);
                if (!pLevelInfoOld.isLocked)
                {
                    pLevelInfo.isLocked = false;
                }
            }
            btnPlay.isEnabled = !pLevelInfo.isLocked;
            showInfo(GameManager.Instance.ChoosedLevel, index);
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
