using UnityEngine;

namespace TheTD.ScriptableFiniteStateMachine
{
    [CreateAssetMenu(fileName = "ReadyToTravel", menuName = "ScriptableObjects/FiniteStateMachine/Decisions/ReadyToTravel")]
    public class ReadyToTravelDecision : DecisionScriptableObject
    {
        public override bool Decide(FiniteStateMachine fsm)
        {
            var isReadyToTravelAgain = fsm.PathControl.HasPath && !fsm.HasReachedEnd;
            return isReadyToTravelAgain;
        }
    }
}