﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using EazyEngine.Tools;
using NodeCanvas.Framework;
using System.Linq;
using EazyEngine.Timer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EazyEngine.Space {

    public class SkillInputData
    {
        public SkillInfoInstance _info;
        public Character _targetOwner;
        public SkillInputData (SkillInfoInstance pInfo,Character pTarget)
        {
            _targetOwner = pTarget;
            _info = pInfo;
        }
    }
    public class ButtonSkill : BaseItem<SkillInputData>
    {
        public UI2DSprite fillCoolDown;
        public TimeController time;
        public void press()
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("PressSkill");
            for (int i = 0;  i < Data._info.Info.VariableDict.Count; ++i)
            {
                var pDict = Data._info.Info.VariableDict;
                LevelManger.Instance.players[0].GetComponent<Blackboard>().AddVariable(pDict.Keys.ElementAt(i), pDict[pDict.Keys.ElementAt(i)]);
            }
          
            EzEventManager.TriggerEvent(new InputButtonTrigger(  Data._info.Info.ItemID,Data._info.Info.categoryItem));
            fillCoolDown.gameObject.SetActive(true);
            fillCoolDown.fillAmount = 1;
            var tween = DOTween.To(()=> fillCoolDown.fillAmount,x => fillCoolDown.fillAmount = x,0, Data._info.Info.CoolDownTime).OnComplete(() => fillCoolDown.gameObject.SetActive(false));
            tween.OnUpdate(() => tween.timeScale = time.TimScale);
          //  fillCoolDown.do(0, Data._info.info.coolDownTime).OnComplete(() => fillCoolDown.gameObject.SetActive(false));
        }

        public override SkillInputData Data
        {
            get
            {
                return base.Data;
            }

            set
            {
                GetComponent<UI2DSprite>().sprite2D = value._info.Info.iconGame;
                base.Data = value;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            time = TimeKeeper.Instance.getTimer("Global");
        }

        // Update is called once per frame
        void Update()
        {

        }
        
    }
}
