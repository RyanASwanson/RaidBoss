using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanguineWrathMissionModifier : MissionModifierBase
{
    [SerializeField] private float _healingModifier;
    [SerializeField] private float _healingDamageMultiplier;

    [Space]
    [SerializeField] private GameObject _sanguineWrathProjectile;

    private void HeroHealed(HeroBase hero, float healingAmount)
    {
        // Divide by the healing damage multiplier to get the healing the hero would've received prior to the modifier
        healingAmount /= _healingModifier;

        Instantiate(_sanguineWrathProjectile, hero.transform.position, Quaternion.identity)
            .GetComponent<HeroMoveTowardsBossProjectile>().SetUpProjectile(hero,healingAmount * _healingDamageMultiplier,0);
    }
    
    #region BaseMissionModifier

    public override void AdjustHeroStatsModifier(HeroStats heroStats)
    {
        base.AdjustHeroStatsModifier(heroStats);
        heroStats.ChangeCurrentHeroHealingReceivedMultiplicativeMultiplier(_healingModifier);
    }

    
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        HeroesManager.Instance.GetOnHeroHealedEvent().AddListener(HeroHealed);
    }

    protected override void UnsubscribeFromEvents()
    {
        base.SubscribeToEvents();
        HeroesManager.Instance.GetOnHeroHealedEvent().RemoveListener(HeroHealed);
    }
    #endregion
}
