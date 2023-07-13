using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBase : ActionScriptableObject
{
    public override void Run(FiniteStateMachine fsm)
    {
        this.fsm = fsm;
    }
}
