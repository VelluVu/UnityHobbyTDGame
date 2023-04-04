using System.Collections.Generic;
using TheTD.DamageSystem;
using UnityEngine;

public class DamageProperties : MonoBehaviour
{
    [SerializeField] internal int baseDamage = 5;
    [SerializeField] internal int criticalChange = 5;
    [SerializeField] internal int criticalDamagePercent = 25;
    [SerializeField] internal IDamageType damageType;
    [SerializeField] internal List<IDamageModifier> damageModifiers;
    [SerializeField] internal List<IOvertimeEffect> overtimeEffects;
}
