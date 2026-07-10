using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_RayOfHope : BossProjectileFramework
{
    [SerializeField] private float _baseHealing;
    
    [Space]
    [SerializeField] private GeneralBossBuffArea _bossBuffArea;

    public void HitHero(HeroBase heroBase)
    {
        /*
         * Potentially implement more clear solution on basing healing on amount of living Heroes
         * Healing has to be reduced as Heroes die, or with only 1 hero alive the healing would be all going into 1 Hero.
         */
        _bossBuffArea.DealHealing(heroBase, _baseHealing*HeroesManager.Instance.GetAmountOfLivingHeroes());
        RemoveRayOfHope();
    }
    
    public void RemoveRayOfHope()
    {
        UnsubscribeFromEvents();
        Destroy(this.gameObject);
    }

    private void SubscribeToEvents()
    {
        _bossBuffArea.GetGeneralHitEvent().AddListener(HitHero);
    }

    private void UnsubscribeFromEvents()
    {
        _bossBuffArea.GetGeneralHitEvent().RemoveListener(HitHero);
    }
    
    #region BaseProjectile
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        SubscribeToEvents();
    }
    #endregion
}
