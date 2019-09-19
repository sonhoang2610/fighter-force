using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Utility")]
    public class Wait : ActionTask
    {

        public BBParameter<float> waitTime = 1f;
        public CompactStatus finishStatus = CompactStatus.Success;
	    public BBParameter<TimeControlBehavior> timeBehavior;
	    
	    protected float currentTime;

	    protected override string info {
            get { return string.Format("Wait {0} sec.", waitTime); }
        }
        
	    protected override void OnExecute()
	    {
	    	base.OnExecute();
	    	if(timeBehavior.value == null){
	    		timeBehavior.value = ownerAgent.GetComponent<TimeControlBehavior>();
	    		
	    	}
	    	currentTime = 0;
	    
	    }

	    protected override void OnUpdate() {
		    if(timeBehavior.value!= null){
	    		currentTime += timeBehavior.value.time.deltaTime;
	    	}else{
	    		currentTime += UnityEngine.Time.deltaTime;
	    	}
            if ( currentTime >= waitTime.value ) {
                EndAction(finishStatus == CompactStatus.Success ? true : false);
            }
        }
    }
}