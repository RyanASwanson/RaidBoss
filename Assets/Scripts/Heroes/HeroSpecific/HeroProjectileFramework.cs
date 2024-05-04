using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroProjectileFramework : MonoBehaviour
{
    protected HeroBase _ownerHeroBase;
    protected SpecificHeroFramework _mySpecificHero;

    public virtual void SetUpProjectile(HeroBase heroBase)
    {
        _ownerHeroBase = heroBase;
        _mySpecificHero = heroBase.GetSpecificHeroScript();
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        
    }
}
