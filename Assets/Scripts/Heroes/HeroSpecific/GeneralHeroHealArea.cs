using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneralHeroHealArea : MonoBehaviour
{
    [SerializeField] private Collider _healingCollider;

    [Header("Enter")]
    [SerializeField] private float _enterHealing;
    [SerializeField] private UnityEvent<Collider> _enterEvent;

    [Header("Stay")]
    [SerializeField] private float _stayHealingPerTick;
    [SerializeField] private float _stayHealingTickRate;
    [SerializeField] private UnityEvent<Collider> _stayEvent;

    [Header("Exit")]
    [SerializeField] private float _exitHealing;
    [SerializeField] private UnityEvent<Collider> _exitEvent;

    private bool DoesColliderBelongToHero(Collider collision)
    {
        return collision.gameObject.CompareTag(TagStringData.GetHeroHitboxTagName());
    }

    private void OnTriggerEnter(Collider collision)
    {
        HitHero(collision, _enterEvent, _enterHealing);
    }

    private void OnTriggerStay(Collider collision)
    {
        if (HitHero(collision, _stayEvent, _stayHealingPerTick) && _stayHealingTickRate > 0)
        {
            StartCoroutine(DisableColliderForDuration(_stayHealingTickRate));
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        HitHero(collision, _exitEvent, _exitHealing);
    }

    private bool HitHero(Collider collision, UnityEvent<Collider> hitEvent, float abilityHealing)
    {
        if (DoesColliderBelongToHero(collision) && 
            !collision.GetComponentInParent<HeroBase>().GetHeroStats().IsHeroMaxHealth())
        {
            hitEvent?.Invoke(collision);

            DealHealing(collision.GetComponentInParent<HeroBase>(), abilityHealing);

            return true;
        }
        return false;
    }

    private void DealHealing(HeroBase heroBase, float abilityHealing)
    {
        if (abilityHealing > 0)
            heroBase.GetHeroStats().HealHero(abilityHealing);
    }

    public void ToggleProjectileCollider(bool colliderEnabled)
    {
        _healingCollider.enabled = colliderEnabled;
    }

    private IEnumerator DisableColliderForDuration(float duration)
    {
        ToggleProjectileCollider(false);
        yield return new WaitForSeconds(duration);
        ToggleProjectileCollider(true);
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
