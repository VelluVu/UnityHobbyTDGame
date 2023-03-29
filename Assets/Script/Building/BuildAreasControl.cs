using System.Collections.Generic;
using System.Linq;
using TheTD.UI;
using UnityEngine;

public class BuildAreasControl : MonoBehaviour
{
    public static BuildAreasControl Instance { get; private set; }

    private BuildingHolder _buildingHolder;
    public BuildingHolder BuildingHolder { get => _buildingHolder = _buildingHolder != null ? _buildingHolder : FindObjectOfType<BuildingHolder>(); }

    private List<BuildArea> _buildAreas;
    public List<BuildArea> BuildAreas { get => _buildAreas = _buildAreas != null ? CheckBuildAreas() : GetBuildAreas(); }

    private BuildArea selectedBuildArea = null;

    private void Awake()
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

    private void OnBuildButtonClicked(TowerData towerData)
    {
        BuildOnSelectedArea(towerData);
    }

    public void BuildOnSelectedArea(TowerData towerData)
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
