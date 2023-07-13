using Unity.AI.Navigation;
using UnityEngine;

namespace TheTD.Building
{
    /// <summary>
    /// THIS IS OLD, USING A* PROJECT PRO INSTEAD OF UNITY NAVMESH
    /// </summary>
    public class NavMeshControl : MonoBehaviour
    {
        public static NavMeshControl Instance { get; private set; }

        private NavMeshSurface navMeshSurface;
        public NavMeshSurface NavMeshSurface { get => navMeshSurface = navMeshSurface != null ? navMeshSurface : GetComponent<NavMeshSurface>(); }

        public delegate void NavMeshSurfaceDelegate(NavMeshSurface surface);
        public static event NavMeshSurfaceDelegate OnNavMeshRebuild;

        private void Awake()
        {
            CheckSingleton();
        }

        private void CheckSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void AddListeners()
        {
            WorldObstacleControl.OnObstacleRemove += OnObstacleRemove;
            WorldObstacleControl.OnObstacleAdd += OnObstacleAdd;
            BuildArea.OnBuild += OnBuild;
            BuildArea.OnBuildingRemove += OnBuildingRemove;
        }

        private void OnObstacleAdd(Obstacle obstacle)
        {
            RebuildNavMesh();
        }

        private void OnObstacleRemove(Obstacle obstacle)
        {
            RebuildNavMesh();
        }

        private void OnBuild(Construction building)
        {
            RebuildNavMesh();
        }

        private void OnBuildingRemove(Construction building)
        {
            RebuildNavMesh();
        }

        public void RebuildNavMesh()
        {
            NavMeshSurface.BuildNavMesh();
            OnNavMeshRebuild?.Invoke(NavMeshSurface);
        }
    }
}