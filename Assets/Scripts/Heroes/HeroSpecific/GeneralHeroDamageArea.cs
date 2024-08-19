using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains the functionality that is generally used for hero attacks to do damage
/// </summary>
public class GeneralHeroDamageArea : GeneralAbilityAreaFramework
{

    [Header("Enter")]
    [SerializeField] private float _enterDamage;
    [SerializeField] private float _enterStagger;
    [SerializeField] private UnityEvent<Collider> _enterEvent;

    [Header("Stay")]
    [SerializeField] private float _stayDamagePerTick;
    [SerializeField] private float _stayStaggerPerTick;
    [SerializeField] private float _stayDamageTickRate;
    [SerializeField] private UnityEvent<Collider> _stayEvent;

    [Header("Exit")]
    [SerializeField] private float _exitDamage;
    [SerializeField] private float _exitStagger;
    [SerializeField] private UnityEvent<Collider> _exitEvent;

    private HeroBase _myHeroBase;

    private float _damageMultiplier = 1;
    private float _staggerMultiplier = 1;

    public void SetUpDamageArea(HeroBase heroBase)
    {
        _myHeroBase = heroBase;
    }

    #region Collision
    private bool DoesColliderBelongToBoss(Collider collision)
    {
        return collision.gameObject.CompareTag(TagStringData.GetBossHitboxTagName());
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!enabled) return;

        HitBoss(collision, _enterEvent, _enterDamage, _enterStagger);
    }

    private void OnTriggerStay(Collider collision)
    {
        if (!enabled) return;

        if (HitBoss(collision, _stayEvent,
            _stayDamagePerTick, _stayStaggerPerTick) && (_stayDamageTickRate > 0))
        {
            StartCoroutine(DisableColliderForDuration(_stayDamageTickRate));
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!enabled) return;

        HitBoss(collision, _exitEvent, _exitDamage, _exitStagger);
    }

    private bool HitBoss(Collider collision, UnityEvent<Collider> hitEvent, float abilityDamage, float abilityStagger)
    {
        if(DoesColliderBelongToBoss(collision))
        {
            hitEvent?.Invoke(collision);

            DealDamageAndStagger(abilityDamage, abilityStagger);

            return true;
        }
        return false;
    }

    private void DealDamageAndStagger(float abilityDamage, float abilityStagger)
    {
        if (_myHeroBase == null)
            Debug.Log("Cant Find Hero Base");

        if (abilityDamage > 0)
            _myHeroBase.GetSpecificHeroScript().DamageBoss(abilityDamage * _damageMultiplier);
            //bossBase.GetBossStats().DealDamageToBoss(abilityDamage);

        if (abilityStagger > 0)
            _myHeroBase.GetSpecificHeroScript().StaggerBoss(abilityStagger * _staggerMultiplier);
            //bossBase.GetBossStats().DealStaggerToBoss(abilityStagger);
    }

    #endregion

    #region Setters

    public void SetDamageMultiplier(float newDamageMultiplier)
    {
        _damageMultiplier = newDamageMultiplier;
    }
    public void SetStaggerMultiplier(float newStaggerMultiplier)
    {
        _staggerMultiplier = newStaggerMultiplier;
    }

    public void IncreaseDamageMultiplierByAmount(float changeAmount)
    {
        SetDamageMultiplier(_damageMultiplier + changeAmount);
    }

    public void IncreaseStaggerMultiplierByAmount(float changeAmount)
    {
        SetStaggerMultiplier(_staggerMultiplier + changeAmount);
    }

    #endregion

    #region Getters
    public UnityEvent<Collider> GetEnterEvent() => _enterEvent;
    public UnityEvent<Collider> GetStayEvent() => _stayEvent;
    public UnityEvent<Collider> GetExitEvent() => _exitEvent;
    #endregion
}
