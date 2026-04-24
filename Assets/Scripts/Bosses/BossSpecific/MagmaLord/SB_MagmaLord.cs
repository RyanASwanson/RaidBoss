using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Magma Lord boss
/// </summary>
public class SB_MagmaLord : SpecificBossFramework
{
    public static SB_MagmaLord Instance;

    [Space] 
    [SerializeField] private SBA_Volcano _volcano;
    
    [Space] 
    [SerializeField] private GameObject _deathEffect;

    private bool _hasVolcanoBeenUsed = false;
    
    #region BaseBoss
    protected override void CreateSpecificBossInstance()
    {
        Instance = this;
    }

    protected override void StartFight()
    {
        base.StartFight();
        _volcano.BattleStarted();
    }
    
    protected override void BossDied()
    {
        base.BossDied();
        Instantiate(_deathEffect, transform);
    }

    protected override void CheckToUnlockSpecialistAchievement()
    {
        base.CheckToUnlockSpecialistAchievement();
        
        if (SelectionManager.Instance.GetSelectedDifficulty() < EGameDifficulty.Mythic)
        {
            return;
        }
        
        if (!_hasVolcanoBeenUsed)
        {
            UnlockedSpecialistAchievement();
        }
    }
    
    #endregion

    #region Setters

    public void SetHasVolcanoBeenUsed(bool hasVolcanoBeenUsed) => _hasVolcanoBeenUsed = hasVolcanoBeenUsed;

    #endregion
}
