using UnityEngine;

public abstract class DecisionScriptableObject : ScriptableObject
{
    protected FiniteStateMachine fsm;
    public abstract bool Decide(FiniteStateMachine fsm);
}
