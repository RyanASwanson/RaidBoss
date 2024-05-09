using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCenter : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void PlayLevel()
    {
        UniversalManagers.Instance.GetSceneLoadManager().LoadSceneByLevelSO(
            UniversalManagers.Instance.GetSelectionManager().GetSelectedLevel());
    }

    public void SetDifficulty(GameDifficulty newDifficulty)
    {
        UniversalManagers.Instance.GetSelectionManager().SetSelectedDifficulty(newDifficulty);
    }
    
}
