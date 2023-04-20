using TheTD.StatSystem;
using UnityEngine;

namespace TheTD.Projectiles
{
    public class DynamicProjectileStats
    {
        private ProjectileBaseStats BaseStats;

        [SerializeField] private Stat _damage;
        public Stat Damage { get => _damage; }

        [SerializeField] private Stat _criticalChange;
        public Stat CriticalChange { get => _criticalChange; }

        [SerializeField] private Stat _criticalDamageMultiplier;
        public Stat CriticalDamageMultiplier { get => _criticalDamageMultiplier; }

        [SerializeField] private Stat _projectileLifeTime;
        public Stat ProjectileLifeTime { get => _projectileLifeTime; }

        [SerializeField] private Stat _projectileSpeed;
        public Stat ProjectileSpeed { get => _projectileSpeed; }

        [SerializeReference] private DamageType _damageType;
        public DamageType DamageType { get => _damageType; }

        public DynamicProjectileStats(ProjectileBaseStats baseStats)
        {
            this.BaseStats = baseStats;
            _damage = new Stat(BaseStats.Damage);
            _criticalChange = new Stat(BaseStats.CriticalChange);
            _criticalDamageMultiplier = new Stat(BaseStats.CriticalDamageMultiplier);
            _projectileLifeTime = new Stat(BaseStats.ProjectileLifeTime);
            _projectileSpeed = new Stat(BaseStats.ProjectileSpeed);
            _damageType = BaseStats.DamageType;
        }
    }
}