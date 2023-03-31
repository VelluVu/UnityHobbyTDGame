using System.Collections.Generic;
using System.Linq;
using TheTD.Building;
using UnityEngine;
using UnityEngine.ProBuilder;

public class CustomGrid : MonoBehaviour
{
    private const string BUILD_AREA_LAYER_NAME = "BuildArea";
    private const string BUILD_AREA_GRID_GAMEOBJECT_NAME = "Grid";
    private const string GRID_MATERIAL_PATH = "Materials/GridMaterial";
    private const string OBSTACLE_ON_GRID = "Obstacle was on the grid";
    private const string TILING_SHADER_PROPERTY_NAME = "_Tiling";

    [Header("Build Area Grid Variables")]
    [SerializeField] private Vector2Int cellsAmount = new Vector2Int(10, 10);
    [SerializeField] private float offsetHeight = 0.1f;
    [SerializeField] private float cellSize = 1f;
    private float spacing = 0f; //need to make shader work with spacing, if want to allow spacing
    public List<BuildSpot> buildSpots = new List<BuildSpot>();

    public Vector2 Size { get => new Vector2(cellsAmount.x * cellSize + cellsAmount.x * spacing, cellsAmount.y * cellSize + cellsAmount.y * spacing); }
    public Vector3 Center { get => new Vector3(Size.x * 0.5f, transform.position.y, Size.y * 0.5f); }

    [SerializeField] private bool _isVisible = true;
    public bool IsVisible { get => _isVisible; set => SetIsVisible(value); }

    private Renderer _renderer;
    public Renderer Renderer { get => _renderer = _renderer != null ? _renderer : Mesh.GetComponent<Renderer>(); }

    private ProBuilderMesh _mesh;
    public ProBuilderMesh Mesh { get => _mesh = _mesh != null ? _mesh : GetComponentInChildren<ProBuilderMesh>() != null ? GetComponentInChildren<ProBuilderMesh>() : GetProBuilderPlaneMesh(); }

    private Material _gridMaterial;
    public Material GridMaterial { get => _gridMaterial = _gridMaterial != null ? _gridMaterial : Resources.Load<Material>(GRID_MATERIAL_PATH); }

    public void Start()
    {
        AddListeners();
    }

    public void AddListeners()
    {
        Obstacle.OnPositionChange += OnObstaclePositionChange;
    }

    public bool IsPositionInsideGridBounds(Vector3 position)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = buildSpots.Last().TopRightCornerPositionInWorld;
        var isInBounds = (position.x >= startPosition.x) && (position.z <= endPosition.z) && (position.x <= endPosition.x) && (position.z >= startPosition.z) && (position.y >= startPosition.y);
        return isInBounds;
    }

    public BuildSpot FindBuildSpotInClickPosition(Vector3 position)
    {
        var query = buildSpots.FindAll(o => o.IsPositionInBounds(position));
        return query.First();
    }

    public void Create()
    {
        DestroyMesh(); //Destroys the old grid, TODO: Update vertexes of old ProBuilderMesh.
        buildSpots.Clear();
        for (int x = 0; x < cellsAmount.x; x++)
        {
            for (int z = 0; z < cellsAmount.y; z++)
            {
                CreateBuildSpot(x, z);
            }
        }
        SetupProBuilderPlaneMesh();
    }

    public void DestroyMesh()
    {
        if (Mesh == null) return;
        Mesh.Clear();
        DestroyImmediate(Mesh.gameObject);
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
    public void SetupProBuilderPlaneMesh()
    {
        //TODO: if there is a mesh, update the old mesh.
        Mesh.gameObject.name = BUILD_AREA_GRID_GAMEOBJECT_NAME;
        Mesh.transform.SetParent(transform);
        Mesh.transform.position = transform.position + Vector3.up * offsetHeight;
        Mesh.gameObject.layer = LayerMask.NameToLayer(BUILD_AREA_LAYER_NAME);
        SetupMeshRenderer();
        CreateCollider();
    }

    private void SetupMeshRenderer()
    {
        Renderer.sharedMaterial = GridMaterial;
        Renderer.sharedMaterial.SetVector(TILING_SHADER_PROPERTY_NAME, new Vector4(1f / cellSize, 1f / cellSize, 0, 0));
        Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private void CreateCollider()
    {
        var collider = Mesh.GetComponent<BoxCollider>();
        collider = collider != null ? collider : Mesh.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = new Vector3(Center.x, -offsetHeight, Center.z);
    }

    private void SetIsVisible(bool value)
    {
        if (_isVisible == value) return;
        _isVisible = value;
        Mesh.gameObject.SetActive(_isVisible);
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

    private bool IsBoundingBoxInsideGrid(Vector3[] bounds)
    {
        for (int i = 0; i < bounds.Length; i++)
        {
            if (IsPositionInsideGridBounds(bounds[i])) return true;
        }
        return false;
    }

    private void OnObstaclePositionChange(Vector3[] bounds)
    {
        CheckIFObstacleBlocksBuildSpots(bounds);
    }

    private void CheckIFObstacleBlocksBuildSpots(Vector3[] bounds)
    {
        if (IsBoundingBoxInsideGrid(bounds))
        {
            Debug.LogFormat(OBSTACLE_ON_GRID);
            var buildSpotsInArea = FindBuildSpotsBetweenPositions(bounds);
            buildSpotsInArea.ForEach(o => o.IsOccupied = true);
        }
    }

    private ProBuilderMesh GetProBuilderPlaneMesh()
    {
        if (_mesh != null) return _mesh;
        if (GetComponentInChildren<ProBuilderMesh>() != null) return GetComponentInChildren<ProBuilderMesh>();
        return ShapeGenerator.GeneratePlane(PivotLocation.FirstVertex, Size.x, Size.y, cellsAmount.x, cellsAmount.y, Axis.Up);
    }
}
