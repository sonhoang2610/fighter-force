using EazyEngine.Tools;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace EazyEngine.Space{

	[Category("Space")]
	public class WaitEventTakenDamage : ActionTask, EzEventListener<DamageTakenEvent>
    {
        public BBParameter<float> damageTaken;
        protected bool isComplete = false;

        protected float damageTakenTotal = 0;
        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit(){
            ownerAgent.GetComponent<Health>().onDeath.RemoveListener(onRespawn);
            ownerAgent.GetComponent<Health>().onDeath.AddListener(onRespawn);

            return null;
		}

        public void onRespawn()
        {
            damageTakenTotal = 0;
            isComplete =false ;
        }

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){
            EzEventManager.AddListener(this);
            damageTakenTotal = 0;
            isComplete = false;
        }

        //Called once per frame while the action is active.
        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        //Called when the task is disabled.
        protected override void OnStop(){
			
		}

		//Called when the task is paused.
		protected override void OnPause(){
			
		}

        public void OnEzEvent(DamageTakenEvent eventType)
        {
            if(eventType.AffectedCharacter.gameObject == ownerAgent.gameObject)
            {
                damageTakenTotal += eventType.DamageCaused;
                if(damageTakenTotal >= damageTaken.value)
                {
                    isComplete = true;
                    EndAction(true);
                }
            }
        }
    }
}