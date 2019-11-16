using EazyEngine.Tools;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using FlowCanvas;
using UnityEngine;

namespace EazyEngine.Space
{

    [Category("Space")]
    public class WaitEventButtonTrigger : ActionTask, EzEventListener<InputButtonTrigger>
    {
        public BBParameter<FlowScript> storeInfo;
        public BBParameter<ItemGame> infoItem;
        protected bool isComplete = false;

        protected override void OnExecute()
        {
            EzEventManager.AddListener(this);
            isComplete = false;
            base.OnExecute();
        }

        protected override void OnStop()
        {
            EzEventManager.RemoveListener(this);
            base.OnStop();
        }
   
        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit()
        {
            return null;
        }


        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isComplete)
            {
                EndAction(true);
            }
        }

        public void OnEzEvent(InputButtonTrigger eventType)
        {
            var player = LevelManger.Instance.players[eventType.indexTarget];
            if (player.gameObject == ownerAgent.gameObject)
            {
                infoItem.value = (ItemGame)GameDatabase.Instance.getItem(eventType.trigger, (CategoryItem)eventType.cateGory);
                storeInfo.value = ((ItemGame) GameDatabase.Instance.getItem(eventType.trigger,(CategoryItem) eventType.cateGory)).controller;
                isComplete = true;
                EndAction(true);
            }
        }
    }
}