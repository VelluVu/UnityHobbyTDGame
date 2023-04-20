using TheTD.Core;
using UnityEngine;
using UnityEngine.AI;

namespace TheTD.Building
{
    public class Construction : MonoBehaviour
    {
        private const string CONSTRUCTION_GAME_OBJECT_NAME = "Construction";
        private const string NAV_MESH_OBSTACLE_PREFAB_PATH = "Prefabs/NavMeshObstacle";

        [SerializeField]private int buildOnWave = 0;
        public bool IsNew { get; internal set; }

        private ITower tower;
        public ITower Tower { get => tower; set => SetTower(value); }

        private BuildSpot buildSpot;
        public BuildSpot BuildSpot { get => buildSpot; private set => buildSpot = value; }

        private NavMeshObstacle navMeshObstacle;
        public NavMeshObstacle NavMeshObstacle { get => navMeshObstacle; private set => navMeshObstacle = value; }

        private GameObject navMeshObstaclePrefab;
        public GameObject NavMeshObstaclePrefab { get => navMeshObstaclePrefab = navMeshObstaclePrefab != null ? navMeshObstaclePrefab : Resources.Load<GameObject>(NAV_MESH_OBSTACLE_PREFAB_PATH); }

        public void Start()
        {
            GameControl.Instance.OnStartWave += OnStartWave;
        }

        private void OnStartWave(int wave)
        {
            CheckBuildingStatus(wave);
        }

        public void InitBuilding(BuildSpot buildSpot, ITowerLoadData towerData)
        {
            IsNew = true;
            buildOnWave = GameControl.Instance.CurrentSpawnWave;
            gameObject.name = CONSTRUCTION_GAME_OBJECT_NAME;
            BuildSpot = buildSpot;
            transform.position = BuildSpot.CenterPositionInWorld;
            NavMeshObstacle = Instantiate(NavMeshObstaclePrefab, BuildSpot.CenterPositionInWorld + Vector3.up * 0.5f, Quaternion.identity).GetComponent<NavMeshObstacle>();
            NavMeshObstacle.transform.SetParent(transform);
            Tower = towerData.Tower;           
        }

        private void SetTower(ITower value)
        {
            if (tower == value) return;
            tower = value;
            Tower.BuildTower(transform);
        }

        public void CheckBuildingStatus(int wave)
        {
            IsNew = buildOnWave >= wave;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(1f, 1f, 1f));
        }
    }
}