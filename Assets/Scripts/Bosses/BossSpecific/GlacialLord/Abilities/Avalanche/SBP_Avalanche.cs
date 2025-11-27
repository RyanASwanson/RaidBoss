using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The projectile for the Avalanche ability
/// SBP stands for Specific Boss Ability
/// </summary>
public class SBP_Avalanche : BossProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _accelerationTime;
    [SerializeField] private AnimationCurve _speedCurve;

    private float _projectileAcceleration = 0;

    [Space]
    [SerializeField] private float _wallContactDistance;
    [SerializeField] private Vector3 _wallContactHalfExtents;
    [SerializeField] private LayerMask _edgeOfMapLayer;
    [Space]
    [SerializeField] protected CinemachineCameraShakeData _screenShakeData;

    [Space]
    [SerializeField] private GlacialLordSelfMinionHit _minionHit;
    [Space]
    [SerializeField] private GeneralBossDamageArea _damageArea;

    private SBA_Avalanche _specificBossAbility;
    
    /// <summary>
    /// Makes the projectile look at the target hero and start moving 
    /// </summary>
    private void StartProjectileMovement()
    {
        StartCoroutine(ProjectileAcceleration());
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
            transform.position += transform.forward * (_projectileSpeed * _projectileAcceleration * Time.deltaTime);
            CheckForEnd();
            yield return null;
        }

    }

    /// <summary>
    /// The acceleration for the projectile
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProjectileAcceleration()
    {
        float timeCounter = 0;

        while (timeCounter < 1)
        {
            timeCounter += Time.deltaTime / _accelerationTime;
            _projectileAcceleration = _speedCurve.Evaluate(timeCounter);
            yield return null;
        }

        _projectileAcceleration = 1;
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

    private void CheckForEnd()
    {
        if(Physics.BoxCast(transform.position,_wallContactHalfExtents,
            transform.forward,Quaternion.identity,_wallContactDistance,_edgeOfMapLayer))
        {
            AtEndOfPath();
        }
    }

    /// <summary>
    /// Called when the projectile reaches the end of its path
    /// </summary>
    private void AtEndOfPath()
    {
        _specificBossAbility.ProjectileReachedEndOfPath();
        PlayAvalancheEndScreenShake();
        _minionHit.MinionContactFromDistance();
        _damageArea.DestroyProjectile();
    }
    
    private void PlayAvalancheEndScreenShake()
    {
        CameraGameManager.Instance.StartCameraShake(_screenShakeData);
    }

    #region Base Ability

    public void AdditionalSetUp(Vector3 lookDirection, SBA_Avalanche specificAbility)
    {
        _specificBossAbility = specificAbility;
        ProjectileLookAt(lookDirection);
        StartProjectileMovement();
    }
    #endregion
}
