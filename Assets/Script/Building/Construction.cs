using TheTD.Core;
using UnityEngine;

namespace TheTD.Building
{
    public class Construction : MonoBehaviour, IEventListener
    {
        private const string CONSTRUCTION_GAME_OBJECT_NAME = "Construction";
        //private const string NAV_MESH_OBSTACLE_PREFAB_PATH = "Prefabs/NavMeshObstacle";

        [SerializeField]private int buildOnWave = 0;
        public bool IsNew { get; internal set; }

        private ITower tower;
        public ITower Tower { get => tower; set => SetTower(value); }

        private BuildSpot buildSpot;
        public BuildSpot BuildSpot { get => buildSpot; private set => buildSpot = value; }

        public void Start()
        {
            AddListeners();   
        }

        public void InitBuilding(BuildSpot buildSpot, ITowerLoadData towerData)
        {
            IsNew = true;
            buildOnWave = GameControl.Instance.CurrentSpawnWave;
            gameObject.name = CONSTRUCTION_GAME_OBJECT_NAME;
            gameObject.layer = LayerMask.NameToLayer("Building");
            BuildSpot = buildSpot;
            transform.position = BuildSpot.CenterPositionInWorld;          
            Tower = towerData.Tower;           
        }

        public void CheckBuildingStatus(int wave)
        {
            IsNew = buildOnWave >= wave;
        }

        public void AddListeners()
        {
            GameControl.Instance.OnStartWave += OnStartWave;
        }

        public void RemoveListeners()
        {
            GameControl.Instance.OnStartWave -= OnStartWave;
        }

        private void OnStartWave(int wave)
        {
            CheckBuildingStatus(wave);
        }

        private void SetTower(ITower value)
        {
            if (tower == value) return;
            tower = value;
            Tower.BuildTower(transform);
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(1f, 1f, 1f));
        }
    }
}