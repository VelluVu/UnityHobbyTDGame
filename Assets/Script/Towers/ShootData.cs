using UnityEngine;

namespace TheTD.Towers
{
    public struct ShootData
    {
        public Vector3 Velocity { get; set; }
        public Vector3 Position { get; private set; }
        public float Angle { get; private set; }
        public float DeltaXZ { get; private set; }
        public float DeltaY { get; private set; }
        public float ReguiredShootForce { get; private set; }

        public ShootData(Vector3 position, Vector3 velocity, float angle, float deltaXZ, float deltaY, float reguiredShootForce)
        {
            Position = position;
            Velocity = velocity;
            Angle = angle;
            DeltaXZ = deltaXZ;
            DeltaY = deltaY;
            ReguiredShootForce = reguiredShootForce;
        }
    }
}