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
    [SerializeField] private float _stayDamagePerSecond;
    [SerializeField] private UnityEvent<Collider> _stayEvent;

    [Header("Exit")]
    [SerializeField] private float _exitDamage;
    [SerializeField] private UnityEvent<Collider> _exitEvent;

    private bool DoesColliderBelongToHero(Collider collision)
    {
        return collision.gameObject.CompareTag(TagStringData.GetHeroHitboxTagName());
    }

    private void OnTriggerEnter(Collider collision)
    {
        HitHero(collision, _enterEvent, _enterDamage);
    }

    private void OnTriggerStay(Collider collision)
    {
        HitHero(collision, _stayEvent, _stayDamagePerSecond * Time.deltaTime);
    }

    private void OnTriggerExit(Collider collision)
    {
        HitHero(collision, _exitEvent, _exitDamage);
    }

    private void HitHero(Collider collision, UnityEvent<Collider> hitEvent, float abilityDamage)
    {
        if (DoesColliderBelongToHero(collision))
        {
            hitEvent?.Invoke(collision);

            DealDamage(collision.GetComponentInParent<HeroBase>(), abilityDamage);
        }
    }

    private void DealDamage(HeroBase heroBase, float abilityDamage)
    {
        if (abilityDamage >= 0)
            heroBase.GetHeroStats().DealDamageToHero(abilityDamage);
    }
}
