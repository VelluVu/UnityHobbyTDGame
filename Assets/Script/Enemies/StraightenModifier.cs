using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class StraightenModifier : MonoModifier
{
    public int maxNodesInRow = 3;
    public float maxDistanceToNextPosition = 1f;
    public LayerMask hitMask;
    private List<Vector3> positionsToRemove = new List<Vector3>();

    public override int Order => 10;

    public override void Apply(Path path)
    {
        if (maxNodesInRow < 2 || (path.vectorPath.Count <= maxNodesInRow)) return;
        positionsToRemove.Clear();
        List<Vector3> positions = path.vectorPath;
        Vector3 currentPosition = positions[0];
        Vector3 nextPosition = positions[1];
        int lastNodeInScopeIndex = maxNodesInRow;

        for (int i = 0; i < positions.Count; i++)
        {
            lastNodeInScopeIndex = i + maxNodesInRow;
            if (lastNodeInScopeIndex >= positions.Count) continue;

            currentPosition = positions[i];
            nextPosition = positions[i + 1];
            
            float distanceToNextPosition = Vector3.Distance(currentPosition, nextPosition);
            if (distanceToNextPosition > maxDistanceToNextPosition) continue;

            Vector3 lastPositionInScope = positions[lastNodeInScopeIndex];
            Vector3 vectorFromCurrentToLastPosition = lastPositionInScope - currentPosition;
            Vector3 vectorFromCurrentToNextPosition = nextPosition - currentPosition;
            Vector3 directionFromCurrentToLastPosition = vectorFromCurrentToLastPosition.normalized;
            Vector3 directionFromCurrentToNextPosition = vectorFromCurrentToNextPosition.normalized;
            float distanceToLastPosition = vectorFromCurrentToLastPosition.magnitude;

            float angleDir = AngleDirection(directionFromCurrentToNextPosition, directionFromCurrentToLastPosition, Vector3.up);
            //Debug.Log("Angle Direction: " + angleDir);
            if(angleDir == 0f) continue;
            
            float halfExtends = 0.5f;
            Vector3 rightVectorOfDirectionToLastPosition = Vector3.Cross(directionFromCurrentToLastPosition, Vector3.up);

            Vector3 castCenterOffset = new Vector3(
                (rightVectorOfDirectionToLastPosition.x + halfExtends) * angleDir,
                halfExtends,
                (rightVectorOfDirectionToLastPosition.z + halfExtends) * angleDir);

            Vector3 castCenter = new Vector3(
                currentPosition.x + castCenterOffset.x, 
                currentPosition.y + castCenterOffset.y, 
                currentPosition.z + castCenterOffset.z);

            Debug.Log("Box casting: from " + castCenter + " direction: " + directionFromCurrentToLastPosition + " distance: " + distanceToLastPosition);

            RaycastHit hitInfo;
            bool hits = Physics.BoxCast(
                castCenter,
                Vector3.one * halfExtends,
                directionFromCurrentToLastPosition,
                out hitInfo,
                Quaternion.identity,
                distanceToLastPosition,
                hitMask);


            //Debug.Log("Current Node: " + currentPosition + " iteration index: " + i);
            //Debug.Log("Next Node Position: " + nextPosition + " iteration index: " + (i + 1));
            //Debug.Log("Check Node Position: " + lastPositionInScope + " iteration index: " + lastNodeInScopeIndex);
            //Debug.Log("Cast Center: " + castCenter);

            if (hits)
            {
                Debug.Log("Straighten modifier hits:" + hitInfo.transform.gameObject.name);
                continue;
            }
            
            for(int j = i + 1; j < lastNodeInScopeIndex; j++)
            {
                //Debug.Log("Adding Position: " + positions[j] + " iteration index: " + j + " to be removed");
                AddRemovePosition(positions[j]);
            }  
        }

        for (int i = 0; i < positionsToRemove.Count; i++)
        {
            //Debug.Log("Removing Positiont: " + positionsToRemove[i] + " from original vector path");
            path.vectorPath.Remove(positionsToRemove[i]);
        }
        
    }

    private void AddRemovePosition(Vector3 position)
    {
        if(positionsToRemove.Contains(position)) return;
        positionsToRemove.Add(position);
    }

    private float AngleDirection(Vector3 directionFromCurrentToNextPosition, Vector3 targetDirection, Vector3 up)
    {
        Vector3 perpendicular = Vector3.Cross(directionFromCurrentToNextPosition, targetDirection);
        float dot = Vector3.Dot(perpendicular, up);

        if (dot > 0.0f)
        {
            return 1.0f;
        }
        else if (dot < 0.0f)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
    }
}
