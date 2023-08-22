using UnityEngine;
using Pathfinding;
using UnityEngine.Events;
using TheTD.Building;
using System.Collections.Generic;

[RequireComponent(typeof(Seeker))]
public class PathControl : MonoBehaviour
{
    private const string END_POINT_TAG = "EndPoint";
    public bool IsNewPath = true;
    [SerializeField] public int currentNodeIndex = 0;
    public List<Vector3> currentPath;
    [SerializeField] public Vector3 currentNode;
    [SerializeField] public Vector3 nextNode;

    //public Vector3 NextNodePosition { get => (Vector3)nextNode.position; }
    public bool HasPath { get; protected set; }

    private Vector3 destinationYOffset = Vector3.zero;
    public Vector3 DestinationYOffset { set => destinationYOffset = value; }

    private Vector3 _destination;
    public Vector3 Destination { get => _destination = _destination != null ? _destination : GameObject.FindGameObjectWithTag(END_POINT_TAG).transform.position; set => _destination = value; }

    public UnityAction<List<Vector3>> OnNewPath;
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
        HasPath = false;
        Seeker.StartPath(start, destination, OnPathComplete);
    }

    public void OnPathComplete (Path newPath) 
    {  
        if (newPath.error) 
        {
            Debug.Log(newPath.errorLog);
            HasPath = false;
            OnPathFail?.Invoke();
            return;
        } 

        currentPath = newPath.vectorPath;
        IsNewPath = true;
        HasPath = true;
        OnNewPath?.Invoke(currentPath);
    }
}
