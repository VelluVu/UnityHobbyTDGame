using TheTD.DamageSystem;
using UnityEngine;

public interface IProjectile
{
    bool IsActive { get; }
    void Launch(TrajectoryData trajectoryData, Transform parent, Damage damage, ITargetable target);
    void ReadyForPool();
}