using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    private Collider _collider;
    virtual internal Collider Collider { get => _collider = _collider != null ? _collider : GetComponent<Collider>(); }

    internal delegate void CollisionDelegate(Collider other);
    internal event CollisionDelegate OnCollision;

    virtual public Vector3 BodyCenter { get => transform.localPosition; }

    virtual protected void OnTriggerEnter(Collider other)
    {
        OnCollision?.Invoke(other);  
    }
}
