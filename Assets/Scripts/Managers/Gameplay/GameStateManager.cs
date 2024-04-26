using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : BaseGameplayManager
{
    private GameplayStates _currentGameplayState = GameplayStates.HeroSelection;

    private UnityEvent _startOfFightEvent = new UnityEvent();
    private UnityEvent _
    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetGameplayState(GameplayStates newGameplayState)
    {
        if (_currentGameplayState == newGameplayState) return;

        _currentGameplayState = newGameplayState;

        switch(_currentGameplayState)
        {
            case (GameplayStates.Battle):
                break;
        }
    }

    #region BaseManager
    public override void SubscribeToEvents()
    {
        
    }

    public 

    #endregion
}

public enum GameplayStates
{
    HeroSelection,
    Battle,
    PostBattle
};