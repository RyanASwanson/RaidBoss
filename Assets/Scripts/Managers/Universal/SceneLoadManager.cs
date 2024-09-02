using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneLoadManager : BaseUniversalManager
{
    [Header("SceneTransitions")]
    [SerializeField] private Animator _sceneTransitionAnimator;
    private const string _closeInFromSidesAnimTrigger = "CloseInFromSides";

    private const float _sceneTransitionTime = 1;

    private const int _mainMenuSceneID = 0;
    private const int _selectionSceneID = 1;

    private Coroutine _sceneTransitionCoroutine;

    private UnityEvent _startOfSceneLoadEvent = new UnityEvent();
    private UnityEvent _endOfSceneLoadEvent = new UnityEvent();



    /// <summary>
    /// Loads a scene using the build ID
    /// </summary>
    /// <param name="id"></param>
    public void LoadSceneByID(int id)
    {
        if(CanLoadScene())
            _sceneTransitionCoroutine = StartCoroutine(SceneLoadProcess(id));
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
        InvokeStartOfSceneLoadEvent();

        _sceneTransitionAnimator.SetTrigger(_closeInFromSidesAnimTrigger);

        //Loads the scene after half of the screen transition has occurred
        yield return new WaitForSeconds(_sceneTransitionTime / 2);
        SceneManager.LoadScene(id);
        yield return new WaitForSeconds(_sceneTransitionTime / 2);

        InvokeEndOfSceneLoadEvent();
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
    /// Loads a scene based on which 
    /// </summary>
    public void LoadCurrentlySelectedLevelSO()
    {
        LoadSceneByLevelSO(UniversalManagers.Instance.GetSelectionManager().GetSelectedLevel());
    }

    public void LoadMainMenuScene()
    {
        LoadSceneByID(_mainMenuSceneID);
    }

    public void LoadSelectionScene()
    {
        UniversalManagers.Instance.GetSelectionManager().ResetSelectionData();
        LoadSceneByID(_selectionSceneID);
    }

    public void ReloadCurrentScene()
    {
        LoadSceneByID(SceneManager.GetActiveScene().buildIndex);
    }

    #region BaseManager

    #endregion

    #region Events
    private void InvokeStartOfSceneLoadEvent()
    {
        _startOfSceneLoadEvent?.Invoke();
    }
    private void InvokeEndOfSceneLoadEvent()
    {
        _endOfSceneLoadEvent?.Invoke();
    }
    #endregion

    #region Getters
    public UnityEvent GetStartOfSceneLoadEvent() => _startOfSceneLoadEvent;
    public UnityEvent GetEndOfSceneLoadEvent() => _endOfSceneLoadEvent;
    #endregion
}
