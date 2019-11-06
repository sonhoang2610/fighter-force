using NodeCanvas.Framework;
using ParadoxNotion.Design;
using EazyEngine.Tools;
using UnityEngine;

namespace EazyEngine.Space{

	[Category("Space")]
	public class CheckEventGame : ConditionTask,EzEventListener<MessageGamePlayEvent>{

        public BBParameter<string> eventName;
        protected bool isRegister = false;
		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit(){
            return null;
		}
        
		//Called once per frame while the condition is active.
		//Return whether the condition is success or failure.
		protected override bool OnCheck(){
            if (!isRegister)
            {
                EzEventManager.AddListener(this);
                isRegister = true;
            }
			return false;
		}

        protected override void OnEnable()
        {
         
            base.OnEnable();
        }

        protected override void OnDisable()
        {
          //  EzEventManager.RemoveListener(this);
            base.OnDisable();
        }

        public void OnEzEvent(MessageGamePlayEvent eventType)
        {
            if (ownerAgent.IsDestroyed())
            {
                EzEventManager.RemoveListener(this);
            }
            if(eventType._objects == null || eventType._objects.Length ==0 || eventType._objects[0] == null ||(GameObject) eventType._objects[0] == ownerAgent.gameObject)
            {
               if(eventType._message == eventName.value)
                {
                    EzEventManager.RemoveListener(this);
                    isRegister = false;
                    YieldReturn(true);
                }
            }
        }
    }
}