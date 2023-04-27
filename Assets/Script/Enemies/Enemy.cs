using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheTD.DamageSystem;
using UnityEngine;
using UnityEngine.AI;

namespace TheTD.Enemies
{
    public abstract class Enemy : MonoBehaviour, IDamageable, ITargetable
    {
        //TODO:
        //should have enemy state machine to control enemy pathfinding and change from MOVE to different states like STUN, or FLEEING, MOVEFAST, DEAD, STOP
        //path should never be blocked, make build area to handle bad wrong tower placements!
        //should listen the state machine changes and launch events depending on the event.

        private const string END_POINT_TAG = "EndPoint";
        private const string SHADER_ALPHACLIPSTATE_PROPERTY_NAME = "_AlphaClipScale";
        private const string ENEMY_PATH_BLOCK_ERROR = "ERROR : ENEMY PATH BLOCKED";
        private const string PASSTHROUGH_LAYER_NAME = "PassThrough";
        private const string ENEMY_LAYER_NAME = "Enemy";
        protected const string PATH_TO_STATS_FOLDER = "ScriptableObjects/Enemies/Stats/";
        protected const string BASE_STATS_NAME = "BaseStats";
        public float originalAgentRadius = 0.3f;
        public float shrinkedAgentRadius = 0.1f;
        private float dissolveDelay = 1f;

        [SerializeField] protected EnemyType type = EnemyType.Goblin;
        protected List<IOvertimeEffect> OnGoingOvertimeEffects = new List<IOvertimeEffect>();
        protected List<Coroutine> damageTakeCoroutines = new List<Coroutine>();

        protected virtual string FULL_PATH_TO_ENEMY_STATS { get => PATH_TO_STATS_FOLDER + type.ToString() + BASE_STATS_NAME; }
        
        protected bool _isReachedEnd = false;
        public bool IsReachedEnd { get => _isReachedEnd; private set => SetReachedEnd(value); }

        private bool _isDead;
        virtual public bool IsDead { get => _isDead; set => SetIsDead(value); }

        [SerializeField] protected EnemyBaseStats _baseStats;
        public EnemyBaseStats BaseStats { get => _baseStats = _baseStats != null ? _baseStats : GetEnemyBaseStats(); }
        
        [SerializeField] protected DynamicEnemyStats _stats;
        public DynamicEnemyStats Stats { get => _stats; }

        protected NavMeshAgent _agent;
        virtual public NavMeshAgent Agent { get => _agent = _agent != null ? _agent : GetComponentInChildren<NavMeshAgent>(); }

        protected Transform _target;
        virtual public Transform Target { get => _target = _target != null ? _target : GameObject.FindGameObjectWithTag(END_POINT_TAG).transform; set => _target = value; }

        protected Renderer _renderer;
        virtual public Renderer Renderer { get => _renderer = _renderer != null ? _renderer : GetComponentInChildren<Renderer>(); }

        private Collider _collider;
        virtual internal Collider Collider { get => _collider = _collider != null ? _collider : GetComponent<Collider>(); }

        private Rigidbody _rigidBody;
        public Rigidbody Rigidbody { get => _rigidBody = _rigidBody != null ? _rigidBody : GetComponent<Rigidbody>(); }

        protected EnemyBody _enemyBody;

        virtual public EnemyBody Body { get => _enemyBody = _enemyBody != null ? _enemyBody : GetComponentInChildren<EnemyBody>(); }

        public bool IsDestroyed => IsDead;

        public string Name => Name;

        public float XPReward => Stats.ExperienceReward.Value;

        public float Health => Stats.CurrentHealth.Value;

        public Vector3 Position => transform.position;

        public Vector3 BodyCenter => Body.CenterLocal;

        public Vector3 Velocity => Agent.velocity;

        public delegate void EnemyDelegate(Enemy enemy);
        public static event EnemyDelegate OnReachEnd;
        public static event EnemyDelegate OnPathBlocked;

        public delegate void EnemyDeathDelegate(Enemy enemy, Damage damage);
        public static event EnemyDeathDelegate OnDeath;

        public event Action<IDamageable, Damage> OnTakeRawDamage;
        public event Action<IDamageable, Damage, IOvertimeEffect> OnDamage;
        public event Action<ITargetable, Damage> OnEliminated;
        
        private void Awake()
        {
            _stats = new DynamicEnemyStats(BaseStats, Agent);
        }

        virtual public void StartMoving()
        {
            ResetEnemy();
            StartCoroutine(CheckNavMeshState());
        }

        virtual public void ResetEnemy()
        {
            IsReachedEnd = false;
            IsDead = false;
            Stats.ResetStatsBaseValues();
            Agent.destination = Target.transform.position + Vector3.up * Body.CenterLocal.y;
        }

        virtual protected void Start()
        {
            StartMoving();
        }

        virtual protected void OnTriggerEnter(Collider other)
        {
            OnBodyCollision(other);
        }

        virtual protected void OnBodyCollision(Collider other)
        {
            ReachEnd(other);
        }

        private void ReachEnd(Collider other)
        {
            if (other.CompareTag(END_POINT_TAG))
            {
                IsReachedEnd = true;
            }
        }

        virtual protected bool CheckIfDie()
        {         
            return Stats.CurrentHealth.BaseValue <= 0f;
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

        virtual protected IEnumerator DestroySmoothly(float delay)
        {
            float dissolveTime = 0f;
            yield return new WaitForSeconds(delay); //give physics body time to take impact before dissolve
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
            var projectiles = GetComponentsInChildren<IProjectile>();
            if (!projectiles.Any()) return;
            for (int i = 0; i < projectiles.Length; i++)
            {
                projectiles[i].ReadyForPool();
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
        }

        private void StopDamageTakeCoroutines()
        {
            damageTakeCoroutines.TrimExcess();
            damageTakeCoroutines.ForEach(o => StopCoroutine(o));
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
            Body.gameObject.layer = _isDead ? LayerMask.NameToLayer(PASSTHROUGH_LAYER_NAME) : LayerMask.NameToLayer(ENEMY_LAYER_NAME);
            gameObject.layer = _isDead ? LayerMask.NameToLayer(PASSTHROUGH_LAYER_NAME) : LayerMask.NameToLayer(ENEMY_LAYER_NAME);
        }

        virtual protected void SetReachedEnd(bool value)
        {
            if (_isReachedEnd == value) return;
            _isReachedEnd = value;
            if (_isReachedEnd)
            {
                StopDamageTakeCoroutines();
                StartCoroutine(DestroySmoothly(0f));
                OnReachEnd?.Invoke(this);
            }
        }

        public void TakeDamage(Damage damage)
        {
            damage.OnDamageCalculated += OnTakeDamageCalculated;
            OnTakeRawDamage?.Invoke(this, damage);
        }

        private void OnTakeDamageCalculated(Damage finalDamage)
        {
            finalDamage.OnDamageCalculated -= OnTakeDamageCalculated;
            ReduceTheHealth(finalDamage);
           
            if (finalDamage.OvertimeEffects != null && finalDamage.OvertimeEffects.Any())
            {              
                finalDamage.OvertimeEffects.ForEach(o => damageTakeCoroutines.Add(StartCoroutine(TakeOvertimeDamage(o, finalDamage))));
            }
        }

        private void ReduceTheHealth(Damage damage, IOvertimeEffect overtimeEffect = null)
        {
            Stats.CurrentHealth.BaseValue -= overtimeEffect == null ? damage.DamageStat.RoundedValue : overtimeEffect.TickDamage;
            DamageNumberManager.Instance.SpawnFloatingDamageNumber(damage, transform.position + Body.HeadPointLocal, overtimeEffect);
            OnDamage?.Invoke(this, damage, overtimeEffect);
            IsDead = CheckIfDie();

            if (IsDead)
            {
                StopDamageTakeCoroutines();
                StartCoroutine(DestroySmoothly(dissolveDelay));
                OnDeath?.Invoke(this, damage);
                OnEliminated?.Invoke(this, damage);
            }
        }

        private IEnumerator TakeOvertimeDamage(IOvertimeEffect overtimeEffect, Damage damage)
        {
            OnGoingOvertimeEffects.Add(overtimeEffect);
            for (int i = 0; i < overtimeEffect.NumberOfTicks; i++)
            {        
                yield return new WaitForSeconds(overtimeEffect.Interval);
                ReduceTheHealth(damage, overtimeEffect);
            }
            OnGoingOvertimeEffects.Remove(overtimeEffect);
            damageTakeCoroutines.TrimExcess();
        }

        virtual protected EnemyBaseStats GetEnemyBaseStats()
        {
            var loadedBaseStats = Resources.Load<EnemyBaseStats>(FULL_PATH_TO_ENEMY_STATS);
            return loadedBaseStats;
        }

        /// <summary>
        /// Get these from items, 
        /// or creature specific, 
        /// these are applied to reduce, 
        /// or increase the taken damage.
        /// </summary>
        /// <returns></returns>
        public abstract List<IModifier> GetDefensiveModifiers();
    }
}