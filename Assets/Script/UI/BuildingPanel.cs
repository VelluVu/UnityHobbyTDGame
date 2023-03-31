using System;
using System.Collections.Generic;
using System.Linq;
using TheTD.Core;
using TheTD.Towers;
using UnityEngine;
using UnityEngine.UI;

namespace TheTD.UI
{
    public class BuildingPanel : MonoBehaviour
    {
        private const string PATH_TO_BUILDING_OPTION = "Prefabs/UI/BuildingOption";
        private const string SELECTED_TOWER_IS_NULL = "Selected tower is null, please select tower to build from building panel";
        private const string BUILD_BUTTON_IS_NULL = "Build button is null, please drag the build button reference for Building Panel";
        public Button buildButton;
        public Button sellButton;

        private ScrollRect buildingScrollRect;
        public ScrollRect BuildingScrollRect { get => buildingScrollRect = buildingScrollRect != null ? buildingScrollRect : GetComponentInChildren<ScrollRect>(); }

        private BuildingOption buildingOption;
        public BuildingOption BuildingOption { get => buildingOption = buildingOption != null ? buildingOption : Resources.Load<BuildingOption>(PATH_TO_BUILDING_OPTION); }

        public List<BuildingOption> buildingOptions = new List<BuildingOption>();

        private TowerData selectedTower = null;

        public delegate void BuildingOptionDelegate(TowerData towerData);
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

        private void CheckBuildingOptions(List<TowerData> unlockedTowers)
        {
            var query = unlockedTowers.Where(o => !buildingOptions.Any(p => p.Tower.towerType == o.towerType));
            var towersNotInOptions = query.ToList();
            InstantiateBuildingOptions(towersNotInOptions);
        }

        private void InstantiateBuildingOptions(List<TowerData> unlockedTowers)
        {
            unlockedTowers.ForEach(o =>
            {
                var buildingOption = Instantiate(BuildingOption, BuildingScrollRect.content).GetComponent<BuildingOption>();
                buildingOption.InitBuildingOption(o);
                BuildingOption.OnSelectTowerOption += OnSelectTowerOption;
                buildingOptions.Add(buildingOption);
            });
        }

        private void OnSelectTowerOption(BuildingOption buildingOption)
        {
            selectedTower = buildingOption.Tower;
            buildingOptions.ForEach(o => o.IsSelected = false);
            buildingOption.IsSelected = true;
        }
    }
}