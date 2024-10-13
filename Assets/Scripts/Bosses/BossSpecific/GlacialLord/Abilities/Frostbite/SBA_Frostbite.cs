using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Frostbite : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _frostBite;

    private List<GlacialLord_FrostFiend> _attackingFiends = new();

    private SB_GlacialLord _glacialLord;

    public override void AbilitySetup(BossBase bossBase)
    {
        base.AbilitySetup(bossBase);
        _glacialLord = (SB_GlacialLord)_mySpecificBoss;
    }

    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();
        foreach(GlacialLord_FrostFiend frostFiend in _glacialLord.GetAllFrostFiends())
        {
            if (frostFiend.IsMinionFrozen()) continue;

            GameObject newTargetZone = Instantiate(_targetZone, frostFiend.transform.position,Quaternion.identity);

            newTargetZone.transform.position = new Vector3(newTargetZone.transform.position.x,
                _specificAreaTarget.y, newTargetZone.transform.position.z);

            newTargetZone.transform.LookAt(_glacialLord.transform);
            newTargetZone.transform.eulerAngles = new Vector3(0, newTargetZone.transform.eulerAngles.y, 0);

            _currentTargetZones.Add(newTargetZone);

            _attackingFiends.Add(frostFiend);

            frostFiend.FrostbiteAttack();
        }    
    }

    protected override void AbilityStart()
    {
        foreach (GlacialLord_FrostFiend frostFiend in _attackingFiends)
        {
            if (frostFiend.IsMinionFrozen()) continue;

            Vector3 spawnLoc = new Vector3(frostFiend.transform.position.x, _specificAreaTarget.y, frostFiend.transform.position.z);

            GameObject newFrostbite = Instantiate(_frostBite, spawnLoc, Quaternion.identity);
            SBP_Frostbite frostbiteFunc = newFrostbite.GetComponent<SBP_Frostbite>();
            frostbiteFunc.SetUpProjectile(_myBossBase);

            newFrostbite.transform.LookAt(_glacialLord.transform);
            newFrostbite.transform.eulerAngles = new Vector3(0, newFrostbite.transform.eulerAngles.y, 0);
        }

        base.AbilityStart();

        _attackingFiends.Clear();
    }
}
