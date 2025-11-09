using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_VampirePassiveProjectile : HeroProjectileFramework
{
    [SerializeField] private MoveBetween _moveBetween;
    
    private float _storedHeal;
    
    private SH_Vampire _vampireHero;

    public void ActivateHeroPassive()
    {
        _vampireHero.ActivatePassiveHeal(_storedHeal);
    }
    
    public void AdditionalSetup(SH_Vampire heroScript, float healing)
    {
        _vampireHero = heroScript;
        _storedHeal = healing;
        
        _moveBetween.StartMoveProcess(_vampireHero.gameObject);
    }
}
