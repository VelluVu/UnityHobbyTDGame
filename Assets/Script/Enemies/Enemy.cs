using TheTD.ScriptableFiniteStateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheTD.DamageSystem;
using UnityEngine;

namespace TheTD.Enemies
{
    public abstract class Enemy : MonoBehaviour, IDamageable, ITargetable
    {
        private const string SHADER_ALPHACLIPSTATE_PROPERTY_NAME = "_AlphaClipScale";
        private const string PASSTHROUGH_LAYER_NAME = "PassThrough";
        private const string ENEMY_LAYER_NAME = "Enemy";
        protected const string PATH_TO_STATS_FOLDER = "ScriptableObjects/Enemies/Stats/";
        protected const string BASE_STATS_NAME = "BaseStats";
        private const string END_POINT_TAG = "EndPoint";
        private float dissolveDelay = 1f;

        [SerializeField] protected LayerMask obstacleLayer;
        [SerializeField] protected EnemyType type = EnemyType.Goblin;
        protected List<IOvertimeEffect> OnGoingOvertimeEffects = new List<IOvertimeEffect>();
        protected List<Coroutine> damageTakeCoroutines = new List<Coroutine>();

        protected virtual string FULL_PATH_TO_ENEMY_STATS { get => PATH_TO_STATS_FOLDER + type.ToString() + BASE_STATS_NAME; }

        protected bool _hasReachedEnd = false;
        public bool HasReachedEnd { get => _hasReachedEnd; private set => SetReachedEnd(value); }

        private bool _isDead;
        virtual public bool IsDead { get => _isDead; set => SetIsDead(value); }

        [Tooltip("Automatically loads if reference is left empty, uses enemy name when loading from resources.")]
        [SerializeField] protected EnemyBaseStats _baseStats;
        public EnemyBaseStats BaseStats { get => _baseStats = _baseStats != null ? _baseStats : GetEnemyBaseStats(); }

        [SerializeField] protected DynamicEnemyStats _stats;
        public DynamicEnemyStats Stats { get => _stats; }

        protected EnemyFiniteStateMachine _fsm;
        public EnemyFiniteStateMachine FSM { get => _fsm = _fsm != null ? _fsm : GetComponent<EnemyFiniteStateMachine>(); }

        protected Renderer _renderer;
        virtual public Renderer Renderer { get => _renderer = _renderer != null ? _renderer : GetComponentInChildren<Renderer>(); }

        private Collider _collider;
        virtual internal Collider Collider { get => _collider = _collider != null ? _collider : GetComponentInChildren<Collider>(); }

        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody { get => GetRigidbody(); }

        protected EnemyBody _body;
        virtual public EnemyBody Body { get => _body = _body != null ? _body : GetComponentInChildren<EnemyBody>(); }

        public bool IsDestroyed => IsDead;

        public string Name => Name;

        public float XPReward => Stats.ExperienceReward.Value;

        public float Health => Stats.CurrentHealth.Value;

        public Vector3 Position => transform.position;

        public Vector3 BodyCenter => Body.CenterLocal;

        public Vector3 Velocity => Rigidbody.velocity;

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
            _stats = new DynamicEnemyStats(BaseStats);
        }

        virtual public void StartMoving()
        {
            ResetEnemy();
        }

        virtual public void ResetEnemy()
        {
            HasReachedEnd = false;
            IsDead = false;
            Stats.ResetStatsBaseValues();
        }

        virtual protected void Start()
        {
            StartMoving();
        }

        virtual protected void OnTriggerEnter(Collider other)
        {
            OnBodyCollision(other);
        }

        virtual protected void OnCollisionEnter(Collision other) 
        {
            CheckIfCollidesWithBlockingObject(other);
        }

        virtual protected void OnBodyCollision(Collider other)
        {
            CheckIfReachEnd(other);    
        }

        private void CheckIfCollidesWithBlockingObject(Collision other)
        {
            if(obstacleLayer.Contains(other.gameObject.layer))
            {
                Vector3 newPathStart = new Vector3(other.contacts[0].normal.x, 0f, other.contacts[0].normal.z) * 0.5f + transform.position;
                FSM.PathControl.FindPath(newPathStart, FSM.PathControl.Destination);
            }
        }

        private void CheckIfReachEnd(Collider other)
        {
            if (other.CompareTag(END_POINT_TAG))
            {
                HasReachedEnd = true;
            }
        }

        virtual protected bool CheckIfDie()
        {
            return Stats.CurrentHealth.BaseValue <= 0f;
        }

        virtual protected IEnumerator DestroySmoothly(float delay)
        {
            float dissolveTime = 0f;
            yield return new WaitForSeconds(delay);
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
            EnableEnemyBodyRotationWhenDead();
        }

        private void StopDamageTakeCoroutines()
        {
            damageTakeCoroutines.TrimExcess();
            damageTakeCoroutines.ForEach(o => StopCoroutine(o));
        }

        virtual protected void EnableEnemyBodyRotationWhenDead()
        {
            Rigidbody.constraints = _isDead ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        }

        virtual protected void EnableEnemyPassThroughLayerWhenDead()
        {
            Body.gameObject.layer = _isDead ? LayerMask.NameToLayer(PASSTHROUGH_LAYER_NAME) : LayerMask.NameToLayer(ENEMY_LAYER_NAME);
            gameObject.layer = _isDead ? LayerMask.NameToLayer(PASSTHROUGH_LAYER_NAME) : LayerMask.NameToLayer(ENEMY_LAYER_NAME);
        }

        virtual protected void SetReachedEnd(bool value)
        {
            if (_hasReachedEnd == value) return;
            Debug.Log("Reached End: " + value);
            _hasReachedEnd = value;
            if (_hasReachedEnd)
            {
                StopDamageTakeCoroutines();
                StartCoroutine(DestroySmoothly(dissolveDelay));
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

        virtual protected Rigidbody GetRigidbody()
        {
            if(_rigidbody != null) {
                return _rigidbody;
            }
            _rigidbody = GetComponent<Rigidbody>();
            return _rigidbody;
        }

        public abstract List<IModifier> GetDefensiveModifiers();
    }
}