using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGameManager : BaseGameplayManager
{
    [SerializeField] private Camera _gameplayCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    #region BaseManager
    public override void SubscribeToEvents()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Getters
    public Camera GetGameplayCamera() => _gameplayCamera;
    #endregion
}
