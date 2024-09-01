using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Fae : SpecificHeroFramework
{
    [Space]
    [SerializeField] private List<Vector3> _primaryAttackEulers;
    [SerializeField] private float _projectileSpawnDistance;
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private float _manualDuration;
    [SerializeField] private float _manualSpeedMultiplier;
    [SerializeField] private float _manualWallDistanceRange;
    private bool _manualActive = false;

    private Vector3 _currentManualDirection;

    private Coroutine _manualCoroutine;


    [Space]
    [SerializeField] private float _passiveBasicAttackSpeedChange;
    private float _currentPassiveBasicAttackSpeed = 1;

    private HeroStats _heroStats;
    private EnvironmentManager _environmentManager;

    #region Basic Abilities

    protected override void CooldownAddToBasicAbilityCharge(float addedAmount)
    {
        base.CooldownAddToBasicAbilityCharge(addedAmount * _currentPassiveBasicAttackSpeed);
    }

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    public override bool ConditionsToActivateBasicAbilities()
    {
        return true;
    }

    protected void CreateBasicAttackProjectiles()
    {
        foreach(Vector3 attackEuler in _primaryAttackEulers)
        {
            GameObject newestProjectile = Instantiate(_basicProjectile, transform.position, Quaternion.Euler(attackEuler));
            newestProjectile.transform.position = newestProjectile.transform.position + 
                (newestProjectile.transform.forward * _projectileSpawnDistance);

            newestProjectile.GetComponent<SHP_FaeBasicProjectile>().SetUpProjectile(_myHeroBase);

            newestProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        }
    }


    #endregion

    #region Manual Abilities

    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        _currentManualDirection = attackLocation - transform.position;
        _currentManualDirection = new Vector3(_currentManualDirection.x, 0, _currentManualDirection.z).normalized;

        StartCoroutine(ManualProcess());
    }

    private IEnumerator ManualProcess()
    {
        _myHeroBase.GetPathfinding().StopAbilityToMove();

        float manualProgress = 0;
        while (manualProgress < _manualDuration)
        {
            CheckManualRedirect();

            _myHeroBase.gameObject.transform.position += _currentManualDirection * 
                _heroStats.GetCurrentSpeed() *_manualSpeedMultiplier * Time.deltaTime;

            manualProgress += Time.deltaTime;
            yield return null;
        }

        _myHeroBase.GetPathfinding().EnableAbilityToMove();
    }

    private void CheckManualRedirect()
    {
        
        if(_environmentManager.GetEdgeOfMapRayHit(transform.position,
            _currentManualDirection, _manualWallDistanceRange, out RaycastHit rayHit))
        {
            print(rayHit.normal);

            _currentManualDirection = Vector3.Reflect(_currentManualDirection, rayHit.normal);
        }
        
    }

    #endregion

    #region Passive Abilities
    private void IncreaseBasicAttackSpeed()
    {
        _currentPassiveBasicAttackSpeed += _passiveBasicAttackSpeedChange;
    }

    private void DecreaseBasicAttackSpeed()
    {
        _currentPassiveBasicAttackSpeed -= _passiveBasicAttackSpeedChange;
    }
    #endregion


    public override void SetupSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        _heroStats = heroBase.GetHeroStats();
        _environmentManager = GameplayManagers.Instance.GetEnvironmentManager();

        base.SetupSpecificHero(heroBase, heroSO);
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _myHeroBase.GetHeroStartedMovingEvent().AddListener(IncreaseBasicAttackSpeed);
        _myHeroBase.GetHeroStoppedMovingEvent().AddListener(DecreaseBasicAttackSpeed);
    }
}
