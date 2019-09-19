using EazyEngine.Tools;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace EazyEngine.Space{

	[Category("Space")]
	public class WaitEventGame : ActionTask, EzEventListener<MessageGamePlayEvent>
    {

        public string messCompare;
        protected bool isComplete = false;

        protected override string info { get { return base.info + messCompare; } }

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
        protected override string OnInit(){
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

        public void OnEzEvent(MessageGamePlayEvent eventType)
        {
            if (!string.IsNullOrEmpty(messCompare) && (eventType._objects == null || eventType._objects.Length == 0 || eventType._objects[0] == (object)ownerSystem.agent.gameObject))
            {
                if (messCompare == eventType._message)
                {
                    isComplete = true;
                }
            }
        }
    }
}