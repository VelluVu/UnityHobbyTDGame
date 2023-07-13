using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    const string STATE_IS_ALREADY_RUNNING_FORMAT = "{0} state is already running";
    public List<StateSerializeObject> states = new List<StateSerializeObject>();
    
    private bool _hasReachedEnd = false;
    public bool HasReachedEnd => _hasReachedEnd;

    private PathControl _pathControl;
    public PathControl PathControl {get => _pathControl = _pathControl != null ? _pathControl : GetComponent<PathControl>(); }

    [SerializeField] private StateSerializeObject currentState;
    public StateScriptableObject CurrentState { get => currentState.state; set => SetCurrentState(value); }


    private void SetCurrentState(StateScriptableObject value)
    {
        if(value == currentState.state) 
        { 
            Debug.LogFormat(STATE_IS_ALREADY_RUNNING_FORMAT, value.name);
            return;
        }

        currentState.state.Exit();
        var newState = states.Find(o => o.Name == value.name);
        newState.state.Enter(this);
        currentState = newState;
    }

    private void Awake() {
        SelectDefaultState();
    }

    public void Update()
    {
        if(currentState == null) return;
        currentState.state.Run();
    }

    private void SelectDefaultState()
    {
        currentState.state.Enter(this);
        currentState = states.Find(o => o.isDefaultState);
    }
}
