using TheTD.Projectiles;
using UnityEngine;

// Create interface for damage and damageable
// Remove projectile reference

namespace TheTD.DamageSystem
{
    [System.Serializable]
    public class Damage
    {
        public float value;
        public float force;
        public float radius;
        public DamageType type;
        public Projectile projectile;
        public Collision collision;
    }
}