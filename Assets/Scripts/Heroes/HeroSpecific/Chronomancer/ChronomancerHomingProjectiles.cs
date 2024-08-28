using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Detach()
    {
        transform.parent = null;
    }

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

    private bool IsValidHomingDirection(Vector3 contactObject)
    {
        Vector3 objectDirection = (contactObject - transform.position).normalized;
        return Vector3.Dot(objectDirection, _associatedProjectile.GetDirection()) >= 0;
    }

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
