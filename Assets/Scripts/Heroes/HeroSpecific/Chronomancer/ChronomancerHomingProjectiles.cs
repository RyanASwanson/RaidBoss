using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronomancerHomingProjectiles : MonoBehaviour
{
    [SerializeField] private SHP_ChronomancerBasicProjectile _associatedProjectile;

    private Transform _targetTransform;

    private HeroBase _ignoreHero;

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
        print("trigger Enter");
        if (DoesColliderBelongToHero(collision))
        {
            _associatedProjectile.SetHomingTarget(collision.transform);
        }
    }

    private bool DoesColliderBelongToHero(Collider collision)
    {
        HeroBase hitHero = collision.GetComponentInParent<HeroBase>();
        Debug.Log(" " + hitHero != null + " " + hitHero != _ignoreHero);
        return (hitHero != null && hitHero != _ignoreHero && hitHero.GetHeroStats().ShouldOverrideHealing());
    }
}
