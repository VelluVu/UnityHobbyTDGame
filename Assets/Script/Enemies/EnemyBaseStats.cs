using TheTD.StatSystem;
using UnityEngine;

namespace TheTD.Enemies
{
    [CreateAssetMenu(menuName = "Enemy/Stats/BaseStats")]
    public class EnemyBaseStats : ScriptableObject
    {
        [SerializeField] private Stat _maxHealth = null;
        public Stat MaxHealth { get => _maxHealth; }

        [SerializeField] private Stat _damage = null;
        public Stat Damage { get => _damage; }

        [SerializeField] private Stat _armor = null;
        public Stat Armor { get => _armor; }

        [SerializeField] private Stat _fireMagicResistance = null;
        public Stat FireMagicResistance { get => _fireMagicResistance; }

        [SerializeField] private Stat _coldMagicResistance = null;
        public Stat ColdMagicResistance { get => _coldMagicResistance; }

        [SerializeField] private Stat _poisonMagicResistance = null;
        public Stat PoisonMagicResistance { get => _poisonMagicResistance; }

        [SerializeField] private Stat _blackMagicResistance = null;
        public Stat BlackMagicResistance { get => _blackMagicResistance; }

        [SerializeField] private Stat _goldReward = null;
        public Stat GoldReward { get => _goldReward; }

        [SerializeField] private Stat _experienceReward = null;
        public Stat ExperienceReward { get => _experienceReward; }

        public MovementStats movementStats;
    }
}