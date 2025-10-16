using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Blizzard : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _blizzard;

    [SerializeField] private List<BlizzardTargets> _allTargets;

    private int _setUpTargetsCounter = 0;
    private bool _verticalTarget;

    private List<BlizzardTargets> _currentTargets = new();
    private List<BlizzardTargets> _activeTargets = new();

    private SB_GlacialLord _glacialLord;

    private List<BlizzardTargets> DetermineTargets()
    {
        List<BlizzardTargets> newTargets = new();

        for (int i = 0; i < _allTargets.Count; i++)
        {
            if ((i % 2 == 0) == _verticalTarget)
                newTargets.Add(_allTargets[i]);
        }

        _verticalTarget = !_verticalTarget;

        return newTargets;
    }

    private void CreateTargetZone(Vector3 location)
    {
        GameObject newTargetZone = Instantiate(_targetZone, location, Quaternion.identity);
        _currentTargetZones.Add(newTargetZone);
    }

    #region Base Ability
    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
        _glacialLord = (SB_GlacialLord)_mySpecificBoss;

        _glacialLord.GetFrostFiendSpawnedEvent().AddListener(FrostFiendSpawned);
    }

    private void FrostFiendSpawned(GlacialLord_FrostFiend newFiend)
    {
        int currentTarget = _setUpTargetsCounter;

        _allTargets[currentTarget].AddFrostFiendToList(newFiend);

        currentTarget--;
        if (currentTarget < 0) currentTarget = 3;

        _allTargets[currentTarget].AddFrostFiendToList(newFiend);

        _setUpTargetsCounter++;
    }

    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();

        _currentTargets = DetermineTargets();

        foreach(BlizzardTargets targets in _currentTargets)
        {
            if (targets.AreAnyMinionsFrozen())
            {
                targets.CallFailedOnUnfrozenMinions();
                continue;
            }

            CreateTargetZone(targets.GetAttackLocation());

            targets.CallBlizzardAttackOnMinions();

            _activeTargets.Add(targets);
        }
    }

    protected override void AbilityStart()
    {
        base.AbilityStart();

        foreach (BlizzardTargets targets in _activeTargets)
        {
            if (targets.AreAnyMinionsFrozen()) continue;

            Instantiate(_blizzard, targets.GetAttackLocation(), Quaternion.identity);
        }

        _activeTargets.Clear();
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

    public GlacialLord_FrostFiend GetUnfrozenFiend()
    {
        foreach(GlacialLord_FrostFiend fiend in _associatedFiends)
        {
            if (!fiend.IsMinionFrozen())
                return fiend;
        }
        return null;
    }

    public void CallBlizzardAttackOnMinions()
    {
        foreach (GlacialLord_FrostFiend fiend in _associatedFiends)
        {
            fiend.BlizzardAttack();
        }
    }

    public void CallFailedOnUnfrozenMinions()
    {
        foreach (GlacialLord_FrostFiend fiend in _associatedFiends)
        {
            if(!fiend.IsMinionFrozen())
            {
                fiend.BlizzardFailed();
            }
        }
    }

    public void AddFrostFiendToList(GlacialLord_FrostFiend fiend)
    {
        _associatedFiends.Add(fiend);
    }
}