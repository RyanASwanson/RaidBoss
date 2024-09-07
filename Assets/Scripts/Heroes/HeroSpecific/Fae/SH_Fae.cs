using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Fae hero
/// </summary>
public class SH_Fae : SpecificHeroFramework
{
    [Space]
    [SerializeField] private List<Vector3> _primaryAttackEulers;
    [SerializeField] private float _projectileSpawnDistance;
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private float _manualContactDamage;
    [SerializeField] private float _manualContactStagger;
    [Space]
    [SerializeField] private float _manualDuration;
    [SerializeField] private float _accelerateTime;
    [SerializeField] private float _manualSpeedMultiplier;
    [SerializeField] private float _manualWallDistanceRange;
    [Range(0,1)][SerializeField] private float _manualBossHoming;
    [SerializeField] private Vector3 _manualWallExtents;

    [SerializeField] private LayerMask _bounceLayers;
    private bool _manualActive = false;

    private Vector3 _currentManualDirection;
    private float _currentAccelerationMultiplier;

    private Coroutine _manualCoroutine;


    [Space]
    [SerializeField] private float _passiveBasicAttackSpeedChangeWalking;
    [SerializeField] private float _passiveBasicAttackSpeedChangeManual;

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
        _manualCoroutine = StartCoroutine(ManualProcess());

        StartCoroutine(ManualAccelerationAndDeceleration());
    }


    /// <summary>
    /// Forcibly moves the hero by sending them in the direction they set as their manual attack location
    /// </summary>
    /// <returns></returns>
    private IEnumerator ManualProcess()
    {
        //Starts by stopping the pathfinding so that they aren't being moved by multiple sources
        _myHeroBase.GetPathfinding().StopAbilityToMove();

        IncreaseBasicAttackSpeedOnManualStart();

        //The manual goes on until the duration has run out
        float manualProgress = 0;
        while (manualProgress < _manualDuration)
        {
            //Checks if there is any surface to bounce off of
            CheckManualRedirect();

            //Moves the character in the manual direction
            //Speed determined by movement speed of the character and the multipler for the manual
            _myHeroBase.gameObject.transform.position += _currentManualDirection * 
                _heroStats.GetCurrentSpeed() *_manualSpeedMultiplier * _currentAccelerationMultiplier* Time.deltaTime;

            manualProgress += Time.deltaTime;
            yield return null;
        }

        //Reenables the pathfinding functionality
        _myHeroBase.GetPathfinding().EnableAbilityToMove();
        //Makes sure that the hero doesn't try to continue any previous pathfinding
        _myHeroBase.GetPathfinding().DirectNavigationTo(_environmentManager.GetClosestPointToFloor(transform.position));

        DecreaseBasicAttackSpeedOnManualEnd();

        print("end1");
    }

    private IEnumerator ManualAccelerationAndDeceleration()
    {
        while (_currentAccelerationMultiplier < 1)
        {
            _currentAccelerationMultiplier += Time.deltaTime / _accelerateTime;
            yield return null;
        }
        _currentAccelerationMultiplier = 1;

        yield return new WaitForSeconds(_manualDuration - (2 * _accelerateTime));


        while (_currentAccelerationMultiplier > 0)
        {
            _currentAccelerationMultiplier -= Time.deltaTime / _accelerateTime;
            yield return null;
        }
        _currentAccelerationMultiplier = 0;

        print("end2");

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

            if (ManualHitBoss(rayHit))
            {
                DamageBoss(_manualContactDamage);
                StaggerBoss(_manualContactStagger);
                return;
            }
                
            

            _currentManualDirection = Vector3.Lerp(_currentManualDirection, 
                GameplayManagers.Instance.GetBossManager().GetDirectionToBoss(transform.position), _manualBossHoming).normalized;
        }

    }

    private bool ManualHitBoss(RaycastHit rayHit)
    {
        return rayHit.collider.gameObject.tag == TagStringData.GetBossHitboxTagName();
    }

    #endregion

    #region Passive Abilities
    private void IncreaseBasicAttackSpeedOnMoveStart()
    {
        _currentPassiveBasicAttackSpeed += _passiveBasicAttackSpeedChangeWalking;
    }

    private void DecreaseBasicAttackSpeedOnMoveEnd()
    {
        _currentPassiveBasicAttackSpeed -= _passiveBasicAttackSpeedChangeWalking;
    }

    private void IncreaseBasicAttackSpeedOnManualStart()
    {
        _currentPassiveBasicAttackSpeed += _passiveBasicAttackSpeedChangeManual;
    }

    private void DecreaseBasicAttackSpeedOnManualEnd()
    {
        _currentPassiveBasicAttackSpeed -= _passiveBasicAttackSpeedChangeManual;
    }

    #endregion




    #region Base Hero
    public override void SetupSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        _heroStats = heroBase.GetHeroStats();
        _environmentManager = GameplayManagers.Instance.GetEnvironmentManager();

        base.SetupSpecificHero(heroBase, heroSO);
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _myHeroBase.GetHeroStartedMovingEvent().AddListener(IncreaseBasicAttackSpeedOnMoveStart);
        _myHeroBase.GetHeroStoppedMovingEvent().AddListener(DecreaseBasicAttackSpeedOnMoveEnd);
    }
    #endregion
}
