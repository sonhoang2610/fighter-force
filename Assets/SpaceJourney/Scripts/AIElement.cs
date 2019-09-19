using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools.Space;
using EazyEngine.Tools;

namespace EazyEngine.Space
{
    public class AIElement : AIBrain,EzEventListener<AttackStatus>
    {
        public bool ignoreAIMAchine = false;
        protected Health _heath;
        protected bool isDeath = false;
        protected AIMachine parent;
        public AIMachine ParentMachine
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            _heath = gameObject.GetComponent<Health>();
            isDeath = false;
        }
        public override void attack()
        {
            if (!ignoreAIMAchine)
            {
                base.attack();
            }
        }
        public override void OnEnable()
        {
            base.OnEnable();
            _heath = gameObject.GetComponent<Health>();
            _heath.onDeath.AddListener(onDeath);
            isDeath = false;
        }
        public override void OnDisable()
        {
            base.OnDisable();
            _heath.onDeath.RemoveListener(onDeath);
        }
        public void onDeath()
        {
            CanThinking = false;
            isDeath = true;
            if (parent)
            {
                parent.triggerFromElement("Death", this);
            }
        }
        public override void EveryFrameThinking()
        {
            base.EveryFrameThinking();
        }
        public override bool CanAttack
        {
            get
            {
                return base.CanAttack && !isDeath && (_info.conditionStart != ConditionMaChineStart.InSideScreen || LevelManger.Instance.mainPlayCamera.Rect().Contains(transform.position)) ;
            }
        }
        // Start is called before the first frame update
    

        public override void onRespawn()
        {
            base.onRespawn();
            isDeath = false;
        }

        public void OnEzEvent(AttackStatus eventType)
        {
            
        }
    }
}
