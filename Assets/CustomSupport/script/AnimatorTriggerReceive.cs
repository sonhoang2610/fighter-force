using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using EazyEngine.Space;
using Spine.Unity;
using Spine;

namespace EazyEngine.Tools
{
    public interface IListenerTriggerAnimator
    {
        bool registerListen();
        void TriggerFromAnimator(AnimationEvent pEvent);
    }

    public class AnimatorTriggerReceive : SerializedMonoBehaviour
    {
        public Dictionary<string, UnityEvent> _onTrigger = new Dictionary<string, UnityEvent>();
        public Character _charOwner;
        public bool triggerNearestParrent = false;

        public void trigger(AnimationEvent pEvent)
        {
            if (_onTrigger.ContainsKey(pEvent.stringParameter))
            {
                _onTrigger[pEvent.stringParameter].Invoke();
            }

            if (_charOwner != null)
            {
                var pTriggers = _charOwner.GetComponents<IListenerTriggerAnimator>();
                foreach (var pTrigger in pTriggers)
                {
                    pTrigger.TriggerFromAnimator(pEvent);
                }
            }

            if (triggerNearestParrent)
            {
                var pTrigger = GetComponentInParent<IListenerTriggerAnimator>();
                if (pTrigger != null)
                {
                    pTrigger.TriggerFromAnimator(pEvent);
                }
            }
        }

        public void trigger(string pEvent)
        {
            trigger(new AnimationEvent() {stringParameter = pEvent});
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public SkeletonAnimation skeleton;

        private void Awake()
        {
            if (skeleton)
            {
                skeleton.AnimationState.Event += HandleEvent;
            }
        }

        void HandleEvent(TrackEntry trackEntry, Spine.Event e)
        {
            trigger(new AnimationEvent() {stringParameter = e.Data.String});
        }

        public void TriggerSelf(string pTrigger)
        {
            GetComponent<Animator>().SetTrigger(pTrigger);
        }
    }
}