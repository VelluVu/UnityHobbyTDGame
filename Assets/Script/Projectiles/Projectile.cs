using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheTD.DamageSystem;
using UnityEngine;

namespace TheTD.Projectiles
{
    public abstract class Projectile : MonoBehaviour, IProjectile
    {
        protected const string PATH_TO_PROJECTILE_STATS = "ScriptableObjects/Projectiles/Stats/";

        public LayerMask hitLayerMask;
        protected Damage _totalDamage;
        protected List<IOvertimeEffect> _overtimeEffects = new List<IOvertimeEffect>();
        protected List<IModifier> _listOfModifiers = new List<IModifier>();

        public bool isCollided = false;
        public bool IsCollided { get => isCollided; set => SetIsCollided(value); }

        protected ProjectileBaseStats _baseStats;
        public ProjectileBaseStats BaseStats { get => GetBaseStats(); }

        protected DynamicProjectileStats _stats;
        public DynamicProjectileStats Stats { get => GetDynamicStats(); }
        public Transform OriginalParent { get; private set; }
        public ITargetable Target { get; private set; }

        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody { get => _rigidbody = _rigidbody != null ? _rigidbody : GetComponent<Rigidbody>(); }

        private Collider _collider;
        public Collider Collider { get => _collider = _collider != null ? _collider : GetComponentInChildren<Collider>(); }

        public bool IsActive => gameObject.activeSelf;

        virtual protected void Start()
        {
            SetProjectileBasedOvertimeEffects();
            SetProjectileBasedDamageModifiers();         
        }

        virtual public void Launch(TrajectoryData trajectoryData, Transform parent, Damage damage, ITargetable target)
        {
            StopAllCoroutines();
            Target = target;
            CreateCombinedDamage(damage);
            InitProjectileOnLaunch(trajectoryData.StartPosition, parent);
            SetupRigidBodyOnLaunch();
            Rigidbody.velocity = trajectoryData.Velocity;
            StartCoroutine(DeactivateInTime(this.Stats.ProjectileLifeTime.Value));
        }

        private void CreateCombinedDamage(Damage damage)
        {
            var damageTypes = damage.DamageTypes.ToList();
            damageTypes.Add(Stats.DamageType);
            var overtimeEffects = damage.OvertimeEffects.ToList();
            if (_overtimeEffects != null || _overtimeEffects.Any())
            {               
                overtimeEffects.AddRange(_overtimeEffects);
            }

            _totalDamage = new Damage(
                Stats.Damage.Value + damage.DamageStat.Value,
                Stats.CriticalChange.Value + damage.CriticalChance.Value,
                Stats.CriticalDamageMultiplier.Value + damage.CriticalDamageMultiplier.Value,
                damageTypes,
                overtimeEffects,
                damage.Source
                );
        }

        protected virtual void InitProjectileOnLaunch(Vector3 startPosition, Transform parent)
        {
            transform.position = startPosition;
            transform.gameObject.SetActive(true);
            OriginalParent = parent;
            transform.SetParent(OriginalParent);
            Collider.enabled = true;
            IsCollided = false;
        }

        protected virtual void SetupRigidBodyOnLaunch()
        {
            Rigidbody.useGravity = true;
            Rigidbody.isKinematic = false;
        }

        protected IEnumerator DeactivateInTime(float time)
        {
            yield return new WaitForSeconds(time);
            ReadyForPool();
        }

        virtual protected void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !isCollided)
            {
                HitEnemy(collision);
            }
            IsCollided = true;
        }

        virtual protected void FixedUpdate()
        {

        }

        virtual public void ReadyForPool()
        {
            ResetScale();
            if (OriginalParent != null) 
            {
                transform.SetParent(OriginalParent);
            }
            enabled = true;
            gameObject.SetActive(false);
        }

        virtual protected void ResetScale()
        {
            gameObject.transform.localScale = Vector3.one;
        }

        virtual protected void HitEnemy(Collision collision)
        {
            var hitVector = collision.transform.position - transform.position;
            hitVector.y = 0;
            var hitDirection = hitVector.normalized;
            var impactVector = hitDirection * Stats.Impact.Value;
            var targetable = collision.gameObject.GetComponentInParent<ITargetable>();
            targetable.Rigidbody.AddForce(impactVector, ForceMode.Impulse);
            collision.gameObject.GetComponentInParent<IDamageable>().TakeDamage(_totalDamage);
        }

        virtual protected void SetIsCollided(bool value)
        {
            if (isCollided == value) return;
            isCollided = value;
        }

        virtual protected ProjectileBaseStats GetBaseStats()
        {
            if (_baseStats != null) return _baseStats;
            string fileName = name.Replace("(Clone)", "") + "Stats";
            _baseStats = Resources.Load<ProjectileBaseStats>(PATH_TO_PROJECTILE_STATS + fileName);
            return _baseStats;
        }

        virtual protected DynamicProjectileStats GetDynamicStats()
        {
            if (_stats != null) return _stats;
            _stats = new DynamicProjectileStats(BaseStats);
            return _stats;
        }

        protected abstract void SetProjectileBasedDamageModifiers();
        protected abstract void SetProjectileBasedOvertimeEffects();
    }
}
