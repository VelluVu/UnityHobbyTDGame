using NUnit.Framework.Constraints;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldObstacleControl))]
public class WorldObstacleControlEditor : Editor
{
    public const string FIND_OBSTACLES_BUTTON_LABEL = "Find Obstacles";
    public const string INFORM_OBSTACLE_POSITIONS_BUTTON_LABEL = "Inform Obstacle Positions";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var obstacleControl = (WorldObstacleControl)target;
        if(GUILayout.Button(FIND_OBSTACLES_BUTTON_LABEL))
        {
            obstacleControl.FindAllObstacles();
        }
        if (GUILayout.Button(INFORM_OBSTACLE_POSITIONS_BUTTON_LABEL))
        {
            obstacleControl.InformObstaclePositions();
        }
    }
}
