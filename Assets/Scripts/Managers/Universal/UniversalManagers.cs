using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalManagers : MonoBehaviour
{
    public static UniversalManagers Instance;

    [SerializeField] private SceneLoadManager _sceneLoadManager;
    [SerializeField] private SelectionManager _selectionManager;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            ConfirmedUniversalManager();
            return;
        }

        Destroy(gameObject);
    }

    private void ConfirmedUniversalManager()
    {
        foreach (BaseUniversalManager bgm in GetComponentsInChildren<BaseUniversalManager>())
            bgm.SetupUniversalManager();
    }

    #region Get Managers
    public SceneLoadManager GetSceneLoadManager() => _sceneLoadManager;
    public SelectionManager GetSelectionManager() => _selectionManager;

    #endregion
}
