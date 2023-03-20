using System;
using UnityEngine;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    const string BUILDING_HOLDER_IS_NULL_LOG_FORMAT = "Building holder is null, assign the reference in inspector to spawn buildings on the parent.";

    private Tower tower;
    public Tower Tower { get => tower; set => SetTower(value); }

    private BuildSpot buildSpot;
    public BuildSpot BuildSpot { get => buildSpot; private set => buildSpot = value; }

    private NavMeshObstacle navMeshObstacle;
    public NavMeshObstacle NavMeshObstacle { get => navMeshObstacle; private set => navMeshObstacle = value; }

    private GameObject navMeshObstaclePrefab;
    public GameObject NavMeshObstaclePrefab { get => navMeshObstaclePrefab = navMeshObstaclePrefab != null ? navMeshObstaclePrefab : Resources.Load<GameObject>("Prefabs/NavMeshObstacle"); }

    public void InitBuilding(BuildSpot buildSpot, Transform parent, TowerData towerData)
    {
        gameObject.name = "Building";
        BuildSpot = buildSpot;
        transform.position = BuildSpot.CenterPositionInWorld;
        NavMeshObstacle = Instantiate(NavMeshObstaclePrefab, BuildSpot.CenterPositionInWorld + Vector3.up * 0.5f, Quaternion.identity).GetComponent<NavMeshObstacle>();        
        NavMeshObstacle.transform.SetParent(transform);
        Tower = towerData.Tower;  
        if (!parent) Debug.LogFormat(BUILDING_HOLDER_IS_NULL_LOG_FORMAT);
        else transform.SetParent(parent);
    }

    private void SetTower(Tower value)
    {
        if (tower == value) return;
        tower = value;
        Tower.BuildTower(transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(1f, 1f, 1f));
    }
}
