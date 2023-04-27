using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheTD.Core;
using TheTD.Spawning;
using UnityEngine;
using UnityEngine.AI;

namespace TheTD.Building
{
    public class BuildArea : MonoBehaviour
    {
        private const string NOT_ENOUGH_GOLD_LOG_MESSAGE = "Not enough gold to build the tower";
        private const string PATH_TO_TEST_OBSTACLE = "Prefabs/Obstacles/TestPathObstacle";

        public List<Construction> constructions = new List<Construction>();
        private List<BuildSpot> invalidSpots = new List<BuildSpot>();

        private BuildSpot selectedBuildSpot;
        private NavMeshQueryFilter navMeshQueryFilter;
        private Vector3 obstacleUpOffset = Vector3.up * 0.5f;

        private CustomGrid _customGrid;
        public CustomGrid CustomGrid { get => _customGrid = _customGrid != null ? _customGrid : GetComponentInChildren<CustomGrid>(); }

        private NavMeshPath _testPath;
        public NavMeshPath TestPath { get => _testPath = _testPath != null ? _testPath : new NavMeshPath(); }

        private GameObject _testObstaclePrefab;
        public GameObject TestObstaclePrefab { get => _testObstaclePrefab = _testObstaclePrefab != null ? _testObstaclePrefab : Instantiate(Resources.Load<GameObject>(PATH_TO_TEST_OBSTACLE)); }

        private NavMeshObstacle _testObstacle;
        public NavMeshObstacle TestObstacle { get => _testObstacle = _testObstacle != null ? _testObstacle : TestObstaclePrefab.GetComponent<NavMeshObstacle>(); }

        public delegate void SelectBuildSpotDelegate(BuildSpot selectedSpot);
        public static event SelectBuildSpotDelegate OnSelectedBuildSpotChange;

        public delegate void BuildAreaDelegate(BuildArea buildArea);
        public static event BuildAreaDelegate OnBuildAreaClicked;

        public delegate void BuildDelegate(Construction construction);
        public static event BuildDelegate OnBuild;
        public static event BuildDelegate OnSell;
        public static event BuildDelegate OnBuildingRemove;

        private void Start()
        {
            AddListeners();
            navMeshQueryFilter = new NavMeshQueryFilter();
            navMeshQueryFilter.agentTypeID = NavMeshControl.Instance.NavMeshSurface.agentTypeID;
            navMeshQueryFilter.areaMask = NavMeshControl.Instance.NavMeshSurface.layerMask;
            CheckBlockedSpots();
        }

        public void AddListeners()
        {
            ClickPosition.OnClickPosition += OnClickPosition;
        }

        public void BuildOnSelectedSpot(ITowerLoadData towerLoadData)
        {
            if (IsUnableToBuildOnSelectedSpot()) return;
            if (!GameControl.Instance.Player.Gold.HasGoldForTower(towerLoadData.Tower.BuildCost))
            {
                Debug.Log(NOT_ENOUGH_GOLD_LOG_MESSAGE);
                return;
            }
            var construction = CreateBuilding(towerLoadData);

            CheckIfAdjacentSpotsWouldBeBlockingPath(selectedBuildSpot);
            //THIS IS VERY SLOW, MAKE FASTER!
            CheckIfConstructionsNeighboursWouldBlockPath();

            OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            OnBuild?.Invoke(construction);
        }

        public void SellOnSelectedSpot()
        {
            if (selectedBuildSpot == null) return;
            if (selectedBuildSpot.Construction == null) return;
            var construction = selectedBuildSpot.Construction;
            DestroyBuilding(construction);
            OnSell?.Invoke(selectedBuildSpot.Construction);
            //THIS IS VERY SLOW, MAKE FASTER!
            CheckIfConstructionsNeighboursWouldBlockPath();
        }

        private void OnClickPosition(Vector3 clickPosition)
        {
            if (clickPosition == -Vector3.zero)
            {
                selectedBuildSpot = null;
                OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
                return;
            }

            bool isClickOnTheGrid = CustomGrid.IsPositionInsideGridBounds(clickPosition);

            if (isClickOnTheGrid)
            {
                var clickedBuildSpot = CustomGrid.FindBuildSpotInWorldPosition(clickPosition);
                OnBuildAreaClicked?.Invoke(this);
                selectedBuildSpot = clickedBuildSpot;
                OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            }
            else
            {
                selectedBuildSpot = null;
                OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            }
        }

        private bool IsUnableToBuildOnSelectedSpot()
        {
            return selectedBuildSpot == null || selectedBuildSpot.HasConstruction || selectedBuildSpot.IsInvalidSpot;
        }

        private void CheckIfAdjacentSpotsWouldBeBlockingPath(BuildSpot buildSpot)
        {        
            foreach (var spot in buildSpot.NeighbourBuildSpots.Values)
            {
                if (spot.HasConstruction) continue;
                CheckIfPopulatedBuildSpotWouldBlockPath(spot);             
            }        
        }

        private void CheckIfPopulatedBuildSpotWouldBlockPath(BuildSpot buildSpot)
        {
            GameControl.Instance.EnableBuilding = false;
            TestObstaclePrefab.transform.position = buildSpot.CenterPositionInWorld + obstacleUpOffset;
            TestObstaclePrefab.gameObject.SetActive(true);

            NavMeshControl.Instance.RebuildNavMesh();

            var spawners = SpawnersControl.Instance.AllSpawners;

            foreach (var spawner in spawners)
            {
                TestPathForSpawner(spawner, buildSpot);
                if (AddOrRemoveInvalidSpot(buildSpot) == false) break;
            }

            TestObstaclePrefab.gameObject.SetActive(false);
            GameControl.Instance.EnableBuilding = true;
        }

        private bool AddOrRemoveInvalidSpot(BuildSpot spot)
        {
            if (!spot.IsInvalidSpot)
            {
                invalidSpots.Remove(spot);
            }
            else
            {
                if (!invalidSpots.Contains(spot))
                {
                    invalidSpots.Add(spot);
                }
            }
            return spot.IsInvalidSpot;
        }

        private void TestPathForSpawner(Spawner spawner, BuildSpot spot)
        {
            var spawnerPosition = spawner.transform.position;
            var isPathAvailable = NavMesh.CalculatePath(spawnerPosition, spawner.EndPointForSpawn.position, navMeshQueryFilter, TestPath);
            spot.IsInvalidSpot = !isPathAvailable || (TestPath.status == NavMeshPathStatus.PathPartial);
            //Debug.Log("Testing spot: " + spot.GridPosition + " , is build spot invalid: " + spot.IsInvalidSpot + " , is Path available: " + isPathAvailable + " , test path status: " + TestPath.status);
        }

        //THIS IS VERY SLOW, MAKE FASTER!
        private void CheckIfConstructionsNeighboursWouldBlockPath()
        {
            foreach (var item in constructions)
            {
                CheckIfAdjacentSpotsWouldBeBlockingPath(item.BuildSpot);
            }
        }

        public void DestroyBuilding(Construction construction)
        {
            constructions.Remove(construction);
            OnBuildingRemove?.Invoke(construction);
            construction.BuildSpot.HasConstruction = false;
            OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            Destroy(construction.gameObject);
            constructions.TrimExcess();
        }

        private Construction CreateBuilding(ITowerLoadData towerLoadData)
        {
            GameObject constructionGO = new GameObject();
            var construction = constructionGO.AddComponent<Construction>();
            construction.InitBuilding(selectedBuildSpot, towerLoadData);
            selectedBuildSpot.Construction = construction;
            constructions.Add(construction);
            selectedBuildSpot.HasConstruction = true;
            return construction;
        }

        private void CheckBlockedSpots()
        {
            var spawners = SpawnersControl.Instance.AllSpawners;
            MakeEndPointsUnblockable(spawners);
            CheckIfSpawnersBlockPaths(spawners);
            CheckIfObstaclesBlockPaths();
        }

        private void MakeEndPointsUnblockable(List<Spawner> spawners)
        {
            foreach (var spawner in spawners)
            {
                var buildSpotNextToEndPoint = CustomGrid.FindClosestBuildSpot(spawner.EndPointForSpawn.position);
                CheckIfPopulatedBuildSpotWouldBlockPath(buildSpotNextToEndPoint);
            }
        }

        private void CheckIfSpawnersBlockPaths(List<Spawner> spawners)
        {
            foreach (var spawner in spawners)
            {
                var closestBuildSpot = CustomGrid.FindClosestBuildSpot(spawner.transform.position);
                CheckIfPopulatedBuildSpotWouldBlockPath(closestBuildSpot);
            }
        }

        private void CheckIfObstaclesBlockPaths()
        {
            var obstacles = WorldObstacleControl.Instance.obstacles;
            if (obstacles != null || obstacles.Any())
            {
                foreach (var obstacle in obstacles)
                {
                    if (!obstacle.gameObject.activeSelf) continue;

                    var closestSpot = CustomGrid.FindClosestBuildSpot(obstacle.transform.position);
                    CheckIfAdjacentSpotsWouldBeBlockingPath(closestSpot);
                }
            }
        }
    }
}