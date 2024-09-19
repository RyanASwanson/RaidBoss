using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Blizzard : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _blizzard;

    [SerializeField] private List<BlizzardTargets> _allTargets;
    private int _setupTargetsCounter = 0;
    private bool _verticalTarget;

    private SB_GlacialLord _glacialLord;



    #region Base Ability
    public override void AbilitySetup(BossBase bossBase)
    {
        base.AbilitySetup(bossBase);
        _glacialLord = (SB_GlacialLord)_mySpecificBoss;

        _glacialLord.GetFrostFiendSpawnedEvent().AddListener(FrostFiendSpawned);
    }

    private void FrostFiendSpawned(GlacialLord_FrostFiend newFiend)
    {
        int currentTarget = _setupTargetsCounter;

        _allTargets[currentTarget].AddFrostFiendToList(newFiend);

        currentTarget--;
        if (currentTarget < 0) currentTarget = 3;

        _allTargets[currentTarget].AddFrostFiendToList(newFiend);

        _setupTargetsCounter++;
    }


    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();

        foreach(BlizzardTargets targets in DetermineTargets())
        {
            if (targets.AreAnyMinionsFrozen()) continue;

            CreateTargetZone(targets.GetAttackLocation());
        }
        
    }

    private List<BlizzardTargets> DetermineTargets()
    {
        List<BlizzardTargets> newTargets = new();

        for(int i = 0; i < _allTargets.Count; i++)
        {
            if ((i % 2 == 0) == _verticalTarget)
                newTargets.Add(_allTargets[i]);
        }

        _verticalTarget = !_verticalTarget;

        return newTargets;
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
    private List<GlacialLord_FrostFiend> _associatedFiends = new();

    public Vector3 GetAttackLocation() => _attackLocation;
    public List<GlacialLord_FrostFiend> GetAssociatedFiends() => _associatedFiends;

    public bool AreAnyMinionsFrozen()
    {
        foreach (GlacialLord_FrostFiend fiend in _associatedFiends)
        {
            if (fiend.IsMinionFrozen()) return true;
        }
        return false;
    }

    public void AddFrostFiendToList(GlacialLord_FrostFiend fiend)
    {
        _associatedFiends.Add(fiend);
    }
}