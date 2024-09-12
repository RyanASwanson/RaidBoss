using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Frostbite : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _frostBite;

    private SB_GlacialLord _glacialLord;
    private BossManager _bossManager;

    public override void AbilitySetup(BossBase bossBase)
    {
        base.AbilitySetup(bossBase);
        _glacialLord = (SB_GlacialLord)_mySpecificBoss;
        _bossManager = GameplayManagers.Instance.GetBossManager();
    }

    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();
        foreach(GlacialLord_FrostFiend frostFiend in _glacialLord.GetAllFrostFiends())
        {
            if (frostFiend.IsMinionFrozen()) continue;

            GameObject newTargetZone = Instantiate(_targetZone, frostFiend.transform.position,
                Quaternion.Euler(_bossManager.GetDirectionToBoss(transform.position)));

            _currentTargetZones.Add(newTargetZone);
        }    
    }

    protected override void AbilityStart()
    {
        foreach(GameObject targetZone in _currentTargetZones)
        {
            //Spawns the magma wave damage zone
            GameObject newFrostbite = Instantiate(_frostBite, targetZone.transform.position, targetZone.transform.rotation);
            //Sets up the projectile
            /*SBP_Avalanche avalanche = _storedAvalanche.GetComponent<SBP_Avalanche>();
            avalanche.SetUpProjectile(_myBossBase);
            avalanche.AdditionalSetup(_storedTarget.transform.position);*/
        }
        

        base.AbilityStart();
    }

}
