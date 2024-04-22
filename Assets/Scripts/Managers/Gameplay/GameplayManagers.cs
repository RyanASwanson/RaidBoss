using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManagers : MonoBehaviour
{
    public static GameplayManagers Instance;

    [SerializeField] private PlayerInputGameplayManager _playerInputManager;
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private CameraGameManager _cameraManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    #region Getters
    public PlayerInputGameplayManager GetPlayerInputManager => _playerInputManager;
    public GameStateManager GetGameStateManager => _gameStateManager;
    public CameraGameManager GetCameraManager => _cameraManager;
    #endregion
}
