using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AchievementManager : MainUniversalManagerFramework
{
    public static AchievementManager Instance;

    [SerializeField] private AchievementSO[] _achievementsInGame;
    
    private UnityEvent<AchievementSO> _onAchievementUnlocked = new UnityEvent<AchievementSO>();


    public void UnlockAchievement(int id)
    {
        UnlockAchievement(_achievementsInGame[id]);
    }

    public void UnlockAchievement(AchievementSO achievement)
    {
        Debug.Log("Achievement Unlocked" + achievement.name);
        InvokeOnAchievementUnlocked(achievement);
    }
    
    #region BaseManager
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    #endregion
    
    #region Events

    public void InvokeOnAchievementUnlocked(AchievementSO achievementData)
    {
        _onAchievementUnlocked.Invoke(achievementData);
    }
    #endregion
    
    #region Getters
    public UnityEvent<AchievementSO> GetOnAchievementUnlocked() => _onAchievementUnlocked;
    #endregion
}