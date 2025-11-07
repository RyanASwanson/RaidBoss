using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Performs general management of the environment and provides accessibility to other scripts to certain functionality
/// </summary>
public class EnvironmentManager : MainGameplayManagerFramework
{
    public static EnvironmentManager Instance;
    
    [SerializeField] private float _mapRadius;
    
    [Tooltip("The offset that is generally used for the height of projectiles")]
    [SerializeField] private Vector3 _floorOffsetHeight;

    [SerializeField] private LayerMask _mapBorderLayer;

    [SerializeField] private List<GameObject> _heroSpawnLocations;

    [SerializeField] private Collider _floorCollider;

    [Tooltip("The max distance that is checked to find the edge of the map")]
    private const float _distanceToEdgeOfMap = 25;

    [Space]
    [SerializeField] private float _battleWonEffectDuration;
    
    [SerializeField] private GameObject _battleWonEffect;

    private void BattleWon()
    {
        StartCoroutine(BattleWonEffect());
    }

    private IEnumerator BattleWonEffect()
    {
        _battleWonEffect.SetActive(true);
        
        yield return new WaitForSeconds(_battleWonEffectDuration);
        
        _battleWonEffect.SetActive(false);
    }

    #region BaseManager
    /// <summary>
    /// Establishes the Instance for the Environment manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    protected override void SubscribeToEvents()
    {
        //GameStateManager.Instance.GetBattleWonEvent().AddListener(BattleWon);
    }
    #endregion

    #region Getters
    public float GetMapRadius() => _mapRadius;
    public Vector3 GetFloorOffset() => _floorOffsetHeight;
    public LayerMask GetMapBorderLayer() => _mapBorderLayer;

    public List<GameObject> GetHeroSpawnLocations() => _heroSpawnLocations;

    #region Raycasts
    public Vector3 GetClosestPointToFloor(Vector3 startPoint) => Physics.ClosestPoint(startPoint, 
        _floorCollider, _floorCollider.gameObject.transform.position, _floorCollider.gameObject.transform.rotation);

    /// <summary>
    /// Raycasts with a specific layer, aligning the y values of start and direction
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <param name="layer"></param>
    /// <param name="raycastHit"></param>
    /// <returns></returns>
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
    /// <param name="raycastHit"></param>
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

    #endregion
    #endregion

    #region Setters

    #endregion
}
