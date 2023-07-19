using System;
using System.Collections.Generic;
using TheTD.StatSystem;
using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    [RequireComponent(typeof(PathControl))]
    public abstract class FiniteStateMachine : MonoBehaviour
    {
        const string STATE_IS_ALREADY_RUNNING_FORMAT = "{0} state is already running";
        private const string CHANGING_STATE_TO_FORMAT = "Changing state to {0}";
        [SerializeField] protected List<StateScriptableObject> states = new List<StateScriptableObject>();

        public bool IsDead { get; protected set; }
        public bool HasReachedEnd { get; protected set; }

        public Vector3 HeightOffset => GetHeightOffset();

        protected Rigidbody _rigidBody;
        public Rigidbody Rigidbody { get => _rigidBody = _rigidBody != null ? _rigidBody : GetRigidBody(); }

        protected MovementStats _movementStats;
        public MovementStats MovementStats { get => _movementStats = _movementStats != null ? _movementStats : GetMovementStats(); }

        protected PathControl _pathControl;
        public PathControl PathControl { get => _pathControl = _pathControl != null ? _pathControl : GetComponent<PathControl>(); }

        [SerializeField] protected StateScriptableObject _defaultState;
        public StateScriptableObject DefaultState { get => _defaultState = _defaultState != null ? _defaultState : states[0]; }

        [SerializeField] protected StateScriptableObject _currentState;
        public StateScriptableObject CurrentState { get => _currentState; private set => SetCurrentState(value); }

        public delegate void OnStateDelegate(FiniteStateMachine fsm, StateScriptableObject state);
        public event OnStateDelegate onStateEnter;
        public event OnStateDelegate onStateExit;

        virtual protected void SetCurrentState(StateScriptableObject value)
        {
            Debug.LogFormat(CHANGING_STATE_TO_FORMAT, value.name);
            _currentState.Exit(this);
            onStateExit?.Invoke(this, _currentState);
            var newState = states.Find(o => o == value);
            newState.Enter(this);
            onStateEnter?.Invoke(this, newState);
            _currentState = newState;
        }

        virtual protected void Awake()
        {
            AddListeners();
            SelectDefaultState();
        }

        virtual protected void OnDisable()
        {
            RemoveListeners();
        }

        virtual protected void FixedUpdate()
        {
            if (_currentState == null) return;
            _currentState.Run(this);
        }

        virtual protected void SelectDefaultState()
        {
            _currentState = DefaultState;
            _currentState.Enter(this);
        }

        protected abstract void AddListeners();
        protected abstract void RemoveListeners();
        protected abstract MovementStats GetMovementStats();
        protected abstract Vector3 GetHeightOffset();
        protected abstract Rigidbody GetRigidBody();

        virtual internal void ChangeState(StateScriptableObject newState)
        {
            if (_currentState != null && newState == _currentState) return;
            var state = states.Find(o => o == newState);
            CurrentState = state;
        }
    }
}