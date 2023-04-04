using System.Collections;
using TheTD.DamageSystem;
using UnityEngine;

namespace TheTD.Projectiles
{
    [RequireComponent(typeof(DamageProperties))]
    public class Projectile : MonoBehaviour
    {
        public bool isCollided = false;
        public bool IsCollided { get => isCollided; set => SetIsCollided(value); }

        public LayerMask hitLayerMask;
        [SerializeField] protected float timeToDeactivateAfterHit = 5f;
        [SerializeField] protected float projectileSpeed = 4f;

        protected DamageProperties _damageProperties;
        public DamageProperties DamageProperties { get => _damageProperties = _damageProperties != null ? _damageProperties : GetComponent<DamageProperties>(); }
        public Transform OriginalParent { get; private set; }

        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody { get => _rigidbody = _rigidbody != null ? _rigidbody : GetComponent<Rigidbody>(); }

        private Collider _collider;
        public Collider Collider { get => _collider = _collider != null ? _collider : GetComponentInChildren<Collider>(); }

        virtual protected void Start()
        {
            SetDamageType();
        }

        virtual public void Launch(Vector3 startPosition, Vector3 velocity, Transform parent = null)
        {
            transform.position = startPosition;
            transform.gameObject.SetActive(true);
            if (parent != null)
            {
                transform.SetParent(parent);
                OriginalParent = parent;
            }
            Collider.enabled = true;
            Rigidbody.useGravity = true;
            Rigidbody.isKinematic = false;
            IsCollided = false;
            Rigidbody.velocity = velocity;
            StartCoroutine(DeactivateGameObjectInTime(timeToDeactivateAfterHit));
        }

        protected virtual void SetDamageType()
        {
            DamageProperties.damageType = new PhysicalDamageType();
        }

        protected IEnumerator DeactivateGameObjectInTime(float time)
        {
            yield return new WaitForSeconds(time);
            transform.SetParent(OriginalParent);
            transform.gameObject.SetActive(false);
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
            transform.SetParent(OriginalParent);
            enabled = true;
            gameObject.SetActive(false);
        }

        virtual protected void ResetScale()
        {
            gameObject.transform.localScale = Vector3.one;
        }

        virtual protected void HitEnemy(Collision collision)
        {
            collision.gameObject.GetComponentInParent<IDamageable>().TakeDamage(new Damage(DamageProperties));
        }

        virtual protected void SetIsCollided(bool value)
        {
            if (isCollided == value) return;
            isCollided = value;
        }

    }
}