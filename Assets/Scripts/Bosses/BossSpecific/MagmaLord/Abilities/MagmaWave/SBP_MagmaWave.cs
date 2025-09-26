using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_MagmaWave : BossProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _distanceThreshold;

    [Space]
    [SerializeField] private Animator _waveAnimator;

    private const string REMOVE_PROJECTILE_ANIM_TRIGGER = "WaveEnd";

    

    /// <summary>
    /// Makes the projectile look at the target hero and start moving 
    /// </summary>
    /// <param name="heroBase"></param>
    private void StartProjectileMovement()
    {
        ProjectileLookAt(_myBossBase.transform.position);

        StartCoroutine(MoveProjectile());
    }

    /// <summary>
    /// Moves the projectile along the path to the boss
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveProjectile()
    {
        Vector3 moveDirection = (_myBossBase.transform.position - transform.position).normalized;
        while (true)
        {
            transform.position += moveDirection * _projectileSpeed * Time.deltaTime;
            CheckBossDistance();
            yield return null;
        }

    }

    /// <summary>
    /// Causes the projectile to look in the direction of the boss
    /// </summary>
    /// <param name="lookLocation"></param>
    private void ProjectileLookAt(Vector3 lookLocation)
    {
        
        transform.LookAt(lookLocation);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    /// <summary>
    /// Checks if the projectile is close enough to remove
    /// </summary>
    private void CheckBossDistance()
    {
        if (Vector3.Distance(transform.position, _myBossBase.transform.position) < _distanceThreshold)
        {
            ProjectileReachedEndOfPath();
        }
    }

    /// <summary>
    /// Called when the projectile is close to the boss
    /// Triggers the animation to remove the projectile
    /// </summary>
    private void ProjectileReachedEndOfPath()
    {
        _waveAnimator.SetTrigger(REMOVE_PROJECTILE_ANIM_TRIGGER);
    }

    #region Base Ability
    /// <summary>
    /// Called when projectile is created
    /// Provides the projectile with any additional information it may need
    /// </summary>
    /// <param name="heroBase"></param>
    /// <param name= "newAbilityID"></param>
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        StartProjectileMovement();
    }
    #endregion
}
