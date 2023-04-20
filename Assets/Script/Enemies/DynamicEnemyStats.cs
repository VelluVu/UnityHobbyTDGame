using TheTD.StatSystem;
using UnityEngine;
using UnityEngine.AI;

namespace TheTD.Enemies
{
    [System.Serializable]
    public class DynamicEnemyStats
    {
        private NavMeshAgent agent;
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

        [SerializeField] private Stat _moveSpeed;
        public Stat MoveSpeed { get => _moveSpeed; internal set => _moveSpeed = value; }

        [SerializeField] private Stat _turnSpeed;
        public Stat TurnSpeed { get => _turnSpeed; internal set => _turnSpeed = value; }

        [SerializeField] private Stat _acceleration;
        public Stat Acceleration { get => _acceleration; internal set => _acceleration = value; }

        public DynamicEnemyStats(EnemyBaseStats baseStats, NavMeshAgent agent)
        {
            BaseStats = baseStats;
            this.agent = agent;
            MaxHealth = new Stat(BaseStats.MaxHealth);
            CurrentHealth = new Stat(MaxHealth.Value, StatFlags.CurrentHealth);
            Damage = new Stat(BaseStats.Damage);
            GoldReward = new Stat(BaseStats.GoldReward);
            ExperienceReward = new Stat(BaseStats.ExperienceReward);
            MoveSpeed = new Stat(BaseStats.MoveSpeed);
            TurnSpeed = new Stat(BaseStats.TurnSpeed);
            Acceleration = new Stat(BaseStats.Acceleration);
            agent.speed = MoveSpeed.BaseValue;
            agent.angularSpeed = TurnSpeed.BaseValue;
            agent.acceleration = Acceleration.BaseValue;
            ListenAgentStatsChange();
        }

        public void ResetStatsBaseValues()
        {           
            MaxHealth.BaseValue = BaseStats.MaxHealth.BaseValue;
            CurrentHealth.BaseValue = MaxHealth.BaseValue;
            Damage.BaseValue = BaseStats.Damage.BaseValue;
            GoldReward.BaseValue = BaseStats.GoldReward.BaseValue;
            ExperienceReward.BaseValue = BaseStats.ExperienceReward.BaseValue;
            MoveSpeed.BaseValue = BaseStats.MoveSpeed.BaseValue;
            TurnSpeed.BaseValue = BaseStats.TurnSpeed.BaseValue;
            Acceleration.BaseValue = BaseStats.Acceleration.BaseValue;
            agent.speed = MoveSpeed.BaseValue;
            agent.angularSpeed = TurnSpeed.BaseValue;
            agent.acceleration = Acceleration.BaseValue;
        }

        public void ListenAgentStatsChange()
        {
            MoveSpeed.OnStatChange += OnMoveSpeedChange;
            TurnSpeed.OnStatChange += OnTurningSpeedChange;
            Acceleration.OnStatChange += OnAccelerationChange;
        }

        private void OnAccelerationChange(Stat stat)
        {
            agent.acceleration = stat.Value;
        }

        private void OnTurningSpeedChange(Stat stat)
        {
            agent.angularSpeed = stat.Value;
        }

        private void OnMoveSpeedChange(Stat stat)
        {
            agent.speed = stat.Value;
        }
    }
}