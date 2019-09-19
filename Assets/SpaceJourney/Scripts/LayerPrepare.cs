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
        public UILabel level, highScore;
        public override MapInfoInstanced Data { get => base.Data; set {
                base.Data = value;
                level.text = "[FFDC00]MAP " + value.level.ToString()+":[-]" + value.nameMap;
                highScore.text = value.highScore.ToString();
            }
        }
        private void OnEnable()
        {
           // showInfo(GameManager.Instance.ChoosedLevel,0);
        }
        public void showInfo(int pLevel,int pHard)
        {
            var levelinfo = GameManager.Instance.container.getLevelInfo(pLevel, pHard);
            var mapInfo = GameDatabase.Instance.getMapInfo(pLevel);
            Data = new MapInfoInstanced() { highScore = levelinfo.infos.score, level = pLevel, nameMap = mapInfo.mapName.Value };
        }

        public void chooseHardMode(int index)
        {
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
