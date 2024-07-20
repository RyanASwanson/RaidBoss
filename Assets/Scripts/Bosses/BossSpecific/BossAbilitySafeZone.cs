using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAbilitySafeZone : BossProjectileFramework
{
    private List<HeroBase> _heroesInRange = new List<HeroBase>();

    public void AddHeroInRange(HeroBase enterHero)
    {
        _heroesInRange.Add(enterHero.gameObject.GetComponent<HeroBase>());
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

    private void OnTriggerEnter(Collider collision)
    {
        HeroBase tempHero = collision.GetComponentInParent<HeroBase>();
        if (tempHero != null)
            AddHeroInRange(tempHero);
    }

    private void OnTriggerExit(Collider collision)
    {
        HeroBase tempHero = collision.GetComponentInParent<HeroBase>();
        if (tempHero != null)
            RemoveHeroInRange(tempHero);
    }
}
