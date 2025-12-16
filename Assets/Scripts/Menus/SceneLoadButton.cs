using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadButton : MonoBehaviour
{
    [SerializeField] private ELoadableScenes _sceneToLoad;
    
    private Button _associatedButton;
    
    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out _associatedButton))
        {
            _associatedButton.onClick.AddListener(LoadAssociatedScene);
        }
    }

    void OnDestroy()
    {
        if (!_associatedButton.IsUnityNull())
        {
            _associatedButton.onClick.RemoveListener(LoadAssociatedScene);
        }
    }

    private void LoadAssociatedScene()
    {
        SceneLoadManager.Instance.LoadSceneByEnum(_sceneToLoad);
    }
    
    #region Setters
    // Using these specific set functions as buttons cannot directly call SetSceneToLoad because of the enum
    public void SetSceneToLoadToMainMenu()
    {
        SetSceneToLoad(ELoadableScenes.MainMenu);
    }
    
    public void SetSceneToLoadToMap()
    {
        SetSceneToLoad(ELoadableScenes.Map);
    }
    
    public void SetSceneToLoadToSelection()
    {
        SetSceneToLoad(ELoadableScenes.Selection);
    }
    
    public void SetSceneToLoad(ELoadableScenes scene)
    {
        _sceneToLoad = scene;
    }

    #endregion
}
