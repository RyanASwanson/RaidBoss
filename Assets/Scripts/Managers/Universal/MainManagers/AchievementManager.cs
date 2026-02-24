using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AchievementManager : MainUniversalManagerFramework
{
    public static AchievementManager Instance;

    [SerializeField] private AchievementData[] _achievementsInGame;
    
    private UnityEvent<AchievementData> _onAchievementUnlocked = new UnityEvent<AchievementData>();


    public void UnlockAchievement(int id)
    {
        UnlockAchievement(_achievementsInGame[id]);
    }

    public void UnlockAchievement(AchievementData achievement)
    {
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

    public void InvokeOnAchievementUnlocked(AchievementData achievementData)
    {
        _onAchievementUnlocked.Invoke(achievementData);
    }
    #endregion
    
    #region Getters
    public UnityEvent<AchievementData> GetOnAchievementUnlocked() => _onAchievementUnlocked;
    #endregion
}

[System.Serializable]
public class AchievementData
{
    public string AchievmentName;
    public string AchievementUnlockName;
    public int AchievmentID;
    [TextArea(1, 2)] public string AchievementDescription;
    
}
