using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas.BehaviourTrees;

namespace EazyEngine.Extension
{
    [System.Serializable]
    public class EventSkeleton : UnityEvent<TrackEntry, Spine.Event>
    {

    }
    public class EazySpineAnimator : MonoBehaviour
    {
        public SkeletonAnimation skeleton;
        public EventSkeleton onEvent;

        protected BehaviourTreeOwner behavior;
        private void Awake()
        {
            skeleton.AnimationState.Event += HandleEvent;
            behavior = GetComponent<BehaviourTreeOwner>();
        }

        void HandleEvent(TrackEntry trackEntry, Spine.Event e)
        {
            onEvent.Invoke(trackEntry, e);
            behavior.SendEvent(e.String, e.Data.String);
        }

        public void setTrigger(string pTrigger)
        {
            if (behavior)
            {
                behavior.SendEvent("SetTrigger", pTrigger);
            }
        }

        public void runAnimation(int layer,string nameAnim,bool isLoop)
        {
          //  skeleton.Anima
           var track = skeleton.AnimationState.SetAnimation(layer, nameAnim, isLoop);
        }
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
