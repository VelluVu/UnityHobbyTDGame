using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    public abstract class DecisionScriptableObject : ScriptableObject
    {
        public abstract bool Decide(FiniteStateMachine fsm);
    }
}