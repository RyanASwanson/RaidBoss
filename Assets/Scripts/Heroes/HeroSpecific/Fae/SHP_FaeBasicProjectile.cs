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
    [SerializeField] private float _projectileSpeed;

    /// <summary>
    /// Moves the projectile in a given direction
    /// </summary>
    /// <param name="direction"> The direction to move in</param>
    /// <returns></returns>
    private IEnumerator MoveProjectile(Vector3 direction)
    {
        while (!gameObject.IsUnityNull())
        {
            transform.position += direction * _projectileSpeed * Time.deltaTime;
            yield return null;
        }
    }

    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
        StartCoroutine(MoveProjectile(transform.forward));
    }
    #endregion
}
