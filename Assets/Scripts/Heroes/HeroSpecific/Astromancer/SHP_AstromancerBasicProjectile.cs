using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_AstromancerBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _damageScalingPerSecond;
    [Space]

    [SerializeField] private GeneralHeroDamageArea _generalDamageArea;
    [SerializeField] private GeneralHeroHealArea _generalHealArea;

    private bool _hasHitBoss;

    private SH_Astromancer _astromancerScript;

    private Vector3 _storedDirection;

    private float _damageScalingAmount = 0;

    


    private IEnumerator MoveProjectile()
    {
        while (true)
        {
            transform.position += _storedDirection * _projectileSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator DamageScaling()
    {
        while(true)
        {
            _damageScalingAmount += _damageScalingPerSecond * Time.deltaTime;
            yield return null;
        }
    }

    private void AdjustDamageAreaScaling()
    {
        _generalDamageArea.IncreaseDamageMultiplierByAmount(_damageScalingAmount);
    }

    public void HitBoss(Collider collider)
    {
        AdjustDamageAreaScaling();
        if (!_hasHitBoss)
        {
            _damageScalingAmount = 0; 

            _hasHitBoss = true;

            _generalDamageArea.enabled = false;
            _generalHealArea.enabled = true;

            FlipDirection();
        }
           
        else
        {
            _generalDamageArea.DestroyProjectile();
        }
            
    }
    
    /// <summary>
    /// Checks if the projectile made contact with the astromancer
    /// </summary>
    /// <param name="collider"></param>
    public void HitHero(Collider collider)
    {

        HeroBase storedSpecificHero = collider.gameObject.GetComponentInParent<HeroBase>();
        if (storedSpecificHero != _myHeroBase) return;

        _generalDamageArea.enabled = true;
        _generalHealArea.enabled = false;

        FlipDirection();
        TriggerHeroPassive();
    }

    /// <summary>
    /// Causes the astromancer projectile to turn around
    /// </summary>
    private void FlipDirection()
    {
        _storedDirection *= -1;
    }

    public void TriggerHeroPassive()
    {
        _astromancerScript.ActivatePassiveAbilities();
    }


    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
        SubscribeToEvents();
    }

    public void AdditionalSetup(SH_Astromancer heroScript, Vector3 direction)
    {
        _astromancerScript = heroScript;

        _storedDirection = direction;
        StartCoroutine(MoveProjectile());
        StartCoroutine(DamageScaling());
    }

    private void SubscribeToEvents()
    {
        _generalDamageArea.GetEnterEvent().AddListener(HitBoss);
        _generalDamageArea.GetStayEvent().AddListener(HitBoss);
        _generalHealArea.GetEnterEvent().AddListener(HitHero);
        _generalHealArea.GetStayEvent().AddListener(HitHero);
    }
    #endregion
}
