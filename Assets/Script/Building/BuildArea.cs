using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheTD.Core;
using TheTD.Spawning;
using UnityEngine;

namespace TheTD.Building
{
    public class BuildArea : MonoBehaviour
    {
        private const string NOT_ENOUGH_GOLD_LOG_MESSAGE = "Not enough gold to build the tower";
        private const string PATH_TO_TEST_OBSTACLE = "Prefabs/Obstacles/TestPathObstacle";

        public List<Construction> constructions = new List<Construction>();

        private BuildSpot selectedBuildSpot;
        //private NavMeshQueryFilter navMeshQueryFilter;
        private Vector3 obstacleUpOffset = Vector3.up * 0.5f;

        private System.Diagnostics.Stopwatch _stopWatch;
        public System.Diagnostics.Stopwatch StopWatch { get => _stopWatch = _stopWatch != null ? _stopWatch : new System.Diagnostics.Stopwatch(); }

        private CustomGrid _customGrid;
        public CustomGrid CustomGrid { get => _customGrid = _customGrid != null ? _customGrid : GetComponentInChildren<CustomGrid>(); }

        private GameObject _testObstaclePrefab;
        public GameObject TestObstaclePrefab { get => _testObstaclePrefab = _testObstaclePrefab != null ? _testObstaclePrefab : Resources.Load<GameObject>(PATH_TO_TEST_OBSTACLE); }

        private GameObject _testObstacleGameObject;
        public GameObject TestObstacleGameObject { get => _testObstacleGameObject = _testObstacleGameObject != null ? _testObstacleGameObject : Instantiate(TestObstaclePrefab); }

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
            ChangeNodeWalkable(construction.BuildSpot, false);
            CheckConstructionsNeighbourSpotsAvailability();
            OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            OnBuild?.Invoke(construction);        
        }

        public void SellOnSelectedSpot()
        {
            if (selectedBuildSpot == null) return;
            if (selectedBuildSpot.Construction == null) return;
            DestroyBuilding(selectedBuildSpot.Construction);
            ChangeNodeWalkable(selectedBuildSpot, true);
            CheckConstructionsNeighbourSpotsAvailability();
            OnSell?.Invoke(selectedBuildSpot.Construction);
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
            return selectedBuildSpot == null || selectedBuildSpot.IsOccupied || selectedBuildSpot.IsInvalidSpot;
        }

        private void CheckConstructionsNeighbourSpotsAvailability()
        {
            foreach (var item in constructions)
            {
                if (!item.BuildSpot.HasOccupiedNeighbours) continue;
                CheckIfObstacleInNeighboursWouldBlockPath(item.BuildSpot);
            }
        }

        private void MakeEndPointsUnblockable(List<Spawner> spawners)
        {
            foreach (var spawner in spawners)
            {
                var closestBuildSpot = CustomGrid.FindClosestBuildSpot(spawner.EndTransform.position);
                ChangeBuildSpotIsValid(closestBuildSpot, false);
                CheckIfObstacleInNeighboursWouldBlockPath(closestBuildSpot);
            }
        }

        private void CheckIfSpawnersBlockPaths(List<Spawner> spawners)
        {
            foreach (var spawner in spawners)
            {
                var closestBuildSpot = CustomGrid.FindClosestBuildSpot(spawner.transform.position);
                ChangeBuildSpotIsValid(closestBuildSpot, false);
                CheckIfObstacleInNeighboursWouldBlockPath(closestBuildSpot);
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
                    CheckIfObstacleInNeighboursWouldBlockPath(closestSpot);
                }
            }        
        }

        #region PathBlockPreCheck_AstarProjectPro

        private void ChangeNodeWalkable(BuildSpot buildSpot, bool isWalkable)
        {
            var buildSpotNode = AstarPath.active.GetNearest(buildSpot.CenterPositionInWorld).node;
            Debug.Log("Changing node: " + (Vector3)buildSpotNode.position + " walkable: " + isWalkable);
            buildSpot.IsInvalidSpot = !isWalkable;
            buildSpotNode.Walkable = isWalkable;      
        }

        private void ChangeBuildSpotIsValid(BuildSpot buildSpot, bool isValid)
        {
            buildSpot.IsInvalidSpot = !isValid;
        }

        private void CheckIfObstacleInNeighboursWouldBlockPath(BuildSpot buildSpot)
        {
            var neighbours = buildSpot.NeighbourBuildSpots;
            var spawners = SpawnersControl.Instance.AllSpawners;
     
            TestObstacleGameObject.SetActive(true);

            foreach (var neighbour in neighbours.Values)
            {
                if (neighbour.IsOccupied) continue;

                foreach (var spawner in spawners)
                    if (!CheckIfObstacleInBuildSpotWouldBlockPath(neighbour, spawner.transform.position, spawner.EndTransform.position)) break;     
            }         
            TestObstacleGameObject.SetActive(false);
            AstarPath.active.Scan();       
        }

        public bool CheckForBlocks(Collider collider, Vector3 start, Vector3 destination)
        {
            var guo = new GraphUpdateObject(collider.bounds);
            var startPointNode = AstarPath.active.GetNearest(start, NNConstraint.None).node;
            var goalNode = AstarPath.active.GetNearest(destination, NNConstraint.None).node;
            return GraphUpdateUtilities.UpdateGraphsNoBlock(guo, startPointNode, goalNode);
        }

        public bool IsPathPossible(Vector3 start, Vector3 destination)
        {
            var startNode = AstarPath.active.GetNearest(start, NNConstraint.None).node;
            var destinationNode = AstarPath.active.GetNearest(destination, NNConstraint.None).node;
            return PathUtilities.IsPathPossible(startNode, destinationNode);
        }

        private bool CheckIfObstacleInBuildSpotWouldBlockPath(BuildSpot buildSpot, Vector3 spawnPosition, Vector3 endPosition)
        {
            //TODO: Make more efficient, replace Scans!
            TestObstacleGameObject.transform.position = buildSpot.CenterPositionInWorld + Vector3.up * 0.5f;
            Physics.SyncTransforms();
            var buildSpotNode = AstarPath.active.GetNearest(buildSpot.CenterPositionInWorld, NNConstraint.None).node;
            var isBuildSpotWalkable = buildSpotNode.Walkable;
            
            buildSpotNode.Walkable = false;
            AstarPath.active.Scan();

            bool isAvailablePosition = IsPathPossible(spawnPosition, endPosition);
            buildSpot.IsInvalidSpot = !isAvailablePosition;

            buildSpotNode.Walkable = isBuildSpotWalkable;
            AstarPath.active.Scan();

            return isAvailablePosition;
        }

        private void UpdateGraphWithBounds(Bounds bounds)
        {
            var guo = new GraphUpdateObject(bounds);
            guo.updatePhysics = true;
            AstarPath.active.UpdateGraphs(guo);
        }

        private void ValidateBuildSpot(BuildSpot buildSpot)
        {
            var spawners = SpawnersControl.Instance.AllSpawners;
            TestObstacleGameObject.SetActive(true);

            foreach (var spawner in spawners)
                if (!CheckIfObstacleInBuildSpotWouldBlockPath(buildSpot, spawner.transform.position, spawner.EndTransform.position)) break; 

            TestObstacleGameObject.SetActive(false);
            AstarPath.active.Scan();
        }

        #endregion

        #region PathBlockPreCheck_UnityNavMesh

        private void CheckIfAdjacentSpotsWouldBeBlockingPathUnityNavMesh(BuildSpot buildSpot)
        {
            foreach (var spot in buildSpot.NeighbourBuildSpots.Values)
            {
                if (spot.IsOccupied || !spot.HasOccupiedNeighbours) continue;              
                StartCoroutine(CheckIfPopulatedBuildSpotWouldBlockPathUnityNavMesh(spot));
            }
        }

        private IEnumerator CheckIfPopulatedBuildSpotWouldBlockPathUnityNavMesh(BuildSpot buildSpot)
        {
            GameControl.Instance.EnableBuilding = false;
            //TestObstaclePrefab.transform.position = buildSpot.CenterPositionInWorld + obstacleUpOffset;
            //TestObstaclePrefab.gameObject.SetActive(true);

            NavMeshControl.Instance.RebuildNavMesh();

            var spawners = SpawnersControl.Instance.AllSpawners;

            foreach (var spawner in spawners)
            {
                TestPathForSpawner(spawner, buildSpot);
                if (buildSpot.IsInvalidSpot == true) break;
            }

            //TestObstaclePrefab.gameObject.SetActive(false);
            GameControl.Instance.EnableBuilding = true;

            yield return null;
        }

        private void TestPathForSpawner(Spawner spawner, BuildSpot spot)
        {
            //var spawnerPosition = spawner.transform.position;
            //var isPathAvailable = NavMesh.CalculatePath(spawnerPosition, spawner.EndPointForSpawn.position, navMeshQueryFilter, TestPath);
            //spot.IsInvalidSpot = !isPathAvailable || (TestPath.status == NavMeshPathStatus.PathPartial);
            //Debug.Log("Testing spot: " + spot.GridPosition + " , is build spot invalid: " + spot.IsInvalidSpot + " , is Path available: " + isPathAvailable + " , test path status: " + TestPath.status);
        }
        #endregion

        public void DestroyBuilding(Construction construction)
        {
            constructions.Remove(construction);
            OnBuildingRemove?.Invoke(construction);
            construction.BuildSpot.IsOccupied = false;
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
            selectedBuildSpot.IsOccupied = true;
            return construction;
        }

        private void CheckBlockedSpots()
        {
            var spawners = SpawnersControl.Instance.AllSpawners;
            MakeEndPointsUnblockable(spawners);
            CheckIfSpawnersBlockPaths(spawners);
            CheckIfObstaclesBlockPaths();
            AstarPath.active.Scan();
        }

        private void CheckAndResetStopWatch(string message)
        {
            StopWatch.Stop();
            Debug.Log(message + " " + StopWatch.Elapsed);
            StopWatch.Reset();
        }
    }
}