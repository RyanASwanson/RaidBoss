using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_AstromancerBasicProjectile : HeroProjectileFramework
{
    [SerializeField] private float _projectileSpeed;
    [Space]

    [SerializeField] private GeneralHeroDamageArea _generalDamageArea;
    [SerializeField] private GeneralHeroHealArea _generalHealArea;

    private bool _hasHitBoss;

    private SH_Astromancer _astromancerScript;

    private Vector3 _storedDirection;

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
    }

    private void SubscribeToEvents()
    {
        _generalDamageArea.GetEnterEvent().AddListener(HitBoss);
        _generalDamageArea.GetStayEvent().AddListener(HitBoss);
        _generalHealArea.GetEnterEvent().AddListener(HitHero);
        _generalHealArea.GetStayEvent().AddListener(HitHero);
    }


    private IEnumerator MoveProjectile()
    {
        while (true)
        {
            transform.position += _storedDirection * _projectileSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public void HitBoss(Collider collider)
    {
        if (!_hasHitBoss)
        {
            _hasHitBoss = true;

            _generalDamageArea.enabled = false;
            _generalHealArea.enabled = true;

            FlipDirection();
        }
           
        else
            _generalDamageArea.DestroyProjectile();
    }
    
    public void HitHero(Collider collider)
    {

        HeroBase storedSpecificHero = collider.gameObject.GetComponentInParent<HeroBase>();
        if (storedSpecificHero != _myHeroBase) return;

        _generalDamageArea.enabled = true;
        _generalHealArea.enabled = false;

        FlipDirection();
        TriggerHeroPassive();
    }

    private void FlipDirection()
    {
        _storedDirection *= -1;
    }

    public void TriggerHeroPassive()
    {
        _astromancerScript.ActivatePassiveAbilities();
    }
}