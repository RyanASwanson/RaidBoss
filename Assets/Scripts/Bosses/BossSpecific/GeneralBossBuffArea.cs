using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneralBossBuffArea : GeneralAbilityAreaFramework
{
    [Space]
    [SerializeField] private bool _canPreventReHitWithoutInitialHealing;
    [SerializeField] private bool _canHitFullHealthHeroes;
    [SerializeField] private float _preventReHitDuration;
    private float _multiplicativeHealingMultiplier = 1;
    [Space]

    [Header("Enter")]
    [SerializeField] private float _enterHealing;
    [SerializeField] private UnityEvent<Collider> _enterEvent;

    [Header("Stay")]
    [SerializeField] private float _stayHealingPerTick;
    [SerializeField] private UnityEvent<Collider> _stayEvent;

    [Header("Exit")]
    [SerializeField] private float _exitHealing;
    [SerializeField] private UnityEvent<Collider> _exitEvent;

    [Space] 
    [Header("Hit")]
    [SerializeField] private UnityEvent<HeroBase> _generalHitEvent;

    private List<HeroBase> _heroesToIgnore = new();

    #region Collision
    private void OnTriggerEnter(Collider collision)
    {
        if (!IsCollisionActive())
        {
            return;
        }

        HitHero(collision, _enterEvent, _enterHealing);
    }
    
    private void OnTriggerStay(Collider collision)
    {
        if (!IsCollisionActive())
        {
            return;
        }

        HitHero(collision, _stayEvent, _stayHealingPerTick);
    }
    
    private void OnTriggerExit(Collider collision)
    {
        if (!IsCollisionActive())
        {
            return;
        }

        HitHero(collision, _exitEvent, _exitHealing);
    }


    private bool HitHero(Collider collision, UnityEvent<Collider> hitEvent, float abilityHealing)
    {
        //Checks if the attack hit a hero
        if (TagStringData.DoesColliderBelongToHero(collision))
        {
            //If the hero should be ignored then return
            HeroBase heroBase = collision.GetComponentInParent<HeroBase>();
            if (_heroesToIgnore.Contains(heroBase))
            {
                return false;
            }

            if (!_canHitFullHealthHeroes && heroBase.GetHeroStats().IsHeroMaxHealth())
            {
                return false;
            }

            hitEvent?.Invoke(collision);
            
            _generalHitEvent?.Invoke(heroBase);

            //Ignores the hero for a duration
            if (_preventReHitDuration > 0 && (_canPreventReHitWithoutInitialHealing || abilityHealing > 0))
            {
                StartCoroutine(IgnoreHeroForDuration(heroBase));
            }

            //Deals damage to the hero
            DealHealing(heroBase, abilityHealing);

            return true;
        }
        return false;
    }
    #endregion
    
    public void DealHealing(HeroBase heroBase, float abilityHealing)
    {
        BossBase.Instance.GetSpecificBossScript().HealHero(heroBase,abilityHealing*_multiplicativeHealingMultiplier);
    }
    
    private IEnumerator IgnoreHeroForDuration(HeroBase heroBase)
    {
        _heroesToIgnore.Add(heroBase);
        yield return new WaitForSeconds(_preventReHitDuration);
        _heroesToIgnore.Remove(heroBase);
    }
    
    #region Getters

    public UnityEvent<Collider> GetEnterEvent() => _enterEvent;
    public UnityEvent<Collider> GetStayEvent() => _stayEvent;
    public UnityEvent<Collider> GetExitEvent() => _exitEvent;
    
    public UnityEvent<HeroBase> GetGeneralHitEvent() => _generalHitEvent;

    #endregion
    
    #region Setters
    public void MultiplyHealingMultiplier(float multiplier)
    {
        _multiplicativeHealingMultiplier *= multiplier;
    }
    
    public void SetEnterHealing(float healing) => _enterHealing = healing;
    public void SetStayHealing(float healing) => _stayHealingPerTick = healing;
    public void SetExitHealing(float healing) => _exitHealing = healing;
    #endregion
}
