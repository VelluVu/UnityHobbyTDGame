using UnityEngine;

public class ArrowProjectile : Projectile
{   
    protected override void Hit(Collision collision)
    {
        base.Hit(collision);
        isCollided = true;
        gameObject.transform.SetParent(collision.transform.parent);
        Rigidbody.useGravity = false;
        Rigidbody.isKinematic = true;
        transform.rotation = Quaternion.LookRotation(collision.contacts[0].normal);
    }

    private void FixedUpdate()
    {
        if (gameObject.activeSelf && !isCollided)
        {
            transform.rotation = Quaternion.LookRotation(Rigidbody.velocity);
        }
    }
}
