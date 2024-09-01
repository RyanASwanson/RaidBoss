using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : BaseGameplayManager
{
    [SerializeField] private float _mapRadius;

    [SerializeField] private LayerMask _mapBorderLayer;
    [SerializeField] private LayerMask _bossLayer;

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

    #region Raycasts
    public Vector3 GetClosestPointToFloor(Vector3 startPoint) => Physics.ClosestPoint(startPoint, 
        _floorCollider, _floorCollider.gameObject.transform.position, _floorCollider.gameObject.transform.rotation);


    public bool GetLayerRayHit(Vector3 startPoint, Vector3 direction, float distance, LayerMask layer, out RaycastHit raycastHit)
    {
        startPoint = new Vector3(startPoint.x, 0, startPoint.z);
        direction = new Vector3(direction.x, 0, direction.z);

        return (Physics.Raycast(startPoint, direction, out raycastHit, distance, layer));
    }

    public Vector3 GetEdgeOfMapLoc(Vector3 startPoint, Vector3 direction)
    {
        GetEdgeOfMapWithDistanceAndDirection(startPoint, direction, _distanceToEdgeOfMap, out RaycastHit raycastHit);
        return raycastHit.point;
    }

    public Vector3 GetEdgeOfMapLoc(Vector3 startPoint, Vector3 direction, float distance)
    {
        GetEdgeOfMapWithDistanceAndDirection(startPoint, direction, distance, out RaycastHit raycastHit);
        return raycastHit.point;
    }

    /// <summary>
    /// Finds the closest point to the edge of the map
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool GetEdgeOfMapWithDirection(Vector3 startPoint, Vector3 direction, out RaycastHit raycastHit)
    {
        return GetEdgeOfMapWithDistanceAndDirection(startPoint, direction, _distanceToEdgeOfMap, out raycastHit);
    }

    public bool GetEdgeOfMapWithDistanceAndDirection(Vector3 startPoint, Vector3 direction, float distance, out RaycastHit raycastHit)
    {
        return GetLayerRayHit(startPoint, direction, distance, _mapBorderLayer, out raycastHit);
        
    }
    

    public float DistanceToEdgeOfMap(Vector3 startPoint, Vector3 direction)
    {
        RaycastHit rayHit;
        GetEdgeOfMapWithDirection(startPoint, direction, out rayHit);
        return Vector3.Distance(startPoint, rayHit.point) ;
    }

    public Vector3 GetBossWithDistanceAndDirection(Vector3 startPoint, Vector3 direction, float distance)
    {
        return Vector3.zero;
    }

        #endregion
    #endregion

    #region Setters

    #endregion
}
