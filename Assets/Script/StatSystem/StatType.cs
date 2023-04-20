using System;

[Flags]
public enum StatFlags
{
    None = 0,
    CurrentHealth = 1,
    MaxHealth = 2,
    Damage = 4,
    Armor = 8,
    FireMagicResistance = 16,
    ColdMagicResistance = 32,
    PoisonMagicResistance = 64,
    BlackMagicResistance = 128,
    CriticalChange = 256,
    CriticalDamageMultiplier = 512,
    Speed = 1024,
    TurnSpeed = 2048,
    MaxRange = 4096,
    Acceleration = 8192,
    Gold = 16384,
    Experience = 32768,
}