using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_DespairHex : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _despairHex;
    
    private BossTargetZoneParent _newestTargetZone;
    
    private List<HeroBase> _hexedHeroes = new List<HeroBase>();
    
    #region Base Ability

    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
    }

    protected override void StartShowTargetZone()
    {
        //Spawns the target area
        _newestTargetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        //Adds the target area to the list of target areas
        _currentTargetZones.Add(_newestTargetZone);

        _newestTargetZone.GetComponent<FollowObject>().StartFollowingObject(_storedTarget.gameObject);
        
        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        SBP_DespairHex hex = Instantiate(_despairHex, _storedTargetLocation, Quaternion.identity).GetComponent<SBP_DespairHex>();
        hex.SetUpProjectile(_myBossBase, _abilityID);
        hex.AdditionalSetUp(_storedTarget);
        base.AbilityStart();
    }
    
    protected override void AbilityDurationEnded()
    {
        base.AbilityDurationEnded();
    }

    public override void StopBossAbility()
    {
        base.StopBossAbility();
    }
    #endregion
}
