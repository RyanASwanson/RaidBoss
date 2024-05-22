using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Contains the functionality for the shamans basic attack
/// </summary>
public class SHP_ShamanBasicAbility : HeroProjectileFramework
{
    [SerializeField] private float _projectileLifetime;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup()
    {
        Destroy(gameObject, _projectileLifetime);
    }

}
