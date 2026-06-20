using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality and movement for hero projectiles that move into the boss
/// </summary>
public class HeroMoveTowardsBossProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileDamage;
    [SerializeField] private float _projectileStagger;

    [Space] 
    [SerializeField] private MoveBetween _moveBetween;
    
    public void SetUpProjectile(HeroBase heroBase, float damage, float stagger)
    {
        _myHeroBase = heroBase;
        
        _projectileDamage = damage;
        _projectileStagger = stagger;
        
        _moveBetween.StartMoveProcess(BossBase.Instance.gameObject);
    }
    
    /// <summary>
    /// Is called when the projectile reaches the location of the boss
    /// </summary>
    public void EndOfMovement()
    {
        if (!_mySpecificHero.IsUnityNull())
        {
            _mySpecificHero.DamageBoss(_projectileDamage);
            _mySpecificHero.StaggerBoss(_projectileStagger);
        }
        else
        {
            BossStats.Instance.DealDamageToBossFromNonHeroSource(_projectileDamage);
            BossStats.Instance.DealStaggerToBossFromNonHeroSource(_projectileStagger);
        }

        Destroy(gameObject);
    }
    
    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase, EHeroAbilityType heroAbilityType)
    {
        base.SetUpProjectile(heroBase, heroAbilityType);

        if (_moveBetween.IsUnityNull())
        {
            Debug.LogError("Did not find move between script");
        }
        
        _moveBetween.StartMoveProcess(BossBase.Instance.gameObject);
    }
    #endregion
}
