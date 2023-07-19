using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    public abstract class StateScriptableObject : ScriptableObject
    {
        public List<ActionSeriliazeObject> actions;
        public List<TransitionSeriliazeObject> transitions;
        public List<DecisionSeriliazeObject> decisions;

        public abstract void Enter(FiniteStateMachine fsm);
        public abstract void Run(FiniteStateMachine fsm);
        public abstract void Exit(FiniteStateMachine fsm);
    }
}