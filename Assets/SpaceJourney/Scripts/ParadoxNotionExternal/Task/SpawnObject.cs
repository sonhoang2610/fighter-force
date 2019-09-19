using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace EazyEngine.Space{

	[Category("Space")]
	public class SpawnObject : ActionTask{
        public BBParameter<Vector3> startPos;
        public bool posMainPlayer = false;
        public int quantity = 1;
        public BBParameter<GameObject> prefab;
        public BBParameter<GameObject> storeObject;
        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit(){
			return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){
            if (prefab.value == null) {
                EndAction(true);
                return;
            }
            for (int i = 0; i < quantity; ++i)
            {
                GameObject pObject = PoolManagerComon.Instance.createNewObject(prefab.value);
                pObject.transform.position = !posMainPlayer ? startPos.value : LevelManger.Instance.CurrentPlayer.transform.position;
                var respawns = pObject.GetComponents<IRespawn>();
                if (respawns != null)
                {
                    for (int j = 0; j < respawns.Length; ++j)
                    {
                        respawns[i].onRespawn();
                    }
                }
                var health = pObject.GetComponent<Health>();
                if (health)
                {
                    health.Revive();
                }
                pObject.SetActive(true);
                storeObject.value = pObject;          
            }
            EndAction(true);
        }

		//Called once per frame while the action is active.
		protected override void OnUpdate(){
			
		}

		//Called when the task is disabled.
		protected override void OnStop(){
			
		}

		//Called when the task is paused.
		protected override void OnPause(){
			
		}
	}
}