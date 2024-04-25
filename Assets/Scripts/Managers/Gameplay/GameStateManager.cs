using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : BaseGameplayManager
{
    private GameplayStates _currentGameplayState = GameplayStates.HeroSelection;
    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void IncrementGameplayState()
    {
        _currentGameplayState++;
        switch(_currentGameplayState)
        {
            case (GameplayStates.Battle):
                return;
        }
    }

    #region BaseManager
    public override void SubscribeToEvents()
    {
        
    }

    #endregion
}

public enum GameplayStates
{
    HeroSelection,
    Battle,
    PostBattle
};