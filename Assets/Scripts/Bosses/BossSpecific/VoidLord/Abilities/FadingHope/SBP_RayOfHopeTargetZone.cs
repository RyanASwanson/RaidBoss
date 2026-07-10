using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_RayOfHopeTargetZone : BossProjectileFramework
{
    [SerializeField] private BossTargetZoneParent _targetZoneParent;
    
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
    
    #region BaseProjectile
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
    }
    #endregion
}
