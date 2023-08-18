using UnityEngine;

namespace TheTD.ScriptableFiniteStateMachine
{
    [CreateAssetMenu(fileName = "DoNothing", menuName = "ScriptableObjects/FiniteStateMachine/Actions/DoNothing")]
    public class DoNothing : ActionScriptableObject
    {  
        public override void Act(FiniteStateMachine fsm)
        {
        }
    }
}