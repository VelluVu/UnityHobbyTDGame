using System.Collections;
using System.Linq;
using TheTD.DamageSystem;
using TheTD.Projectiles;
using UnityEngine;
using UnityEngine.AI;

namespace TheTD.Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        //TODO:
        //should implement IDamageable interface
        //should have enemy stats scriptable object injected depending on enemy type, to remove the stat variables from here.
        //should have enemy state machine to control enemy pathfinding and change from MOVE to different states like STUN, or FLEEING, MOVEFAST, DEAD, STOP
        //path should never be blocked, make build area to handle bad wrong tower placements!
        //should listen the state machine changes and launch events depending on the event.

        private const string END_POINT_TAG = "EndPoint";
        private const string SHADER_ALPHACLIPSTATE_PROPERTY_NAME = "_AlphaClipScale";
        private const string ENEMY_PATH_BLOCK_ERROR = "ERROR : ENEMY PATH BLOCKED";
        private const string PASSTHROUGH_LAYER_NAME = "PassThrough";
        private const string ENEMY_LAYER_NAME = "Enemy";
        public float originalAgentRadius = 0.3f;
        public float shrinkedAgentRadius = 0.1f;

        protected EnemyType type = EnemyType.Goblin;

        protected bool _isReachedEnd = false;
        public bool IsReachedEnd { get => _isReachedEnd; private set => SetReachedEnd(value); }

        private bool _isDead;
        virtual public bool IsDead { get => _isDead; set => SetIsDead(value); }

        [SerializeField] protected float _currentHealth = 20f;
        virtual public float CurrentHealth { get => _currentHealth; private set => _currentHealth = value; }

        [SerializeField] protected float _maxHealth = 20f;
        virtual public float MaxHealth { get => _maxHealth; private set => _maxHealth = value; }

        // enemies have only one type of damage for player base... so int is sufficient
        [SerializeField] protected int _damage = 1;
        virtual public int Damage { get => _damage; private set => _damage = value; }

        [SerializeField] protected int _goldValue = 1;
        virtual public int GoldValue { get => _goldValue; private set => _goldValue = value; }

        protected NavMeshAgent _agent;
        virtual public NavMeshAgent Agent { get => _agent = _agent != null ? _agent : GetComponentInChildren<NavMeshAgent>(); }

        protected Transform _target;
        virtual public Transform Target { get => _target = _target != null ? _target : GameObject.FindGameObjectWithTag(END_POINT_TAG).transform; }

        protected Renderer _renderer;
        virtual public Renderer Renderer { get => _renderer = _renderer != null ? _renderer : GetComponentInChildren<Renderer>(); }

        private Collider _collider;
        virtual internal Collider Collider { get => _collider = _collider != null ? _collider : GetComponent<Collider>(); }

        private Rigidbody _rigidBody;
        public Rigidbody Rigidbody { get => _rigidBody = _rigidBody != null ? _rigidBody : GetComponent<Rigidbody>(); }

        protected EnemyBody _enemyBody;
        virtual public EnemyBody EnemyBody { get => _enemyBody = _enemyBody != null ? _enemyBody : GetComponentInChildren<EnemyBody>(); }

        public delegate void EnemyDelegate(Enemy enemy);
        public static event EnemyDelegate OnReachEnd;
        public static event EnemyDelegate OnDeath;
        public static event EnemyDelegate OnPathBlocked;

        virtual public void StartMoving()
        {
            ResetEnemy();
            StartCoroutine(CheckNavMeshState());
        }

        virtual public void ResetEnemy()
        {
            IsReachedEnd = false;
            IsDead = false;
            CurrentHealth = MaxHealth;
            Agent.destination = Target.transform.position + Vector3.up * transform.position.y;
        }

        virtual public void TakeDamage(Damage damage)
        {
            CurrentHealth -= damage.value;
            CheckDeath(damage);
        }

        virtual protected void Start()
        {
            AddListeners();
            StartMoving();
        }

        virtual protected void AddListeners()
        {

        }

        virtual protected void OnTriggerEnter(Collider other)
        {
            OnBodyCollision(other);
        }

        virtual protected void OnBodyCollision(Collider other)
        {
            if (other.CompareTag(END_POINT_TAG))
            {
                IsReachedEnd = true;
                IsDead = true;
            }
        }

        virtual protected void CheckDeath(Damage damage)
        {
            if (CurrentHealth > 0f) return;
            IsDead = true;
        }

        virtual protected IEnumerator CheckNavMeshState()
        {
            while (true)
            {
                if (Agent.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    Debug.Log(ENEMY_PATH_BLOCK_ERROR);
                    OnPathBlocked?.Invoke(this);
                }
                yield return new WaitForSeconds(1f);
            }
        }

        virtual protected IEnumerator DestroySmoothly()
        {
            float dissolveTime = 0f;
            yield return new WaitForSeconds(2f); //give physics body time to take impact before dissolve
            ClearStuckProjectiles();

            while (dissolveTime <= 1f)
            {
                dissolveTime += Time.deltaTime;
                Renderer.material.SetFloat(SHADER_ALPHACLIPSTATE_PROPERTY_NAME, dissolveTime);
                yield return null;
            }
            Destroy(gameObject);
        }

        virtual protected void OnDestroy()
        {
            ClearStuckProjectiles();
        }

        virtual protected void ClearStuckProjectiles()
        {
            var projectiles = GetComponentsInChildren<Projectile>();
            if (!projectiles.Any()) return;
            for (int i = 0; i < projectiles.Length; i++)
            {
                projectiles[i].ReadyForBool();
            }
        }

        virtual protected void SetIsDead(bool value)
        {
            if (_isDead == value) return;
            _isDead = value;
            EnableEnemyPassThroughLayerWhenDead();
            StopEnemyWhenDead();
            ChangeAgentRadius(_isDead ? shrinkedAgentRadius : originalAgentRadius);
            EnableEnemyBodyRotationWhenDead();

            if (_isDead)
            {
                StartCoroutine(DestroySmoothly());
                OnDeath?.Invoke(this);
            }
        }

        virtual protected void ChangeAgentRadius(float newAgentRadius)
        {
            Agent.radius = newAgentRadius;
        }

        virtual protected void EnableEnemyBodyRotationWhenDead()
        {
            Rigidbody.constraints = _isDead ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        }

        virtual protected void StopEnemyWhenDead()
        {
            Agent.isStopped = _isDead;
            Agent.velocity = _isDead ? Vector3.zero : Agent.velocity;
        }

        virtual protected void EnableEnemyPassThroughLayerWhenDead()
        {
            EnemyBody.gameObject.layer = _isDead ? LayerMask.NameToLayer(PASSTHROUGH_LAYER_NAME) : LayerMask.NameToLayer(ENEMY_LAYER_NAME);
            gameObject.layer = _isDead ? LayerMask.NameToLayer(PASSTHROUGH_LAYER_NAME) : LayerMask.NameToLayer(ENEMY_LAYER_NAME);
        }

        virtual protected void SetReachedEnd(bool value)
        {
            if (_isReachedEnd == value) return;
            _isReachedEnd = value;
            if (_isReachedEnd)
            {
                OnReachEnd?.Invoke(this);
            }
        }
    }
}