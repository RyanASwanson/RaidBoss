using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Astromancer's manual ability
/// </summary>
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

    /// <summary>
    /// Performs the set up needed to make the ability work
    /// </summary>
    private void InitialSetup()
    {
        Vector3 heroLoc = new Vector3(_myHeroBase.transform.position.x, 
            _myHeroBase.transform.position.y + _yOffset, _myHeroBase.transform.position.z);
        

        Vector3 bossLoc = BossManager.Instance.GetBossBaseGameObject().transform.position;
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
        
        _createdVFX.Add(heroSideVFX);

        GameObject bossSideVFX = Instantiate(_bossSideAttackVFX, _bossSideVFXHolder.transform.position, Quaternion.identity);
        bossSideVFX.transform.LookAt(heroLoc);
        bossSideVFX.transform.eulerAngles = new Vector3(0, bossSideVFX.transform.eulerAngles.y, 0);

        _createdVFX.Add(bossSideVFX);
    }
    
    /// <summary>
    /// Starts updating the visuals and damage/stagger dealt
    /// </summary>
    private void StartProcesses()
    {
        StartCoroutine(UpdateVisuals());
        StartCoroutine(DamageTick());
    }

    private IEnumerator UpdateVisuals()
    {
        while(gameObject != null)
        {
            
            _visualsHolder.transform.Rotate(new Vector3(0, 0, Time.deltaTime * _rotationSpeed));
            yield return null;
        }
    }
    
    /// <summary>
    /// Deals damage and stagger to the boss every tick relative to the attack rate
    /// </summary>
    /// <returns></returns>
    private IEnumerator DamageTick()
    {
        while(!gameObject.IsUnityNull())
        {
            _myHeroBase.GetSpecificHeroScript().DamageBoss(_damage);
            _myHeroBase.GetSpecificHeroScript().StaggerBoss(_stagger);
            yield return new WaitForSeconds(_attackRate);
        }
    }
    
    /// <summary>
    /// Stops the manual ability and removes the vfx
    /// </summary>
    public void StopManual()
    {
        DestroyVFX();
        Destroy(gameObject);
    }

    /// <summary>
    /// Removes all vfx associated will the ability
    /// </summary>
    private void DestroyVFX()
    {
        //TODO rework to have _createdVFX store the actual vfx script
        foreach(GameObject vfx in _createdVFX)
        {
            GeneralVFXFunctionality generalVFX = vfx.GetComponent<GeneralVFXFunctionality>();
            //Stops the looping of the vfx
            generalVFX.SetLoopOfParticleSystems(false);
            //Makes the vfx not childed to the ability
            generalVFX.Detach();
            //Starts the destruction of the vfx
            generalVFX.StartDelayedLifetime();
        }
    }

    #region Base Ability
    public override void SetUpProjectile(HeroBase heroBase)
    {
        base.SetUpProjectile(heroBase);
        InitialSetup();
        SubscribeToEvents();
        StartProcesses();
    }

    private void SubscribeToEvents()
    {
        _myHeroBase.GetHeroStartedMovingEvent().AddListener(StopManual);
    }

    #endregion
}
