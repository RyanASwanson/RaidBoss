using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Contains the functionality for the chronomancers basic attack
/// </summary>
public class SHP_ChronomancerBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _maxHomingSpeedIncrease;
    [SerializeField] private float _timeToReachMaxHomingStrength;
    private float _currentProjectileSpeed;
    private float _currentHomingStrength = 0;

    private Vector3 _storedDirection;


    private float _basicAbilityCooldownReduction;

    [Space]
    [SerializeField] private ChronomancerHomingProjectiles _homing;
    Transform _targetLoc;
    private bool _hasTarget = false;

    private SH_Chronomancer _chronomancerHero;


    /// <summary>
    /// Sends the projectile moving in a direction until it is destroyed
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private IEnumerator MoveProjectile(Vector3 direction)
    {
        while (true)
        {
            //Moves the projectile in it's set direction
            transform.position += direction * _currentProjectileSpeed * Time.deltaTime;

            //If the projectile is homing in on a target
            if(_hasTarget)
            {
                //Move the projectile towards its homing target
                transform.position = Vector3.MoveTowards(transform.position, _targetLoc.transform.position, 
                    _maxHomingSpeedIncrease * _currentHomingStrength* Time.deltaTime);
            }
                
            yield return null;
        }
    }

    /// <summary>
    /// Gradually adjusts how strong the homing strength of the projectile is
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateHomingStrength()
    {
        while(_currentHomingStrength < 1)
        {
            //Increases homing strength over time
            _currentHomingStrength += Time.deltaTime / _timeToReachMaxHomingStrength;
            //Decreases normal projectile speed
            _currentProjectileSpeed = Mathf.Lerp(_projectileSpeed, 0, _currentHomingStrength);
            yield return null;
        }

        //Makes sure that the speed values are exact
        _currentHomingStrength = 1;
        _currentProjectileSpeed = 0;
    }


    /// <summary>
    /// Assigns a target for the projectile to home in on
    /// </summary>
    /// <param name="targetTransform"></param>
    public void SetHomingTarget(Transform targetTransform)
    {
        _targetLoc = targetTransform;
        _hasTarget = true;
        StartCoroutine(UpdateHomingStrength());
    }


    /// <summary>
    /// When the projectile comes into contact with a hero it activates
    ///     the chronomancer's passive ability on the hero
    /// </summary>
    /// <param name="collider"></param>
    private void ReduceBasicCooldownOfTarget(Collider collider)
    {
        //Gets the hero that it collided with
        HeroBase heroTarget = collider.GetComponentInParent<HeroBase>();

        if (!heroTarget.IsUnityNull())
        {
            //Activates the chronomancer's passive ability on the hero
            _chronomancerHero.PassiveReduceBasicCooldownOfHero(heroTarget);
        }
    }


    #region Base Ability
    public void AdditionalSetup(Vector3 direction, float cooldownReduction, SH_Chronomancer chrono)
    {
        //Sets up initial values
        _currentProjectileSpeed = _projectileSpeed;

        _basicAbilityCooldownReduction = cooldownReduction;
        _storedDirection = direction;
        _chronomancerHero = chrono;

        _homing.SetupHoming(_myHeroBase);

        GetComponent<GeneralHeroHealArea>().GetEnterEvent().AddListener(ReduceBasicCooldownOfTarget);
        StartCoroutine(MoveProjectile(direction));

    }
    #endregion

    #region Getters

    public Vector3 GetDirection() => _storedDirection;
    #endregion
}
