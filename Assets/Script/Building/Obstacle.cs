using System.Linq;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Mesh mesh;
    public Mesh Mesh { get => mesh = mesh != null ? mesh : GetComponent<MeshFilter>().sharedMesh; }

    public Vector3[] BoxCorners { get => GetBoxCorners(); }

    public delegate void ObstacleSpawnDelegate(Vector3[] bounds);
    public static event ObstacleSpawnDelegate OnPositionChange;

    public void InformPosition()
    {
        OnPositionChange?.Invoke(BoxCorners);
    }

    public Vector3[] GetBoxCorners()
    {
        var boundsInWorldPosition = new Vector3[8];
        var bottomLeftLowCorner = transform.position - ListHelperFunctions.MinVectorValueFromList(Mesh.vertices.ToList());
        var topRightHighCorner = transform.position + ListHelperFunctions.MaxVectorValueFromList(Mesh.vertices.ToList());
        var bottomRightLowCorner = new Vector3(topRightHighCorner.x, bottomLeftLowCorner.y, bottomLeftLowCorner.z);
        var topLeftLowCorner = new Vector3(bottomLeftLowCorner.x, bottomLeftLowCorner.y, topRightHighCorner.z);
        var topRightLowCorner = new Vector3(topRightHighCorner.x, bottomLeftLowCorner.y, topRightHighCorner.z);
        var bottomLeftHighCorner = new Vector3(bottomLeftLowCorner.x, topRightHighCorner.y, bottomLeftLowCorner.z);
        var bottomRightHighCorner = new Vector3(topRightHighCorner.x, topRightHighCorner.y, bottomLeftLowCorner.z);
        var topLeftHighCorner = new Vector3(bottomLeftLowCorner.x, topRightHighCorner.y, topRightHighCorner.z);   
        boundsInWorldPosition[0] = bottomLeftLowCorner;
        boundsInWorldPosition[1] = topLeftLowCorner;
        boundsInWorldPosition[2] = bottomRightLowCorner;
        boundsInWorldPosition[3] = topRightLowCorner;
        boundsInWorldPosition[4] = bottomLeftHighCorner;
        boundsInWorldPosition[5] = topLeftHighCorner;
        boundsInWorldPosition[6] = bottomRightHighCorner;
        boundsInWorldPosition[7] = topRightHighCorner;
        return boundsInWorldPosition;
    }
}
