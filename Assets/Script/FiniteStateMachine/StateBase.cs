using System.Linq;
using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    public class StateBase : StateScriptableObject
    {
        private const string ENTER_STATE_FORMAT = "Enter {0} state";
        private const string LEAVE_STATE_FORMAT = "Leave {0} state";

        public override void Enter(FiniteStateMachine fsm)
        {
            Debug.LogFormat(ENTER_STATE_FORMAT, name);
            actions = actions.OrderBy(o => o.order).ToList();
        }

        public override void Exit(FiniteStateMachine fsm)
        {
            Debug.LogFormat(LEAVE_STATE_FORMAT, name);
        }

        public override void Run(FiniteStateMachine fsm)
        {
            //Debug.Log("Running " + name);

            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].action.Run(fsm);
            }

            for (int i = 0; i < transitions.Count; i++)
            {
                transitions[i].transition.Run(fsm);
            }
        }
    }
}