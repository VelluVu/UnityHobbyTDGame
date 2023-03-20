using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isCollided = false;
    public LayerMask hitLayerMask;
    [SerializeField] protected float timeToDeactivateAfterHit = 5f;
    [SerializeField] protected float projectileSpeed = 4f;
    [SerializeField] protected float damage = 10f;

    public Transform OriginalParent { get; private set; }

    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody { get => _rigidbody = _rigidbody != null ? _rigidbody : GetComponent<Rigidbody>(); }

    virtual public void Launch(Vector3 startPosition, Vector3 velocity, Transform parent = null)
    {
        transform.position = startPosition;
        transform.gameObject.SetActive(true);
        if(parent != null)
        {
            transform.SetParent(parent);
            OriginalParent = parent;
        }
        Rigidbody.useGravity = true;
        Rigidbody.isKinematic = false;
        Rigidbody.velocity = velocity;
        isCollided = false;
        StartCoroutine(DeactivateGameObjectInTime(timeToDeactivateAfterHit));
    }

    protected IEnumerator DeactivateGameObjectInTime(float time)
    {
        yield return new WaitForSeconds(time);
        transform.SetParent(OriginalParent);
        transform.gameObject.SetActive(false);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        Hit(collision);
    }

    virtual protected void Hit(Collision collision)
    {        
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && !isCollided)
        {
            collision.gameObject.GetComponentInParent<Enemy>().TakeDamage(damage);
        }
    }
}
