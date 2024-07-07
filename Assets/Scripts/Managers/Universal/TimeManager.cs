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

    [SerializeField] private float _bossDeathTimeSpeed;
    [SerializeField] private float _bossDeathDuration;

    [Space]
    [SerializeField] private float _heroDeathTimeSpeed;
    [SerializeField] private float _heroDeathDuration;

    private bool _gamePaused = false;

    private UnityEvent _gamePausedEvent = new UnityEvent();
    private UnityEvent _gameUnpausedEvent = new UnityEvent();

    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            AddNewTimeVariationForDuration(.2f, .5f);
        }
    }*/

    public void PressGamePauseButton()
    {
        _gamePaused = !_gamePaused;
        if (_gamePaused)
            FreezeTime();
        else
            UnfreezeTime();
    }


    /// <summary>
    /// Adds a new value to the appliedTimeVariations
    /// </summary>
    /// <param name="timeVariation"></param> the speed the time is being set to
    /// <param name="duration"></param> duration is relative to the current time scale
    public void AddNewTimeVariationForDuration(float timeVariation, float duration)
    {
        if (!_canUpdateTimeVariation) return;

        StartCoroutine(AddTimeVariationProcess(timeVariation, duration));
    }

    private IEnumerator AddTimeVariationProcess(float timeVariation, float duration)
    {
        _appliedSlowedTimeVariations.Add(timeVariation);
        DetermineCurrentTimeSpeedBasedOnList();
        yield return new WaitForSeconds(duration);
        _appliedSlowedTimeVariations.Remove(timeVariation);
        DetermineCurrentTimeSpeedBasedOnList();
    }

    private void DetermineCurrentTimeSpeedBasedOnList()
    {
        float currentLowestTime = 1;

        foreach(float timeVar in _appliedSlowedTimeVariations)
        {
            if(timeVar < currentLowestTime)
            {
                currentLowestTime = timeVar;
            }
        }

        SetTimeScale(currentLowestTime);
    }

    public void BossStaggeredTimeSlow()
    {
        AddNewTimeVariationForDuration(_bossStaggerTimeSpeed, _bossStaggerDuration);
    }

    public void BossDiedTimeSlow()
    {
        AddNewTimeVariationForDuration(_bossDeathTimeSpeed, _bossDeathDuration);
    }

    public void HeroDiedTimeSlow()
    {
        AddNewTimeVariationForDuration(_heroDeathTimeSpeed, _heroDeathDuration);
    }

    public void SetTimeToNormalSpeedOverride()
    {
        SetTimeScale(1);
    }

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void FreezeTime()
    {
        SetTimeScale(0);
        InvokeGamePausedEvent();
    }

    public void UnfreezeTime()
    {
        DetermineCurrentTimeSpeedBasedOnList();
        InvokeGameUnpausedEvent();
    }

    private void SceneLoadStart()
    {
        _canUpdateTimeVariation = false;
        SetTimeToNormalSpeedOverride();
    }

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
