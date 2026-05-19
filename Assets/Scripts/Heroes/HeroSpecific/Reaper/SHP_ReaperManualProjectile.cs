using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Contains the functionality for the reapers manual attack
/// After the projectile is created it will move towards the reaper
///     dealing damage along its path
///     The initial location is done by the reaper script
/// </summary>
public class SHP_ReaperManualProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;

    [SerializeField] private float _projectileHitboxMaxShrinkRange;
    [SerializeField] private float _projectileHitboxMinimumSize;
    private float _projectileHitboxMaximumSize;
    
    [Space]
    [SerializeField] private CurveProgression _removalCurve;
    [SerializeField] private GeneralVFXFunctionality _projectileVFX;
    [SerializeField] private SphereCollider _projectileCollider;
    
    private Animator _projectileAnimator;

    private const string HIT_ANIM_TRIGGER = "HitEnemy";
    
    private IEnumerator MoveProjectile()
    {
        Vector3 targetLocation = _myHeroBase.gameObject.transform.position;
        float distanceToTarget;
        
        while (true)
        {
            if (_myHeroBase.IsUnityNull())
            {
                HeroNotFound();
            }
            else
            {
                targetLocation = _myHeroBase.gameObject.transform.position;
            }
            
            transform.position = Vector3.MoveTowards(transform.position,
                targetLocation, _projectileSpeed * Time.deltaTime);

            transform.LookAt(targetLocation);
            transform.eulerAngles.Set(0, transform.eulerAngles.y, 0);
            
            distanceToTarget = Vector3.Distance(transform.position, targetLocation);
            if (distanceToTarget < _projectileHitboxMaxShrinkRange)
            {
                _projectileCollider.radius = Mathf.Lerp(distanceToTarget/_projectileHitboxMaxShrinkRange, _projectileHitboxMinimumSize, _projectileHitboxMaximumSize);
            }
            
            yield return null;
        }
    }
    
    public void PlayProjectileHitFlickerAnimation()
    {
        _projectileAnimator.SetTrigger(HIT_ANIM_TRIGGER);
    }

    private void HeroNotFound()
    {
        _removalCurve.StartMovingDownOnCurve();
    }

    public void ProjectileRemovedEarly()
    {
        _projectileVFX.StopAllParticleSystems();
        _projectileVFX.DetachVisualEffect();
        _projectileVFX.StartDelayedLifetime();
        Destroy(gameObject);
    }

    #region Base Ability
    /// <summary>
    /// Performs needed set up on the projectile
    /// </summary>
    /// <param name="heroBase"> The associated hero </param>
    public override void SetUpProjectile(HeroBase heroBase, EHeroAbilityType heroAbilityType)
    {
        base.SetUpProjectile(heroBase, heroAbilityType);
        _projectileAnimator = GetComponentInChildren<Animator>();
        _projectileHitboxMaximumSize = _projectileCollider.radius;
        
        StartCoroutine(MoveProjectile());
    }
    #endregion
}
