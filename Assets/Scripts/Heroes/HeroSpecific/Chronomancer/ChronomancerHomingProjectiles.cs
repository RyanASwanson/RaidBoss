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

    private void OnTriggerEnter(Collider collision)
    {
        if (DoesColliderBelongToHero(collision))
        {
            StartHoming(collision.transform);
        }
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
