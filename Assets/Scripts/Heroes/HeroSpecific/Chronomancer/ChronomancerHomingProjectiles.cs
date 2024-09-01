using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the homing functionality for the chronomancer basic projectile
/// </summary>
public class ChronomancerHomingProjectiles : MonoBehaviour
{
    [SerializeField] private SHP_ChronomancerBasicProjectile _associatedProjectile;

    private HeroBase _ignoreHero;
    private bool _isHoming = false;

    public void SetupHoming(HeroBase ignoreHero)
    {
        _ignoreHero = ignoreHero;
        Detach();
        StartCoroutine(FollowProjectile());
    }

    /// <summary>
    /// Causes the homing projectile to no longer be a child of the projectile
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
    }

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
        return Vector3.Dot(objectDirection, _associatedProjectile.GetDirection()) >= 0;
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

    private bool DoesColliderBelongToHero(Collider collision)
    {
        HeroBase hitHero = collision.GetComponentInParent<HeroBase>();

        return (hitHero != null && hitHero != _ignoreHero && !_isHoming &&
            hitHero.GetHeroStats().CanHeroBeHealed() && _associatedProjectile != null);
    }
}
