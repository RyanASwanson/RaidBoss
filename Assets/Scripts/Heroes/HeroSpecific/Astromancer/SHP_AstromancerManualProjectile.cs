using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_AstromancerManualProjectile : HeroProjectileFramework
{
    [SerializeField] private float _damage;
    [SerializeField] private float _stagger;
    [SerializeField] private float _attackRate;

    [SerializeField] private LineRenderer _lineRenderer;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
        SubscribeToEvents();
        StartProcesses();
    }

    /*public void AdditionalSetup()
    {
        
    }*/

    private void SubscribeToEvents()
    {
        _myHeroBase.GetHeroStartedMovingEvent().AddListener(StopLaser);
    }

    private void StartProcesses()
    {
        _lineRenderer.SetPosition(1, GameplayManagers.Instance.GetBossManager().GetBossBaseGameObject().transform.position);
        StartCoroutine(UpdateVisuals());
        StartCoroutine(DamageTick());
    }

    private IEnumerator UpdateVisuals()
    {
        while(gameObject != null)
        {
            _lineRenderer.SetPosition(0, transform.position);
            yield return null;
        }
    }
    
    private IEnumerator DamageTick()
    {
        while(gameObject != null)
        {
            _myHeroBase.GetSpecificHeroScript().DamageBoss(_damage);
            _myHeroBase.GetSpecificHeroScript().StaggerBoss(_stagger);
            yield return new WaitForSeconds(_attackRate);
        }
    }

    private void StopLaser()
    {
        Destroy(gameObject);
    }
}
