using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SCRIPT INTENDED FOR DEBUGGING PURPOSES ONLY
/// </summary>
public class HeroStatTrackingDisplay : MonoBehaviour
{
    #if UNITY_EDITOR || DEVELOPMENT_BUILD

    [SerializeField] private TextWithBackground _battleTimer;

    [SerializeField] private HeroStatTrackingCategory _damageCategory;
    [SerializeField] private HeroStatTrackingCategory _staggerCategory;
    [SerializeField] private HeroStatTrackingCategory _healingCategory;

    private void Start()
    {
        PerformInitialStatSetUp();
    }
    
    private void PerformInitialStatSetUp()
    {
        _damageCategory.SetUpStats();
        _staggerCategory.SetUpStats();
        _healingCategory.SetUpStats();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBattleTimer();
        
        UpdateHeroDamageStats();
        UpdateHeroStaggerStats();
        UpdateHeroHealingStats();
    }

    private void UpdateBattleTimer()
    {
        _battleTimer.UpdateText(GameStateManager.Instance.GetBattleDuration().ToString("F1"));
    }

    private void UpdateHeroDamageStats()
    {
        _damageCategory.UpdateDamageDealtStatCategory();
    }

    public void UpdateHeroStaggerStats()
    {
        _staggerCategory.UpdateStaggerDealtStatCategory();
    }

    public void UpdateHeroHealingStats()
    {
        _healingCategory.UpdateHealingDealtStatCategory();
    }
    #endif
}
