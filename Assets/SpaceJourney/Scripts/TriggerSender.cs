using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using NodeCanvas.Framework;

namespace EazyEngine.Space {
    public class TriggerSender : MonoBehaviour
    {
        public GameObject _target = null;
        public void sendTriggerEveryOne(string pTrigger)
        {
            EzEventManager.TriggerEvent(new MessageGamePlayEvent(pTrigger));
        }
        public void sendTriggerTargetSetBefore(string pTrigger)
        {
            EzEventManager.TriggerEvent(new MessageGamePlayEvent(pTrigger, _target));
        }
        public void SendGraphMess(string pTrigger)
        {
            _target.GetComponent<GraphOwner>().SendEvent(pTrigger);
        }
        public void setObjectTarget(GameObject pObject)
        {
            _target = pObject;
        }
        public void sendTriggerOwner(string pTrigger)
        {
            EzEventManager.TriggerEvent(new MessageGamePlayEvent(pTrigger, gameObject));
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void LateUpdate()
        {
            _target = null;
        }
    }
}