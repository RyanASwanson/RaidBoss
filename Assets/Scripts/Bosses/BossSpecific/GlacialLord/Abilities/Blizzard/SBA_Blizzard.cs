using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Blizzard : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _blizzard;

    [SerializeField] private List<BlizzardTargets> _verticalTargets;
    [SerializeField] private List<BlizzardTargets> _horizontalTargets;

    private bool _targetDirectionVertical = false;

    private SB_GlacialLord _glacialLord;


    #region Base Ability
    public override void AbilitySetup(BossBase bossBase)
    {
        base.AbilitySetup(bossBase);
        _glacialLord = (SB_GlacialLord)_mySpecificBoss;
        SetupTargets();
    }

    private void SetupTargets()
    {
        List<GlacialLord_FrostFiend> allFiends = _glacialLord.GetAllFrostFiends();

        List<BlizzardTargets> allTargets = new();

        foreach(BlizzardTargets target in _verticalTargets)
        {
            allTargets.Add(target);
        }
        foreach(BlizzardTargets target in _horizontalTargets)
        {
            allTargets.Add(target);
        }

        for(int i = 0; i < allTargets.Count; i++)
        {
            if (i % 2 == 0)
                _verticalTargets[i].AddFrostFiendToList(allFiends[i]);
            else
                _horizontalTargets[i].AddFrostFiendToList(allFiends[i]);
        }
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
        return _targetDirectionVertical ? _verticalTargets : _verticalTargets;
    }

    private void CreateTargetZone(Vector3 location)
    {
        GameObject newTargetZone = Instantiate(_targetZone, location, Quaternion.identity) ;
        _currentTargetZones.Add(newTargetZone);
    }
    #endregion
}

[System.Serializable]
public class BlizzardTargets
{
    [SerializeField] private Vector3 _attackLocation;
    private List<GlacialLord_FrostFiend> _associatedFiends;

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

    public void AddFrostFiendToList(GlacialLord_FrostFiend fiend)
    {
        _associatedFiends.Add(fiend);
    }
}