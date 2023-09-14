using System.Collections.Generic;
using System.Linq;
using TheTD.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TheTD.UI
{
    public class BuildingPanel : MonoBehaviour, IEventListener
    {
        private const string PATH_TO_BUILDING_OPTION = "Prefabs/UI/BuildingOption";
        private const string SELECTED_TOWER_IS_NULL = "Selected tower is null, please select tower to build from building panel";
        private const string BUILD_BUTTON_IS_NULL = "Build button is null, please drag the build button reference for Building Panel";
        public Button buildButton;
        public Button sellButton;

        private ScrollRect _buildingScrollRect;
        public ScrollRect BuildingScrollRect { get => _buildingScrollRect = _buildingScrollRect != null ? _buildingScrollRect : GetComponentInChildren<ScrollRect>(); }

        private BuildingOption _buildingOption = null;
        public BuildingOption BuildingOption { get => _buildingOption = _buildingOption != null ? _buildingOption : Resources.Load<BuildingOption>(PATH_TO_BUILDING_OPTION); }

        public List<BuildingOption> buildingOptions = new List<BuildingOption>();

        private ITowerLoadData selectedTower = null;

        public delegate void BuildingOptionDelegate(ITowerLoadData towerData);
        public static event BuildingOptionDelegate OnBuildClick;

        private void Start()
        {
            AddListeners();
        }

        public void AddListeners()
        {
            GameProgress.OnTowerProgressChange += OnTowerProgressChange;
            if (buildButton == null)
            {
                Debug.LogFormat(BUILD_BUTTON_IS_NULL);
                return;
            }
            buildButton.onClick.AddListener(BuildSelectedTower);
        }

        public void BuildSelectedTower()
        {
            if (selectedTower == null)
            {
                Debug.LogFormat(SELECTED_TOWER_IS_NULL);
                return;
            }
            OnBuildClick?.Invoke(selectedTower);
        }

        public void RemoveListeners()
        {
            GameProgress.OnTowerProgressChange -= OnTowerProgressChange;
            if(buildButton != null) buildButton.onClick.RemoveAllListeners();
        }

        private void OnTowerProgressChange()
        {
            var unlockedTowers = GameProgress.Instance.GetUnlockedTowers();
            if (!buildingOptions.Any())
            {
                InstantiateBuildingOptions(unlockedTowers);
            }
            else
            {
                CheckBuildingOptions(unlockedTowers);
            }
        }

        private void CheckBuildingOptions(List<ITowerLoadData> unlockedTowers)
        {
            var query = unlockedTowers.Where(o => !buildingOptions.Any(p => p.TowerLoadData.TowerType == o.TowerType));
            var towersNotInOptions = query.ToList();
            InstantiateBuildingOptions(towersNotInOptions);
        }

        private void InstantiateBuildingOptions(List<ITowerLoadData> unlockedTowers)
        {
            unlockedTowers.ForEach(o =>
            {
                var _buildingOption = Instantiate(BuildingOption, BuildingScrollRect.content).GetComponent<BuildingOption>();
                _buildingOption.InitBuildingOption(o);
                BuildingOption.OnSelectTowerOption += OnSelectTowerOption;
                buildingOptions.Add(_buildingOption);
            });
        }

        private void OnSelectTowerOption(BuildingOption _buildingOption)
        {
            selectedTower = _buildingOption.TowerLoadData;
            buildingOptions.ForEach(o => o.IsSelected = false);
            _buildingOption.IsSelected = true;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
    }
}