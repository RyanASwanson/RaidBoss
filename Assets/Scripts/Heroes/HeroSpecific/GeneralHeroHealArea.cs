using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneralHeroHealArea : GeneralAbilityAreaFramework
{

    [SerializeField] private bool _ignoreFullHealth;

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

    private HeroBase _myHeroBase;

    public void SetUpHealingArea(HeroBase heroBase)
    {
        _myHeroBase = heroBase;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!enabled) return;

        HitHero(collision, _enterEvent, _enterHealing);
    }

    private void OnTriggerStay(Collider collision)
    {
        if (!enabled) return;

        if (HitHero(collision, _stayEvent, _stayHealingPerTick) && _stayHealingTickRate > 0)
        {
            // TODO Rework to ignore hero rather than disable collider
            StartDisableColliderForDuration(_stayHealingTickRate);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!enabled) return;

        HitHero(collision, _exitEvent, _exitHealing);
    }

    private bool HitHero(Collider collision, UnityEvent<Collider> hitEvent, float abilityHealing)
    {
        if (TagStringData.DoesColliderBelongToHero(collision))
        {
            HeroBase heroBase = collision.GetComponentInParent<HeroBase>();
            
            if (heroBase.GetHeroStats().ShouldOverrideHealing())
            {
                return false;
            }
            
            if (_ignoreFullHealth && heroBase.GetHeroStats().IsHeroMaxHealth())
            {
                return false;
            }

            hitEvent?.Invoke(collision);

            DealHealing(heroBase, abilityHealing);

            return true;
        }
        return false;
    }

    private void DealHealing(HeroBase heroBase, float abilityHealing)
    {
        if (abilityHealing > 0)
        {
            _myHeroBase.GetSpecificHeroScript().HealTargetHero(abilityHealing, heroBase);
        }
            
    }

    #region Getters
    public UnityEvent<Collider> GetEnterEvent() => _enterEvent;
    public UnityEvent<Collider> GetStayEvent() => _stayEvent;
    public UnityEvent<Collider> GetExitEvent() => _exitEvent;
    #endregion
}
