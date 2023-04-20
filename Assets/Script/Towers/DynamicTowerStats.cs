using System.Collections.Generic;
using TheTD.DamageSystem;
using TheTD.StatSystem;
using UnityEngine;

namespace TheTD.Towers
{
    [System.Serializable]
    public class DynamicTowerStats
    {
        public TowerBaseStats BaseStats { get; }

        [SerializeField] private Stat _buildCost;
        public Stat BuildCost { get => _buildCost; private set => _buildCost = value; }

        [SerializeField] private Stat _findTargetInterval;
        public Stat FindTargetInterval { get => _findTargetInterval; private set => _findTargetInterval = value; }

        [SerializeField] private Stat _turnSpeed;
        public Stat TurnSpeed { get => _turnSpeed; private set => _turnSpeed = value; }

        [SerializeField] private Stat _shootInterval;
        public Stat ShootInterval { get => _shootInterval; private set => _shootInterval = value; }

        [SerializeField] private Stat _maxRange;
        public Stat MaxRange { get => _maxRange; private set => _maxRange = value; }

        [SerializeField] private Stat _damage;
        public Stat Damage { get => _damage; private set => _damage = value; }

        [SerializeField] private Stat _criticalChange;
        public Stat CriticalChange { get => _criticalChange; private set => _criticalChange = value; }

        [SerializeField] private Stat _criticalDamageMultiplier;
        public Stat CriticalDamageMultiplier { get => _criticalDamageMultiplier; private set => _criticalDamageMultiplier = value; }

        [SerializeReference] private DamageType _damageType;
        public DamageType DamageType { get => _damageType; set => _damageType = value; }
        public List<IOvertimeEffect> OvertimeEffects { get; private set; }

        public DynamicTowerStats(TowerBaseStats baseStats)
        {
            BaseStats = baseStats;
            BuildCost = new Stat(BaseStats.BuildCost);
            FindTargetInterval = new Stat(BaseStats.FindTargetInterval);
            TurnSpeed = new Stat(BaseStats.TurnSpeed);
            ShootInterval = new Stat(BaseStats.ShootInterval);
            MaxRange = new Stat(BaseStats.MaxRange);
            Damage = new Stat(BaseStats.BaseDamage);
            CriticalChange = new Stat(BaseStats.CriticalChange);
            CriticalDamageMultiplier = new Stat(BaseStats.CriticalDamageMultiplier);
            DamageType = BaseStats.DamageType;
        }
    }
}