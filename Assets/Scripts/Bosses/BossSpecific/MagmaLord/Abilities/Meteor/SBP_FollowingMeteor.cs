using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that handles the Magma Lord meteor
/// Specifically handles the part of the meteor that moves towards the player
/// </summary>
public class SBP_FollowingMeteor : BossProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _accelerationTime;
    private const float SPEED_SCALAR_MAX = 1;
    
    [SerializeField] private AnimationCurve _projectileSpeedCurve;

    [Space]
    [SerializeField] private float _randomDirectionThreshold;
    
    [Space] 
    [SerializeField] private CurveProgression _rotationCurveProgression;

    /// <summary>
    /// Makes the projectile look at the target hero and start moving 
    /// </summary>
    /// <param name="heroBase"></param>
    private void StartProjectileMovement(HeroBase heroBase)
    {
        ProjectileLookAt(heroBase.transform.position);
        
        _rotationCurveProgression.StartMovingUpOnCurve();
        StartCoroutine(MoveProjectile(DetermineMovementDirection(heroBase.transform.position)));
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

            yield return null;
        }

    }

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
    public void AdditionalSetUp(HeroBase heroBase)
    {
        StartProjectileMovement(heroBase);
    }
    #endregion
}
