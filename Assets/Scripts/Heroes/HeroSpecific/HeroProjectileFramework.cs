using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroProjectileFramework : MonoBehaviour
{
    protected HeroBase _myHeroBase;
    protected SpecificHeroFramework _mySpecificHero;

    public virtual void SetUpProjectile(HeroBase heroBase)
    {
        _myHeroBase = heroBase;
        _mySpecificHero = heroBase.GetSpecificHeroScript();
    }

    public virtual void SetUpProjectile()
    {

    }
}
