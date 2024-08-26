using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHP_AstromancerManualProjectile : HeroProjectileFramework
{
    [SerializeField] private float _damage;
    [SerializeField] private float _stagger;
    [SerializeField] private float _attackRate;

    [Space]
    [SerializeField] private float _rotationSpeed;

    [Space]
    [SerializeField] private GameObject _visualsHolder;

    //[SerializeField] private LineRenderer _lineRenderer;

    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
        InitialSetup();
        SubscribeToEvents();
        StartProcesses();
    }

    /*public void AdditionalSetup()
    {
        
    }*/

    private void InitialSetup()
    {
        

        Vector3 heroLoc = new Vector3(_myHeroBase.transform.position.x, 0, _myHeroBase.transform.position.z);
        Vector3 bossLoc = GameplayManagers.Instance.GetBossManager().GetBossBaseGameObject().transform.position;
        bossLoc = new Vector3(bossLoc.x, 0, bossLoc.z);

        float length = Vector3.Distance(heroLoc, bossLoc);
        Debug.Log(length);

        transform.position = Vector3.Lerp(heroLoc, bossLoc, .5f);

        transform.LookAt(_myHeroBase.transform.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        _visualsHolder.transform.localScale = new Vector3(_visualsHolder.transform.localScale.x,
            _visualsHolder.transform.localScale.y, length * _visualsHolder.transform.localScale.z);
    }
    

    private void StartProcesses()
    {
        //_lineRenderer.SetPosition(1, GameplayManagers.Instance.GetBossManager().GetBossBaseGameObject().transform.position);
        StartCoroutine(UpdateVisuals());
        StartCoroutine(DamageTick());
    }

    private IEnumerator UpdateVisuals()
    {
        while(gameObject != null)
        {
            //_lineRenderer.SetPosition(0, transform.position);
            _visualsHolder.transform.Rotate(new Vector3(0, 0, Time.deltaTime * _rotationSpeed));
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

    public void StopLaser()
    {
        Destroy(gameObject);
    }

    private void SubscribeToEvents()
    {
        _myHeroBase.GetHeroStartedMovingEvent().AddListener(StopLaser);
    }
}
