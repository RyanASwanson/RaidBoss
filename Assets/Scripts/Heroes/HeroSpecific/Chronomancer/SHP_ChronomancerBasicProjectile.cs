using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the functionality for the chronomancers basic attack
/// </summary>
public class SHP_ChronomancerBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _homingSpeedIncrease;
    private Vector3 _moveDirection;

    private float _basicAbilityCooldownReduction;

    [Space]
    [SerializeField] private ChronomancerHomingProjectiles _homing;
    Transform _targetLoc;
    private bool _hasTarget = false;


    /// <summary>
    /// Sends the projectile moving in a direction until it is destroyed
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private IEnumerator MoveProjectile(Vector3 direction)
    {
        while (true)
        {
            transform.position += direction * _projectileSpeed * Time.deltaTime;

            if(_hasTarget)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetLoc.transform.position, 
                    _homingSpeedIncrease * Time.deltaTime);
            }
                
            yield return null;
        }
    }

    public void SetHomingTarget(Transform targetTransform)
    {
        _targetLoc = targetTransform;
        _hasTarget = true;
    }


    private void ReduceManualCooldownOfTarget(Collider collider)
    {
        SpecificHeroFramework specificHeroFramework = collider.GetComponentInParent<HeroBase>().GetSpecificHeroScript();

        if(specificHeroFramework != null)
        {
            specificHeroFramework.AddToBasicAbilityChargeTime(_basicAbilityCooldownReduction);
        }
    }

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup(Vector3 direction, float cooldownReduction)
    {
        _moveDirection = direction;
        _basicAbilityCooldownReduction = cooldownReduction;

        _homing.SetupHoming(_myHeroBase);

        GetComponent<GeneralHeroHealArea>().GetEnterEvent().AddListener(ReduceManualCooldownOfTarget);
        StartCoroutine(MoveProjectile(direction));
    }
}
