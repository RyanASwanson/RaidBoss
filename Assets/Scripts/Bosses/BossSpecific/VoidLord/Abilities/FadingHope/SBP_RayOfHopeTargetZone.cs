using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_RayOfHopeTargetZone : BossProjectileFramework
{
    [SerializeField] private float _rayHeight;
    [SerializeField] private float _minimumRayDistance;
    [SerializeField] private float _maximumRayDistance;
    
    [Space]
    [SerializeField] private BossTargetZoneParent _targetZoneParent;
    private Vector3 _rayPosition;
    private Vector3 _bossPosition;
    private float _rayDistance;
    
    [Space]
    [SerializeField] private GameObject _rayOfHope;
    
    public void SpawnRayOnDelay(float spawnDelay)
    {
        StartCoroutine(SpawnRayDelay(spawnDelay));
    }

    private IEnumerator SpawnRayDelay(float spawnDelay)
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnRayOfHope();
    }

    public void SpawnRayOfHope()
    {
        _targetZoneParent.RemoveBossTargetZones();
        
        Instantiate(_rayOfHope,transform.position,transform.rotation)
            .GetComponent<SBP_RayOfHope>().SetUpProjectile(_myBossBase,_abilityID);
    }

    public void SetRayPosition(Vector3 rayPosition)
    {
        if (Mathf.Approximately(rayPosition.x, transform.position.x) && Mathf.Approximately(rayPosition.z, transform.position.z))
        {
            return;
        }
        
        _rayPosition.Set(rayPosition.x, 0, rayPosition.z);

        // Handles if the Ray is at the max distance
        rayPosition = Quaternion.AngleAxis(-45, Vector3.up) * rayPosition;
        rayPosition.Set(Mathf.Clamp(rayPosition.x,-_maximumRayDistance,_maximumRayDistance),0,
            Mathf.Clamp(rayPosition.z,-_maximumRayDistance,_maximumRayDistance));
        rayPosition = Quaternion.AngleAxis(45, Vector3.up) * rayPosition;
        
        if (Mathf.Abs(_rayPosition.x) > Mathf.Abs(rayPosition.x))
        {
            _rayPosition.Set(rayPosition.x,_rayPosition.y,_rayPosition.z);
        }
        if (Mathf.Abs(_rayPosition.z) > Mathf.Abs(rayPosition.z))
        {
            _rayPosition.Set(_rayPosition.x,_rayPosition.y,rayPosition.z);
        }
        // End of handling if the Ray is at max distance
        
        // Handles if the Ray is too close to the Boss
        _bossPosition = BossBase.Instance.transform.position;
        _bossPosition.y = 0;
        
        _rayDistance = Vector3.Distance(_rayPosition,_bossPosition);
        if (_rayDistance < _minimumRayDistance)
        {
            _rayPosition = _rayPosition.normalized * _minimumRayDistance;
        }
        // End of handling if Ray is too close to Boss
        
        _rayPosition.Set(_rayPosition.x, _rayHeight, _rayPosition.z);
        
        transform.position = _rayPosition;
    }

    public void RemoveRay()
    {
        _targetZoneParent.RemoveBossTargetZones();
    }
    
    #region BaseProjectile
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        SetRayPosition(transform.position);
    }
    #endregion
}
