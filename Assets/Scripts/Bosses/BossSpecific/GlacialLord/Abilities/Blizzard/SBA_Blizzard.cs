using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Blizzard : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _blizzard;

    [SerializeField] private List<BlizzardTargets> _horizontalTargets;
    [SerializeField] private List<BlizzardTargets> _verticalTargets;

    private bool _targetDirectionVertical = false;

    private SB_GlacialLord _glacialLord;


    #region Base Ability
    public override void AbilitySetup(BossBase bossBase)
    {
        base.AbilitySetup(bossBase);
        _glacialLord = (SB_GlacialLord)_mySpecificBoss;
    }


    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();

        List<BlizzardTargets> currentTargets = DetermineTargetsForAttack();

        foreach(BlizzardTargets target in currentTargets)
        {
            if(target.AreAllMinionsNotFrozen())
            {
                CreateTargetZone(target.GetAttackLocation());
            }
        }

        
    }

    private List<BlizzardTargets> DetermineTargetsForAttack()
    {
        return _targetDirectionVertical ? _horizontalTargets : _verticalTargets;
    }

    private void CreateTargetZone(Vector3 location)
    {
        GameObject newTargetZone = Instantiate(_targetZone, location, Quaternion.identity) ;
        _currentTargetZones.Add(newTargetZone);
    }
    #endregion
}

public class BlizzardTargets
{
    [SerializeField] private Vector3 _attackLocation;
    [SerializeField] private List<GlacialLord_FrostFiend> _associatedFiends;

    public Vector3 GetAttackLocation() => _attackLocation;
    public List<GlacialLord_FrostFiend> GetAssociatedFiends() => _associatedFiends;

    public bool AreAllMinionsNotFrozen()
    {
        foreach (GlacialLord_FrostFiend fiend in _associatedFiends)
        {
            if (fiend.IsMinionFrozen()) return false;
        }
        return true;
    }
}