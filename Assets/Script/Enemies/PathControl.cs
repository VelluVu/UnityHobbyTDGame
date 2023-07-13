using UnityEngine;
using Pathfinding;
using System;
using UnityEngine.Events;

public class PathControl : MonoBehaviour
{
    private Path _currentPath;

    public bool HasPath { get => _currentPath != null; }

    public UnityAction<Path> OnNewPath;
    public UnityAction OnPathFail;

    private Seeker _seeker;
    public Seeker Seeker { get => _seeker = _seeker != null ? _seeker : GetComponent<Seeker>(); }
    
    public void FindPath(Vector3 destination, Vector3 start ) 
    {
        Seeker.StartPath(start, destination, OnPathComplete);
    }

    public void OnPathComplete (Path newPath) 
    {  
        if (newPath.error) 
        {
            Debug.Log(newPath.errorLog);
            _currentPath = null;
            OnPathFail?.Invoke();
            return;
        } 
       
        _currentPath = newPath;
        OnNewPath?.Invoke(_currentPath);
    }
}
