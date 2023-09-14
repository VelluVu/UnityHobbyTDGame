using System.Collections.Generic;
using System.Linq;
using TheTD.Core;
using TheTD.UI;
using UnityEngine;

namespace TheTD.Building
{
    public class BuildAreasControl : MonoBehaviour, IEventListener
    {
        public static BuildAreasControl Instance { get; private set; }
        private const string PATH_TO_TEST_OBSTACLE = "Prefabs/Obstacles/TestPathObstacle";

        private GameObject _testObstaclePrefab;
        public GameObject TestObstaclePrefab { get => _testObstaclePrefab = _testObstaclePrefab != null ? _testObstaclePrefab : Resources.Load<GameObject>(PATH_TO_TEST_OBSTACLE); }

        private GameObject _testObstacleGameObject;
        public GameObject TestObstacleGameObject { get => _testObstacleGameObject = _testObstacleGameObject != null ? _testObstacleGameObject : Instantiate(TestObstaclePrefab); }

        private ConstructionHolder _buildingHolder;
        public ConstructionHolder BuildingHolder { get => _buildingHolder = _buildingHolder != null ? _buildingHolder : FindObjectOfType<ConstructionHolder>(); }

        private List<BuildArea> _buildAreas;
        public List<BuildArea> BuildAreas { get => _buildAreas = _buildAreas != null ? CheckBuildAreas() : GetBuildAreas(); }

        private BuildArea selectedBuildArea = null;

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

        private void Start()
        {
            AddListeners();
        }

        public void AddListeners()
        {
            BuildArea.OnBuildAreaClicked += OnBuildAreaClicked;
            BuildingPanel.OnBuildClick += OnBuildButtonClicked;
        }
        
        public void RemoveListeners()
        {
            BuildArea.OnBuildAreaClicked -= OnBuildAreaClicked;
            BuildingPanel.OnBuildClick -= OnBuildButtonClicked;
        }

        private void OnBuildButtonClicked(ITowerLoadData towerData)
        {
            BuildOnSelectedSpot(towerData);
        }

        public void SellOnSelectedSpot()
        {
            if (selectedBuildArea == null || !GameControl.Instance.EnableBuilding) return;
            selectedBuildArea.SellOnSelectedSpot();
        }

        public void BuildOnSelectedSpot(ITowerLoadData towerData)
        {
            if (selectedBuildArea == null || !GameControl.Instance.EnableBuilding) return;
            selectedBuildArea.BuildOnSelectedSpot(towerData);
        }

        private void OnBuildAreaClicked(BuildArea buildArea)
        {
            if (buildArea == null) return;
            selectedBuildArea = buildArea;
        }

        private List<BuildArea> CheckBuildAreas()
        {
            return HasNewBuildAreas() ? GetBuildAreas() : _buildAreas;
        }

        private bool HasNewBuildAreas()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var buildArea = transform.GetChild(i).GetComponent<BuildArea>();
                if (!_buildAreas.Contains(buildArea)) return true;
            }
            return false;
        }

        private List<BuildArea> GetBuildAreas()
        {
            return GetComponentsInChildren<BuildArea>().ToList();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
    }
}