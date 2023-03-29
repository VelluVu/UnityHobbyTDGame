using System.Numerics;
using UnityEngine;

[System.Serializable]
public class Damage
{
    public float value;
    public float force;
    public float radius;
    public DamageType type;
    public Projectile projectile;
    public Collision collision;
}

public enum DamageType
{
    Pierce,
    Explosion,
    Flame,
    Frost,
    Acid,
}