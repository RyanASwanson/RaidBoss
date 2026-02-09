using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroProjectileFramework : MonoBehaviour
{
    protected HeroBase _myHeroBase;
    protected SpecificHeroFramework _mySpecificHero;
    protected EHeroAbilityType _heroAbilityType;
    
    public virtual void SetUpProjectile(HeroBase heroBase, EHeroAbilityType heroAbilityType)
    {
        _myHeroBase = heroBase;
        _mySpecificHero = heroBase.GetSpecificHeroScript();
        _heroAbilityType = heroAbilityType;
    }

    public virtual void SetUpProjectile()
    {

    }
    
}
