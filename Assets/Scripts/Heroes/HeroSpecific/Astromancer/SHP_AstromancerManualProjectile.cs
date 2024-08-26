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
    [SerializeField] private float _yOffset;
    
    [Space]
    [SerializeField] private GameObject _visualsHolder;
    [SerializeField] private GameObject _heroSideVFXHolder;
    [SerializeField] private GameObject _bossSideVFXHolder;

    [Space]
    [SerializeField] private GameObject _heroSideAttackVFX;
    [SerializeField] private GameObject _bossSideAttackVFX;

    private List<GameObject> _createdVFX = new List<GameObject>();
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


        Vector3 heroLoc = new Vector3(_myHeroBase.transform.position.x, 
            _myHeroBase.transform.position.y + _yOffset, _myHeroBase.transform.position.z);
        //Vector3 heroLoc = _myHeroBase.transform.position;
        Vector3 bossLoc = GameplayManagers.Instance.GetBossManager().GetBossBaseGameObject().transform.position;
        bossLoc = new Vector3(bossLoc.x, 0, bossLoc.z);

        float length = Vector3.Distance(heroLoc, bossLoc);

        transform.position = Vector3.Lerp(heroLoc, bossLoc, .5f);

        transform.LookAt(_myHeroBase.transform.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        _visualsHolder.transform.localScale = new Vector3(_visualsHolder.transform.localScale.x,
            _visualsHolder.transform.localScale.y, length * _visualsHolder.transform.localScale.z);

        CreateInitialVFX(heroLoc, bossLoc, length);
    }

    private void CreateInitialVFX(Vector3 heroLoc, Vector3 bossLoc, float length)
    {
        

        GameObject heroSideVFX = Instantiate(_heroSideAttackVFX, _heroSideVFXHolder.transform.position, Quaternion.identity);
        heroSideVFX.transform.LookAt(bossLoc);
        heroSideVFX.transform.eulerAngles = new Vector3(0,heroSideVFX.transform.eulerAngles.y, 0);

        /*GameObject heroSideVFXScale = heroSideVFX.GetComponent<GeneralVFXFunctionality>().GetParticleSystems()[0].gameObject;
        heroSideVFXScale.transform.localScale = new Vector3(heroSideVFXScale.transform.localScale.x,
            heroSideVFXScale.transform.localScale.y, _visualsHolder.transform.localScale.z);*/

        _createdVFX.Add(heroSideVFX);

        GameObject bossSideVFX = Instantiate(_bossSideAttackVFX, _bossSideVFXHolder.transform.position, Quaternion.identity);
        bossSideVFX.transform.LookAt(heroLoc);
        bossSideVFX.transform.eulerAngles = new Vector3(0, bossSideVFX.transform.eulerAngles.y, 0);

        _createdVFX.Add(bossSideVFX);
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
        DestroyVFX();
        Destroy(gameObject);
    }

    private void DestroyVFX()
    {
        foreach(GameObject vfx in _createdVFX)
        {
            GeneralVFXFunctionality generalVFX = vfx.GetComponent<GeneralVFXFunctionality>();
            generalVFX.SetLoopOfParticleSystems(false);
            generalVFX.Detach();
            generalVFX.StartDelayedLifetime();
        }
    }

    private void SubscribeToEvents()
    {
        _myHeroBase.GetHeroStartedMovingEvent().AddListener(StopLaser);
    }
}
