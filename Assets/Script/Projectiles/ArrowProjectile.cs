using TheTD.DamageSystem;
using UnityEngine;

namespace TheTD.Projectiles
{
    public class ArrowProjectile : Projectile
    {
        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
        }

        protected override void HitEnemy(Collision collision)
        {
            base.HitEnemy(collision);
            gameObject.transform.SetParent(collision.transform);
            Collider.enabled = false;
            Rigidbody.isKinematic = true;
            transform.rotation = Quaternion.LookRotation((collision.contacts[0].point - transform.position).normalized);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (gameObject.activeSelf && !IsCollided)
            {
                transform.rotation = Quaternion.LookRotation(Rigidbody.velocity);
            }

            if (IsCollided && transform.parent == null)
            {
                ReadyForBool();
            }
        }

        protected override void SetIsCollided(bool value)
        {
            base.SetIsCollided(value);
        }

        protected override void SetProjectileBasedDamageModifiers()
        {
            
        }

        protected override void SetProjectileBasedOvertimeEffects()
        {
            DamageProperties.overtimeEffects.Add(new BleedOvertimeEffect(DamageProperties.baseDamage, 6f, 3));
        }

        protected override void SetProjectileBasedDamageType()
        {
            DamageProperties.damageType = new PhysicalDamageType();
        }
    }
}