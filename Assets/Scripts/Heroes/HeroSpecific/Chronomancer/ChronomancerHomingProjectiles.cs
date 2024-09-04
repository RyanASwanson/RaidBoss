using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the homing functionality for the chronomancer basic projectile
/// </summary>
public class ChronomancerHomingProjectiles : MonoBehaviour
{
    [Range(-1, 1)] [SerializeField] private float _homingDotProduct;

    [SerializeField] private SHP_ChronomancerBasicProjectile _associatedProjectile;

    private HeroBase _ignoreHero;
    private bool _isHoming = false;

    /// <summary>
    /// Prepares the functionality of the homing
    /// </summary>
    /// <param name="ignoreHero"></param>
    public void SetupHoming(HeroBase ignoreHero)
    {
        //Prevents the projectile from homing in on its owner
        _ignoreHero = ignoreHero;
        //Unparents the homing from the projectile itself
        Detach();
        //Starts following the exact position of the associated projectile
        StartCoroutine(FollowProjectile());
    }

    /// <summary>
    /// Causes the homing projectile to no longer be a child of the projectile
    /// If it was attached the General Hero Damage area would use the
    ///     trigger from the homing
    /// </summary>
    private void Detach()
    {
        transform.parent = null;
    }

    /// <summary>
    /// Repeating process that makes the homing functionality stay at the same position as the
    /// basic projectile
    /// </summary>
    /// <returns></returns>
    private IEnumerator FollowProjectile()
    {
        while(_associatedProjectile != null)
        {
            transform.position = _associatedProjectile.transform.position;
            yield return null;
        }

        //Removes the homing functionality once the main projectile is destroyed
        Destroy(gameObject);
    }

    /// <summary>
    /// Checks for a target to home in on
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay(Collider collision)
    {
        
        if (DoesColliderBelongToHero(collision) && IsValidHomingDirection(collision.gameObject.transform.position))
        {
            StartHoming(collision.transform);
        }
    }

    /// <summary>
    /// Checks if the target is roughly in front of the projectile
    /// </summary>
    /// <param name="contactObject"></param>
    /// <returns></returns>
    private bool IsValidHomingDirection(Vector3 contactObject)
    {
        Vector3 objectDirection = (contactObject - transform.position).normalized;
        return Vector3.Dot(objectDirection, _associatedProjectile.GetDirection()) >= _homingDotProduct;
    }

    /// <summary>
    /// Called when all checks have passed and we know we can home in on a target
    /// </summary>
    /// <param name="homingTransform"></param>
    private void StartHoming(Transform homingTransform)
    {
        _isHoming = true;
        _associatedProjectile.SetHomingTarget(homingTransform);
    }

    /// <summary>
    /// Checks if the thing that was collided with is a hero 
    ///     and that hero can receive healing
    /// </summary>
    /// <param name="collision"></param>
    /// <returns></returns>
    private bool DoesColliderBelongToHero(Collider collision)
    {
        HeroBase hitHero = collision.GetComponentInParent<HeroBase>();

        return (hitHero != null && hitHero != _ignoreHero && !_isHoming &&
            hitHero.GetHeroStats().CanHeroBeHealed() && _associatedProjectile != null);
    }
}
