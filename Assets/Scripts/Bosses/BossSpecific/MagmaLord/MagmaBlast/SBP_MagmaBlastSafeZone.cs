using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_MagmaBlastSafeZone : BossProjectileFramework
{
    private List<HeroBase> _heroesInRange = new List<HeroBase>();

    private SBA_MagmaBlast ownerAbility;

    /// <summary>
    /// Called when projectile is created
    /// Provides the projectile with any additional information it may need
    /// </summary>
    /// <param name="heroBase"></param>
    public void AdditionalSetup(SBA_MagmaBlast associatedAbility)
    {
        ownerAbility = associatedAbility;
    }

    public void AddHeroInRange(HeroBase enterHero)
    {
        _heroesInRange.Add(enterHero);
    }

    public void RemoveHeroInRange(HeroBase exitHero)
    {
        if (_heroesInRange.Contains(exitHero))
            _heroesInRange.Remove(exitHero);
    }

    public bool DoesSafeZoneContainHero()
    {
        return (_heroesInRange.Count > 0);
    }
}
