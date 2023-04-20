using TheTD.StatSystem;
using UnityEngine;

namespace TheTD.Enemies
{
    [CreateAssetMenu(menuName = "Enemy/Stats/BaseStats")]
    public class EnemyBaseStats : ScriptableObject
    {
        [SerializeField] private Stat _maxHealth;
        public Stat MaxHealth { get => _maxHealth; }

        [SerializeField] private Stat _damage;
        public Stat Damage { get => _damage; }

        [SerializeField] private Stat _armor;
        public Stat Armor { get => _armor; }

        [SerializeField] private Stat _fireMagicResistance;
        public Stat FireMagicResistance { get => _fireMagicResistance; }

        [SerializeField] private Stat _coldMagicResistance;
        public Stat ColdMagicResistance { get => _coldMagicResistance; }

        [SerializeField] private Stat _poisonMagicResistance;
        public Stat PoisonMagicResistance { get => _poisonMagicResistance; }

        [SerializeField] private Stat _blackMagicResistance;
        public Stat BlackMagicResistance { get => _blackMagicResistance; }

        [SerializeField] private Stat _goldReward;
        public Stat GoldReward { get => _goldReward; }

        [SerializeField] private Stat _experienceReward;
        public Stat ExperienceReward { get => _experienceReward; }

        [SerializeField] private Stat _moveSpeed;
        public Stat MoveSpeed { get => _moveSpeed; }

        [SerializeField] private Stat _turnSpeed;
        public Stat TurnSpeed { get => _turnSpeed; }

        [SerializeField] private Stat _acceleration;
        public Stat Acceleration { get => _acceleration; }
    }
}