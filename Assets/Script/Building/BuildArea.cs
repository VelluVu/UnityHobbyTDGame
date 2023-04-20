using System.Collections.Generic;
using TheTD.Core;
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

        public delegate void BuildDelegate(Construction construction);
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

        public void BuildOnSelectedSpot(ITowerLoadData towerLoadData)
        {
            if (selectedBuildSpot == null || selectedBuildSpot.IsOccupied) return;
            if (!GameControl.Instance.Player.Gold.HasGoldForTower(towerLoadData.Tower.BuildCost))
            {
                Debug.Log(NOT_ENOUGH_GOLD_LOG_MESSAGE);
                return;
            }
            selectedBuildSpot.IsOccupied = true;
            OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            var construction = CreateBuilding(towerLoadData);
            selectedBuildSpot.Construction = construction;
            buildings.Add(construction);
            OnBuild?.Invoke(construction);
        }

        public void SellOnSelectedSpot()
        {
            if (selectedBuildSpot == null) return;
            if (selectedBuildSpot.Construction == null) return;
            var construction = selectedBuildSpot.Construction;
            DestroyBuilding(construction);
            OnSell?.Invoke(selectedBuildSpot.Construction);
        }

        public void DestroyBuilding(Construction construction)
        {
            buildings.Remove(construction);
            OnBuildingRemove?.Invoke(construction);
            construction.BuildSpot.IsOccupied = false;
            OnSelectedBuildSpotChange?.Invoke(selectedBuildSpot);
            Destroy(construction.gameObject);
            buildings.TrimExcess();
        }

        public Construction CreateBuilding(ITowerLoadData towerLoadData)
        {
            GameObject constructionGO = new GameObject();
            var construction = constructionGO.AddComponent<Construction>();
            construction.InitBuilding(selectedBuildSpot, towerLoadData);
            return construction;
        }     
    }
}