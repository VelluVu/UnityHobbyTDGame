using TheTD.StatSystem;
using UnityEngine;

namespace TheTD.Towers
{
    [CreateAssetMenu(menuName = "Tower/Stats/TowerBaseStats")]
    public class TowerBaseStats : ScriptableObject
    {
        [SerializeField] private Stat _buildCost;
        public Stat BuildCost { get => _buildCost; }

        [SerializeField] private Stat _findTargetInterval;
        public Stat FindTargetInterval { get => _findTargetInterval; }

        [SerializeField] private Stat _turnSpeed;
        public Stat TurnSpeed { get => _turnSpeed; }

        [SerializeField] private Stat _shootInterval;
        public Stat ShootInterval { get => _shootInterval; }

        [SerializeField] private Stat _maxRange;
        public Stat MaxRange { get => _maxRange; }

        [SerializeField] private Stat _baseDamage;
        public Stat BaseDamage { get => _baseDamage; private set => _baseDamage = value; }

        [SerializeField] private Stat _criticalChange;
        public Stat CriticalChange { get => _criticalChange; private set => _criticalChange = value; }

        [SerializeField] private Stat _criticalDamageMultiplier;
        public Stat CriticalDamageMultiplier { get => _criticalDamageMultiplier; private set => _criticalDamageMultiplier = value; }

        [SerializeReference] private DamageType _damageType;
        public DamageType DamageType { get => _damageType; private set => _damageType = value; }
    }
}