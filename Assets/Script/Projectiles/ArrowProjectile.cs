using UnityEngine;

public class ArrowProjectile : Projectile
{
    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    protected override void HitEnemy(Collision collision)
    {
        base.HitEnemy(collision);
        collision.rigidbody.AddRelativeForce(Vector3.forward * ThrustForce);
        gameObject.transform.SetParent(collision.transform);
        Collider.enabled = false;
        Rigidbody.isKinematic = true;
        //var targetLocalScaleDifference = CalculateTargetScaleDifference(collision.transform.lossyScale);
        //gameObject.transform.localScale += targetLocalScaleDifference;
        transform.rotation = Quaternion.LookRotation(collision.contacts[0].normal);
    }

    private Vector3 CalculateTargetScaleDifference(Vector3 lossyScale)
    {
        return new Vector3(1f % lossyScale.x, 1f % lossyScale.y, 1f % lossyScale.z);
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
}
