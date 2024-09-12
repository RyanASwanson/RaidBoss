using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Frostbite : BossProjectileFramework
{
    [SerializeField] private float _projectileSpeed;


    /// <summary>
    /// Makes the projectile move towards the boss
    /// </summary>
    /// <param name="heroBase"></param>
    private void StartProjectileMovement()
    {
        StartCoroutine(MoveProjectile());
    }

    /// <summary>
    /// Moves the projectile along the path to the boss
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveProjectile()
    {
        print("startmovement");
        while (true)
        {
            transform.position += transform.forward * _projectileSpeed * Time.deltaTime;
            yield return null;
        }

    }


    #region Base Ability
    /// <summary>
    /// Called when projectile is created
    /// Provides the projectile with any additional information it may need
    /// </summary>
    /// <param name="heroBase"></param>
    public override void SetUpProjectile(BossBase bossBase)
    {
        base.SetUpProjectile(bossBase);
        StartProjectileMovement();
    }

    #endregion
}
