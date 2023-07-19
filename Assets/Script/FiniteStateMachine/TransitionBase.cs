using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    [CreateAssetMenu(fileName = "Transition", menuName = "ScriptableObjects/FiniteStateMachine/Transitions/TransitionBase")]
    public class TransitionBase : TransitionScriptableObject
    {
        public override void Run(FiniteStateMachine fsm)
        {
            if (decision.Decide(fsm) && !(trueState is RemainInState))
                fsm.ChangeState(trueState);
            else if (!decision.Decide(fsm) && !(falseState is RemainInState))
                fsm.ChangeState(falseState);
        }
    }
}