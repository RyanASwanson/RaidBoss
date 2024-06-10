using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all managers used in gameplay scenes
/// </summary>
public class GameplayManagers : MonoBehaviour
{
    public static GameplayManagers Instance;

    [SerializeField] private PlayerInputGameplayManager _playerInputManager;
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private CameraGameManager _cameraManager;
    [SerializeField] private HeroesManager _heroesManager;
    [SerializeField] private EnvironmentManager _environmentManager;
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private BossManager _bossManager;
    [SerializeField] private PauseManager _pauseManager;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        foreach (BaseGameplayManager bgm in GetComponentsInChildren<BaseGameplayManager>())
            bgm.SetupGameplayManager();
    }

    #region Getters
    public PlayerInputGameplayManager GetPlayerInputManager() => _playerInputManager;
    public GameStateManager GetGameStateManager() => _gameStateManager;
    public CameraGameManager GetCameraManager() => _cameraManager;
    public HeroesManager GetHeroesManager() => _heroesManager;
    public EnvironmentManager GetEnvironmentManager() => _environmentManager;
    public GameUIManager GetGameUIManager() => _gameUIManager;
    public BossManager GetBossManager() => _bossManager;
    public PauseManager GetPauseManager() => _pauseManager;
        
    #endregion
}
