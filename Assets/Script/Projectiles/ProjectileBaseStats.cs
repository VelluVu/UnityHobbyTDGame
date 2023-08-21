using TheTD.StatSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Projectile/ProjectileStats")]
public class ProjectileBaseStats : ScriptableObject
{
    [SerializeField] private Stat _damage;
    public Stat Damage { get => _damage; }

    [SerializeField] private Stat _impact;
    public Stat Impact { get => _impact; }

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
}
