using System.Collections;
using TheTD.DamageSystem;
using UnityEngine;

namespace TheTD.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        public LayerMask hitLayerMask;
        [SerializeField] protected float projectileLifeTime = 5f;
        [SerializeField] protected float projectileSpeed = 4f;

        [SerializeField] protected DamageProperties damageProperties;

        public bool isCollided = false;
        public bool IsCollided { get => isCollided; set => SetIsCollided(value); }

        public Transform OriginalParent { get; private set; }

        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody { get => _rigidbody = _rigidbody != null ? _rigidbody : GetComponent<Rigidbody>(); }

        private Collider _collider;
        public Collider Collider { get => _collider = _collider != null ? _collider : GetComponentInChildren<Collider>(); }

        virtual protected void Start()
        {
            SetProjectileBasedDamageType();
            SetProjectileBasedOvertimeEffects();
            SetProjectileBasedDamageModifiers();
        }

        virtual public void Launch(Vector3 startPosition, Vector3 velocity, Transform parent, DamageProperties towerDamageProperties = null)
        {
            StopAllCoroutines();
            InitProjectileOnLaunch(startPosition, parent);   
            SetupRigidBodyOnLaunch(velocity);
            StartCoroutine(DeactivateInTime(projectileLifeTime));
        }

        protected virtual void InitProjectileOnLaunch(Vector3 startPosition, Transform parent, DamageProperties towerDamageProperties = null)
        {
            transform.position = startPosition;
            transform.gameObject.SetActive(true);
            OriginalParent = parent;
            transform.SetParent(OriginalParent);
            Collider.enabled = true;
            IsCollided = false;

            if (towerDamageProperties != null)
            {
                damageProperties.Add(towerDamageProperties);
            }
        }

        protected virtual void SetupRigidBodyOnLaunch(Vector3 velocity)
        {
            Rigidbody.useGravity = true;
            Rigidbody.isKinematic = false;
            Rigidbody.velocity = velocity;
        }

        protected IEnumerator DeactivateInTime(float time)
        {
            yield return new WaitForSeconds(time);
            ReadyForBool();
        }

        virtual protected void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !isCollided)
            {
                HitEnemy(collision);
            }
            IsCollided = true;
        }

        virtual protected void FixedUpdate()
        {

        }

        virtual public void ReadyForBool()
        {
            ResetScale();
            //When closing the game, and this function is called after the destruction of the Original parent.
            //it caused some nasty errors, as trying to set null object as parent.
            //So the null check is needed here.
            if (OriginalParent != null) 
            {
                transform.SetParent(OriginalParent);
            }
            enabled = true;
            gameObject.SetActive(false);
        }

        virtual protected void ResetScale()
        {
            gameObject.transform.localScale = Vector3.one;
        }

        virtual protected void HitEnemy(Collision collision)
        {
            collision.gameObject.GetComponentInParent<IDamageable>().TakeDamage(new Damage(damageProperties, OriginalParent));
        }

        virtual protected void SetIsCollided(bool value)
        {
            if (isCollided == value) return;
            isCollided = value;
        }

        protected abstract void SetProjectileBasedDamageModifiers();
        protected abstract void SetProjectileBasedOvertimeEffects();
        protected abstract void SetProjectileBasedDamageType();
    }
}