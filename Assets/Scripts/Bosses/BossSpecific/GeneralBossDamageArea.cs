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
    [SerializeField] private float _stayDamagePerTick;
    [SerializeField] private float _stayDamageTickRate;
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
        if(HitHero(collision, _stayEvent, _stayDamagePerTick * Time.deltaTime) && (_stayDamageTickRate > 0))
        {
            StartCoroutine(DisableColliderForDuration(_stayDamageTickRate));
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        HitHero(collision, _exitEvent, _exitDamage);
    }

    private bool HitHero(Collider collision, UnityEvent<Collider> hitEvent, float abilityDamage)
    {
        if (DoesColliderBelongToHero(collision))
        {
            hitEvent?.Invoke(collision);

            DealDamage(collision.GetComponentInParent<HeroBase>(), abilityDamage);

            return true;
        }
        return false;
    }

    private void DealDamage(HeroBase heroBase, float abilityDamage)
    {
        if (abilityDamage >= 0)
            heroBase.GetHeroStats().DealDamageToHero(abilityDamage);
    }

    public void ToggleProjectileCollider(bool colliderEnabled)
    {
        _damageCollider.enabled = colliderEnabled;
    }

    private IEnumerator DisableColliderForDuration(float duration)
    {
        ToggleProjectileCollider(false);
        yield return new WaitForSeconds(duration);
        ToggleProjectileCollider(true);
    }
}
