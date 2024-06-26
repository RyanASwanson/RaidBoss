using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : BaseUniversalManager
{
    [Header("SceneTransitions")]
    [SerializeField] private Animator _sceneTransitionAnimator;
    private const string _closeInFromSidesAnimTrigger = "CloseInFromSides";

    private const float _sceneTransitionTime = 1;

    private const int _mainMenuSceneID = 0;
    private const int _selectionSceneID = 1;

    private Coroutine _sceneTransitionCoroutine;

    public override void SetupUniversalManager()
    {

    }

    /// <summary>
    /// Loads a scene using the build ID
    /// </summary>
    /// <param name="id"></param>
    public void LoadSceneByID(int id)
    {
        if(CanLoadScene())
            _sceneTransitionCoroutine = StartCoroutine(SceneLoadProcess(id));
    }

    private bool CanLoadScene()
    {
        return _sceneTransitionCoroutine == null;
    }

    private IEnumerator SceneLoadProcess(int id)
    {
        _sceneTransitionAnimator.SetTrigger(_closeInFromSidesAnimTrigger);
        yield return new WaitForSeconds(_sceneTransitionTime / 2);
        SceneManager.LoadScene(id);
        yield return new WaitForSeconds(_sceneTransitionTime / 2);

        _sceneTransitionCoroutine = null;
    }

    /// <summary>
    /// Loads a scene using the scriptable object associated with gameplay scenes
    /// </summary>
    /// <param name="levelSO"></param>
    public void LoadSceneByLevelSO(LevelSO levelSO)
    {
        LoadSceneByID(levelSO.GetLevelBuildID());
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
}
