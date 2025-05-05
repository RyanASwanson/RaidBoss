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
    [SerializeField] private float _timeToStart;

    private EGameplayStates currentEGameplayState = EGameplayStates.PreBattle;

    private UnityEvent _startOfBattleEvent = new UnityEvent();

    private UnityEvent _battleLostEvent = new UnityEvent();
    private UnityEvent _battleWonEvent = new UnityEvent();

    private UnityEvent _battleWonOrLostEvent = new UnityEvent();

    /// <summary>
    /// Starts the battle
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProgressToStart()
    {
        //Waits for a brief period before the battle is started
        yield return new WaitForSeconds(_timeToStart);
        SetGameplayState(EGameplayStates.Battle);
    }

    /// <summary>
    /// Changes which state the gameplay is to another one and invokes events for that state
    /// </summary>
    /// <param name="newEGameplayState"></param>
    public void SetGameplayState(EGameplayStates newEGameplayState)
    {
        if (currentEGameplayState == newEGameplayState || currentEGameplayState >= EGameplayStates.PostBattleLost) return;

        currentEGameplayState = newEGameplayState;

        switch(currentEGameplayState)
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
        InvokeBattleWonEvent();
    }

    #region BaseManager

    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        StartCoroutine(ProgressToStart());
    }
    #endregion
    
    #region Events
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
    public bool GetIsFightOver() => currentEGameplayState >= EGameplayStates.PostBattleLost;

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