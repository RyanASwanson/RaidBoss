using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    /// <summary>
    /// Performs the set up for the damage area so that it knows it's owner
    /// </summary>
    /// <param name="heroBase"></param>
    public void SetUpDamageArea(HeroBase heroBase)
    {
        _myHeroBase = heroBase;
    }

    #region Collision

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
            StartDisableColliderForDuration(_stayDamageTickRate);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!enabled) return;

        HitBoss(collision, _exitEvent, _exitDamage, _exitStagger);
    }

    private bool HitBoss(Collider collision, UnityEvent<Collider> hitEvent, float abilityDamage, float abilityStagger)
    {
        if(TagStringData.DoesColliderBelongToBoss(collision))
        {
            hitEvent?.Invoke(collision);

            DealDamageAndStagger(abilityDamage, abilityStagger);

            return true;
        }
        return false;
    }

    private void DealDamageAndStagger(float abilityDamage, float abilityStagger)
    {
        if (_myHeroBase.IsUnityNull())
        {
            Debug.Log("Cant Find Hero Base");
            return;
        }

        // Check if damage is more than 0 to prevent negative damage
        if (abilityDamage > 0)
        {
            _myHeroBase.GetSpecificHeroScript().DamageBoss(abilityDamage * _damageMultiplier);
        }

        // Check if stagger is more than 0 to prevent negative damage
        if (abilityStagger > 0)
        {
            _myHeroBase.GetSpecificHeroScript().StaggerBoss(abilityStagger * _staggerMultiplier);
        }
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
