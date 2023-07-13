using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateScriptableObject : ScriptableObject
{
    protected FiniteStateMachine fsm;
    public List<ActionSeriliazeObject> actions;
    public List<TransitionSeriliazeObject> transitions;
    public List<DecisionSeriliazeObject> decisions;
    
    public abstract void Enter(FiniteStateMachine fsm);
    public abstract void Run();
    public abstract void Exit();
}
