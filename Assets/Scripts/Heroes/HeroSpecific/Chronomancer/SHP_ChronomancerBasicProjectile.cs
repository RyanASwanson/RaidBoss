using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the functionality for the chronomancers basic attack
/// </summary>
public class SHP_ChronomancerBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;

    [SerializeField] private float _basicAbilityCooldownReduction;

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
            yield return null;
        }
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

    public void AdditionalSetup(Vector3 direction)
    {
        GetComponent<GeneralHeroHealArea>().GetEnterEvent().AddListener(ReduceManualCooldownOfTarget);
        StartCoroutine(MoveProjectile(direction));
    }
}
