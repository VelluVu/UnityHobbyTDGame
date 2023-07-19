using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    [CreateAssetMenu(fileName = "DoNothing", menuName = "ScriptableObjects/FiniteStateMachine/Actions/DoNothing")]
    public class DoNothing : ActionScriptableObject
    {  
        public override void Run(FiniteStateMachine fsm)
        {
        }
    }
}