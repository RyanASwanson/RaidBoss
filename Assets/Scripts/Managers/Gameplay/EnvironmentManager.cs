using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : BaseGameplayManager
{
    [SerializeField] private List<GameObject> _heroSpawnLocations;

    [SerializeField] private Collider _floorCollider;

    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
    }

    #region Events
    public override void SubscribeToEvents()
    {
        
    }
    #endregion

    #region Getters
    public List<GameObject> GetSpawnLocations() => _heroSpawnLocations;

    public Vector3 GetClosestPointToFloor(Vector3 startPoint) => Physics.ClosestPoint(startPoint, 
        _floorCollider, _floorCollider.gameObject.transform.position, _floorCollider.gameObject.transform.rotation);
    #endregion

    #region Setters

    #endregion
}
