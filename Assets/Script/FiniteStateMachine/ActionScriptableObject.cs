using UnityEngine;

namespace TheTD.ScriptableFiniteStateMachine
{
    public abstract class ActionScriptableObject : ScriptableObject
    {
        public abstract void Act(FiniteStateMachine fsm);
    }
}