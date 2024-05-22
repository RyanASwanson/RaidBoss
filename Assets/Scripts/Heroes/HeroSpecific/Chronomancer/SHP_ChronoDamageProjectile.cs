using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the functionality for the chronomancers basic attack
/// </summary>
public class SHP_ChronoDamageProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileLifetime;
    [SerializeField] private float _projectileSpeed;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
    }

    public void AdditionalSetup(Vector3 direction)
    {
        StartCoroutine(MoveProjectile(direction));
        Destroy(gameObject, _projectileLifetime);
    }

    /// <summary>
    /// Sends the projectile moving in a direction until it is destroyed
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private IEnumerator MoveProjectile(Vector3 direction)
    {
        while(true)
        {
            transform.position += direction * _projectileSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
