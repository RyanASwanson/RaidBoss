using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Astromancer hero
/// </summary>
public class SH_Astromancer : SpecificHeroFramework
{
    [Space]
    [SerializeField] private GameObject _basicProjectile;

    [Space]
    [SerializeField] private float _increasedManualRotationalSpeed;
    
    [SerializeField] private float _minManualProjectileSpawnRate;
    [SerializeField] private float _maxManualProjectileSpawnRate;
    [SerializeField] private float _timeToReachMaxManualSpawnRate;
    [SerializeField] private AnimationCurve _manualProjectileSpawnCurve;
    [SerializeField] private GameObject _purpleManualProjectile;
    [SerializeField] private GameObject _blueManualProjectile;
    private float _manualTime;
    private bool _isManualRight = false;

    private Coroutine _manualProcess;

    private bool _manualActive = false;

    [Space]
    [SerializeField] private float _passiveRechargeManualAmount;

    SHP_AstromancerManualProjectile _storedManual;
    
    private const string MANUAL_LEFT_ANIM_TRIGGER = "ManualLeft";
    private const string MANUAL_RIGHT_ANIM_TRIGGER = "ManualRight";
    
    
    public const int BASIC_PROJECTILE_REFLECT_AUDIO_ID = 0;
    public const int MANUAL_LEFT_AUDIO_ID = 1;
    public const int MANUAL_RIGHT_AUDIO_ID = 2;

    #region Basic Abilities
    public override void ActivateBasicAbilities()
    {
        base.ActivateBasicAbilities();

        CreateBasicAttackProjectiles();
    }

    private void CreateBasicAttackProjectiles()
    {
        GameObject spawnedProjectile = Instantiate(_basicProjectile, transform.position, Quaternion.identity);

        SHP_AstromancerBasicProjectile projectileFunc = spawnedProjectile.GetComponent<SHP_AstromancerBasicProjectile>();
        projectileFunc.SetUpProjectile(_myHeroBase, EHeroAbilityType.Basic);

        Vector3 storedProjectileDirection = BossManager.Instance.GetDirectionToBoss(transform.position);
        projectileFunc.AdditionalSetup(this, storedProjectileDirection);

        //Performs the setup for the damage area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroDamageArea>().SetUpDamageArea(_myHeroBase);
        //Performs the setup for the 'healing' area so that it knows it's owner
        spawnedProjectile.GetComponent<GeneralHeroHealArea>().SetUpHealingArea(_myHeroBase);
    }
    
    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities()
    {
        _manualActive = true;

        //Does everything in the base of this function except for starting the cooldown
        _manualAbilityCurrentCharge = 0;

        _manualAbilityCooldownCoroutine = null;

        TriggerManualAbilityAnimation();

        _myHeroBase.GetHeroStats().ChangeCurrentHeroAngularSpeed(_increasedManualRotationalSpeed);

        _myHeroBase.GetHeroUIManager().ShowManualAbilityChargedIconAboveHero(false);

        StartManualSpawnProcess();
        
        _myHeroBase.GetPathfinding().BriefStopCurrentMovement();
        _myHeroBase.GetHeroStartedMovingEvent().AddListener(EndManualAbility);
    }
    
    private void StartManualSpawnProcess()
    {
        _manualProcess = StartCoroutine(ManualProjectileSpawnProcess());
    }

    private IEnumerator ManualProjectileSpawnProcess()
    {
        float timeSinceLastManualProjectile = 0;
        float currentManualProjectileSpawnRate = _minManualProjectileSpawnRate;
        
        _manualTime = 0;

        SpawnManualProjectile();

        while (true)
        {
            _manualTime += Time.deltaTime;
            timeSinceLastManualProjectile += Time.deltaTime;
            
            if (_manualTime < _timeToReachMaxManualSpawnRate)
            {
                float tempProgress = _manualTime/_timeToReachMaxManualSpawnRate;
                
                currentManualProjectileSpawnRate = Mathf.Lerp(_minManualProjectileSpawnRate,
                    _maxManualProjectileSpawnRate, _manualProjectileSpawnCurve.Evaluate(tempProgress));
                
                //Debug.Log(currentManualProjectileSpawnRate);
            }
            else
            {
                currentManualProjectileSpawnRate = _maxManualProjectileSpawnRate;
            }

            if (timeSinceLastManualProjectile >= currentManualProjectileSpawnRate)
            {
                timeSinceLastManualProjectile -= currentManualProjectileSpawnRate;
                SpawnManualProjectile();
            }
            
            yield return null;
        }
    }

    private void SpawnManualProjectile()
    {
        GameObject spawnedProjectile;
        if (_isManualRight)
        {
            spawnedProjectile = Instantiate(_purpleManualProjectile, transform.position, Quaternion.identity);
            PlayRightManualAudio();
            ManualRightAnimation();
            
        }
        else
        {
            spawnedProjectile = Instantiate(_blueManualProjectile, transform.position, Quaternion.identity);
            PlayLeftManualAudio();
            ManualLeftAnimation();
        }
        
        SHP_AstromancerManualProjectile astromancerManual = spawnedProjectile.GetComponent<SHP_AstromancerManualProjectile>();
        
        astromancerManual.AdditionalSetUp(_isManualRight);
        astromancerManual.SetUpProjectile(_myHeroBase, EHeroAbilityType.Manual);
        
        _isManualRight = !_isManualRight;
    }
    
    private void ManualLeftAnimation()
    {
        _myHeroBase.GetHeroVisuals().HeroSpecificAnimationTrigger(MANUAL_LEFT_ANIM_TRIGGER);
    }
    
    private void ManualRightAnimation()
    {
        _myHeroBase.GetHeroVisuals().HeroSpecificAnimationTrigger(MANUAL_RIGHT_ANIM_TRIGGER);
    }

    private void PlayLeftManualAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificHeroAudio[_myHeroBase.GetHeroSO().GetHeroID()]
                .MiscellaneousHeroAudio[MANUAL_LEFT_AUDIO_ID]);
    }

    private void PlayRightManualAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificHeroAudio[_myHeroBase.GetHeroSO().GetHeroID()]
                .MiscellaneousHeroAudio[MANUAL_RIGHT_AUDIO_ID]);
    }

    private void StopManualProcess()
    {
        if (!_manualProcess.IsUnityNull())
        {
            StopCoroutine(_manualProcess);
            _manualProcess = null;
        }
    }

    protected void EndManualAbility()
    {
        _manualActive = false;

        _myHeroBase.GetHeroStartedMovingEvent().RemoveListener(EndManualAbility);
        
        StopManualProcess();

        _myHeroBase.GetHeroStats().ChangeCurrentHeroAngularSpeed(-_increasedManualRotationalSpeed);

        StartCooldownManualAbility();
    }

    #endregion

    #region Passive Abilities
    public override void ActivatePassiveAbilities()
    {
        if (!_manualActive)
        {
            AddToManualAbilityChargeTime(_passiveRechargeManualAmount);
        }
    }

    #endregion

    #region Base Hero

    #endregion

}

