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
    [SerializeField] private Vector3 _manualWallExtents;

    [SerializeField] private LayerMask _bounceLayers;
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

            //Performs the setup for the damage area so that it knows it's owner
            newestProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        }
    }


    #endregion

    #region Manual Abilities

    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        //Determines the direction between the hero and the location they used the manual at
        _currentManualDirection = attackLocation - transform.position;
        //Makes sure there is no y value then normalizes
        _currentManualDirection = new Vector3(_currentManualDirection.x, 0, _currentManualDirection.z).normalized;

        //Starts the manual ability functionality
        StartCoroutine(ManualProcess());
    }


    /// <summary>
    /// Forcibly moves the hero by sending them in the direction they set as their manual attack location
    /// </summary>
    /// <returns></returns>
    private IEnumerator ManualProcess()
    {
        //Starts by stopping the pathfinding so that they aren't being moved by multiple sources
        _myHeroBase.GetPathfinding().StopAbilityToMove();

        //The manual goes on until the duration has run out
        float manualProgress = 0;
        while (manualProgress < _manualDuration)
        {
            //Checks if there is any surface to bounce off of
            CheckManualRedirect();

            //Moves the character in the manual direction
            //Speed determined by movement speed of the character and the multipler for the manual
            _myHeroBase.gameObject.transform.position += _currentManualDirection * 
                _heroStats.GetCurrentSpeed() *_manualSpeedMultiplier * Time.deltaTime;

            manualProgress += Time.deltaTime;
            yield return null;
        }

        //Reenables the pathfinding functionality
        _myHeroBase.GetPathfinding().EnableAbilityToMove();
        //Makes sure that the hero doesn't try to continue any previous pathfinding
        _myHeroBase.GetPathfinding().DirectNavigationTo(_environmentManager.GetClosestPointToFloor(transform.position));
    }

    /// <summary>
    /// Checks if the Fae's manual ability can bounce off any surface
    /// </summary>
    private void CheckManualRedirect()
    {
        Vector3 startPos = new Vector3(transform.position.x, 0, transform.position.z);
        _currentManualDirection = new Vector3(_currentManualDirection.x, 0, _currentManualDirection.z);

        //Checks for object to bounce off
        if(Physics.BoxCast(startPos,_manualWallExtents,_currentManualDirection, out RaycastHit rayHit, 
            Quaternion.identity,_manualWallDistanceRange,_bounceLayers))
        {
            //Reflect the direction that the manual ability is moving
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
