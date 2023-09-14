using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickPosition : MonoBehaviour
{
    public float maxClickDistance = 1000f;
    [SerializeField]private LayerMask hitMask;

    private Camera playerViewCamera;
    public Camera PlayerViewCamera { get => playerViewCamera = playerViewCamera != null ? playerViewCamera : Camera.main; }

    public delegate void ClickPositionDelegate(Vector3 clickPosition);
    public static event ClickPositionDelegate OnClickPosition;
       
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) return;
            Vector3 clickPosition = GetClickPositionWithPhysicsRaycast(PlayerViewCamera, maxClickDistance, hitMask);
            OnClickPosition?.Invoke(clickPosition);
        }
    }

    private static Vector3 GetClickPositionWithPhysicsRaycast(Camera camera, float maxHitDistance, LayerMask hitMask)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; 
        var isHit = Physics.Raycast(ray, out hit, maxHitDistance, hitMask.value);
        if(isHit)
        {
            return hit.point;
        }
        return Vector3.negativeInfinity;
    }

    private static Vector3 GetClickPositionWithPlane()
    {
        Vector3 clickPosition = Vector3.negativeInfinity;

        Plane plane = new Plane(Vector3.up, 0f);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float distanceToPlane;

        if (plane.Raycast(ray, out distanceToPlane))
        {
            clickPosition = ray.GetPoint(distanceToPlane);           
        }

        return clickPosition;
    }

    public static bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        foreach (RaycastResult raysastResult in raysastResults)
        {
            if (raysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return true;
            }
        }
        return false;
    }
}
