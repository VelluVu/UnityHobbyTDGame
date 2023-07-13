using UnityEngine;

[CreateAssetMenu(fileName = "DestinationReached", menuName = "ScriptableObjects/FiniteStateMachine/Decisions/DestinationReached")]
public class DestinationReached : DecisionScriptableObject
{       
    public override bool Decide(FiniteStateMachine fsm)
    {
        this.fsm = fsm;
        return fsm.HasReachedEnd;
    }
}
