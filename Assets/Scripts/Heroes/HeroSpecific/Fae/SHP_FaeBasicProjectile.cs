using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Contains the functionality for the basic projectile of the Fae.
/// SHP stands for Specific Hero Projectile.
/// </summary>
public class SHP_FaeBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private GeneralTranslate _generalTranslate;

    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase, EHeroAbilityType heroAbilityType)
    {
        base.SetUpProjectile(heroBase, heroAbilityType);
        _generalTranslate.StartMoving(transform.forward);
    }
    #endregion
}
