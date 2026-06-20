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
    [SerializeField] private FaeBasicAttackDirections[] _basicAttackDirections;
    [SerializeField] private float _projectileSpawnDistance;
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private float _manualContactDamage;
    [SerializeField] private float _manualContactStagger;
    [Space]
    [SerializeField] private float _manualDuration;
    [Range(0,1)][SerializeField] private float _heroManualDamageResistance;
    [SerializeField] private float _accelerateTime;
    [SerializeField] private float _manualSpeedMultiplier;
    [SerializeField] private float _manualWallDistanceRange;
    [SerializeField] private float _manualDamageCooldown;
    [Range(-1,1)][SerializeField] private float _manualBossHoming;
    [Range(-1,1)][SerializeField] private float _manualHitHeroHomingChange;
    [Range(-1,1)][SerializeField] private float _manualHitMapBorderHomingChange;
    [Range(-1,1)][SerializeField] private float _manualHitSpawnedEnvironmentHomingChange;
    [Range(-1,1)][SerializeField] private float _manualMinimumBossHoming;
    [SerializeField] private float _manualMinimumDotProduct;
    [SerializeField] private Vector3 _manualWallExtents;
    private WaitForSeconds _manualDamageWait;
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
    private WaitForSeconds _vfxSpawnDelayWait;
    private WaitForSeconds _vfxSpawnRateWait;

    [Space]
    [SerializeField] private LayerMask _bounceLayers;

    private Vector3 _currentManualDirection;
    private float _currentAccelerationMultiplier;
    private float _currentManualBossHoming;
    
    public const int MANUAL_BOUNCE_AUDIO_ID = 0;

    private Coroutine _manualCoroutine;

    [Space]
    [SerializeField] private float _passiveBasicAttackSpeedChangeWalking;
    [SerializeField] private float _passiveBasicAttackSpeedChangeManual;

    private float _currentPassiveBasicAttackSpeed = 1;
    private float _startingPassiveBasicAttackSpeed;
    private bool _passiveBonusActive = false;

    private HeroStats _heroStats;

    #region Basic Abilities
    

    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    public override bool DoesMeetConditionsToActivateBasicAbilities()
    {
        return true;
    }

    protected void CreateBasicAttackProjectiles()
    {
        bool isProjectileAlignedOnX = false;
        bool isProjectileAlignedOnZ = false;
        
        for (int i = 0; i < _basicAttackDirections.Length; i++)
        {
            SHP_FaeBasicProjectile newestProjectile = 
                Instantiate(_basicProjectile, transform.position, 
                    Quaternion.Euler(_basicAttackDirections[i].AttackEulers)).GetComponent<SHP_FaeBasicProjectile>();
            
            newestProjectile.transform.position += (newestProjectile.transform.forward * _projectileSpawnDistance);

            newestProjectile.SetUpProjectile(_myHeroBase, EHeroAbilityType.Basic);

            GeneralHeroDamageArea damageArea = newestProjectile.GetComponent<GeneralHeroDamageArea>();
            
            //Performs the set up for the damage area so that it knows it's owner
            damageArea.SetUpDamageArea(_myHeroBase);

            isProjectileAlignedOnX = _basicAttackDirections[i].IsBossDirectionInPositiveX !=
                                     (_myHeroBase.transform.position.x > 0);
            isProjectileAlignedOnZ = _basicAttackDirections[i].IsBossDirectionInPositiveZ !=
                                     (_myHeroBase.transform.position.z > 0);
            
            /*
             * If the projectile doesn't line up on either direction. The only arrow that will not pass this is the
             * one fired directly at the Boss. The other 3 will enter this if statement
             */
            if (!isProjectileAlignedOnX || !isProjectileAlignedOnZ)
            {
                damageArea.ToggleProjectileCollider(false);
                
                if (isProjectileAlignedOnX != isProjectileAlignedOnZ)
                {
                    // The projectile direction is perpendicular to the boss
                    continue;
                }
            }
            
            newestProjectile.AdditionalSetUp(true);
        }
    }


    #endregion

    #region Manual Abilities

    public override void ActivateManualAbilities()
    {
        base.ActivateManualAbilities();
        
        _myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(_heroManualDamageResistance);
        
        _currentManualBossHoming = _manualBossHoming;
        _myHeroBase.GetPathfinding().SetIsHeroUsingMovementAbility(true);

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
            _myHeroBase.gameObject.transform.position += _currentManualDirection * (GetCurrentManualAbilityMovementSpeed() * Time.deltaTime);

            manualProgress += Time.deltaTime;
            yield return null;
        }

        EndManualAbility();
    }

    public override void EndManualAbility()
    {
        _myHeroBase.GetHeroStats().ChangeCurrentHeroDamageResistance(-_heroManualDamageResistance);
        
        _myHeroBase.GetPathfinding().SetIsHeroUsingMovementAbility(false);
        
        //Re-enables the pathfinding functionality
        _myHeroBase.GetPathfinding().EnableAbilityToMove();
        //Makes sure that the hero doesn't try to continue any previous pathfinding
        _myHeroBase.GetPathfinding().DirectNavigationTo(
            EnvironmentManager.Instance.GetClosestPointToFloor(transform.position));

        DecreaseBasicAttackSpeedOnManualEnd();

        StopManualAudioProcess();
        
        _manualCoroutine = null;
        
        base.EndManualAbility();
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
        while (_isManualAbilityActive)
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

    private void PlayManualBounceAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificHeroAudio[_myHeroBase.GetHeroSO().GetHeroID()]
                .MiscellaneousHeroAudio[MANUAL_BOUNCE_AUDIO_ID]);
    }

    /// <summary>
    /// Creates the swirl effect on the Fae manual ability
    /// </summary>
    private void CreateSwirlVFX()
    {
        Instantiate(_swirlVFX, _vfxWeaponSpawnPoint.transform);
    }

    private IEnumerator WeaponVFXSpawnProcess()
    {
        yield return _vfxSpawnDelayWait;

        while (_isManualAbilityActive)
        {
            //TODO Switch to Unity VFX system instead of spawning game objects
            GameObject newestWeaponVFX = Instantiate(_vfxWeapon, _vfxWeaponSpawnPoint.transform);

            Destroy(newestWeaponVFX, _vfxWeaponDuration);

            Vector3 randomEulerRotation = new Vector3(Random.Range(-_vfxWeaponSpawnEulers.x, _vfxWeaponSpawnEulers.x),
                Random.Range(-_vfxWeaponSpawnEulers.y, _vfxWeaponSpawnEulers.y),
                Random.Range(-_vfxWeaponSpawnEulers.z, _vfxWeaponSpawnEulers.z));
            
            newestWeaponVFX.transform.eulerAngles = randomEulerRotation;

            newestWeaponVFX.transform.position += newestWeaponVFX.transform.forward * _vfxWeaponSpawnDistance;

            yield return _vfxSpawnRateWait;
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

            PlayManualBounceAudio();

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
            else if (ManualHitHero(rayHit))
            {
                ChangeCurrentManualHoming(_manualHitHeroHomingChange);
            }
            else if (ManualHitMapBorder(rayHit))
            {
                ChangeCurrentManualHoming(_manualHitMapBorderHomingChange);
            }
            else if (ManualHitSpawnedEnvironment(rayHit))
            {
                ChangeCurrentManualHoming(_manualHitSpawnedEnvironmentHomingChange);
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
        return  Vector3.LerpUnclamped(_currentManualDirection, 
            BossManager.Instance.GetDirectionToBoss(transform.position), _currentManualBossHoming).normalized;
    }

    private void ChangeCurrentManualHoming(float changeAmount)
    {
        _currentManualBossHoming += changeAmount;
        _currentManualBossHoming = Mathf.Clamp(_currentManualBossHoming, _manualMinimumBossHoming, _manualBossHoming);
    }

    /// <summary>
    /// Provides a delay that prevents the manual ability from landing several hits in a short time 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ManualDamageCooldown()
    {
        _manualCanDamage = false;
        yield return _manualDamageWait;
        _manualCanDamage = true;
    }

    private bool ManualHitBoss(RaycastHit rayHit)
    {
        return TagStringData.DoesColliderBelongToBoss(rayHit.collider);
    }

    private bool ManualHitHero(RaycastHit rayHit)
    {
        return TagStringData.DoesColliderBelongToHero(rayHit.collider);
    }

    private bool ManualHitMapBorder(RaycastHit rayHit)
    {
        return TagStringData.DoesColliderBelongToMapBorder(rayHit.collider);
    }

    private bool ManualHitSpawnedEnvironment(RaycastHit rayHit)
    {
        return TagStringData.DoesColliderBelongToSpawnedEnvironment(rayHit.collider);
    }
    #endregion

    #region Passive Abilities
    private void IncreaseBasicAttackSpeedOnMoveStart()
    {
        if (_passiveBonusActive)
        {
            return;
        }

        _currentPassiveBasicAttackSpeed = _passiveBasicAttackSpeedChangeWalking;
        _myHeroBase.GetHeroStats().ChangeCurrentBasicAbilityCooldownRate(_passiveBasicAttackSpeedChangeWalking);
        _passiveBonusActive = true;
    }

    private void DecreaseBasicAttackSpeedOnMoveEnd()
    {
        if (!_passiveBonusActive)
        {
            return;
        }

        _currentPassiveBasicAttackSpeed = _startingPassiveBasicAttackSpeed;
        _myHeroBase.GetHeroStats().ChangeCurrentBasicAbilityCooldownRate(1/_passiveBasicAttackSpeedChangeWalking);
        _passiveBonusActive = false;
    }

    private void IncreaseBasicAttackSpeedOnManualStart()
    {
        if (Mathf.Approximately(_currentPassiveBasicAttackSpeed, _passiveBasicAttackSpeedChangeWalking))
        {
            DecreaseBasicAttackSpeedOnMoveEnd();
        }
        
        if (_passiveBonusActive)
        {
            return;
        }
        
        _currentPassiveBasicAttackSpeed = _passiveBasicAttackSpeedChangeManual;
        _myHeroBase.GetHeroStats().ChangeCurrentBasicAbilityCooldownRate(_passiveBasicAttackSpeedChangeManual);
        _passiveBonusActive = true;
    }

    private void DecreaseBasicAttackSpeedOnManualEnd()
    {
        if (!_passiveBonusActive)
        {
            return;
        }
        _currentPassiveBasicAttackSpeed = _startingPassiveBasicAttackSpeed;
        _myHeroBase.GetHeroStats().ChangeCurrentBasicAbilityCooldownRate(1/_passiveBasicAttackSpeedChangeManual);
        _passiveBonusActive = false;
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

        _manualDamageWait = new WaitForSeconds(_manualDamageCooldown);
        _manualAudioWaitInterval = new WaitForSeconds(_manualAudioInterval);
        _vfxSpawnDelayWait = new WaitForSeconds(_vfxWeaponDelay);
        _vfxSpawnRateWait = new WaitForSeconds(_vfxWeaponSpawnRate);

        _startingPassiveBasicAttackSpeed = _currentPassiveBasicAttackSpeed;

        base.SetUpSpecificHero(heroBase, heroSO);
    }

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        if (_isSubscribedToEvents)
        {
            return;
        }
        
        _myHeroBase.GetHeroStartedMovingEvent().AddListener(IncreaseBasicAttackSpeedOnMoveStart);
        _myHeroBase.GetHeroStoppedMovingEvent().AddListener(DecreaseBasicAttackSpeedOnMoveEnd);
        
        base.SubscribeToEvents();
    }

    protected override void UnsubscribeFromEvents()
    {
        if (!_isSubscribedToEvents)
        {
            return;
        }
        
        _myHeroBase.GetHeroStartedMovingEvent().RemoveListener(IncreaseBasicAttackSpeedOnMoveStart);
        _myHeroBase.GetHeroStoppedMovingEvent().RemoveListener(DecreaseBasicAttackSpeedOnMoveEnd);
        
        base.UnsubscribeFromEvents();
    }
    #endregion
    
    #region Getters

    public float GetCurrentManualAbilityMovementSpeed() =>
        _heroStats.GetCurrentSpeed() * _manualSpeedMultiplier * _currentAccelerationMultiplier;

    #endregion
}

[System.Serializable]
public class FaeBasicAttackDirections
{
    public Vector3 AttackEulers;
    public bool IsBossDirectionInPositiveX;
    public bool IsBossDirectionInPositiveZ;
}