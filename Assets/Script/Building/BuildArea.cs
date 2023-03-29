using System.Collections.Generic;
using System.Linq;
using TheTD.Core;
using UnityEngine;
using UnityEngine.ProBuilder;

public class BuildArea : MonoBehaviour
{
    const string ON_CLICK_DEBUG_FORMAT = "Click was in bounds of {0}: {1}";
    const string ON_CLICK_HITS_DEBUG_FORMAT = "Click on build spot local position: {0} world position: {1}, is spot occupied: {2}";
    const string OBSTACLE_ON_GRID = "Obstacle was on the grid";

    [SerializeField] private Vector2Int size = new Vector2Int(10, 10);
    [SerializeField] private float offsetHeight = 0.1f;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float spacing = 0.1f;
    public Transform buildingHolder;
    public List<BuildSpot> buildSpots = new List<BuildSpot>();
    public List<Building> buildings = new List<Building>();
    public Color debugLineColor = Color.blue;
    public float durationForDebugLines = 100f;

    private BuildSpot selectedBuildSpot;

    public Vector2 TotalSize { get => new Vector2(size.x * cellSize + size.x * spacing, size.y * cellSize + size.y * spacing); }
    public Vector3 CenterGrid { get => new Vector3(TotalSize.x * 0.5f, transform.position.y, TotalSize.y * 0.5f); }

    [SerializeField]private bool _isGridVisible = true;
    public bool IsGridVisible { get => _isGridVisible;  set => SetIsGridVisible(value); }

    private LineRenderer _lineRenderer;
    public LineRenderer LineRenderer { get => _lineRenderer = _lineRenderer != null ? _lineRenderer : GetComponent<LineRenderer>(); }

    private ProBuilderMesh _mesh;
    public ProBuilderMesh Mesh { get => _mesh = _mesh != null ? _mesh : GetComponentInChildren<ProBuilderMesh>() != null ? GetComponentInChildren<ProBuilderMesh>() : ShapeGenerator.GeneratePlane(PivotLocation.FirstVertex, TotalSize.x, TotalSize.y, size.x, size.y, Axis.Up); }

    private SelectSpotVisual _selectVisual;
    public SelectSpotVisual SelectVisual { get => GetSelectSpotVisual(); }

    private Material _gridMaterial;
    public Material GridMaterial { get => _gridMaterial = _gridMaterial != null ? _gridMaterial : Resources.Load<Material>("Materials/GridMaterial"); }


    public delegate void BuildAreaDelegate(BuildArea buildArea);
    public static event BuildAreaDelegate OnBuildAreaClicked;

    public delegate void BuildDelegate(Building building);
    public static event BuildDelegate OnBuild;
    public static event BuildDelegate OnBuildingRemove;

    private void Start()
    {
        AddListeners();
    }

    public void RemoveListeners()
    {
        ClickPosition.OnClickPosition -= OnClickPosition;
        Obstacle.OnPositionChange -= OnObstaclePositionChange;
        Spawner.OnPathBlocked -= OnPathBlocked;
    }

    public void AddListeners()
    {
        RemoveListeners();
        ClickPosition.OnClickPosition += OnClickPosition;
        Obstacle.OnPositionChange += OnObstaclePositionChange;
        Spawner.OnPathBlocked += OnPathBlocked;
    }

    private bool IsBoundingBoxInGrid(Vector3[]bounds)
    {       
        for (int i = 0; i < bounds.Length; i++)
        {
            if(IsPositionInGridBounds(bounds[i])) return true;
        }
        return false;
    }

    private void OnObstaclePositionChange(Vector3[] bounds)
    {     
        if(IsBoundingBoxInGrid(bounds))
        {
            Debug.LogFormat(OBSTACLE_ON_GRID);
            var buildSpotsInArea = FindBuildSpotsBetweenPositions(bounds);
            buildSpotsInArea.ForEach(o => o.IsOccupied = true);
        }
    }

    private List<BuildSpot> FindBuildSpotsBetweenPositions(Vector3[] bounds)
    {
        var foundSpots = buildSpots.FindAll(o =>
        o.IsPositionInBounds(bounds[0]) ||
        o.IsPositionInBounds(bounds[1]) ||
        o.IsPositionInBounds(bounds[2]) ||
        o.IsPositionInBounds(bounds[3]) ||
        o.IsPositionInBounds(bounds[4]) ||
        o.IsPositionInBounds(bounds[5]) ||
        o.IsPositionInBounds(bounds[6]) ||
        o.IsPositionInBounds(bounds[7]));
        return foundSpots;
    }

    private void OnClickPosition(Vector3 clickPosition)
    {
        if (clickPosition == -Vector3.zero)
        {
            SelectVisual.IsSelected = false;
            selectedBuildSpot = null;
            return;
        }

        bool isClickOnTheGrid = IsPositionInGridBounds(clickPosition);
        //Debug.LogFormat(ON_CLICK_DEBUG_FORMAT, this.name, isClickOnTheGrid);
        if (isClickOnTheGrid)
        {
            var clickedBuildSpot = FindBuildSpotByPosition(clickPosition);
            OnBuildAreaClicked?.Invoke(this);
            //Debug.LogFormat(ON_CLICK_HITS_DEBUG_FORMAT, clickedBuildSpot.BottomLeftLocalPosition, clickedBuildSpot.BottomLeftPositionInWorld, clickedBuildSpot.IsOccupied);            
            SelectVisual.IsSelected = true;
            SelectVisual.SetMaterial(clickedBuildSpot.IsOccupied);
            SelectVisual.Position = clickedBuildSpot.CenterPositionInWorld + Vector3.up * offsetHeight;
            selectedBuildSpot = clickedBuildSpot;
        }
        else
        {
            SelectVisual.IsSelected = false;
            selectedBuildSpot = null;
        }
    }

    public void CreateGrid()
    {
        buildSpots.Clear();
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                CreateBuildSpot(x, z);
            }
        }
        CreateGridPlane();
    }

    private void CreateBuildSpot(int x, int z)
    {
        float xPos = x == 0 ? x * cellSize : x * cellSize + x * spacing;
        float zPos = z == 0 ? z * cellSize : z * cellSize + z * spacing;
        var localPosition = new Vector3(xPos, 0f, zPos);
        var worldPosition = localPosition + transform.position;
        BuildSpot spot = new BuildSpot(localPosition, worldPosition, false, cellSize);
        buildSpots.Add(spot);
    }

    public bool IsPositionInGridBounds(Vector3 position)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = buildSpots.Last().TopRightCornerPositionInWorld;
        var isInBounds = (position.x >= startPosition.x) && (position.z <= endPosition.z) && (position.x <= endPosition.x) && (position.z >= startPosition.z) && (position.y >= startPosition.y);
        return isInBounds;
    }

    public BuildSpot FindBuildSpotByPosition(Vector3 position)
    {
        var query = buildSpots.FindAll(o => o.IsPositionInBounds(position));
        return query.First();
    }

    public void CreateGridPlane()
    {
        var renderer = Mesh.GetComponent<MeshRenderer>();
        Mesh.transform.SetParent(transform);
        Mesh.transform.position = transform.position + Vector3.up * offsetHeight;
        renderer.sharedMaterial = GridMaterial;
        renderer.sharedMaterial.SetVector("_Tiling", new Vector4(1f / cellSize, 1f / cellSize, 0, 0));
        renderer.gameObject.layer = LayerMask.NameToLayer("BuildArea");
        renderer.gameObject.name = "Grid";
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        var collider = renderer.GetComponent<BoxCollider>();
        collider = collider != null ? collider : renderer.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = new Vector3(CenterGrid.x, -offsetHeight, CenterGrid.z);
    }

    public void UpdateGrid()
    {
        DestroyImmediate(Mesh.gameObject);
        CreateGrid();
    }

    public void BuildOnSelectedSpot(TowerData tower)
    {
        if (selectedBuildSpot == null ||selectedBuildSpot.IsOccupied) return;
        if (!GameControl.Instance.Player.Gold.HasGoldForTower(tower.Tower.buildCost))
        {
            Debug.Log("Not enough gold to build the tower");
            return;
        }
        selectedBuildSpot.IsOccupied = true;
        SelectVisual.SetMaterial(selectedBuildSpot.IsOccupied);
        var building = CreateBuilding(tower);
        buildings.Add(building);
        OnBuild?.Invoke(building);
    }

    public void SellOnSelectedSpot()
    {
        if(selectedBuildSpot == null) return;
        if(selectedBuildSpot.Building != null) DestroyBuilding(selectedBuildSpot.Building);
        //Get gold back
    }

    public void DestroyBuilding(Building building)
    {
        buildings.Remove(building);
        OnBuildingRemove?.Invoke(building);
        building.BuildSpot.IsOccupied = false;
        SelectVisual.SetMaterial(building.BuildSpot.IsOccupied);
        Destroy(building.gameObject);
        buildings.TrimExcess();
    }

    public Building CreateBuilding(TowerData tower)
    {
        GameObject buildingGO = new GameObject();
        var building = buildingGO.AddComponent<Building>();  
        building.InitBuilding(selectedBuildSpot, buildingHolder, tower);
        return building;
    }

    private void OnPathBlocked(Vector3 position)
    {
        DestroyClosestBuilding(position);
    }

    private void DestroyClosestBuilding(Vector3 position)
    {
        var building = FindClosestBuilding(position);
        // mark buildspot no longer occupied !
        DestroyBuilding(building);
    }

    public Building FindClosestBuilding(Vector3 position)
    {
        var closest = buildings[0];

        for (int i = 0; i < buildings.Count; i++)
        {
            var shortestDistance = Vector3.Distance(position, closest.transform.position);
            var distanceToCurrentObstacle = Vector3.Distance(position, buildings[i].transform.position);

            if (distanceToCurrentObstacle < shortestDistance)
            {
                closest = buildings[i];
            }
        }

        return closest;
    }

    private void SetIsGridVisible(bool value)
    {
        if(_isGridVisible == value) return;
        _isGridVisible = value;
        Mesh.gameObject.SetActive(_isGridVisible);
    }

    private SelectSpotVisual GetSelectSpotVisual()
    {
        if (_selectVisual != null) return _selectVisual;
        _selectVisual = Instantiate(Resources.Load<GameObject>("Prefabs/SelectSpotVisual"), transform).GetComponent<SelectSpotVisual>();
        return _selectVisual;
    }
}
