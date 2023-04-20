using System;
using TheTD.DamageSystem;
using UnityEngine;

public interface ITargetable
{
    bool IsDestroyed { get; }
    string Name { get; }
    float XPReward { get; }
    float Health { get; }
    Vector3 Velocity { get; }
    Vector3 Position { get; }
    Vector3 BodyCenter { get; }
    event Action<ITargetable, Damage> OnEliminated;
}
