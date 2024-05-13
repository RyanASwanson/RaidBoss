using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains the functionality that is generally used for boss attacks
/// </summary>
public class GeneralBossDamageArea : MonoBehaviour
{
    [SerializeField] private Collider _damageCollider;

    [Header("Enter")]
    [SerializeField] private float _enterDamage;
    [SerializeField] private UnityEvent<Collider> _enterEvent;

    [Header("Stay")]
    [SerializeField] private float _stayDamage;
    [SerializeField] private UnityEvent<Collider> _stayEvent;

    [Header("Exit")]
    [SerializeField] private float _exitDamage;
    [SerializeField] private UnityEvent<Collider> _exitEvent;

    private bool DoesColliderBelongToHero(Collider collision)
    {
        return collision.gameObject.CompareTag(TagStringData.GetHeroHitboxTagName();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(DoesColliderBelongToHero(collision))
        {
            _enterEvent?.Invoke(collision);

            if (_enterDamage <= 0) return;
            collision.GetComponentInParent<HeroBase>().GetHeroStats().DealDamageToHero(_enterDamage);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (DoesColliderBelongToHero(collision))
        {
            _stayEvent?.Invoke(collision);

            if (_stayDamage <= 0) return;
            collision.GetComponentInParent<HeroBase>().GetHeroStats().DealDamageToHero(_stayDamage);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (DoesColliderBelongToHero(collision))
        {
            _exitEvent?.Invoke(collision);

            if (_exitDamage <= 0) return;
            collision.GetComponentInParent<HeroBase>().GetHeroStats().DealDamageToHero(_exitDamage);
        }
    }
}
