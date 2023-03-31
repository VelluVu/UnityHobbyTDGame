using Unity.AI.Navigation;
using UnityEngine;

namespace TheTD.Building
{
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
            BuildArea.OnBuild += OnBuild;
            BuildArea.OnBuildingRemove += OnBuildingRemove;
        }

        private void OnObstacleAdd()
        {
            RebuildNavMesh();
        }

        private void OnObstacleRemove()
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

        private void RebuildNavMesh()
        {
            NavMeshSurface.BuildNavMesh();
            OnNavMeshRebuild?.Invoke(NavMeshSurface);
        }
    }
}