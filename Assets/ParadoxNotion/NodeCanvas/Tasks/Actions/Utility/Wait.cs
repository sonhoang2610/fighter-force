using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Utility")]
    public class Wait : ActionTask
    {

        public BBParameter<float> waitTime = 1f;
        public BBParameter<TimeControlBehavior> timerBehavior;
        public CompactStatus finishStatus = CompactStatus.Success;
        protected float currentTime = 0;

        protected override void OnExecute()
        {
            if (timerBehavior.value == null)
            {
                timerBehavior.value = ownerAgent.GetComponent<TimeControlBehavior>();
            }
            currentTime = 0;
            base.OnExecute();
        }
        protected override string info {
            get { return string.Format("Wait {0} sec.", waitTime); }
        }

        protected override void OnUpdate() {
            currentTime += timerBehavior.value.time.deltaTime;
            if(currentTime >= waitTime.value) {
                EndAction(finishStatus == CompactStatus.Success ? true : false);
            }
            //if ( elapsedTime >= waitTime.value ) {
            //    EndAction(finishStatus == CompactStatus.Success ? true : false);
            //}
        }
    }
}