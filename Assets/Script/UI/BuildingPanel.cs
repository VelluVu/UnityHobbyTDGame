using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanel : MonoBehaviour
{
    private const string PATH_TO_BUILDING_OPTION = "Prefabs/UI/BuildingOption";
    private const string SELECTED_TOWER_IS_NULL = "Selected tower is null, please select tower to build from building panel";

    public Button buildButton;

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
        Progress.OnTowerProgressChange += OnTowerProgressChange;
        if(buildButton == null)
        {
            Debug.LogFormat("Build button is null, please drag the build button reference for Building Panel");
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

    private void OnTowerProgressChange(Progress progress)
    {
        var unlockedTowers = progress.GetUnlockedTowers();
        if(!buildingOptions.Any())
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
