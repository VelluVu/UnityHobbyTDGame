using UnityEngine;
using Pathfinding;
using UnityEngine.Events;
using TheTD.Building;
using System.Collections.Generic;
using Pathfinding.Util;

[RequireComponent(typeof(Seeker))]
public class PathControl : MonoBehaviour, IEventListener
{
    private const string END_POINT_TAG = "EndPoint";
    public bool IsNewPath = true;
    [SerializeField] public int currentNodeIndex = 0;
    public List<Vector3> currentPath;
    [SerializeField] public Vector3 currentNode;
    [SerializeField] public Vector3 nextNode;

    //public Vector3 NextNodePosition { get => (Vector3)nextNode.position; }
    public bool HasPath { get => _interpolator.valid; }

    private Vector3 destinationYOffset = Vector3.zero;
    public Vector3 DestinationYOffset { set => destinationYOffset = value; }

    private Vector3 _destination;
    public Vector3 Destination { get => _destination = _destination != null ? _destination : GameObject.FindGameObjectWithTag(END_POINT_TAG).transform.position; set => _destination = value; }

    public UnityAction<List<Vector3>> OnNewPath;
    public UnityAction OnPathFail;

    private Seeker _seeker;
    public Seeker Seeker { get => _seeker = _seeker != null ? _seeker : GetComponent<Seeker>(); }

    protected PathInterpolator _interpolator = new PathInterpolator();

    public void FindPath(Vector3 start, Vector3 destination) 
    {
        Seeker.StartPath(start, destination, OnPathComplete);
    }

    public void OnPathComplete (Path newPath) 
    {    
        if (newPath.error) 
        {
            Debug.Log(newPath.errorLog);
            _interpolator.SetPath(null);      
            OnPathFail?.Invoke();
            return;
        } 

        currentPath = newPath.vectorPath;
        _interpolator.SetPath(currentPath);
        IsNewPath = true;
        OnNewPath?.Invoke(currentPath);
    }

    private void Start() 
    {
        AddListeners();
        FindPath(transform.position, Destination);
    }

    public void AddListeners()
    {
        BuildArea.OnBuild += OnBuildingPlaced;
        BuildArea.OnSell += OnBuildingRemoved;
    }

    public void RemoveListeners()
    {
        BuildArea.OnBuild -= OnBuildingPlaced;
        BuildArea.OnSell -= OnBuildingRemoved;
    }


    private void OnBuildingPlaced(Construction construction)
    {
        FindPath(transform.position, Destination);
    }

    private void OnBuildingRemoved(Construction construction)
    {
        FindPath(transform.position, Destination);
    }


    private void OnDestroy() 
    {
        RemoveListeners();
    }
}
