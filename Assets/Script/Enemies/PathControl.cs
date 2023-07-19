using UnityEngine;
using Pathfinding;
using UnityEngine.Events;
using TheTD.Building;

[RequireComponent(typeof(Seeker))]
public class PathControl : MonoBehaviour
{
    private const string END_POINT_TAG = "EndPoint";

    [SerializeField] public int currentNodeIndex = 0;

    public Path currentPath;
    [SerializeField] public GraphNode currentNode;
    [SerializeField] public GraphNode nextNode;

    public bool HasPath { get => currentPath != null; }

    public Vector3 NextNodePosition { get => (Vector3)nextNode.position; }

    private Vector3 destinationYOffset = Vector3.zero;
    public Vector3 DestinationYOffset { set => destinationYOffset = value; }

    private Vector3 _destination;
    public Vector3 Destination { get => _destination = _destination != null ? _destination : GameObject.FindGameObjectWithTag(END_POINT_TAG).transform.position; set => _destination = value; }

    public UnityAction<Path> OnNewPath;
    public UnityAction OnPathFail;

    private Seeker _seeker;
    public Seeker Seeker { get => _seeker = _seeker != null ? _seeker : GetComponent<Seeker>(); }

    private void Start() 
    {
        AddListeners();
        FindPath(transform.position, Destination);
    }

    private void AddListeners()
    {
        BuildArea.OnBuild += OnBuildingPlaced;
        BuildArea.OnSell += OnBuildingRemoved;
    }

    private void RemoveListeners()
    {
        BuildArea.OnBuild -= OnBuildingPlaced;
        BuildArea.OnSell -= OnBuildingRemoved;
    }

    private void OnDestroy() 
    {
        RemoveListeners();
    }

    private void OnBuildingPlaced(Construction construction)
    {
        FindPath(transform.position, Destination);
    }

    private void OnBuildingRemoved(Construction construction)
    {
        FindPath(transform.position, Destination);
    }

    public void FindPath(Vector3 start, Vector3 destination) 
    {
        Seeker.StartPath(start, destination, OnPathComplete);
    }

    public void OnPathComplete (Path newPath) 
    {  
        if (newPath.error) 
        {
            Debug.Log(newPath.errorLog);
            currentPath = null;
            OnPathFail?.Invoke();
            return;
        } 
       
        currentPath = newPath;
        OnNewPath?.Invoke(currentPath);
    }
}
