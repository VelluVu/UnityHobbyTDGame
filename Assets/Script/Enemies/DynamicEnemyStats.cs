using Pathfinding;
using TheTD.StatSystem;
using UnityEngine;

namespace TheTD.Enemies
{
    [System.Serializable]
    public class DynamicEnemyStats : Stats
    {
        public EnemyBaseStats BaseStats { get; }

        [SerializeField] protected Stat _currentHealth;
        virtual public Stat CurrentHealth { get => _currentHealth; internal set => _currentHealth = value; }

        [SerializeField] private Stat _maxHealth;
        public Stat MaxHealth { get => _maxHealth; internal set => _maxHealth = value; }

        [SerializeField] private Stat _damage;
        public Stat Damage { get => _damage; internal set => _damage = value; }

        [SerializeField] private Stat _goldReward;
        public Stat GoldReward { get => _goldReward; internal set => _goldReward = value; }

        [SerializeField] private Stat _experienceReward;
        public Stat ExperienceReward { get => _experienceReward; internal set => _experienceReward = value; }

        public DynamicEnemyStats(EnemyBaseStats baseStats)
        {
            BaseStats = baseStats;
            MaxHealth = new Stat(BaseStats.MaxHealth);
            CurrentHealth = new Stat(MaxHealth.Value, StatFlags.CurrentHealth);
            Damage = new Stat(BaseStats.Damage);
            GoldReward = new Stat(BaseStats.GoldReward);
            ExperienceReward = new Stat(BaseStats.ExperienceReward);
            movementStats = new MovementStats(BaseStats.movementStats.MoveSpeed, BaseStats.movementStats.TurnSpeed, BaseStats.movementStats.Acceleration);
        }

        public void ResetStatsBaseValues()
        {
            MaxHealth.BaseValue = BaseStats.MaxHealth.BaseValue;
            CurrentHealth.BaseValue = MaxHealth.BaseValue;
            Damage.BaseValue = BaseStats.Damage.BaseValue;
            GoldReward.BaseValue = BaseStats.GoldReward.BaseValue;
            ExperienceReward.BaseValue = BaseStats.ExperienceReward.BaseValue;
            movementStats.ResetToBaseValue(BaseStats.movementStats.MoveSpeed.BaseValue, BaseStats.movementStats.TurnSpeed.BaseValue, BaseStats.movementStats.Acceleration.BaseValue);
        }
    }
}