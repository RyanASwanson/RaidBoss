using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : BaseUniversalManager
{
    public override void SetupUniversalManager()
    {

    }

    /// <summary>
    /// Loads a scene using the build ID
    /// </summary>
    /// <param name="id"></param>
    public void LoadSceneByID(int id)
    {
        SceneManager.LoadScene(id);
    }

    /// <summary>
    /// Loads a scene using the scriptable object associated with gameplay scenes
    /// </summary>
    /// <param name="levelSO"></param>
    public void LoadSceneByLevelSO(LevelSO levelSO)
    {
        LoadSceneByID(levelSO.GetLevelBuildID());
    }
}
