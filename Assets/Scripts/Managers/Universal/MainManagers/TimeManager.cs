using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the speed at which the game plays
/// </summary>
public class TimeManager : MainUniversalManagerFramework
{
    public static TimeManager Instance;
    
    [Header("Boss Values")]
    [SerializeField] private float _bossStaggerTimeSpeed;
    [SerializeField] private float _bossStaggerDuration;

    [Space]
    [Header("Hero Values")]
    [SerializeField] private float _largeHeroDamageTimeSpeed;
    [SerializeField] private float _largeHeroDamageDuration;
    [Space]

    [SerializeField] private float _heroDeathTimeSpeed;
    [SerializeField] private float _heroDeathDuration;

    [Space] 
    [Header("Battle Values")]
    [SerializeField] private float _battleWonTimeSpeed;
    [SerializeField] private float _battleWonDuration;
    
    [Space]
    [SerializeField] private float _battleLostTimeSpeed;
    [SerializeField] private float _battleLostDuration;

    private List<float> _appliedSlowedTimeVariations = new List<float>();

    private bool _canUpdateTimeVariation = true;

    private bool _isTimeStopped = false;
    private bool _isGamePaused = false;
    
    private UnityEvent _timeStoppedEvent = new UnityEvent();
    private UnityEvent _timeResumedEvent = new UnityEvent();

    private UnityEvent _gamePausedEvent = new UnityEvent();
    private UnityEvent _gameUnpausedEvent = new UnityEvent();
    
    public void PressGamePauseButton()
    {
        ToggleTimeStop(true);
    }

    public void ToggleTimeStop(bool doesPauseToggle)
    {
        _isTimeStopped = !_isTimeStopped;

        if (doesPauseToggle)
        {
            _isGamePaused = !_isGamePaused;
        }
        
        if (_isTimeStopped)
        {
            FreezeTime(doesPauseToggle);
        }
        else
        {
            UnfreezeTime(doesPauseToggle);
        }
    }


    /// <summary>
    /// Starts process of taking into account new time variation
    /// </summary>
    /// <param name="timeVariation"> The speed the time is being set to </param> 
    /// <param name="duration"> The duration is relative to the current time scale</param> 
    public void AddNewTimeVariationForDuration(float timeVariation, float duration)
    {
        if (!_canUpdateTimeVariation)
        {
            return;
        }

        StartCoroutine(AddTimeVariationProcess(timeVariation, duration));
    }

    /// <summary>
    /// Process of taking into account new time variation
    /// </summary>
    /// <param name="timeVariation"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator AddTimeVariationProcess(float timeVariation, float duration)
    {
        //Adds new time variation to the list and evaluates what the current speed should be
        _appliedSlowedTimeVariations.Add(timeVariation);
        DetermineCurrentTimeSpeedBasedOnList();
        //Waits for the duration (Scaled based on the time variation speed)
        yield return new WaitForSeconds(duration);
        //Removes the time variation from the list and reevaluates the current speed
        _appliedSlowedTimeVariations.Remove(timeVariation);
        DetermineCurrentTimeSpeedBasedOnList();
    }

    /// <summary>
    /// Determines what the speed of the game should be
    /// </summary>
    private void DetermineCurrentTimeSpeedBasedOnList()
    {
        // Starts with a lowest time at 1 so if nothing is found lower it returns to normal speed
        float currentLowestTime = 1;

        //Checks a list of all different speeds the game has applied to it
        foreach (float timeVar in _appliedSlowedTimeVariations)
        {
            //Picks the lowest speed in the list
            if(timeVar < currentLowestTime)
            {
                currentLowestTime = timeVar;
            }
        }

        // Sets the time scale to the lowest number
        SetTimeScale(currentLowestTime);
    }

    /// <summary>
    /// Slows time on boss stagger to predetermined values
    /// </summary>
    public void BossStaggeredTimeSlow()
    {
        AddNewTimeVariationForDuration(_bossStaggerTimeSpeed, _bossStaggerDuration);
    }

    /// <summary>
    /// Slows time on boss death to predetermined values
    /// </summary>
    public void BossDiedTimeSlow()
    {
        AddNewTimeVariationForDuration(_battleWonTimeSpeed, _battleWonDuration);
    }

    /// <summary>
    /// Slows time on large damage/stagger from a hero to predetermined values
    /// </summary>
    public void LargeHeroDamageStaggerTimeSlow()
    {
        AddNewTimeVariationForDuration(_largeHeroDamageTimeSpeed, _largeHeroDamageDuration);
    }

    /// <summary>
    /// Slows time on hero death to predetermined values
    /// </summary>
    public void HeroDiedTimeSlow()
    {
        AddNewTimeVariationForDuration(_heroDeathTimeSpeed, _heroDeathDuration);
    }

    public void BattleLostTimeSlow()
    {
        AddNewTimeVariationForDuration(_battleLostTimeSpeed, _battleLostDuration);
    }

    /// <summary>
    /// Sets the time to 1, ignoring any time variations
    /// </summary>
    public void SetTimeToNormalSpeedOverride()
    {
        SetTimeScale(1);
    }

    /// <summary>
    /// Sets the speed at which the game plays
    /// </summary>
    /// <param name="scale"> The new speed we are setting time to. 1 is the default speed </param>
    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    /// <summary>
    /// Freezes the game and invokes game paused event
    /// </summary>
    public void FreezeTime(bool doesPause)
    {
        SetTimeScale(0);
        
        InvokeTimeStoppedEvent();

        if (doesPause)
        {
            InvokeGamePausedEvent();
        }
    }

    /// <summary>
    /// Returns time to normal based on time variations and invokes game unpaused event
    /// </summary>
    public void UnfreezeTime(bool isPaused)
    {
        DetermineCurrentTimeSpeedBasedOnList();
        
        InvokeTimeResumedEvent();

        if (isPaused)
        {
            InvokeGameUnpausedEvent();
        }
    }

    /// <summary>
    /// Makes the game proceed at normal speed during a scene load and prevent adjusting speed
    /// </summary>
    private void SceneLoadStart()
    {
        _isGamePaused = false;
        _canUpdateTimeVariation = false;
        SetTimeToNormalSpeedOverride();
    }

    /// <summary>
    /// Reenables the ability to adjust the speed of the game
    /// </summary>
    private void SceneLoadEnd()
    {
        _canUpdateTimeVariation = true;
    }
    
    #region BaseManager
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    protected override void SubscribeToEvents()
    {
        SceneLoadManager.Instance.GetOnStartOfSceneLoad().AddListener(SceneLoadStart);
        SceneLoadManager.Instance.GetOnEndOfSceneLoad().AddListener(SceneLoadEnd);
    }
    #endregion

    #region Events

    private void InvokeTimeStoppedEvent()
    {
        _timeStoppedEvent?.Invoke();
    }

    private void InvokeTimeResumedEvent()
    {
        _timeResumedEvent?.Invoke();
    }
    
    private void InvokeGamePausedEvent()
    {
        _gamePausedEvent?.Invoke();
    }

    private void InvokeGameUnpausedEvent()
    {
        _gameUnpausedEvent?.Invoke();
    }
    #endregion

    #region Getters
    public bool GetTimeStopped() => _isTimeStopped;
    public bool GetGamePaused() => _isGamePaused;
    
    public UnityEvent GetTimeStoppedEvent() => _timeStoppedEvent;
    public UnityEvent GetTimeResumedEvent() => _timeResumedEvent;

    public UnityEvent GetGamePausedEvent() => _gamePausedEvent;
    public UnityEvent GetGameUnpausedEvent() => _gameUnpausedEvent;
    
    
    #endregion
}
