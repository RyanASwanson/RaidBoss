using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Menu in the middle of the selection screen
/// Handles starting the level and difficulty
/// </summary>
public class SelectCenter : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void PlayLevel()
    {
        UniversalManagers.Instance.GetSceneLoadManager().LoadCurrentlySelectedLevelSO();
    }

    public void SetDifficulty(GameDifficulty newDifficulty)
    {
        UniversalManagers.Instance.GetSelectionManager().SetSelectedDifficulty(newDifficulty);
    }
    
}
