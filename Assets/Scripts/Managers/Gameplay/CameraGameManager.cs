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

    

    float _screenShakeMultiplier = 1;

    [Space]
    [Header("Screen Shake")]
    [SerializeField] private float _minimumIntensity;
    [SerializeField] private float _minimumFrequency;

    [SerializeField] private float _intensityDecayMultiplier;
    [SerializeField] private float _frequencyDecayMultiplier;

    [Header("Boss Stagger")]
    [SerializeField] private float _staggerIntensity;
    [SerializeField] private float _staggerFrequency;
    [SerializeField] private float _staggerDuration;

    [Header("Boss Death")]
    [SerializeField] private float _bossDeathIntensity;
    [SerializeField] private float _bossDeathFrequency;
    [SerializeField] private float _bossDeathDuration;

    private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

    private Coroutine _cameraShakeCoroutine;
    private Coroutine _cameraShakeDecayCoroutine;

    [Space]
    [Header("Camera Rotation")]
    [SerializeField] private Transform _cinemachineTransform;
    private Coroutine _cinemachineCamRotationCoroutine;

    [SerializeField] private Transform _virtualCameraTransform;
    private Coroutine _virtualCamRotationCoroutine;


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
        if (_cameraShakeDecayCoroutine != null)
            StopCoroutine(_cameraShakeDecayCoroutine);

        

        _multiChannelPerlin.m_AmplitudeGain += intensity * _screenShakeMultiplier;
        _multiChannelPerlin.m_FrequencyGain += frequency * _screenShakeMultiplier;

        _cameraShakeCoroutine = StartCoroutine(CameraShake(duration));
    }

    private IEnumerator CameraShake(float duration)
    {
        yield return new WaitForSeconds(duration);
        /*_multiChannelPerlin.m_AmplitudeGain -= intensity;
        _multiChannelPerlin.m_FrequencyGain -= frequency;*/
        _cameraShakeCoroutine = StartCoroutine(CameraShakeDecay());
    }

    /// <summary>
    /// Process of reducing the camera shake over time
    /// </summary>
    /// <returns></returns>
    private IEnumerator CameraShakeDecay()
    {
        while (_multiChannelPerlin.m_AmplitudeGain > _minimumIntensity || _multiChannelPerlin.m_FrequencyGain > _minimumFrequency)
        {
            if(_multiChannelPerlin.m_AmplitudeGain > +_minimumIntensity)
            {
                _multiChannelPerlin.m_AmplitudeGain -= Time.deltaTime * _intensityDecayMultiplier;
                if (_multiChannelPerlin.m_AmplitudeGain < _minimumIntensity)
                    _multiChannelPerlin.m_AmplitudeGain = _minimumIntensity;
            }
            
            if(_multiChannelPerlin.m_FrequencyGain > _minimumFrequency)
            {
                _multiChannelPerlin.m_FrequencyGain -= Time.deltaTime * _frequencyDecayMultiplier;
                if (_multiChannelPerlin.m_FrequencyGain < _minimumFrequency)
                    _multiChannelPerlin.m_FrequencyGain = _minimumFrequency;
            }
            
            yield return null;
        }
    }

    private void CameraShakeOnBossStagger()
    {
        StartCameraShake(_staggerIntensity,_staggerFrequency,_staggerDuration);
    }

    private void CameraShakeOnBossDeath()
    {
        StartCameraShake(_bossDeathIntensity, _bossDeathFrequency, _bossDeathDuration);
    }

    public void StartRotateCinemachineCamera(float directionMultiplier, float processTime)
    {
        if (_virtualCamRotationCoroutine != null)
            StopCoroutine(_virtualCamRotationCoroutine);

        _virtualCamRotationCoroutine = StartCoroutine(RotateCinemachineCameraProcess(directionMultiplier,processTime));
    }

    private IEnumerator RotateCinemachineCameraProcess(float directionMultiplier, float processTime)
    {
        float coroutineTimer = 0;

        while(coroutineTimer < processTime)
        {
            coroutineTimer += Time.deltaTime;
            _virtualCameraTransform.Rotate(new Vector3(0, 0, Time.deltaTime* directionMultiplier));
            
            yield return null;
        }

        _virtualCamRotationCoroutine = null;
    }

    

    private void StartingValues()
    {
        _screenShakeMultiplier = UniversalManagers.Instance.GetSaveManager().GSD._screenShakeStrength;

        _multiChannelPerlin.m_AmplitudeGain = _minimumIntensity * _screenShakeMultiplier;
        _multiChannelPerlin.m_FrequencyGain = _minimumFrequency * _screenShakeMultiplier;
    }


    #region BaseManager
    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();

        _multiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        StartingValues();

    }

    protected override void SubscribeToEvents()
    {
        GameplayManagers.Instance.GetBossManager().GetBossBase()
            .GetBossStaggeredEvent().AddListener(CameraShakeOnBossStagger);
        GameplayManagers.Instance.GetGameStateManager().GetBattleWonEvent()
            .AddListener(CameraShakeOnBossDeath);
    }
    #endregion

    #region Getters
    public Camera GetGameplayCamera() => _gameplayCamera;

    
    #endregion
}
