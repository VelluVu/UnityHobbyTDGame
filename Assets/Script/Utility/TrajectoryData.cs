using UnityEngine;

[System.Serializable]
public struct TrajectoryData
{
    public Vector3 StartPosition { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    public Vector3 Velocity { get; private set; }
    public float Time { get; private set; }
    public float DeltaXZ { get; private set; }
    public float DeltaY { get; private set; }
    public float Angle { get; private set; }
    public float Distance => Vector3.Distance(StartPosition, TargetPosition);
    public Vector3 AimDirection => Velocity.normalized;
    public Vector3 Direction => (TargetPosition - StartPosition).normalized;
    public Vector3 GroundDirection => new Vector3(Direction.x, 0f, Direction.z);

    public TrajectoryData(Vector3 startPosition, Vector3 targetPosition, Vector3 velocity, float angle, float deltaXZ, float deltaY, float time)
    {
        StartPosition = startPosition;
        TargetPosition = targetPosition;
        Velocity = velocity;
        Angle = angle;
        DeltaXZ = deltaXZ;
        DeltaY = deltaY;
        Time = time;
    }
}
