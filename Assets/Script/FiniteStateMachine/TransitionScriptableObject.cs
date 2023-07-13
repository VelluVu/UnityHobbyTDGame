using UnityEngine;

public abstract class TransitionScriptableObject : ScriptableObject
{
    protected FiniteStateMachine fsm;
    public DecisionScriptableObject decision;
    public StateScriptableObject trueState;
    public StateScriptableObject falseState;

    public abstract void Run(FiniteStateMachine fsm);
}
