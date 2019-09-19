using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace EazyEngine.Space{

	[Category("Space")]
	public class RunState : ActionTask{

        public string nameState;
        public BBParameter<bool> wait = true;
         protected LevelState _state;
        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit(){
			return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){
            _state = LevelStateManager.Instance.runState(nameState);
            if (!wait.value)
            {
                EndAction(true);
            }
            //EndAction(true);
        }

		//Called once per frame while the action is active.
		protected override void OnUpdate(){
            if ( _state != null && _state.IsComplete)
            {
                 EndAction(true);
            }
        }

		//Called when the task is disabled.
		protected override void OnStop(){
			
		}

		//Called when the task is paused.
		protected override void OnPause(){
			
		}
	}
}