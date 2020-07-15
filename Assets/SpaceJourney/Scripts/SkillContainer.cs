using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;
using NodeCanvas.Framework;
using System.Linq;
using EazyEngine.Tools;
using EazyEngine.Space.UI;
using EazyEngine.Timer;
using DG.Tweening;

namespace EazyEngine.Space
{

    public class SkillContainer : BaseBoxSingleton<SkillContainer,ButtonSkill, SkillInputData>,EzEventListener<MessageGamePlayEvent>
    {
        protected bool isGuideExist = false;
        public override void setDataItem(SkillInputData pData, ButtonSkill pItem)
        {
            base.setDataItem(pData, pItem);
            if(pData._info.Info.ItemID == "BigLaser")
            {
                pItem.name = "CoreBigLaser";
                isGuideExist = true;
            }
        }
        public override ObservableList<SkillInputData> DataSource { get => base.DataSource;
            set {
                List<SkillInputData> pData = new List<SkillInputData>();
                for(int i = 0; i <value.Count; ++i)
                {
                    if (value[i]._info.Info.isActive)
                    {
                        if (value[i]._info.CurrentLevelSkill > 0 || GameManager.Instance.isFree)
                            pData.Add(value[i]);
                    }
                    else
                    {
                        if (value[i]._info.CurrentLevelSkill > 0 || GameManager.Instance.isFree)
                        {
                            GameObject pOriginal = value[i]._targetOwner.transform.Find("DefaultDeactiveSkill").gameObject;
                            GameObject pControler = Instantiate(pOriginal, pOriginal.transform.parent);
                            var pBlackBoard = pControler.GetComponent<Blackboard>();
                            pBlackBoard.SetValue("Condition", value[i]._info.Info.condition);
                            pBlackBoard.SetValue("Controller", value[i]._info.Info.controller);
                            pBlackBoard.SetValue("Info", value[i]._info.Info);
                            pBlackBoard.SetValue("Duration", value[i]._info.Info.Duration);
                            pBlackBoard.SetValue("Cooldown", value[i]._info.Info.CoolDownTime);
                            pBlackBoard.SetValue("CurrentLevel", value[i]._info.CurrentLevelSkill);
                            for (int j = 0; j < value[i]._info.skillBlackBoard.Count; ++j)
                            {
                                var pKey = value[i]._info.skillBlackBoard.Keys.ElementAt(j);
                                pBlackBoard.AddVariable(value[i]._info.skillBlackBoard.Keys.ElementAt(j), value[i]._info.skillBlackBoard[pKey]);
                            }
                            var pSkillSelfVar = value[i]._info.Info.VariableDict;
                            for (int j = 0; j < pSkillSelfVar.Count; ++j)
                            {
                                var pKey = pSkillSelfVar.Keys.ElementAt(j);
                                pBlackBoard.AddVariable(pSkillSelfVar.Keys.ElementAt(j), pSkillSelfVar[pKey]);
                            }
                            pControler.gameObject.SetActive(true);
                        }
                    }
                }
                base.DataSource = pData.ToObservableList();
          
            } }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        protected bool isGuideskill = false;
        private void OnEnable()
        {
            isGuideskill = PlayerPrefs.GetInt(StringKeyGuide.FirstGuideSkill, 0) != 0;
            EzEventManager.AddListener(this);
        }

        private void OnDisable()
        {
            EzEventManager.RemoveListener(this);
        }

        public void OnEzEvent(MessageGamePlayEvent eventType)
        {
           if(eventType._message == "LoadLevelComplete" )
            {
                if (!isGuideskill && isGuideExist)
                {
                    EzEventManager.TriggerEvent(new GuideEvent(StringKeyGuide.FirstGuideSkill));
                    PlayerPrefs.SetInt(StringKeyGuide.FirstGuideSkill, 1);
                }
            }
        }
    }
}
