using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "ScriptableObjects/FiniteStateMachine/Actions/Move")]
public class Move : ActionScriptableObject
{
    public override void Run(FiniteStateMachine fsm)
    {
        this.fsm = fsm;

        Debug.Log("Moving");
    }
}
