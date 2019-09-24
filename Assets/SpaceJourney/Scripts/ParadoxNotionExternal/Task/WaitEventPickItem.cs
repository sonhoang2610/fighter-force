using NodeCanvas.Framework;
using ParadoxNotion.Design;
using FlowCanvas.Macros;
using EazyEngine.Tools;
using System.Linq;

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
            if (eventType._owner == ownerAgent.gameObject  && !isComplete)
            {
                nameItem.value = eventType._nameItem;
                quantity.value = eventType._quantityItem;
                flow.value = eventType.flow;
                isComplete = true;             
                if (eventType.Variables != null)
                {
                    for (int i = 0; i < eventType.Variables.Count; ++i)
                    {
                        ownerBlackboard.AddVariable(eventType.Variables.Keys.ElementAt(i), eventType.Variables[eventType.Variables.Keys.ElementAt(i)]);
                    }
                }
                EndAction(true);
            }
        }
        protected override string OnInit(){
			return null;
		}

        protected override void OnExecute()
        {
            isComplete = false;
            EzEventManager.AddListener(this);
            base.OnExecute();
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