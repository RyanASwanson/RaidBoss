using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManagers : MonoBehaviour
{
    [SerializeField] private PlayerInputGameplayManager _playerInputManager;
    [SerializeField] private GameStateManager _gameStateManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    #region Getters
    public PlayerInputGameplayManager GetPlayerInputManager => _playerInputManager;
    public GameStateManager GetGameStateManager => _gameStateManager;
    #endregion
}
