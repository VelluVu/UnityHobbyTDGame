using UnityEngine;

[CreateAssetMenu(fileName = "Transition", menuName = "ScriptableObjects/FiniteStateMachine/Transitions/TransitionBase")]
public class TransitionBase : TransitionScriptableObject
{
    public override void Run(FiniteStateMachine fsm)
    {
        this.fsm = fsm;
        if(decision.Decide(fsm) && !(trueState is RemainInState))
            fsm.CurrentState = trueState;
        else if(!(falseState is RemainInState))
            fsm.CurrentState = falseState;
    }
}
