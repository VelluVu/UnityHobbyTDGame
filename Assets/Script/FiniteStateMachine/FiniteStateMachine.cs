using System.Collections.Generic;
using TheTD.StatSystem;
using UnityEngine;

namespace TheTD.ScriptableFiniteStateMachine
{
    [RequireComponent(typeof(PathControl))]
    public abstract class FiniteStateMachine : MonoBehaviour
    {
        const string STATE_IS_ALREADY_RUNNING_FORMAT = "{0} state is already running";
        private const string CHANGING_STATE_TO_FORMAT = "Changing state to {0}";

        public LayerMask moveLayerMask;
        public float maxSlopeAngle = 45f;
        public float slopeAngle = 0f;
        public float velocityMagnitude;
        public Vector3 Velocity;
        internal RaycastHit groundHit;
        internal RaycastHit slopeHit;
        internal Vector3 moveDirection;
        protected float downGroundRaycastLength = 0.05f;
        protected float downSlopeRaycastLength = 1f;
        protected Vector3 _groundRaycastYOffset => transform.up * 0.02f;
        [SerializeField] protected List<StateScriptableObject> states = new List<StateScriptableObject>();

        public Vector3 Acceleration { get; private set;}
        public Vector3 LastVelocity { get; private set; }
        public bool IsOnSlope { get; internal set; }
        public bool IsGrounded { get; internal set; }
        public bool IsTurning { get; internal set; }
        public bool IsDead { get; protected set; }
        public bool HasReachedEnd { get; protected set; }

        public Vector3 HeightOffset => GetHeightOffset();

        protected Rigidbody _rigidBody;
        public Rigidbody Rigidbody { get => _rigidBody = _rigidBody != null ? _rigidBody : GetRigidBody(); }

        protected Collider _collider;
        public Collider Collider { get => _collider = _collider != null ? _collider : GetComponentInChildren<Collider>(); }

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
            //Debug.LogFormat(CHANGING_STATE_TO_FORMAT, value.name);
            _currentState.Exit(this);
            onStateExit?.Invoke(this, _currentState);
            var newState = states.Find(o => o == value);
            newState.Enter(this);
            onStateEnter?.Invoke(this, newState);
            _currentState = newState;
        }

        virtual protected void Awake()
        {
            moveDirection = Rigidbody.transform.forward;
            AddListeners();
            SelectDefaultState();
        }

        virtual protected void OnDisable()
        {
            RemoveListeners();
        }

        virtual protected void FixedUpdate()
        {
            velocityMagnitude = Rigidbody.velocity.magnitude;
            Acceleration = (Rigidbody.velocity - LastVelocity) / Time.deltaTime;
            LastVelocity = Rigidbody.velocity;
            CheckGrounded();
            CheckGroundAngle();
            if (_currentState == null) return;
            _currentState.Run(this);
        }

        virtual protected void CheckGrounded()
        {  
            Vector3 raycastPosition = transform.position + _groundRaycastYOffset;
            bool hit1 = Physics.Raycast(raycastPosition, -transform.up, out groundHit, downGroundRaycastLength, moveLayerMask);
            raycastPosition = raycastPosition+ (Collider.bounds.extents.y * Rigidbody.transform.up) + (Collider.bounds.extents.z * Rigidbody.transform.forward);
            bool hit2 = Physics.Raycast(raycastPosition, -transform.up, out groundHit,  Collider.bounds.extents.y + downGroundRaycastLength - _groundRaycastYOffset.y, moveLayerMask);
            IsGrounded = hit1 || hit2;
        }

        virtual protected bool RaycastDown(Vector3 from, float length, RaycastHit hit)
        {
            Debug.DrawLine(from, from + -transform.up * length, Color.yellow, 1f);
            return Physics.Raycast(from, -transform.up, out hit, length, moveLayerMask);
        }

        public void CheckGroundAngle()
        {
            Vector3 raycastPosition = transform.position + Vector3.up * Collider.bounds.extents.y;
            bool hits = Physics.Raycast(raycastPosition, -transform.up, out slopeHit, downSlopeRaycastLength, moveLayerMask);
            
            if(hits)
            {
                slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                IsOnSlope = slopeAngle > 0;
            }
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