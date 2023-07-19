using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    public abstract class ActionScriptableObject : ScriptableObject
    {
        public abstract void Run(FiniteStateMachine fsm);
    }
}