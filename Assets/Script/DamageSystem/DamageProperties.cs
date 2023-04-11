using System.Collections.Generic;
using TheTD.DamageSystem;
using UnityEngine;

public class DamageProperties : MonoBehaviour
{
    [SerializeField] internal int baseDamage = 5;
    [SerializeField] internal float criticalChange = 0.05f;
    [SerializeField] internal float criticalDamagePercent = 0.25f;
    [SerializeField] internal IDamageType damageType;
    [SerializeField] internal List<IDamageModifier> damageModifiers = new List<IDamageModifier>();
    [SerializeField] internal List<IOvertimeEffect> overtimeEffects = new List<IOvertimeEffect>();
}
