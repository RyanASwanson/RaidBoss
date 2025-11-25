using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_Blizzard : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _blizzard;

    [Space] 
    [SerializeField] private GameObject _previewGlow;

    [Space]
    [SerializeField] private List<BlizzardTargets> _allTargets;

    private int _setUpTargetsCounter = 0;
    private bool _verticalTarget;

    private List<BlizzardTargets> _currentTargets = new();
    private List<BlizzardTargets> _activeTargets = new();
    
    private List<BlizzardPreviewGlow> _currentPreviewZones = new();

    private SB_GlacialLord _glacialLord;

    private void DetermineTargets(bool swapTargets)
    {
        List<BlizzardTargets> newTargets = new();

        for (int i = 0; i < _allTargets.Count; i++)
        {
            if ((i % 2 == 0) == _verticalTarget)
            {
                newTargets.Add(_allTargets[i]);
            }
        }

        if (swapTargets)
        {
            _verticalTarget = !_verticalTarget;
        }

        _currentTargets = newTargets;
    }
    
    

    private void CreateTargetZone(Vector3 location, bool isDeactivated, BlizzardTargets targets)
    {
        BossTargetZoneParent newTargetZone = Instantiate(_targetZone, location, Quaternion.identity).GetComponent<BossTargetZoneParent>();

        foreach (GlacialLord_FrostFiend fiend in targets.GetAssociatedFiends())
        {
            fiend.SetCurrentTargetZone(newTargetZone);
        }

        if (isDeactivated)
        {
            newTargetZone.SetTargetZoneDeactivatedStatesOfAllTargetZones(true);
        }
        
        _currentTargetZones.Add(newTargetZone);
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
    
    #region PreviewZones

    private void CreateInitialPreviewZones()
    {
        DetermineTargets(true);

        for (int i = 0; i < _currentTargets.Count; i++)
        {
            CreatePreviewZone();
        }

        SetPreviewZoneLocations();
    }

    private void SetPreviewZoneLocations()
    {
        for (int i = 0; i < _currentTargets.Count; i++)
        {
            _currentPreviewZones[i].MovePreviewGlowToTargetLocation(_currentTargets[i].GetAttackLocation());
        }
    }

    private void CreatePreviewZone()
    {
        _currentPreviewZones.Add(Instantiate(_previewGlow, transform.position, Quaternion.identity).GetComponent<BlizzardPreviewGlow>());
    }
    
    #endregion

    #region Base Ability
    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
        _glacialLord = (SB_GlacialLord)_mySpecificBoss;

        _glacialLord.GetFrostFiendSpawnedEvent().AddListener(FrostFiendSpawned);
        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(CreateInitialPreviewZones);
    }
    

    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();

        foreach(BlizzardTargets targets in _currentTargets)
        {
            if (targets.AreAnyMinionsFrozen())
            {
                CreateTargetZone(targets.GetAttackLocation(),true, targets);
                targets.CallFailedOnUnfrozenMinions();
                continue;
            }
            
            CreateTargetZone(targets.GetAttackLocation(),false, targets);
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

    protected override void AbilityDurationEnded()
    {
        DetermineTargets(true);
        SetPreviewZoneLocations();
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

    /*public void CallBlizzardAttackOnMinions()
    {
        foreach (GlacialLord_FrostFiend fiend in _associatedFiends)
        {
            if (!fiend.IsMinionFrozen())
            {
                fiend.BlizzardAttack(_attackLocation);
            }
            else
            {
                fiend.BlizzardFailed();
            }
            
        }
    }*/
    public void CallBlizzardAttackOnMinions()
    {
        foreach (GlacialLord_FrostFiend fiend in _associatedFiends)
        {
            fiend.BlizzardAttack();
            fiend.PlayBlizzardMinionEffect(_attackLocation);
        }
    }

    public void CallFailedOnUnfrozenMinions()
    {
        foreach (GlacialLord_FrostFiend fiend in _associatedFiends)
        {
            if(!fiend.IsMinionFrozen())
            {
                fiend.BlizzardFailed();
                fiend.PlayBlizzardMinionEffect(_attackLocation);
            }
        }
    }

    public void AddFrostFiendToList(GlacialLord_FrostFiend fiend)
    {
        _associatedFiends.Add(fiend);
    }
}