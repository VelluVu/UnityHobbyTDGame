using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    public abstract class TransitionScriptableObject : ScriptableObject
    {
        public DecisionScriptableObject decision;
        public StateScriptableObject trueState;
        public StateScriptableObject falseState;

        public abstract void Run(FiniteStateMachine fsm);
    }
}