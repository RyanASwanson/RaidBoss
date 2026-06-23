using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_VoidLord : SpecificBossFramework
{
    public static SB_VoidLord Instance;
    
    #region BaseBoss
    protected override void CreateSpecificBossInstance()
    {
        Instance = this;
    }

    protected override void StartFight()
    {
        base.StartFight();
    }
    
    protected override void BossDied()
    {
        base.BossDied();
    }

    protected override void CheckToUnlockSpecialistAchievement()
    {
        base.CheckToUnlockSpecialistAchievement();
        
        if (SelectionManager.Instance.GetSelectedDifficulty() < EGameDifficulty.Mythic)
        {
            return;
        }
        
        /*if ()
        {
            UnlockedSpecialistAchievement();
        }*/
    }
    
    #endregion
}
