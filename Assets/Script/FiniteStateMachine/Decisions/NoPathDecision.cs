using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    [CreateAssetMenu(fileName = "NoPath", menuName = "ScriptableObjects/FiniteStateMachine/Decisions/NoPath")]
    public class NoPathDecision : DecisionScriptableObject
    {
        public override bool Decide(FiniteStateMachine fsm)
        {
            return !fsm.PathControl.HasPath;
        }
    }
}