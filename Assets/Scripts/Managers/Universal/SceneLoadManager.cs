using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadSceneByID(int id)
    {
        SceneManager.LoadScene(id);
    }

    public void LoadSceneByLevelSO(LevelSO levelSO)
    {
        LoadSceneByID(levelSO.GetLevelBuildID());
    }
}
