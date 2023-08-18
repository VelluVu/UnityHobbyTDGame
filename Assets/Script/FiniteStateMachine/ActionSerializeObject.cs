using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheTD.ScriptableFiniteStateMachine
{
    [System.Serializable]
    public class ActionSeriliazeObject
    {
        public ActionScriptableObject action;
        public int order = 0;
        public string Name => action.name;
    }
}