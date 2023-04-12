using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheTD.DamageSystem
{

    [System.Serializable]
    public class DamageProperties
    {
        [SerializeField] internal int baseDamage = 5;
        [SerializeField] internal float criticalChange = 5f;
        [SerializeField] internal float criticalDamageMultiplier = 0.25f;
        [SerializeField] internal IDamageType damageType;
        [SerializeField] internal List<IDamageModifier> damageModifiers = new List<IDamageModifier>();
        [SerializeField] internal List<IOvertimeEffect> overtimeEffects = new List<IOvertimeEffect>();

        internal void Add(DamageProperties towerDamageProperties)
        {
            baseDamage += towerDamageProperties.baseDamage;
            criticalChange += towerDamageProperties.criticalChange;
            criticalDamageMultiplier += towerDamageProperties.criticalDamageMultiplier;
            damageType = towerDamageProperties.damageType;
            damageModifiers.AddRange(towerDamageProperties.damageModifiers);
            overtimeEffects.AddRange(towerDamageProperties.overtimeEffects);
        }
    }
}