using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class WaitRandom : Wait{
        public BBParameter<Vector2> random;
        protected override void OnExecute()
        {
            waitTime.value = Random.Range(random.value.x, random.value.y);
            base.OnExecute();
           
        }
        protected override string info
        {
            get { return string.Format("Wait random from {0} to {1} sec.", random.value.x, random.value.y); }
        }
    }
}