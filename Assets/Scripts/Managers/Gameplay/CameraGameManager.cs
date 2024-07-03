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

    [Header("Boss Stagger")]
    [SerializeField] private float _staggerIntensity;
    [SerializeField] private float _staggerFrequency;
    [SerializeField] private float _staggerDuration;

    private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

    private Coroutine _cameraShakeCoroutine;

    /// <summary>
    /// Initiates the camera shake process
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
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

    private void CameraShakeOnBossStagger()
    {
        StartCameraShake(_staggerIntensity,_staggerFrequency,_staggerDuration);
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
        GameplayManagers.Instance.GetBossManager().GetBossBase()
            .GetBossStaggeredEvent().AddListener(CameraShakeOnBossStagger);
    }
    #endregion

    #region Getters
    public Camera GetGameplayCamera() => _gameplayCamera;

    
    #endregion
}
