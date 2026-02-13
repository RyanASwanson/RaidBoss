using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides the functionality for the game state throughout gameplay scenes
/// Battle starts in the pre battle state
/// </summary>
public class GameStateManager : MainGameplayManagerFramework
{
    public static GameStateManager Instance;
    
    [SerializeField] private float _characterSpawnDelay;
    [Tooltip("The time before the battle begins")]
    [SerializeField] private float _timeToStart;

    [Space]
    [SerializeField] private float _battleLostAudioDelay;
    [SerializeField] private float _battleWonAudioDelay;

    [Space] 
    [SerializeField] private float _battleEndMusicVolume;
    [SerializeField] private float _battleEndMusicVolumeChangeTime;

    private EGameplayStates _currentEGameplayState = EGameplayStates.PreBattle;

    private UnityEvent _startOfCharacterSpawningEvent = new UnityEvent();
    
    private UnityEvent _startOfBattleEvent = new UnityEvent();

    private UnityEvent _battleLostEvent = new UnityEvent();
    private UnityEvent _battleWonEvent = new UnityEvent();

    private UnityEvent _battleWonOrLostEvent = new UnityEvent();
    
    public void StartProgressToStart()
    {
        StartCoroutine(ProgressToStart());
    }
    
    /// <summary>
    /// Starts the battle with a delay
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProgressToStart()
    {
        //Waits for a brief period before the battle is started
        yield return new WaitForSeconds(_timeToStart);
        StartBattle();
    }

    private void StartBattle()
    {
        SetGameplayState(EGameplayStates.Battle);
    }

    /// <summary>
    /// Changes which state the gameplay is to another one and invokes events for that state
    /// </summary>
    /// <param name="newEGameplayState"> The state we are setting the gameplay to </param>
    public void SetGameplayState(EGameplayStates newEGameplayState)
    {
        if (_currentEGameplayState == newEGameplayState || _currentEGameplayState >= EGameplayStates.PostBattleLost)
        {
            return;
        }

        _currentEGameplayState = newEGameplayState;

        switch(_currentEGameplayState)
        {
            case (EGameplayStates.Battle):
                InvokeStartOfBattleEvent();
                break;
            case (EGameplayStates.PostBattleLost):
                BattleLost();
                break;
            case (EGameplayStates.PostBattleWon):
                BattleWon();
                break;
        }
    }

    /// <summary>
    /// Called when the battle is lost by all heroes dying
    /// </summary>
    private void BattleLost()
    {
        /*AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.GameplayUserInterfaceAudio.BattleLost);*/
        StartCoroutine(DelayBattleEndAudio(AudioManager.Instance.UserInterfaceAudio.GameplayUserInterfaceAudio.BattleLost,_battleLostAudioDelay));

        BattleEnded();
        
        InvokeBattleLostEvent();
    }

    /// <summary>
    /// Called when the battle is won by reducing the boss health to 0
    /// </summary>
    private void BattleWon()
    {
        //Slows time
        TimeManager.Instance.BossDiedTimeSlow();

        //Unlocks the next boss and hero
        //Saves the best difficulty beaten for each hero
        SaveManager.Instance.BossDead();
        
        /*AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.GameplayUserInterfaceAudio.BattleWon);*/
        StartCoroutine(DelayBattleEndAudio(AudioManager.Instance.UserInterfaceAudio.GameplayUserInterfaceAudio.BattleWon, _battleWonAudioDelay));

        BattleEnded();
        
        InvokeBattleWonEvent();
    }

    private IEnumerator DelayBattleEndAudio(SpecificAudio audio, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance.PlaySpecificAudio(audio);
    }

    private void BattleEnded()
    {
        AudioManager.Instance.StartChangeCurrentMusicVolume(_battleEndMusicVolume, _battleEndMusicVolumeChangeTime);
    }

    #region BaseManager
    /// <summary>
    /// Establishes the Instance for the GameStateManager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        InvokeStartOfCharacterSpawningEvent();

        if (SelectionManager.Instance.GetSelectedMissionOut(out MissionSO mission) &&
            mission.GetTutorialPages().Length > 0)
        {
            
        }
        else
        {
            StartProgressToStart();
        }
        
    }
    #endregion
    
    #region Events

    public void InvokeStartOfCharacterSpawningEvent()
    {
        _startOfCharacterSpawningEvent?.Invoke();
    }
    public void InvokeStartOfBattleEvent()
    {
        _startOfBattleEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the lost event and then invokes the won or lost event
    /// </summary>
    public void InvokeBattleLostEvent()
    {
        _battleLostEvent?.Invoke();
        InvokeBattleWonOrLostEvent();
    }
    
    /// <summary>
    /// Invokes the won event and then invokes the won or lost event
    /// </summary>
    public void InvokeBattleWonEvent()
    {
        _battleWonEvent?.Invoke();
        InvokeBattleWonOrLostEvent();
    }

    /// <summary>
    /// Called if the game ends, regardless of win or loss.
    /// </summary>
    public void InvokeBattleWonOrLostEvent()
    {
        _battleWonOrLostEvent?.Invoke();
    }
    #endregion
    
    #region Getters
    public bool GetIsFightOver() => _currentEGameplayState >= EGameplayStates.PostBattleLost;

    public UnityEvent GetStartOfCharacterSpawningEvent() => _startOfCharacterSpawningEvent;
    public UnityEvent GetStartOfBattleEvent() => _startOfBattleEvent;
    public UnityEvent GetBattleLostEvent() => _battleLostEvent;
    public UnityEvent GetBattleWonEvent() => _battleWonEvent;
    public UnityEvent GetBattleWonOrLostEvent() => _battleWonOrLostEvent;
    #endregion
}

public enum EGameplayStates
{
    PreBattle,
    Battle,
    PostBattleLost,
    PostBattleWon
};