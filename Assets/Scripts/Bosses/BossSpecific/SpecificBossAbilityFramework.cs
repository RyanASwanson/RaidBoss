using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Provides the framework for boss abilities
/// </summary>
public abstract class SpecificBossAbilityFramework : MonoBehaviour
{
    [SerializeField] protected int _abilityID;
    [SerializeField] protected EBossAbilityTargetMethod _targetMethod;
    [SerializeField] protected bool _doesBossFollowTarget;

    [Space]
    [SerializeField] protected Vector3 _specificAreaTarget;
    [SerializeField] protected Vector3 _specificLookTarget;
    [Space]

    [Tooltip("The duration of the target zones")]
    [SerializeField] protected float _targetZoneDuration;
    [SerializeField] protected float _abilityWindUpTime;
    [SerializeField] protected float _timeUntilNextAbility;

    [Space]
    [Tooltip("If the ability has any screen shake")]
    [SerializeField] protected bool _hasScreenShake;
    [SerializeField] protected float _screenShakeIntensity;
    [SerializeField] protected float _screenShakeFrequency;
    [SerializeField] protected float _screenShakeDuration;

    [Space]
    [SerializeField] protected string _animationTriggerName;

    protected List<BossTargetZoneParent> _currentTargetZones = new List<BossTargetZoneParent>();
    
    protected Vector3 _storedTargetLocation;
    protected HeroBase _storedTarget;

    protected Coroutine _abilityWindUpProcess;
    protected Coroutine _targetZoneRemovalProcess;

    protected BossBase _myBossBase;
    protected SpecificBossFramework _mySpecificBoss;

    /// <summary>
    /// Sets up the ability with the boss base and specific boss framework
    /// </summary>
    /// <param name="bossBase"></param>
    public virtual void AbilitySetUp(BossBase bossBase)
    {
        _myBossBase = bossBase;
        _mySpecificBoss = bossBase.GetSpecificBossScript();

        _timeUntilNextAbility /= SelectionManager.Instance.GetSpeedMultiplierFromDifficulty();
    }

    /// <summary>
    /// Function that is called to activate the boss ability
    /// Stores the target location / target hero
    /// </summary>
    /// <param name="targetLocation"></param>
    public virtual void ActivateAbility(Vector3 targetLocation, HeroBase targetHeroBase)
    {
        _storedTargetLocation = targetLocation;
        if (!targetHeroBase.IsUnityNull())
        {
            _storedTarget = targetHeroBase;
        }

        AbilityPrep();

        StartBossAbilityAnimation();
    }

    /// <summary>
    /// Does the initial preparation for the ability to be used
    /// Shows the target zone if it has one
    /// </summary>
    protected virtual void AbilityPrep()
    {
        StartShowTargetZone();
        PlayAbilityPrepAudio();
        StartAbilityWindUp();
    }

    /// <summary>
    /// Starts the animation associated with the boss attack
    /// </summary>
    protected virtual void StartBossAbilityAnimation()
    {
        BossVisuals.Instance.StartBossSpecificAnimationTrigger(_animationTriggerName);
    }

    #region Target Zone
    /// <summary>
    /// Starts the process of visualizing the target zone
    /// </summary>
    protected virtual void StartShowTargetZone()
    {
        PlayTargetZoneSpawnedAudio();
        _targetZoneRemovalProcess = StartCoroutine(TargetZonesProcess());
    }

    protected virtual void StopTargetZoneRemovalProcess()
    {
        if (!_targetZoneRemovalProcess.IsUnityNull())
        {
            StopCoroutine(_targetZoneRemovalProcess);
        }
    }

    /// <summary>
    /// Wait for the target zone duration
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator TargetZonesProcess()
    {
        yield return new WaitForSeconds(_targetZoneDuration);
        RemoveTargetZones();
    }

    /// <summary>
    /// Removes all target zones
    /// </summary>
    protected virtual void RemoveTargetZones()
    {
        StopTargetZoneRemovalProcess();
        
        if (_currentTargetZones.Count == 0)
        {
            return;
        }

        //Iterates through all target zones and removes them
        foreach(BossTargetZoneParent currentZone in _currentTargetZones)
        {
            currentZone.RemoveBossTargetZones();
        }
        _currentTargetZones.Clear();
    }

    protected virtual void SetStateOfCurrentTargetZonesToDeactivated()
    {
        SetDeactivatedStateOfCurrentTargetZones(true);
    }
    
    protected virtual void SetStateOfCurrentTargetZonesToActivated()
    {
        SetDeactivatedStateOfCurrentTargetZones(false);
    }
    
    protected virtual void SetDeactivatedStateOfCurrentTargetZones(bool shouldDeactivate)
    {
        foreach (BossTargetZoneParent targetZone in _currentTargetZones)
        {
            targetZone.SetTargetZoneDeactivatedStatesOfAllTargetZones(shouldDeactivate);
        }
    }
    #endregion

    /// <summary>
    /// Starts the actual activation of the ability.
    /// Happens after any prep that is needed such as targeting zones.
    /// </summary>
    protected virtual void StartAbilityWindUp()
    {
        _abilityWindUpProcess = StartCoroutine(AbilityWindUp());
    }

    protected virtual void StopAbilityWindUp()
    {
        if (!_abilityWindUpProcess.IsUnityNull())
        {
            StopCoroutine(_abilityWindUpProcess);
        }
    }

    /// <summary>
    /// Provides a delay between the ability wind up and the ability start
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator AbilityWindUp()
    {
        yield return new WaitForSeconds(_abilityWindUpTime);

        AbilityStart();
    }

    /// <summary>
    /// Starts the ability
    /// Generally spawns the damage area of the attack
    /// </summary>
    protected virtual void AbilityStart()
    {
        ScreenShakeCheck();
        PlayAbilityStartAudio();
    }

    /// <summary>
    /// Checks if the ability has screen shake on it. If it does it plays a screen shake
    /// </summary>
    protected virtual void ScreenShakeCheck()
    {
        if (_hasScreenShake)
        {
            AbilityScreenShake();
        }
    }

    /// <summary>
    /// Starts the screen shake of the ability if the ability has screen shake
    /// </summary>
    protected virtual void AbilityScreenShake()
    {
        CameraGameManager.Instance.StartCameraShake
            (_screenShakeIntensity, _screenShakeFrequency, _screenShakeDuration);
    }

    public virtual void StopBossAbility()
    {
        StopAbilityWindUp();
        RemoveTargetZones();
    }

    #region AbilityAudio

    protected virtual void PlayTargetZoneSpawnedAudio()
    {
        if (AudioManager.Instance.PlaySpecificAudio(
                AudioManager.Instance.GeneralBossAudio.AbilityAudio.TargetZoneSpawned, out EventInstance eventInstance))
        {
            TargetZoneSpawnedAudioPlayed(eventInstance);
        }
    }

    protected virtual void TargetZoneSpawnedAudioPlayed(EventInstance eventInstance)
    {
        
    }
    

    protected virtual void PlayAbilityPrepAudio()
    {
        if (AudioManager.Instance.PlaySpecificAudio(
                AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].BossAbilityAudio[_abilityID].AbilityPrep,
                out EventInstance eventInstance))
        {
            AbilityAudioPrepPlayed(eventInstance);
        }
    }

    protected virtual void AbilityAudioPrepPlayed(EventInstance eventInstance)
    {
        
    }
    
    
    /// <summary>
    /// Plays the audio associated with the ability being started
    /// </summary>
    protected virtual void PlayAbilityStartAudio()
    {
        if (AudioManager.Instance.PlaySpecificAudio(
                AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].BossAbilityAudio[_abilityID].AbilityStart,
                out EventInstance eventInstance))
        {
            AbilityAudioStartPlayed(eventInstance);
        }
    }

    protected virtual void AbilityAudioStartPlayed(EventInstance eventInstance)
    {
        
    }
    
    #endregion AbilityAudio

    #region RETIRED Targeting

    /// <summary>
    /// Provides the target for an ability with a hero target with an ignore
    /// </summary>
    /// <returns></returns>
    protected virtual HeroBase GetIgnoreHeroTarget()
    {
        return null;
    }

    /// <summary>
    /// Provides the target for an ability with a specific hero target
    /// </summary>
    /// <returns></returns>
    protected virtual HeroBase GetSpecificHeroTarget()
    {
        return null;
    }

    #endregion

    #region Getters

    public int GetAbilityID() => _abilityID;
    public EBossAbilityTargetMethod GetTargetMethod() => _targetMethod;
    public bool GetDoesBossFollowTarget() => _doesBossFollowTarget;
    public Vector3 GetSpecificAreaTarget() => _specificAreaTarget;
    public Vector3 GetSpecificLookTarget() => _specificLookTarget;

    public float GetTimeUntilNextAbility() => _timeUntilNextAbility;
    public float GetAbilityWindUpTime() => _abilityWindUpTime;

    #endregion
}

public enum EBossAbilityTargetMethod
{
    HeroTarget,
    HeroTargetWithIgnore,
    SpecificHeroTarget,
    SpecificAreaTarget,
};
