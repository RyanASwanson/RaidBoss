using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
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
    [SerializeField] private float _manualDamageCooldown;
    [Range(0,1)][SerializeField] private float _manualBossHoming;
    [SerializeField] private float _manualMinimumDotProduct;
    [SerializeField] private Vector3 _manualWallExtents;
    private bool _manualCanDamage = true;

    [Space] 
    [SerializeField] private float _manualAudioInterval;
    private WaitForSeconds _manualAudioWaitInterval;
    private Coroutine _manualAudioCoroutine;
    private EventInstance _manualAudioInstance;
    
    [Space]
    [SerializeField] private GameObject _swirlVFX;
    [SerializeField] private float _vfxWeaponSpawnRate;
    [SerializeField] private float _vfxWeaponDuration;
    [SerializeField] private float _vfxWeaponSpawnDistance;
    [SerializeField] private float _vfxWeaponDelay;
    [SerializeField] private Vector3 _vfxWeaponSpawnEulers;
    [SerializeField] private GameObject _vfxWeapon;
    [SerializeField] private Transform _vfxWeaponSpawnPoint;

    [Space]
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

            newestProjectile.GetComponent<SHP_FaeBasicProjectile>().SetUpProjectile(_myHeroBase, EHeroAbilityType.Basic);

            //Performs the set up for the damage area so that it knows it's owner
            newestProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        }
    }


    #endregion

    #region Manual Abilities

    public override void ActivateManualAbilities()
    {
        base.ActivateManualAbilities();

        // Old way to target boss
        /*//Determines the direction between the hero and the location they used the manual at
        _currentManualDirection = attackLocation - transform.position;
        //Makes sure there is no y value then normalizes
        _currentManualDirection = new Vector3(_currentManualDirection.x, 0, _currentManualDirection.z).normalized;*/

        _currentManualDirection = BossManager.Instance.GetDirectionToBoss(transform.position);

        CreateSwirlVFX();

        //Starts the manual ability functionality
        _manualCoroutine = StartCoroutine(ManualProcess());
        StartCoroutine(WeaponVFXSpawnProcess());
        StartCoroutine(ManualAccelerationAndDeceleration());

        StartManualAudioProcess();
    }


    /// <summary>
    /// Forcibly moves the hero by sending them in the direction they set as their manual attack location
    /// </summary>
    /// <returns></returns>
    private IEnumerator ManualProcess()
    {
        _manualActive = true;

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
            //Speed determined by movement speed of the character and the multiplier for the manual
            _myHeroBase.gameObject.transform.position += _currentManualDirection * 
                _heroStats.GetCurrentSpeed() *_manualSpeedMultiplier * _currentAccelerationMultiplier* Time.deltaTime;

            manualProgress += Time.deltaTime;
            yield return null;
        }

        ManualProcessEnded();
    }

    private void ManualProcessEnded()
    {
        //Re-enables the pathfinding functionality
        _myHeroBase.GetPathfinding().EnableAbilityToMove();
        //Makes sure that the hero doesn't try to continue any previous pathfinding
        _myHeroBase.GetPathfinding().DirectNavigationTo(
            EnvironmentManager.Instance.GetClosestPointToFloor(transform.position));

        DecreaseBasicAttackSpeedOnManualEnd();

        _manualActive = false;

        StopManualAudioProcess();
        
        _manualCoroutine = null;
    }

    /// <summary>
    /// Accelerates and decelerates the manual ability speed
    /// </summary>
    /// <returns></returns>
    private IEnumerator ManualAccelerationAndDeceleration()
    {
        //TODO Switch to using animation curve
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

    }
    
    protected override void ManualAbilityAudioPlayed(EventInstance eventInstance)
    {
        _manualAudioInstance = eventInstance;
    }

    private void StartManualAudioProcess()
    {
        _manualAudioCoroutine = StartCoroutine(ManualAudioProcess());
    }

    private IEnumerator ManualAudioProcess()
    {
        while (_manualActive)
        {
            yield return _manualAudioWaitInterval;
            StopManualAudio();
            PlayManualAbilityAudio();
        }
    }

    private void StopManualAudioProcess()
    {
        if (!_manualAudioCoroutine.IsUnityNull())
        {
            StopCoroutine(_manualAudioCoroutine);
        }

        StopManualAudio();
    }

    private void StopManualAudio()
    {
        AudioManager.Instance.StartFadeOutStopInstance(_manualAudioInstance,
            AudioManager.Instance.AllSpecificHeroAudio[_myHeroBase.GetHeroSO().GetHeroID()].ManualAbilityUsed);
    }

    /// <summary>
    /// Creates the swirl effect on the Fae manual ability
    /// </summary>
    private void CreateSwirlVFX()
    {
        //TODO check for deletion
        Instantiate(_swirlVFX, _vfxWeaponSpawnPoint.transform);
    }

    private IEnumerator WeaponVFXSpawnProcess()
    {
        yield return new WaitForSeconds(_vfxWeaponDelay);

        while (_manualActive)
        {
            //TODO Switch to Unity VFX system instead of spawning game objects
            GameObject newestWeaponVFX = Instantiate(_vfxWeapon, _vfxWeaponSpawnPoint.transform);

            Destroy(newestWeaponVFX, _vfxWeaponDuration);

            Vector3 randomEulerRotation = new Vector3(Random.Range(-_vfxWeaponSpawnEulers.x, _vfxWeaponSpawnEulers.x),
                Random.Range(-_vfxWeaponSpawnEulers.y, _vfxWeaponSpawnEulers.y),
                Random.Range(-_vfxWeaponSpawnEulers.z, _vfxWeaponSpawnEulers.z));

            //newestWeaponVFX.transform.rotation = Random.rotation;
            newestWeaponVFX.transform.eulerAngles = randomEulerRotation;

            newestWeaponVFX.transform.position += newestWeaponVFX.transform.forward * _vfxWeaponSpawnDistance;

            yield return new WaitForSeconds(_vfxWeaponSpawnRate);
        }
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
                if(_manualCanDamage)
                {
                    DamageBoss(_manualContactDamage);
                    StaggerBoss(_manualContactStagger);
                    StartCoroutine(ManualDamageCooldown());
                }
                return;
            }
            
            Vector3 directionToBoss = ManualDirectionToBoss();
            float bossDirectionDotProduct = Vector3.Dot(_currentManualDirection, directionToBoss);
            if (bossDirectionDotProduct > _manualMinimumDotProduct)
            {
                _currentManualDirection = directionToBoss;
            }
        }
    }

    private Vector3 ManualDirectionToBoss()
    {
        return  Vector3.Lerp(_currentManualDirection, 
            BossManager.Instance.GetDirectionToBoss(transform.position), _manualBossHoming).normalized;
    }

    /// <summary>
    /// Provides a delay that prevents the manual ability from landing several hits in a short time 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ManualDamageCooldown()
    {
        _manualCanDamage = false;
        yield return new WaitForSeconds(_manualDamageCooldown);
        _manualCanDamage = true;
    }

    private bool ManualHitBoss(RaycastHit rayHit)
    {
        return TagStringData.DoesColliderBelongToBoss(rayHit.collider);
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
    /// <summary>
    /// Performs set up for the specific hero
    /// </summary>
    /// <param name="heroBase"></param>
    /// <param name="heroSO"></param>
    public override void SetUpSpecificHero(HeroBase heroBase, HeroSO heroSO)
    {
        _heroStats = heroBase.GetHeroStats();
        
        _manualAudioWaitInterval = new WaitForSeconds(_manualAudioInterval);

        base.SetUpSpecificHero(heroBase, heroSO);
    }

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();

        _myHeroBase.GetHeroStartedMovingEvent().AddListener(IncreaseBasicAttackSpeedOnMoveStart);
        _myHeroBase.GetHeroStoppedMovingEvent().AddListener(DecreaseBasicAttackSpeedOnMoveEnd);
    }
    #endregion
}
