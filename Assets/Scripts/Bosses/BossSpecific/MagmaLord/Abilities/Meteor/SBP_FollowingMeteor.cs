using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Script that handles the Magma Lord meteor
/// Specifically handles the part of the meteor that moves towards the player
/// </summary>
public class SBP_FollowingMeteor : BossProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _accelerationTime;
    [SerializeField] private float _moveDelay;
    
    [SerializeField] private AnimationCurve _projectileSpeedCurve;

    [Space]
    [SerializeField] private float _randomDirectionThreshold;

    [SerializeField] private float _scaleDownRemovalDelay;
    [SerializeField] private float _enrageScaleDownDelayMultiplier;
    [SerializeField] private float _mapEdgeRemovalDelay;
    
    [Space]
    [SerializeField] private GeneralBossDamageArea _damageArea;

    [Space] 
    [SerializeField] private GeneralRotation _generalRotation;

    [SerializeField] private CurveProgression _rotationAccelerationCurve;
    
    [SerializeField] private CurveProgression _projectileScaleCurveProgression;

    private bool _isMeteorBeingRemoved = false;

    /// <summary>
    /// Makes the projectile look at the target hero and start moving 
    /// </summary>
    /// <param name="heroBase"></param>
    private void StartProjectileMovement(Vector3 storedTargetLocation)
    {
        ProjectileLookAt(storedTargetLocation);
        
        _generalRotation.BeginRotation();
        _rotationAccelerationCurve.StartMovingUpOnCurve();
        StartCoroutine(MoveProjectile(DetermineMovementDirection(storedTargetLocation)));
    }

    /// <summary>
    /// Determines what direction to make the projectile move in
    /// </summary>
    /// <param name="heroLocation"></param>
    /// <returns></returns>
    private Vector3 DetermineMovementDirection(Vector3 heroLocation)
    {
        //Go in direction of hero
        Vector3 direction = heroLocation - transform.position;

        //If that direction is close to zero choose a random direction instead
        if (Mathf.Abs(direction.x) < _randomDirectionThreshold && Mathf.Abs(direction.z) < _randomDirectionThreshold)
        {
            direction = Random.insideUnitCircle.normalized;
            direction = new Vector3(direction.x, 0, direction.y);

            ProjectileLookAt(transform.position + direction);
        }

        //Return the direction with 0 in the y
        return new Vector3(direction.x, 0, direction.z).normalized;
    }

    private IEnumerator MoveProjectile(Vector3 moveDirection)
    {
        float speedScalar = 0;
        float speedProgress = 0;
        
        yield return new WaitForSeconds(_moveDelay);

        while(true)
        {
            if(speedProgress < 1)
            {
                speedProgress += Time.deltaTime /_accelerationTime;
                if (speedProgress > 1)
                {
                    speedProgress = 1;
                }

                speedScalar = _projectileSpeedCurve.Evaluate(speedProgress);
            }
            
            transform.position += moveDirection * (_projectileSpeed * speedScalar * Time.deltaTime);
            
            float centerDistance = Mathf.Abs(transform.position.x) + Mathf.Abs(transform.position.z);
            if (!_isMeteorBeingRemoved && centerDistance > EnvironmentManager.Instance.GetMapRadius())
            {
                StartMapEdgeRemoval();
            }

            yield return null;
        }

    }

    private void ProjectileLookAt(Vector3 lookLocation)
    {
        transform.LookAt(lookLocation);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    private void StartRemovalDelay()
    {
        StartCoroutine(RemovalDelay());
    }

    private IEnumerator RemovalDelay()
    {
        yield return new WaitForSeconds(_scaleDownRemovalDelay);
        _projectileScaleCurveProgression.StartMovingUpOnCurve();
        _isMeteorBeingRemoved = true;
    }

    public void StartMapEdgeRemoval()
    {
        _isMeteorBeingRemoved = true;
        StartCoroutine(MapEdgeRemovalDelay());
    }

    private IEnumerator MapEdgeRemovalDelay()
    {
        yield return new WaitForSeconds(_mapEdgeRemovalDelay);
        _projectileScaleCurveProgression.StartMovingUpOnCurve();
        _damageArea.ToggleProjectileCollider(false);
    }

    #region Base Ability
    /// <summary>
    /// Called when projectile is created
    /// Provides the projectile with any additional information it may need
    /// </summary>
    /// <param name="storedTargetLocation"></param>
    public void AdditionalSetUp(Vector3 storedTargetLocation)
    {
        if (_wasBossEnragedOnAbilityActivation)
        {
            _scaleDownRemovalDelay *= _enrageScaleDownDelayMultiplier;
        }
        
        _damageArea.SetProjectileColliderLifeTime(_scaleDownRemovalDelay);
        _damageArea.StartColliderLifetime();
            
        StartRemovalDelay();
        StartProjectileMovement(storedTargetLocation);
    }
    #endregion
}
