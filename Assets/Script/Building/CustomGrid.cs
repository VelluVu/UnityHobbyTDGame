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

    [SerializeField]private List<Vector2IntBuildSpotPair> buildSpotsSerializable = new List<Vector2IntBuildSpotPair>();
    public Dictionary<Vector2Int, BuildSpot> buildSpots = new Dictionary<Vector2Int,BuildSpot>();
    
    public Vector2Int GridBottomLeftPositionInWorldGridCoordinates { get => ConvertWorldPosToGridPosition(transform.position); }
    public Vector2Int GridTopRightPositionInWorldGridCoordinates { get => new Vector2Int(GridBottomLeftPositionInWorldGridCoordinates.x + cellsAmount.x - 1, GridBottomLeftPositionInWorldGridCoordinates.y + cellsAmount.y - 1); }
    public Vector3 BottomLeftPositionInWorld { get => transform.position; }
    public Vector3 TopRightPositionInWorld { get => new Vector3(BottomLeftPositionInWorld.x + cellsAmount.x * cellSize, BottomLeftPositionInWorld.y, BottomLeftPositionInWorld.z + cellsAmount.y * cellSize); }
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

    private void Awake()
    {
        foreach (Vector2IntBuildSpotPair pair in buildSpotsSerializable)
            buildSpots[pair.key] = pair.value;
        InitNeighBoursForBuildSpots();
    }

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
        var topRightBuildSpot = buildSpots[cellsAmount - Vector2Int.one];
        Vector3 endPosition = topRightBuildSpot.TopRightCornerPositionInWorld;
        var isInBounds = (position.x >= startPosition.x) && (position.z <= endPosition.z) && (position.x <= endPosition.x) && (position.z >= startPosition.z) && (position.y >= startPosition.y);
        return isInBounds;
    }

    public Vector2Int ConvertWorldPosToGridPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - BottomLeftPositionInWorld;
        int gridPositionX = Mathf.FloorToInt(localPosition.x);
        int gridPositionY = Mathf.FloorToInt(localPosition.z);
        var gridPosition = new Vector2Int(gridPositionX, gridPositionY);
        
        //Debug.Log("World position: " + worldPosition + " converted to grid position: " + gridPosition);
        
        //if(gridPosition.x < 0 || gridPosition.y < 0 || gridPosition.x > cellsAmount.x - 1 || gridPosition.y > cellsAmount.y - 1)
        //{
        //    Debug.Log("World position: " + worldPosition + " in grid position: " + gridPosition + " is not in grid");
        //}

        return gridPosition;
    }

    public BuildSpot FindBuildSpotInWorldPosition(Vector3 position)
    {
        return buildSpots.Values.ToList().Find(o => o.IsPositionInBounds(position));  
    }

    public BuildSpot FindClosestBuildSpot(Vector3 position)
    {
        var positionInGrid = ConvertWorldPosToGridPosition(position);
        var closestGridPosition = SnapToClosestGridPosition(positionInGrid);    
        var closestspot = buildSpots[closestGridPosition];
        //= buildSpots.Values.FirstOrDefault();

        //foreach (var spot in buildSpots.Values)
        //{
        //    if (spot == closestspot) continue;
        //    var distanceToCurrentSpot = Vector3.Distance(position, spot.CenterPositionInWorld);
        //    var distanceToClosestSpot = Vector3.Distance(position, closestspot.CenterPositionInWorld);
        //    if (distanceToCurrentSpot > distanceToClosestSpot) continue;
        //    closestspot = spot;
        //}
        //Debug.Log("FOUND CLOSEST SPOT POSITION IN GRID: " + closestspot.GridPosition);
        return closestspot;
    }

    public Vector2Int SnapToClosestGridPosition(Vector2Int gridPosition)
    {
        var side = SideOfGridPositionComparedToGridInWorldGridPosition(gridPosition);
        var closestGridPosition = gridPosition;

        if (side == Vector2Int.right) closestGridPosition = new Vector2Int(cellsAmount.x - 1, gridPosition.y);
        else if (side == Vector2Int.left) closestGridPosition = new Vector2Int(0, gridPosition.y);
        else if (side == Vector2Int.up) closestGridPosition = new Vector2Int(gridPosition.x, cellsAmount.y - 1);
        else if (side == Vector2Int.down) closestGridPosition = new Vector2Int(gridPosition.x, 0);
        else if (side == Vector2Int.one) closestGridPosition = new Vector2Int(cellsAmount.x - 1, cellsAmount.y - 1);
        else if (side.x == -1 && side.y == 1) closestGridPosition = new Vector2Int(0, cellsAmount.y - 1);
        else if (side.x == 1 && side.y == -1) closestGridPosition = new Vector2Int(cellsAmount.x - 1, 0);
        else if (side == -Vector2Int.one) closestGridPosition = Vector2Int.zero;

        //Debug.Log("Grid Position: " + gridPosition + " is snapped to closest grid position: " + closestGridPosition);
        return closestGridPosition;
    }

    public Vector2Int SideOfGridPositionComparedToGridInWorldGridPosition(Vector2Int gridPosition)
    {
        var minGridPosition = GridBottomLeftPositionInWorldGridCoordinates;
        var maxGridPosition = GridTopRightPositionInWorldGridCoordinates;
        var side = Vector2Int.zero;

        if (gridPosition.x > minGridPosition.x && gridPosition.y <= maxGridPosition.y && gridPosition.y >= minGridPosition.y) side = Vector2Int.right;
        else if (gridPosition.x < minGridPosition.x && gridPosition.y <= maxGridPosition.y && gridPosition.y >= minGridPosition.y) side = Vector2Int.left;       
        else if (gridPosition.y > maxGridPosition.y && gridPosition.x <= maxGridPosition.x && gridPosition.x >= minGridPosition.x) side = Vector2Int.up;     
        else if (gridPosition.y < minGridPosition.y && gridPosition.x <= maxGridPosition.x && gridPosition.x >= minGridPosition.x) side = Vector2Int.down;      
        else if (gridPosition.x < minGridPosition.x && gridPosition.y < minGridPosition.y) side = -Vector2Int.one;
        else if (gridPosition.x < minGridPosition.x && gridPosition.y > maxGridPosition.y) side = new Vector2Int(-1, 1);
        else if (gridPosition.x > maxGridPosition.x && gridPosition.y > maxGridPosition.y) side = Vector2Int.one;
        else if (gridPosition.x > maxGridPosition.x && gridPosition.y < minGridPosition.y) side = new Vector2Int(1, -1);

        //Debug.Log("Grid position: " + gridPosition + " side offset from grid is: " + side);
        return side;
    }

    public Vector2Int WhichSideOfBuildAreaWorldPositionIs(Vector3 position)
    {
        var bottomLeftPositionInWorld = BottomLeftPositionInWorld;
        var topRightPostitionInWorld = TopRightPositionInWorld;

        if (position.x > topRightPostitionInWorld.x && position.z < topRightPostitionInWorld.z && position.z > bottomLeftPositionInWorld.z) return Vector2Int.right;      
        else if (position.x < bottomLeftPositionInWorld.x && position.z < topRightPostitionInWorld.z && position.z > bottomLeftPositionInWorld.z) return Vector2Int.left;
        else if (position.x > bottomLeftPositionInWorld.x && position.z < bottomLeftPositionInWorld.z && position.x < topRightPostitionInWorld.x) return Vector2Int.down;
        else if (position.x < topRightPostitionInWorld.x && position.x > bottomLeftPositionInWorld.x && position.z > topRightPostitionInWorld.z) return Vector2Int.up;
        else if (position.x < bottomLeftPositionInWorld.x && position.z < bottomLeftPositionInWorld.z)  return -Vector2Int.one;        
        else if (position.x < bottomLeftPositionInWorld.x && position.z > topRightPostitionInWorld.z) return new Vector2Int(-1, 1);     
        else if (position.x > bottomLeftPositionInWorld.x && position.z < bottomLeftPositionInWorld.z) return new Vector2Int(1, -1);
        else if (position.x > topRightPostitionInWorld.x && position.z > topRightPostitionInWorld.z) return Vector2Int.one;
        else return Vector2Int.zero;
    }

    public void Create()
    {
        DestroyMesh(); //Destroys the old grid, TODO: Update vertexes of old ProBuilderMesh.
        buildSpotsSerializable.Clear();
        buildSpots.Clear();
        for (int x = 0; x < cellsAmount.x; x++)
        {
            for (int z = 0; z < cellsAmount.y; z++)
            {
                CreateBuildSpot(x, z);
            }
        }
#if !UNITY_EDITOR
        InitNeighBoursForBuildSpots();
#endif
        SetupProBuilderPlaneMesh();
    }

    private void InitNeighBoursForBuildSpots()
    {
        var spots = buildSpots.Values.ToArray();

        for (int i = 0; i < spots.Length; i++)
        {
            var buildSpot = spots[i];
            var neighbourDictionary = new Dictionary<Vector2Int, BuildSpot>();

            if (buildSpot.GridPosition.x < cellsAmount.x -1)
            {
                var rightSpot = buildSpots[buildSpot.GridPosition + Vector2Int.right];
                neighbourDictionary.Add(rightSpot.GridPosition, rightSpot);

                if (buildSpot.GridPosition.y < cellsAmount.y -1)
                {
                    var topRightSpot = buildSpots[buildSpot.GridPosition + Vector2Int.one];
                    neighbourDictionary.Add(topRightSpot.GridPosition, topRightSpot);
                }
            }

            if (buildSpot.GridPosition.y < cellsAmount.y - 1)
            {
                var upSpot = buildSpots[buildSpot.GridPosition + Vector2Int.up];
                neighbourDictionary.Add(upSpot.GridPosition, upSpot);

                if (buildSpot.GridPosition.x > 0)
                {
                    var topLeftSpot = buildSpots[buildSpot.GridPosition + new Vector2Int(-1, 1)];
                    neighbourDictionary.Add(topLeftSpot.GridPosition, topLeftSpot);
                }
            }

            if (buildSpot.GridPosition.x > 0)
            {
                var leftSpot = buildSpots[buildSpot.GridPosition + Vector2Int.left];
                neighbourDictionary.Add(leftSpot.GridPosition, leftSpot);
                if (buildSpot.GridPosition.y > 0)
                {
                    var downLeftSpot = buildSpots[buildSpot.GridPosition + -Vector2Int.one];
                    neighbourDictionary.Add(downLeftSpot.GridPosition, downLeftSpot);
                }
            }
            if (buildSpot.GridPosition.y > 0)
            {
                var downSpot = buildSpots[buildSpot.GridPosition + Vector2Int.down];
                neighbourDictionary.Add(downSpot.GridPosition, downSpot);

                if (buildSpot.GridPosition.x < cellsAmount.x - 1)
                {
                    var downRightSpot = buildSpots[buildSpot.GridPosition + new Vector2Int(1, -1)];
                    neighbourDictionary.Add(downRightSpot.GridPosition, downRightSpot);
                }
            }

            buildSpot.SetNeighbourBuildSpots(neighbourDictionary);
        }
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
        var gridPosition = new Vector2Int(x, z);
        var localPosition = new Vector3(xPos, 0f, zPos);
        var worldPosition = localPosition + transform.position;
        BuildSpot spot = new BuildSpot(gridPosition, localPosition, worldPosition, false, cellSize);
#if !UNITY_EDITOR
        buildSpots.Add(gridPosition,spot);
#endif
        buildSpotsSerializable.Add(new Vector2IntBuildSpotPair(gridPosition, spot));

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
        var foundSpots = buildSpots.Values.ToList().FindAll(o =>
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
            buildSpotsInArea.ForEach(o => o.HasConstruction = true);
        }
    }

    private ProBuilderMesh GetProBuilderPlaneMesh()
    {
        if (_mesh != null) return _mesh;
        if (GetComponentInChildren<ProBuilderMesh>() != null) return GetComponentInChildren<ProBuilderMesh>();
        return ShapeGenerator.GeneratePlane(PivotLocation.FirstVertex, Size.x, Size.y, cellsAmount.x, cellsAmount.y, Axis.Up);
    }
}

[System.Serializable]
public class Vector2IntBuildSpotPair
{
    public Vector2Int key;
    public BuildSpot value;

    public Vector2IntBuildSpotPair(Vector2Int key, BuildSpot value)
    {
        this.key = key;
        this.value = value;
    }
}
