using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains the functionality that is generally used for boss attacks
/// </summary>
public class GeneralBossDamageArea : GeneralAbilityAreaFramework
{
    [SerializeField] private Collider _damageCollider;

    [Space]
    [SerializeField] private bool _hasLifetime;
    [SerializeField] private float _lifetime;

    [Space]
    [SerializeField] private float _preventReHitDuration;
    [Space]
    [SerializeField] private GameObject _hitCenteredVFX;

    [Header("Enter")]
    [SerializeField] private float _enterDamage;
    [SerializeField] private UnityEvent<Collider> _enterEvent;

    [Header("Stay")]
    [SerializeField] private float _stayDamagePerTick;
    [SerializeField] private UnityEvent<Collider> _stayEvent;

    [Header("Exit")]
    [SerializeField] private float _exitDamage;
    [SerializeField] private UnityEvent<Collider> _exitEvent;

    private List<HeroBase> _heroesToIgnore = new List<HeroBase>();


    private bool DoesColliderBelongToHero(Collider collision)
    {
        return collision.gameObject.CompareTag(TagStringData.GetHeroHitboxTagName());
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!enabled) return;

        HitHero(collision, _enterEvent, _enterDamage);
    }

    private void OnTriggerStay(Collider collision)
    {
        if (!enabled) return;

        HitHero(collision, _stayEvent, _stayDamagePerTick);
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!enabled) return;

        HitHero(collision, _exitEvent, _exitDamage);
    }

    private bool HitHero(Collider collision, UnityEvent<Collider> hitEvent, float abilityDamage)
    {
        if (DoesColliderBelongToHero(collision))
        {
            HeroBase heroBase = collision.GetComponentInParent<HeroBase>();
            if (_heroesToIgnore.Contains(heroBase))
                return false;

            hitEvent?.Invoke(collision);

            if (_preventReHitDuration > 0 && abilityDamage > 0)
                StartCoroutine(IgnoreHeroForDuration(collision.gameObject.GetComponentInParent<HeroBase>()));

            DealDamage(heroBase, abilityDamage);

            return true;
        }
        return false;
    }

    private void DealDamage(HeroBase heroBase, float abilityDamage)
    {
        if (abilityDamage > 0)
            heroBase.GetHeroStats().DealDamageToHero(abilityDamage * GameplayManagers.Instance.
                GetBossManager().GetBossBase().GetBossStats().GetBossDamageMultiplier());
    }

    private IEnumerator IgnoreHeroForDuration(HeroBase heroBase)
    {
        _heroesToIgnore.Add(heroBase);
        yield return new WaitForSeconds(_preventReHitDuration);
        _heroesToIgnore.Remove(heroBase);
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

    public void CreateHitDestructionVFX()
    {
        if (_hitCenteredVFX == null) return;
        Instantiate(_hitCenteredVFX, transform.position, Quaternion.identity);
    }

    public void DestroyProjectile()
    {
        CreateHitDestructionVFX();
        Destroy(gameObject);
    }
}
