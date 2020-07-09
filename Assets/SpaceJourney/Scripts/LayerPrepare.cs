using EazyEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

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
        public EazyGroupTabNGUI _chooseHardMode;
        public UILabel desItemSp;
        public BoxItemSkill boxItem;
        public override MapInfoInstanced Data { get => base.Data; set {
                base.Data = value;
                level.text = "[FFDC00]MAP " + value.level.ToString()+":[-]" + value.nameMap;
                highScore.text = StringUtils.addDotMoney( value.highScore);
            }
        }
        
        private void OnEnable()
        {
            // showInfo(GameManager.Instance.ChoosedLevel,0);
            if (GameManager.Instance.ConfigLevel == null)
            {
                GameManager.Instance.ConfigLevel = new LevelConfig();
            }
            StartCoroutine(enable());
            boxItem.gameObject.SetActive(GameManager.Instance.ChoosedLevel != 1 || PlayerPrefs.GetInt(StringKeyGuide.OpenPrepare) > 0);

        }

        private IEnumerator enable()
        {
            yield return new WaitForSeconds(0.1f);
     
            int lastChoosedIndex = 0;
            for (int i = 0; i < 3; ++i)
            {
                var pLevelInfo = GameManager.Instance.container.getLevelInfo(GameManager.Instance.ChoosedLevel, i);
                _chooseHardMode.GroupTab[i].Button.isEnabled = true;
                if (pLevelInfo.isLocked)
                {
                    _chooseHardMode.GroupTab[i].Button.isEnabled = false;
                }
                else
                {
                    _chooseHardMode.GroupTab[i].Button.isEnabled = true;
                    if (i > 0)
                    {
                        var pShow = PlayerPrefs.GetInt("ShowUnlockLevel" + GameManager.Instance.ChoosedLevel + "Mode" + i, 0);
                        if (pShow == 0)
                        {
                            _chooseHardMode.GroupTab[i].transform.GetChild(0).gameObject.SetActive(true);
                            var pColor = _chooseHardMode.GroupTab[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color;
                            pColor.a = 1;
                            _chooseHardMode.GroupTab[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = pColor;
                            PlayerPrefs.SetInt("ShowUnlockLevel" + GameManager.Instance.ChoosedLevel + "Mode" + i, 1);
                        }
                    }
             
                    lastChoosedIndex = i;
                }

            }
        
            if (GameManager.Instance.scehduleUI != ScheduleUIMain.REPLAY)
            {
                _chooseHardMode.changeTab(lastChoosedIndex);
            }
            else
            {
                _chooseHardMode.changeTab(GameManager.Instance.Database.lastPlayStage.y);
            }
            yield return new WaitForSeconds(0.5f);
            if (AssetLoaderManager.Instance.getPercentJob("Main") >= 1 || (!LevelManger.InstanceRaw.IsDestroyed() && GameManager.Instance.CurrentLevelUnlock >= 2 && !LevelManger.Instance.IsPlaying))
            {
                var OpenTime = PlayerPrefs.GetInt(StringKeyGuide.OpenPrepare, 0);
                if (OpenTime == 0)
                {
                    EzEventManager.TriggerEvent(new GuideEvent(StringKeyGuide.OpenPrepare,delegate { EzEventManager.TriggerEvent(new GuideEvent(StringKeyGuide.StartGameNow)); PlayerPrefs.SetInt(StringKeyGuide.OpenPrepare, OpenTime + 1); },false));
                }
            
            }
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
            int pRequireStar = 0;
            for(int i = 0; i < GameDatabase.Instance.LevelConfigScene.Length; ++i)
            {
                if(GameDatabase.Instance.LevelConfigScene[i].level == GameManager.Instance.ChoosedLevel)
                {
                    pRequireStar = GameDatabase.Instance.LevelConfigScene[i].requrireStarToUnlock;
                }
            }
            btnPlay.isEnabled = !pLevelInfo.isLocked && GameManager.Instance.Database.getComonItem("Star").Quantity >= pRequireStar;
            showInfo(GameManager.Instance.ChoosedLevel, index);
        }
        public AudioGroupSelector fightSfx = AudioGroupConstrant.Fight;
        public void play()
        {
            GameManager.Instance.isFree = false;
            SoundManager.Instance.PlaySound(fightSfx, Vector3.zero);
            GameManager.Instance.LoadLevel(GameManager.Instance.ChoosedLevel);
        }
        public void chooseUseItem(object pObject)
        {
            if (pObject == null) return;

            if (GameManager.Instance.ConfigLevel == null)
            {
                GameManager.Instance.ConfigLevel = new LevelConfig();
            }
            var pItem = (ItemOutGameInfo)pObject;
            desItemSp.text = pItem.item.item.descriptionItem.Value;
            if (!pItem.isChoosed)
            {
                desItemSp.gameObject.SetActive(false);
                GameManager.Instance.ConfigLevel.itemUsed.RemoveAll(x=>x.ItemID == pItem.item.item.ItemID);
            }
            else
            {
                desItemSp.gameObject.SetActive(true);
                GameManager.Instance.ConfigLevel.itemUsed.RemoveAll(x => x.ItemID == pItem.item.item.ItemID);
                if (GameManager.Instance.Database.getComonItem(pItem.item.item.ItemID).Quantity > 0)
                {
                    GameManager.Instance.ConfigLevel.itemUsed.Add((ItemGame)pItem.item.item);
                }
            }
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
