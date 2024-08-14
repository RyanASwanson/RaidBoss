using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the speed at which the game plays
/// </summary>
public class TimeManager : BaseUniversalManager
{
    private List<float> _appliedSlowedTimeVariations = new List<float>();

    private bool _canUpdateTimeVariation = true;

    [Space]
    [Header("Boss Values")]
    [SerializeField] private float _bossStaggerTimeSpeed;
    [SerializeField] private float _bossStaggerDuration;
    [Space]

    [SerializeField] private float _bossDeathTimeSpeed;
    [SerializeField] private float _bossDeathDuration;

    [Space]
    [Header("Hero Values")]
    [SerializeField] private float _largeHeroDamageTimeSpeed;
    [SerializeField] private float _largeHeroDamageDuration;
    [Space]

    [SerializeField] private float _heroDeathTimeSpeed;
    [SerializeField] private float _heroDeathDuration;

    private bool _gamePaused = false;

    private UnityEvent _gamePausedEvent = new UnityEvent();
    private UnityEvent _gameUnpausedEvent = new UnityEvent();


    public void PressGamePauseButton()
    {
        _gamePaused = !_gamePaused;
        if (_gamePaused)
            FreezeTime();
        else
            UnfreezeTime();
    }


    /// <summary>
    /// Starts process of taking into account new time variation
    /// </summary>
    /// <param name="timeVariation"></param> the speed the time is being set to
    /// <param name="duration"></param> duration is relative to the current time scale
    public void AddNewTimeVariationForDuration(float timeVariation, float duration)
    {
        if (!_canUpdateTimeVariation) return;

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
        AddNewTimeVariationForDuration(_bossDeathTimeSpeed, _bossDeathDuration);
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
    /// <param name="scale"></param>
    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    /// <summary>
    /// Freezes the game and invokes game paused event
    /// </summary>
    public void FreezeTime()
    {
        SetTimeScale(0);
        InvokeGamePausedEvent();
    }

    /// <summary>
    /// Returns time to normal based on time variations and invokes game unpaused event
    /// </summary>
    public void UnfreezeTime()
    {
        DetermineCurrentTimeSpeedBasedOnList();
        InvokeGameUnpausedEvent();
    }

    /// <summary>
    /// Makes the game proceed at normal speed during a scene load and prevent adjusting speed
    /// </summary>
    private void SceneLoadStart()
    {
        _gamePaused = false;
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

    public override void SetupUniversalManager()
    {
        base.SetupUniversalManager();
    }

    #region Events
    private void InvokeGamePausedEvent()
    {
        _gamePausedEvent?.Invoke();
    }

    private void InvokeGameUnpausedEvent()
    {
        _gameUnpausedEvent?.Invoke();
    }
    #endregion

    public override void SubscribeToEvents()
    {
        UniversalManagers.Instance.GetSceneLoadManager().GetStartOfSceneLoadEvent().AddListener(SceneLoadStart);
        UniversalManagers.Instance.GetSceneLoadManager().GetEndOfSceneLoadEvent().AddListener(SceneLoadEnd);
    }

    #region Getters
    public bool GetGamePaused() => _gamePaused;

    public UnityEvent GetGamePausedEvent() => _gamePausedEvent;
    public UnityEvent GetGameUnpausedEvent() => _gameUnpausedEvent;
    #endregion
}
