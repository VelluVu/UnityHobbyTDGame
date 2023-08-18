using UnityEngine;

namespace TheTD.ScriptableFiniteStateMachine
{
    public abstract class TransitionScriptableObject : ScriptableObject
    {
        public DecisionScriptableObject decision;
        public StateScriptableObject trueState;
        public StateScriptableObject falseState;

        public abstract void Check(FiniteStateMachine fsm);
    }
}