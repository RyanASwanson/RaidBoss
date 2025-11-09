using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality and movement for the passive projectile created by
/// the Alchemist
/// </summary>
public class SHP_AlchemistPassiveProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileDamage;
    [SerializeField] private float _projectileStagger;

    [Space] 
    [SerializeField] private MoveBetween _moveBetween;
    
    
    /// <summary>
    /// Is called when the projectile reaches the location of the boss
    /// </summary>
    public void EndOfMovement()
    {
        _mySpecificHero.DamageBoss(_projectileDamage);
        _mySpecificHero.StaggerBoss(_projectileStagger);

        Destroy(gameObject);
    }

    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase, EHeroAbilityType heroAbilityType)
    {
        base.SetUpProjectile(heroBase, heroAbilityType);
        
        _moveBetween.StartMoveProcess(BossBase.Instance.gameObject);
    }
    #endregion
}
