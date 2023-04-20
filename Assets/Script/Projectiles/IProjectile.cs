using TheTD.DamageSystem;
using UnityEngine;

public interface IProjectile
{
    bool IsActive { get; }
    void Launch(Vector3 startPosition, Vector3 velocity, Transform parent, Damage damage);
    void ReadyForPool();
}