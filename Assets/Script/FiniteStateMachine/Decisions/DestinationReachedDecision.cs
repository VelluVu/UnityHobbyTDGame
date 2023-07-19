using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    [CreateAssetMenu(fileName = "DestinationReached", menuName = "ScriptableObjects/FiniteStateMachine/Decisions/DestinationReached")]
    public class DestinationReachedDecision : DecisionScriptableObject
    {
        public override bool Decide(FiniteStateMachine fsm)
        {
            var destinationReached = fsm.HasReachedEnd;   
            return destinationReached;
        }
    }
}