using System;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshControl : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;
    public NavMeshSurface NavMeshSurface { get => navMeshSurface = navMeshSurface != null ? navMeshSurface : GetComponent<NavMeshSurface>(); }

    public delegate void NavMeshSurfaceDelegate(NavMeshSurface surface);
    public static event NavMeshSurfaceDelegate OnNavMeshRebuild;

    private void Start()
    {
       // AddListeners();
    }

    public void AddListeners()
    {     
        WorldObstacleControl.OnObstacleRemove += OnObstacleRemove;
        WorldObstacleControl.OnObstacleAdd += OnObstacleAdd;
        BuildArea.OnBuildSpotBuilt += OnObstacleAdd;
        BuildArea.OnBuildingRemove += OnObstacleRemove;
    }

    private void OnObstacleAdd()
    {
        RebuildNavMesh();
    }

    private void OnObstacleRemove()
    {
        RebuildNavMesh();
    }

    private void OnBuildSpot()
    {
        RebuildNavMesh();
    }

    private void RebuildNavMesh()
    {
        NavMeshSurface.BuildNavMesh();
        OnNavMeshRebuild?.Invoke(NavMeshSurface);
    }
}
