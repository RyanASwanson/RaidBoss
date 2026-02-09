using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Performs the functionality for loading scenes
/// </summary>
public class SceneLoadManager : MainUniversalManagerFramework
{
    public static SceneLoadManager Instance;
    
    [Header("SceneTransitions")]
    [SerializeField] private Animator _sceneTransitionAnimator;

    private const string ST_CLOSE_IN_FROM_SIDES_ANIM_TRIGGER = "CloseInFromSides";

    private const float _sceneTransitionTime = 1;
    
    private WaitForSeconds _sceneTransitionWait = new WaitForSeconds(_sceneTransitionTime/2);

    private const int MAIN_MENU_SCENE_ID = 0;
    private const int MAP_SCENE_SCENE_ID = 1;
    private const int SELECTION_SCENE_ID = 2;

    private Coroutine _sceneTransitionCoroutine;

    private UnityEvent _onStartOfSceneLoad = new UnityEvent();
    private UnityEvent _onEndOfSceneLoad = new UnityEvent();
    
    private UnityEvent _onGameplaySceneLoaded = new UnityEvent();
    
    /// <summary>
    /// Loads a scene using the build ID
    /// </summary>
    /// <param name="id"></param>
    public void LoadSceneByID(int id)
    {
        if (CanLoadScene())
        {
            _sceneTransitionCoroutine = StartCoroutine(SceneLoadProcess(id));
        }
    }

    public void LoadSceneByEnum(ELoadableScenes scene)
    {

        switch (scene)
        {
            case ELoadableScenes.MainMenu:
                LoadMainMenuScene();
                break;
            case ELoadableScenes.Map:
                LoadMapScene();
                break;
            case ELoadableScenes.Selection:
                LoadSelectionScene();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Returns if a scene load can occur.
    /// Prevented if scene load is currently in process.
    /// </summary>
    /// <returns></returns>
    private bool CanLoadScene()
    {
        return _sceneTransitionCoroutine == null;
    }

    private IEnumerator SceneLoadProcess(int id)
    {
        InvokeOnStartOfSceneLoadEvent();
        EventSystem.current.enabled = false;

        _sceneTransitionAnimator.SetTrigger(ST_CLOSE_IN_FROM_SIDES_ANIM_TRIGGER);
        
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SceneLoadUserInterfaceAudio.SceneLoadStart);

        //Loads the scene after half of the screen transition has occurred
        yield return _sceneTransitionWait;
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(id);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null; // Wait for the next frame
        }
        
        asyncLoad.allowSceneActivation = true;
        
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SceneLoadUserInterfaceAudio.SceneLoadMiddle);
        
        //yield return new WaitForSeconds(.2f);
        
        yield return _sceneTransitionWait;
        
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SceneLoadUserInterfaceAudio.SceneLoadEnd);

        InvokeOnEndOfSceneLoadEvent();
        EventSystem.current.enabled = true;
        _sceneTransitionCoroutine = null;
    }

    /// <summary>
    /// Loads a scene using the scriptable object associated with gameplay scenes.
    /// </summary>
    /// <param name="levelSO"></param>
    public void LoadSceneByLevelSO(LevelSO levelSO)
    {
        LoadSceneByID(levelSO.GetLevelBuildID());
    }

    /// <summary>
    /// Loads a scene based on which one is currently selected
    /// </summary>
    public void LoadCurrentlySelectedLevelSO()
    {
        LoadSceneByLevelSO(SelectionManager.Instance.GetSelectedLevel());
    }

    public void LoadMainMenuScene()
    {
        LoadSceneByID(MAIN_MENU_SCENE_ID);
    }

    public void LoadMapScene()
    {
        SelectionManager.Instance.SetSelectedGameMode(EGameMode.Missions);
        SelectionManager.Instance.ResetSelectionData();
        LoadSceneByID(MAP_SCENE_SCENE_ID);
    }

    public void LoadSelectionScene()
    {
        SelectionManager.Instance.SetSelectedGameMode(EGameMode.Free);
        SelectionManager.Instance.ResetSelectionData();
        LoadSceneByID(SELECTION_SCENE_ID);
    }

    public void ReloadCurrentScene()
    {
        LoadSceneByID(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameplaySceneLoaded()
    {
        InvokeOnGameplaySceneLoaded();
    }

    #region BaseManager
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    #endregion

    #region Events
    private void InvokeOnStartOfSceneLoadEvent()
    {
        _onStartOfSceneLoad?.Invoke();
    }
    private void InvokeOnEndOfSceneLoadEvent()
    {
        _onEndOfSceneLoad?.Invoke();
    }

    private void InvokeOnGameplaySceneLoaded()
    {
        _onGameplaySceneLoaded?.Invoke();
    }
    #endregion

    #region Getters
    public UnityEvent GetOnStartOfSceneLoad() => _onStartOfSceneLoad;
    public UnityEvent GetOnEndOfSceneLoad() => _onEndOfSceneLoad;
    public UnityEvent GetOnGameplaySceneLoaded() => _onGameplaySceneLoaded;
    #endregion
}

public enum ELoadableScenes
{
    MainMenu,
    Map,
    Selection
};