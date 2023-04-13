using System.Collections.Generic;
using TheTD.Core;
using TheTD.Towers;
using UnityEngine;

namespace TheTD.Building
{
    public class BuildArea : MonoBehaviour
    {
        private const string NOT_ENOUGH_GOLD_LOG_MESSAGE = "Not enough gold to build the tower";

        public List<Construction> buildings = new List<Construction>();

        private BuildSpot selectedBuildSpot;

        private CustomGrid _customGrid;
        public CustomGrid CustomGrid { get => _customGrid = _customGrid != null ? _customGrid : GetComponentInChildren<CustomGrid>(); } 
    
        public delegate void SelectBuildSpotDelegate(BuildSpot selectedSpot);
        public static event SelectBuildSpotDelegate OnSelectedBuildSpotChange;

        public delegate void BuildAreaDelegate(BuildArea buildArea);
        public static event BuildAreaDelegate OnBuildAreaClicked;

        public delegate void BuildDelegate(Construction building);
        public static event BuildDelegate OnBuild;
        public static event BuildDelegate OnSell;
        public static event BuildDelegate OnBuildingRemove;

        private void Start()
        {
            AddListeners();
        }

        public void AddListeners()
        {
            ClickPosition.OnClickPosition += OnClickPosition;            
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
                var clickedBuildSpot = CustomGrid.FindBuildSpotInClickPosition(clickPosition);
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

        public void BuildOnSelectedSpot(TowerLoadData tower)
        {
            if (selectedBuildSpot == null || selectedBuildSpot.IsOccupied) return;
            if (!GameControl.Instance.Player.Gold.HasGoldForTower(tower.Tower.BuildCost))
            {
                Debug.Log(NOT_ENOUGH_GOLD_LOG_MESSAGE);
                return;
            }
            selectedBuildSpot.IsOccupied = true;
            OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            var building = CreateBuilding(tower);
            buildings.Add(building);
            OnBuild?.Invoke(building);
        }

        public void SellOnSelectedSpot()
        {
            if (selectedBuildSpot == null) return;
            if (selectedBuildSpot.Building == null) return;
            var building = selectedBuildSpot.Building;
            DestroyBuilding(building);
            OnSell?.Invoke(selectedBuildSpot.Building);
        }

        public void DestroyBuilding(Construction building)
        {
            buildings.Remove(building);
            OnBuildingRemove?.Invoke(building);
            building.BuildSpot.IsOccupied = false;
            OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            Destroy(building.gameObject);
            buildings.TrimExcess();
        }

        public Construction CreateBuilding(TowerLoadData tower)
        {
            GameObject buildingGO = new GameObject();
            var building = buildingGO.AddComponent<Construction>();
            building.InitBuilding(selectedBuildSpot, tower);
            return building;
        }     
    }
}