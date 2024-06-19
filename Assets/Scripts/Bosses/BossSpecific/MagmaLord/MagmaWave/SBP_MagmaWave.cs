using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_MagmaWave : BossProjectileFramework
{
    [SerializeField] private float _projectileSpeed;

    [Space]
    private const float _distanceThreshold = .1f;

    /// <summary>
    /// Called when projectile is created
    /// Provides the projectile with any additional information it may need
    /// </summary>
    /// <param name="heroBase"></param>
    public void AdditionalSetup()
    {
        StartProjectileMovement();
    }

    /// <summary>
    /// Makes the projectile look at the target hero and start moving 
    /// </summary>
    /// <param name="heroBase"></param>
    private void StartProjectileMovement()
    {
        ProjectileLookAt(_ownerBossBase.transform.position);

        StartCoroutine(MoveProjectile());
    }


    private IEnumerator MoveProjectile()
    {
        Vector3 moveDirection = (_ownerBossBase.transform.position - transform.position).normalized;
        while (Vector3.Distance(transform.position, _ownerBossBase.transform.position) > _distanceThreshold)
        {
            transform.position += moveDirection * _projectileSpeed * Time.deltaTime;

            yield return null;
        }

        //ProjectileReachedEndOfPath();
    }

    private void ProjectileLookAt(Vector3 lookLocation)
    {
        transform.LookAt(lookLocation);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    /*private void ProjectileReachedEndOfPath()
    {
        Destroy(gameObject);
    }*/
}
