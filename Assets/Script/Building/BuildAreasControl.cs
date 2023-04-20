using System.Collections.Generic;
using System.Linq;
using TheTD.UI;
using UnityEngine;

namespace TheTD.Building
{
    public class BuildAreasControl : MonoBehaviour
    {
        public static BuildAreasControl Instance { get; private set; }

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

        private void OnBuildButtonClicked(ITowerLoadData towerData)
        {
            BuildOnSelectedArea(towerData);
        }

        public void SellOnSelectedArea()
        {
            if (selectedBuildArea == null) return;
            selectedBuildArea.SellOnSelectedSpot();
        }

        public void BuildOnSelectedArea(ITowerLoadData towerData)
        {
            if (selectedBuildArea == null) return;
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
    }
}