using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : BaseGameplayManager
{
    [SerializeField] private float _mapRadius;

    [SerializeField] private LayerMask _mapBorderLayer;

    [SerializeField] private List<GameObject> _heroSpawnLocations;

    [SerializeField] private Collider _floorCollider;

    private const float _distanceToEdgeOfMap = 25;

    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
    }


    #region Getters
    public float GetMapRadius() => _mapRadius;
    public LayerMask GetMapBorderLayer() => _mapBorderLayer;

    public List<GameObject> GetSpawnLocations() => _heroSpawnLocations;

    public Vector3 GetClosestPointToFloor(Vector3 startPoint) => Physics.ClosestPoint(startPoint, 
        _floorCollider, _floorCollider.gameObject.transform.position, _floorCollider.gameObject.transform.rotation);

    /// <summary>
    /// Finds the closest point to the edge of the map
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Vector3 GetEdgeOfMapWithDirection(Vector3 startPoint, Vector3 direction)
    {
        return GetEdgeOfMapWithDistanceAndDirection(startPoint, direction, _distanceToEdgeOfMap);
    }

    public Vector3 GetEdgeOfMapWithDistanceAndDirection(Vector3 startPoint, Vector3 direction, float distance)
    {
        RaycastHit rayHit;
        if(GetEdgeOfMapRayHit(startPoint, direction, distance, out rayHit))
            return rayHit.point;
        

        Debug.LogError("Couldn't find map edge");

        return Vector3.zero;
        
    }

    public bool GetEdgeOfMapRayHit(Vector3 startPoint, Vector3 direction, float distance, out RaycastHit raycastHit)
    {
        startPoint = new Vector3(startPoint.x, 0, startPoint.z);
        direction = new Vector3(direction.x, 0, direction.z);

        //Debug.DrawRay(startPoint, direction, Color.blue, 3);

        return (Physics.Raycast(startPoint, direction, out raycastHit, distance, _mapBorderLayer));
        /*if (Physics.Raycast(startPoint, direction, out raycastHit, distance, _mapBorderLayer))
            return rayHit;

        return new RaycastHit();*/
    }

    public float DistanceToEdgeOfMap(Vector3 startPoint, Vector3 direction)
    {
        return Vector3.Distance(startPoint, GetEdgeOfMapWithDirection(startPoint, direction));
    }
        
    #endregion

    #region Setters

    #endregion
}
