using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbilityCooldownMissionModifier : MissionModifierBase
{
    [SerializeField] private float _basicAbilityCooldownRateMultiplier = 1;
    [SerializeField] private float _manualAbilityCooldownRateMultiplier = 1;

    public override void AdjustHeroStatsModifier(HeroStats heroStats)
    {
        base.AdjustHeroStatsModifier(heroStats);
        heroStats.ChangeCurrentBasicAbilityCooldownRate(_basicAbilityCooldownRateMultiplier);
        heroStats.ChangeCurrentManualAbilityCooldownRate(_manualAbilityCooldownRateMultiplier);
    }
}
