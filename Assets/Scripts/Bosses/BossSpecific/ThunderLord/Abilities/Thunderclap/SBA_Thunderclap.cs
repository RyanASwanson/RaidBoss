using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Thunderclap : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _thunderclapSafeZone;
    [SerializeField] private GameObject _thunderclapTargetZone;
    
    private BossTargetZoneParent _currentDamageTargetZone;
    
    private Queue<BossTargetZoneParent> _storedSafeZones = new Queue<BossTargetZoneParent>();
    
    /// <summary>
    /// Checks if at least 1 hero is in the safe zone area
    /// </summary>
    /// <returns></returns>
    private bool IsHeroInSafeZone()
    {
        return _storedSafeZones.Dequeue().GetBossTargetZones()[0].DoesZoneContainHero();
    }
    
    /// <summary>
    /// If there is a hero in the safe zone the ability fails and doesn't deal damage
    /// </summary>
    private void AbilityFailed()
    {
        /*Instantiate(_failedVFX, _specificAreaTarget, Quaternion.identity);
        PlayFailedAudio();*/
    }
    
    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();

        float safeZoneRotationalLocation = SB_ThunderLord.Instance.GetImpendingStorm().GetOppositeAttackRotation();
        
        safeZoneRotationalLocation = Mathf.Round(safeZoneRotationalLocation / 90f) * 90f;

        Vector3 safeZoneLocation = Quaternion.Euler(0,safeZoneRotationalLocation,0) * Vector3.forward * EnvironmentManager.Instance.GetMapRadius();
        safeZoneLocation.Set(safeZoneLocation.x,_specificAreaTarget.y,safeZoneLocation.z);
        
        BossTargetZoneParent newSafeZone = Instantiate(_thunderclapSafeZone, safeZoneLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        
        newSafeZone.transform.LookAt(SB_ThunderLord.Instance.transform);
        newSafeZone.transform.eulerAngles = new Vector3(0, newSafeZone.transform.eulerAngles.y, 0);
        
        _currentDamageTargetZone = Instantiate(_thunderclapTargetZone, _storedTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        
        newSafeZone.GetBossTargetZones()[0].GetOnTargetZoneSetToHeroInRange().AddListener(SetStateOfCurrentTargetZonesToDeactivated);
        newSafeZone.GetBossTargetZones()[0].GetOnTargetZoneSetToNoHeroInRange().AddListener(SetStateOfCurrentTargetZonesToActivated);
        
        _storedSafeZones.Enqueue(newSafeZone);
        
        _currentTargetZones.Add(newSafeZone);
        _currentTargetZones.Add(_currentDamageTargetZone);
        
        SB_ThunderLord.Instance.ChildGameObjectToImpendingStorm(_currentDamageTargetZone.gameObject);
    }
    
    protected override void SetStateOfCurrentTargetZonesToDeactivated()
    {
        _currentDamageTargetZone.SetTargetZoneDeactivatedStatesOfAllTargetZones(true);
    }

    protected override void SetStateOfCurrentTargetZonesToActivated()
    {
        _currentDamageTargetZone.SetTargetZoneDeactivatedStatesOfAllTargetZones(false);
    }

    protected override void AbilityStart()
    {
        //Checks if at least 1 hero is in the safe zone
        if (IsHeroInSafeZone())
        {
            //If there is, cancel the ability
            AbilityFailed();
            return;
        }

        //Spawn the damage zone
        //Instantiate(_lavaBlast, _specificAreaTarget, Quaternion.identity);

        base.AbilityStart();
    }

}
