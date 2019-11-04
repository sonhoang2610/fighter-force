using EazyEngine.Tools;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EazyEngine.Space{


    [Name("PickEventExcute")]
    [Category("Nested")]
    [Description("Executes a nested FlowScript. Returns Running while the FlowScript is active. You can Finish the FlowScript with the 'Finish' node and return Success or Failure")]
    [Icon("FS")]
    public class WaitEventPickItemPushQueue : BTNestedFlowScript, EzEventListener<PickEvent>
    {

        protected List<PickEvent> events = new List<PickEvent>();
        protected bool init = false;
        public void OnEzEvent(PickEvent eventType)
        {
            if(graphAgent== null)
            {
                init = false;
                EzEventManager.RemoveListener(this);
                return;
            }
            if (eventType._owner == graphAgent.gameObject)
            {
                for (int i = 0; i < eventType.Variables.Count; ++i)
                {
                    if (!graph.blackboard.variables.ContainsKey(eventType.Variables.ElementAt(i).Key))
                    {
                        graph.blackboard.AddVariable(eventType.Variables.ElementAt(i).Key, eventType.Variables.ElementAt(i).Value);
                    }
                    else
                    {
                        graph.blackboard.SetValue(eventType.Variables.ElementAt(i).Key, eventType.Variables.ElementAt(i).Value);
                    }
                }
                events.Add(eventType);
            }           
        }
        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (!init)
            {
                EzEventManager.AddListener(this);
                init = true;
            }
            if (!flowScript)
            {
               if(events.Count > 0)
                {
                    do
                    {
                        if (events[0].flow)
                        {
                            flowScript = events[0].flow;
                            status = Status.Resting;
                        }
                        events.RemoveAt(0);
                    } while (!flowScript && events.Count > 0);
                }
            
            }
            if (!flowScript)
            {
                return  Status.Running;
            }
            if (status == Status.Resting)
            {
                currentInstance = CheckInstance();
                status = Status.Running;
                currentInstance.StartGraph(agent, blackboard, false, OnFlowScriptFinished);
            }

            if (status == Status.Running)
            {
                currentInstance.UpdateGraph();
            }

            return Status.Running;
        }
        protected override void OnFlowScriptFinished(bool success)
        {
            status = Status.Running;
            flowScript = null;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            EzEventManager.RemoveListener(this);
        }

    }
}