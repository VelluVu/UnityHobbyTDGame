using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionScriptableObject : ScriptableObject
{
    protected FiniteStateMachine fsm;
    public abstract void Run(FiniteStateMachine fsm);
}
