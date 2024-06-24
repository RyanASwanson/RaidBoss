using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_SamuraiBasicProjectile : HeroProjectileFramework
{
    public void DecreaseCooldownOfManual()
    {
        _mySpecificHero.ActivatePassiveAbilities();
    }

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup(float lifeTime)
    {
        Destroy(gameObject, lifeTime);
    }
}
