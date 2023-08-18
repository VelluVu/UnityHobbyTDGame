using UnityEngine;

namespace TheTD.ScriptableFiniteStateMachine
{
    public abstract class DecisionScriptableObject : ScriptableObject
    {
        public abstract bool Decide(FiniteStateMachine fsm);
    }
}