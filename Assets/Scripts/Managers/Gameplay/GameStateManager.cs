using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : BaseGameplayManager
{
    private GameplayStates _currentGameplayState = GameplayStates.HeroSelection;
    // Start is called before the first frame update
    void Start()
    {
        
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
        throw new System.NotImplementedException();
    }
    #endregion
}

public enum GameplayStates
{
    HeroSelection,
    Battle,
    PostBattle
};