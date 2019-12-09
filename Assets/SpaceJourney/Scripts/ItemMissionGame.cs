using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;

namespace EazyEngine.Space.UI
{
    public class ItemMissionGame : BaseItem<MissionItemInstanced>,EzEventListener<MissionEvent>
    {
        public UILabel content;
        public EazyFrameCache status;
        public GameObject iconComplete;
        public ItemInventorySlot itemReward;
        public UIButton btnClaim;
        public UIButton btnWatch;
        public UIProgressBar progress;
        public UILabel progressLabel;
        public GameObject iconClaimed;
        private Tween cacheTween;
        
        public override MissionItemInstanced Data
        {
            get => base.Data;
            set
            {
                base.Data = value;
                value.mission.InstancedModuleId = value.ModuleID;
                content.text = value.mission.Desc;
                status.setFrameIndex((value.Process >= 1 && !value.Claimed) ? 1 : 0);
                if (iconComplete)
                {
                    iconComplete.SetActive(value.Process >= 1);
                }
                itemReward.Data = new BaseItemGameInstanced()
                {
                    item = value.rewards[0].item,
                    quantity = value.rewards[0].Quantity
                };
                btnClaim.gameObject.SetActive(true);
                btnWatch.gameObject.SetActive(false);
                btnClaim.isEnabled = value.Process >= 1;
                progress.value = value.Process;
                progressLabel.text = $"{ (int)(value.Process* value.DestinyFloat) }/{(int) value.DestinyFloat}";
                iconClaimed.gameObject.SetActive(value.Claimed);
                iconClaimed.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                btnClaim.gameObject.SetActive(!value.Claimed);
                //btnWatch.gameObject.SetActive(!value.claimed);
            }
        }

        public void OnEzEvent(MissionEvent eventType)
        {
            if(eventType.missionID == Data.mission.ItemID)
            {
                Data = Data;
            }
        }

        public override void show()
        {
            base.show();
           // if ( Dirty)
            {

                //var widget = GetComponent<UIWidget>();
                //widget.alpha = 0;
                if (cacheTween != null)
                {
                    cacheTween.Kill();
                }
                var widget = GetComponent<UIWidget>();
                widget.alpha = 0;
                var pFade = DOTween.To(() => widget.alpha, x => widget.alpha = x, 1, 0.2f);
                Sequence pSequence = DOTween.Sequence();
                pSequence.AppendInterval(Index * 0.05f);
           
                pSequence.Append(pFade);
                pSequence.Play();
                cacheTween = pSequence;
            }
        }

        private void OnDisable()
        {
            Dirty = true;
            gameObject.SetActive(false);
            EzEventManager.RemoveListener(this);
        }
        private void OnEnable()
        {
           EzEventManager.AddListener(this);
        }

        

        public void claim()
        {
            var pRewards = Data.rewards;
            foreach(var pReward in pRewards)
            {
                var pExist = GameManager.Instance.Database.getComonItem(pReward.item);
                pExist.Quantity += pReward.Quantity;
            }
            Data.Claimed = true;
            Data.currentLevel++;
            if(Data.currentLevel < Data.limitLevel)
            {
                Data.extraInfo();
            }
            EzEventManager.TriggerEvent(new MessageGamePlayEvent("MissionDirty"));
            StartCoroutine(GameManager.Instance.delayAction(0.1f, delegate {
                Data = Data;
                GameManager.Instance.SaveGame();
            }));

            itemReward.showEffect();

        }
        
    }
}