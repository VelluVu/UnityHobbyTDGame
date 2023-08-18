using UnityEngine;

public static class Equations
{

    public static float Gravity => -Physics.gravity.y;

    public static int QuadraticEquation(float a, float b, float c, out float root1, out float root2)
    {
        float discriminant = b * b - 4 * a * c;
        if(discriminant < 0)
        {
            root1 = Mathf.Infinity;
            root2 = -root1;
            return 0;
        }
        root1 = (-b + Mathf.Sqrt(discriminant)) / (2*a);
        root2 = (-b - Mathf.Sqrt(discriminant)) / (2*a);
        return discriminant > 0 ? 2 : 1;
    }

    public static TrajectoryData CalculateVelocity(Vector3 startPosition, Vector3 targetPosition, float initialAngle = 45f, float time = 1f)
    {
        /*
        Vector3 distance = targetPosition - startPosition;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Gravity) * time;

        Vector3 v0 = distanceXZ.normalized;
        v0 *= Vxz;
        v0.y = Vy;
        */
        float angle = initialAngle * Mathf.Deg2Rad;
 
        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(targetPosition.x, 0, targetPosition.z);
        Vector3 planarPostion = new Vector3(startPosition.x, 0, startPosition.z);

        Vector3 displacement = new Vector3(
                targetPosition.x,
                startPosition.y,
                targetPosition.z
            ) - startPosition;
            float deltaY = targetPosition.y - startPosition.y;
            float deltaXZ = displacement.magnitude;

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = startPosition.y - targetPosition.y;
 
        float initialVelocity = 1 / Mathf.Cos(angle) * Mathf.Sqrt(0.5f * Gravity * Mathf.Pow(distance, 2) / (distance * Mathf.Tan(angle) + yOffset));
 
        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
 
        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
 

        TrajectoryData trajectoryData = new TrajectoryData(startPosition, targetPosition, finalVelocity, initialAngle, deltaXZ, deltaY, time);
        return trajectoryData;
    }
}
