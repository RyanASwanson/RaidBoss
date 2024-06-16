using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Controls the functionality of the camera during gameplay
/// </summary>
public class CameraGameManager : BaseGameplayManager
{
    [SerializeField] private Camera _gameplayCamera;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

    private Coroutine _cameraShakeCoroutine;

    public void StartCameraShake(float intensity, float frequency, float duration)
    {
        if (_cameraShakeCoroutine != null)
            StopCoroutine(_cameraShakeCoroutine);

        _cameraShakeCoroutine = StartCoroutine(CameraShake(intensity, frequency,duration));
    }

    private IEnumerator CameraShake(float intensity, float frequency, float duration)
    {
        _multiChannelPerlin.m_AmplitudeGain += intensity;
        _multiChannelPerlin.m_FrequencyGain += frequency;
        yield return new WaitForSeconds(duration);
        _multiChannelPerlin.m_AmplitudeGain -= intensity;
        _multiChannelPerlin.m_FrequencyGain -= frequency;
    }

    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();

        _multiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        //StartCameraShake(2, 2, 3);
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
