using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraGameManager : BaseGameplayManager
{
    [SerializeField] private Camera _gameplayCamera;
    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
    }


    #region BaseManager
    public override void SubscribeToEvents()
    {
        
    }
    #endregion

    #region Getters
    public Camera GetGameplayCamera() => _gameplayCamera;

    
    #endregion
}
