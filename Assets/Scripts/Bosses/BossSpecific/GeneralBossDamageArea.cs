using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains the functionality that is generally used for boss attacks
/// </summary>
public class GeneralBossDamageArea : GeneralAbilityAreaFramework
{
    [Space]
    [SerializeField] private float _preventReHitDuration;
    [Space]

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

    private BossBase _myBossBase;

    protected override void Start()
    {
        base.Start();
        _myBossBase = GameplayManagers.Instance.GetBossManager().GetBossBase();
    }

    #region Collision
    /// <summary>
    /// Checks if the target hit is a hero
    /// </summary>
    /// <param name="collision"></param>
    /// <returns></returns>
    private bool DoesColliderBelongToHero(Collider collision)
    {
        return collision.gameObject.CompareTag(TagStringData.GetHeroHitboxTagName());
    }

    /// <summary>
    /// Deals damage when a hero enters the trigger
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (!enabled) return;

        HitHero(collision, _enterEvent, _enterDamage);
    }

    /// <summary>
    /// Deals damage when a hero stays in the trigger
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay(Collider collision)
    {
        if (!enabled) return;

        HitHero(collision, _stayEvent, _stayDamagePerTick);
    }

    /// <summary>
    /// Deals damage when a hero exits the trigger
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collider collision)
    {
        if (!enabled) return;

        HitHero(collision, _exitEvent, _exitDamage);
    }

    /// <summary>
    /// Checks if the attack hit a hero
    /// If it did it deals damage to them
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="hitEvent"></param>
    /// <param name="abilityDamage"></param>
    /// <returns></returns>
    private bool HitHero(Collider collision, UnityEvent<Collider> hitEvent, float abilityDamage)
    {
        //Checks if the attack hit a hero
        if (DoesColliderBelongToHero(collision))
        {
            //If the hero should be ignored then return
            HeroBase heroBase = collision.GetComponentInParent<HeroBase>();
            if (_heroesToIgnore.Contains(heroBase))
                return false;

            hitEvent?.Invoke(collision);

            //Ignores the hero for a duration
            if (_preventReHitDuration > 0 && abilityDamage > 0)
                StartCoroutine(IgnoreHeroForDuration(collision.gameObject.GetComponentInParent<HeroBase>()));

            //Deals damage to the hero
            DealDamage(heroBase, abilityDamage);

            return true;
        }
        return false;
    }
    #endregion

    /// <summary>
    /// Inflicts damage to the hero that it hit
    /// Damage dealt is scaled by difficulty
    /// </summary>
    /// <param name="heroBase"> The hero we are dealing damage to </param>
    /// <param name="abilityDamage"> The amount of damage being dealt </param>
    private void DealDamage(HeroBase heroBase, float abilityDamage)
    {
        if (abilityDamage > 0)
        {
            heroBase.GetHeroStats()
                .DealDamageToHero(abilityDamage * BossStats.Instance.GetCombinedBossDamageMultiplier());
        }
    }

    /// <summary>
    /// Prevents damage from being dealt to a hero for a duration
    /// </summary>
    /// <param name="heroBase"> The hero to ignore </param>
    /// <returns></returns>
    private IEnumerator IgnoreHeroForDuration(HeroBase heroBase)
    {
        _heroesToIgnore.Add(heroBase);
        yield return new WaitForSeconds(_preventReHitDuration);
        _heroesToIgnore.Remove(heroBase);
    }

}
