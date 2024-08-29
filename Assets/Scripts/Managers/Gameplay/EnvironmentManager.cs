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
        startPoint = new Vector3(startPoint.x, 0, startPoint.z);
        direction = new Vector3(direction.x, 0, direction.z);

        Debug.DrawRay(startPoint, direction, Color.blue, 3);
        if(Physics.Raycast(startPoint, direction, out RaycastHit rayHit, _distanceToEdgeOfMap, _mapBorderLayer))
            return rayHit.point;

        Debug.LogError("Couldn't find edge of map");
        return Vector3.zero;
    }
        
    #endregion

    #region Setters

    #endregion
}
