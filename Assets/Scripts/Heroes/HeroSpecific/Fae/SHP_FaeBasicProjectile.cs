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
    [SerializeField] private bool _doesProjectileBounceOffWalls;
    [SerializeField] private float _wallBounceSpeedMultiplier = 1;

    [Space] 
    [SerializeField] private float _distanceRequiredForPerpendicularProjectileDamage;
    [SerializeField] private Vector3 _projectileAlignedWithBossHitboxMultiplier = Vector3.one;

    [Space]
    [SerializeField] private GameObject _wallBounceEffect;
    
    [Space]
    [SerializeField] private GeneralTranslate _generalTranslate;
    [SerializeField] private GeneralHeroDamageArea _generalDamageArea;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private CurveProgression _scaleCurve;
    [SerializeField] private SwapTextures _swapTextures;
    
    private static SH_Fae _associatedFae;
    private bool _hasHitEdgeOfMap = false;
    private Vector3 _positionOfFaeOnAbilityUse;
    private bool _isProjectileAlignedWithBoss = false;

    private IEnumerator CheckForHitEdgeOfMap()
    {
        while (!_hasHitEdgeOfMap)
        {
            float centerDistance = Mathf.Abs(transform.position.x) + Mathf.Abs(transform.position.z);
            if (centerDistance > EnvironmentManager.Instance.GetMapRadius())
            {
                HitEdgeOfMap();
            }
            yield return null;
        }
    }

    private void HitEdgeOfMap()
    {
        _hasHitEdgeOfMap = true;
        
        _scaleCurve.StartMovingDownOnCurve();
    }

    public void RedirectProjectile()
    {
        if (_associatedFae.IsUnityNull())
        {
            return;
        }
        
        Instantiate(_wallBounceEffect, transform.position, transform.rotation);
        
        Vector3 startDirection = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 newDirection = new Vector3(_associatedFae._myHeroBase.transform.position.x, 0, _associatedFae._myHeroBase.transform.position.z);
        
        transform.LookAt(newDirection);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        
        newDirection -= startDirection;
        newDirection.Normalize();
        
        _generalTranslate.StopMoving();
        _generalTranslate.MultiplySpeed(_wallBounceSpeedMultiplier);
        _generalTranslate.StartMoving(newDirection);
        
        _swapTextures.SwapAllTextures();
        
        

        if (_isProjectileAlignedWithBoss || Vector3.Distance(_positionOfFaeOnAbilityUse, _associatedFae._myHeroBase.transform.position) >= _distanceRequiredForPerpendicularProjectileDamage)
        {
            // Fae moved enough
            _generalDamageArea.ToggleProjectileCollider(true);
        }
    }
    
    public void AdditionalSetUp(bool isAlignedWithBoss)
    {
         /*
          * A projectile is aligned with the Boss when the projectile is fired in the direction towards or directly away
          * from the Boss. Projectiles that are perpendicular to the boss are not aligned.
          * For example if the Fae is in the bottom right of the arena, the arrows fired top left and bottom right are
          * considered aligned. Even if the projectiles aren't going to hit the Boss they are still aligned.
          */
         _isProjectileAlignedWithBoss = isAlignedWithBoss;
    
        if (_isProjectileAlignedWithBoss)
        {
            /*
             * Gives projectiles that are aligned with the Boss a more forgiving hit box.
             * This is not done to unaligned projectiles as it would cause you to be able to hit all 4 arrows by
             * standing close enough to the Boss
             */
            _boxCollider.size = new Vector3(_boxCollider.size.x * _projectileAlignedWithBossHitboxMultiplier.x,
                _boxCollider.size.y *_projectileAlignedWithBossHitboxMultiplier.y, 
                _boxCollider.size.z * _projectileAlignedWithBossHitboxMultiplier.z);
        }
    }
    
    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase, EHeroAbilityType heroAbilityType)
    {
        base.SetUpProjectile(heroBase, heroAbilityType);

        if (_associatedFae.IsUnityNull())
        {
            _associatedFae = (SH_Fae)heroBase.GetSpecificHeroScript();
        }
        
        _positionOfFaeOnAbilityUse = heroBase.transform.position;
        _generalTranslate.StartMoving(transform.forward);

        if (_doesProjectileBounceOffWalls)
        {
            StartCoroutine(CheckForHitEdgeOfMap());
        }
    }
    #endregion
}
