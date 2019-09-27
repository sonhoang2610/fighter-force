using NodeCanvas.Framework;
using ParadoxNotion.Design;
using FlowCanvas.Macros;
using EazyEngine.Tools;
using System.Linq;
using System.Collections.Generic;

namespace EazyEngine.Space{

	[Category("Space")]
	public class WaitEventPickItem : ActionTask, EzEventListener<PickEvent>
    {

        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        bool isComplete = false;
        public BBParameter<string> nameItem;
        public BBParameter<int> quantity;
        public BBParameter<FlowCanvas.FlowScript> flow;
        public void OnEzEvent(PickEvent eventType)
        {
            if (ownerAgent == null) EndAction(false);
            if (eventType._owner == ownerAgent.gameObject && !isComplete)
            {
                executeEvent(eventType);
       
            }
        }

        protected void executeEvent(PickEvent eventType)
        {
            nameItem.value = eventType._nameItem;
            quantity.value = eventType._quantityItem;
            if (eventType.flow != null)
            {
                flow.value = eventType.flow;
            }

            isComplete = true;
            if (eventType.Variables != null)
            {
                for (int i = 0; i < eventType.Variables.Count; ++i)
                {
                    ownerBlackboard.AddVariable(eventType.Variables.Keys.ElementAt(i), eventType.Variables[eventType.Variables.Keys.ElementAt(i)]);
                }
            }
            EndAction(eventType.flow);
        }
        protected override string OnInit(){
			return null;
		}

        protected override void OnExecute()
        {
            base.OnExecute();
                isComplete = false;
                EzEventManager.AddListener(this);
       
       
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
        }
        protected override void OnStop()
        {
                EzEventManager.RemoveListener(this);
          
            base.OnStop();
        }
    }
}