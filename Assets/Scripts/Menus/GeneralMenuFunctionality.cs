using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralMenuFunctionality : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ButtonLoadSceneByID(int sceneID)
    {
        UniversalManagers.Instance.GetSceneLoadManager().LoadSceneByID(sceneID);
    }

    
}
