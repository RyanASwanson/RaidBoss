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
    
    private Animator _projectileAnimator;

    private const string HIT_ANIM_TRIGGER = "HitEnemy";
    
    private IEnumerator MoveProjectile()
    {
        Vector3 targetLocation = _myHeroBase.gameObject.transform.position;
        while (true)
        {
            if (!_myHeroBase.IsUnityNull())
            {
                targetLocation = _myHeroBase.gameObject.transform.position;
            }
            
            transform.position = Vector3.MoveTowards(transform.position,
                targetLocation, _projectileSpeed * Time.deltaTime);

            transform.LookAt(_myHeroBase.gameObject.transform.position);
            transform.eulerAngles.Set(0, transform.eulerAngles.y, 0);
            yield return null;
        }
    }

    public void PlayProjectileHitFlickerAnimation()
    {
        _projectileAnimator.SetTrigger(HIT_ANIM_TRIGGER);
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
        StartCoroutine(MoveProjectile());
    }
    #endregion
}
