using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class RemoveExcessPointsModifier : MonoModifier
{
    public int iterations = 2;
    public override int Order => 1;
    private List<Vector3> positionsToRemove = new List<Vector3>();

    public override void Apply(Path path)
    {
        if(path.vectorPath.Count <= 1) return;
        positionsToRemove.Clear();
        var positions = path.vectorPath;
        var currentPosition = positions[0];
        var nextPosition = positions[0];
        var targetPosition = positions[0];

        for (int j = 0; j < iterations; j++)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if((i + 1 >= positions.Count) || (i + 2 >= positions.Count)) continue;
                currentPosition = positions[i];
                nextPosition = positions[i + 1];
                targetPosition = positions[i + 2];
                var directionToTarget = (targetPosition - currentPosition).normalized;
                var directionToNext = (nextPosition - currentPosition).normalized;
                if(directionToTarget != directionToNext) continue;
                AddRemovePosition(nextPosition);
            }

            for (int k = 0; k < positionsToRemove.Count; k++)
            {
                path.vectorPath.Remove(positionsToRemove[k]);
            }

            positionsToRemove.Clear();
            positions = path.vectorPath;
        }
    }

    private void AddRemovePosition(Vector3 position)
    {
        if(positionsToRemove.Contains(position)) return;
        positionsToRemove.Add(position);
    }
}
