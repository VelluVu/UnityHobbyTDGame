using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    [System.Serializable]
    public class DecisionSeriliazeObject
    {
        public DecisionScriptableObject condition;
        public string Name => condition.name;

    }
}