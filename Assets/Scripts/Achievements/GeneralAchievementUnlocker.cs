using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAchievementUnlocker : MonoBehaviour
{
    [SerializeField] private AchievementSO _achievementSO;

    public void UnlockAchievement()
    {
        AchievementManager.Instance.UnlockAchievement(_achievementSO);
    }
}
