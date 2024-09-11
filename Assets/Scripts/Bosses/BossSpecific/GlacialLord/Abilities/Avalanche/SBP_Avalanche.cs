using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Avalanche : BossProjectileFramework
{
    [SerializeField] private float _projectileSpeed;


    /// <summary>
    /// Makes the projectile look at the target hero and start moving 
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
        while (true)
        {
            transform.position += transform.forward * _projectileSpeed * Time.deltaTime;
            yield return null;
        }

    }

    /// <summary>
    /// Causes the projectile to look in the correct direction
    /// </summary>
    /// <param name="lookLocation"></param>
    private void ProjectileLookAt(Vector3 lookLocation)
    {

        transform.LookAt(lookLocation);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
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

    }

    public void AdditionalSetup(Vector3 lookDirection)
    {
        ProjectileLookAt(lookDirection);
        StartProjectileMovement();
    }
    #endregion
}
