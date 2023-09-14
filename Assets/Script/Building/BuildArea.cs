using Pathfinding;
using System.Collections.Generic;
using TheTD.Core;
using TheTD.Spawning;
using UnityEngine;

namespace TheTD.Building
{
    public class BuildArea : MonoBehaviour, IEventListener
    {
        private const string NOT_ENOUGH_GOLD_LOG_MESSAGE = "Not enough gold to build the tower";
        public bool isDebug = false;

        public List<Construction> constructions = new List<Construction>();

        private BuildSpot selectedBuildSpot;
        
        public CustomGrid customGrid;

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
            CheckAllPathBlocks();
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
            construction.BuildSpot.HasConstruction = true;
            construction.BuildSpot.IsInvalidConstructionSpot = true;
            //construction.BuildSpot.Node.Walkable = false;
            CheckConstructionsNeighbourSpotsAvailability();
            OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            OnBuild?.Invoke(construction);  
            AstarData.active.Scan();      
        }

        public void SellOnSelectedSpot()
        {
            if (selectedBuildSpot == null) return;
            if (selectedBuildSpot.Construction == null) return;
            DestroyBuilding(selectedBuildSpot.Construction);
            selectedBuildSpot.IsInvalidConstructionSpot = false;
            //selectedBuildSpot.Node.Walkable = true;
            CheckConstructionsNeighbourSpotsAvailability();
            CheckIfObstacleInNeighboursWouldBlockPath(selectedBuildSpot);
            OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            OnSell?.Invoke(selectedBuildSpot.Construction);
            selectedBuildSpot.Construction = null;
            Physics.SyncTransforms();
            AstarPath.active.Scan();
        }

        private void OnClickPosition(Vector3 clickPosition)
        {
            Debug.Log(clickPosition);
            if (clickPosition.magnitude == Vector3.negativeInfinity.magnitude)
            {
                selectedBuildSpot = null;
                OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
                return;
            }

            bool isClickOnTheGrid = customGrid.IsPositionInsideGridBounds(clickPosition);

            if (isClickOnTheGrid)
            {
                var clickedBuildSpot = customGrid.FindBuildSpotInWorldPosition(clickPosition);
                OnBuildAreaClicked?.Invoke(this);          
                selectedBuildSpot = clickedBuildSpot;
                OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);           
            }
        }

        private bool IsUnableToBuildOnSelectedSpot()
        {
            return selectedBuildSpot == null || selectedBuildSpot.HasConstruction || selectedBuildSpot.IsInvalidConstructionSpot;
        }

        private void CheckConstructionsNeighbourSpotsAvailability()
        {
            foreach (var item in constructions)
            {
                CheckIfObstacleInNeighboursWouldBlockPath(item.BuildSpot);
            }
        }

        public void CheckAllPathBlocks()
        {
            var spawners = SpawnersControl.Instance.AllSpawners;
            BuildAreasControl.Instance.TestObstacleGameObject.SetActive(true);
            foreach (var spawner in spawners)
            {
                foreach(var buildSpot in customGrid.buildSpots.Values)
                {
                    CheckIfObstacleInBuildSpotWouldBlockPath(buildSpot, spawner.transform.position, spawner.EndTransform.position);
                }
            }
            BuildAreasControl.Instance.TestObstacleGameObject.SetActive(false);
            AstarPath.active.Scan();
        }
        
        #region PathBlockPreCheck_AstarProjectPro

        private void CheckIfObstacleInNeighboursWouldBlockPath(BuildSpot buildSpot)
        {
            var neighbours = buildSpot.NeighbourBuildSpots;
            var spawners = SpawnersControl.Instance.AllSpawners;
     
            BuildAreasControl.Instance.TestObstacleGameObject.SetActive(true);

            foreach (var neighbour in neighbours.Values)
            {
                foreach (var spawner in spawners)
                {
                    CheckIfObstacleInBuildSpotWouldBlockPath(neighbour, spawner.transform.position, spawner.EndTransform.position);                  
                }
            }     

            BuildAreasControl.Instance.TestObstacleGameObject.SetActive(false);
            AstarPath.active.Scan();       
        }

        public bool IsPathPossible(Vector3 start, Vector3 destination)
        {
            var startNode = AstarPath.active.GetNearest(start, NNConstraint.None).node;
            var destinationNode = AstarPath.active.GetNearest(destination, NNConstraint.None).node;
            return PathUtilities.IsPathPossible(startNode, destinationNode);
        }

        private bool CheckIfObstacleInBuildSpotWouldBlockPath(BuildSpot buildSpot, Vector3 spawnPosition, Vector3 endPosition)
        {
            BuildAreasControl.Instance.TestObstacleGameObject.transform.position = buildSpot.CenterPositionInWorld + Vector3.up * 0.5f;
            Physics.SyncTransforms();
            AstarPath.active.Scan();

            bool isAvailablePosition = IsPathPossible(spawnPosition, endPosition);
            buildSpot.IsInvalidConstructionSpot = !isAvailablePosition;
          
            BuildAreasControl.Instance.TestObstacleGameObject.transform.position = Vector3.one * -9999f;
            return isAvailablePosition;
        }

        #endregion

        public void DestroyBuilding(Construction construction)
        {
            constructions.Remove(construction);
            OnBuildingRemove?.Invoke(construction);
            construction.BuildSpot.HasConstruction = false;
            construction.gameObject.SetActive(false);
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

        private void OnDestroy() 
        {
            RemoveListeners();
        }

        public void RemoveListeners()
        {
            ClickPosition.OnClickPosition -= OnClickPosition;
        }
    }
}