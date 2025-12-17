using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.Serialization;

/// <summary>
/// Controls the functionality of the camera during gameplay
/// </summary>
public class CameraGameManager : MainGameplayManagerFramework
{
    public static CameraGameManager Instance;
    
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
    [SerializeField] private CinemachineCameraShakeData _bossStaggerShake;

    [Header("Boss Death")]
    [SerializeField] private CinemachineCameraShakeData _bossDeathShake;

    private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

    private Coroutine _cameraShakeCoroutine;
    private Coroutine _cameraShakeDecayCoroutine;

    [Space]
    [Header("Camera Rotation")]
    [SerializeField] private Transform _cinemachineTransform;
    private Coroutine _cinemachineCamRotationCoroutine;

    [SerializeField] private Transform _virtualCameraTransform;
    private Coroutine _virtualCamRotationCoroutine;
    
    [Space]
    [Header("Camera Zoom")]
    [SerializeField] private float _maxCameraZoom;
    [SerializeField] private float _cameraZoomRate;
    [SerializeField] private AnimationCurve _cameraZoomCurve;
    
    private float _baseCameraZoom;
    private Coroutine _cameraZoomCoroutine;

    #region Camera Shake
    /// <summary>
    /// Initiates the camera shake process
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    public void StartCameraShake(float intensity, float frequency, float duration)
    {
        if (!_cameraShakeCoroutine.IsUnityNull())
        {
            StopCoroutine(_cameraShakeCoroutine);
        }

        if (!_cameraShakeDecayCoroutine.IsUnityNull())
        {
            StopCoroutine(_cameraShakeDecayCoroutine);
        }
        
        _multiChannelPerlin.m_AmplitudeGain += intensity * _screenShakeMultiplier;
        _multiChannelPerlin.m_FrequencyGain += frequency * _screenShakeMultiplier;

        _cameraShakeCoroutine = StartCoroutine(CameraShake(duration));
    }

    public void StartCameraShake(CinemachineCameraShakeData cameraShakeData)
    {
        StartCameraShake(cameraShakeData.Intensity, cameraShakeData.Frequency, cameraShakeData.Duration);
    }

    private IEnumerator CameraShake(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        _cameraShakeDecayCoroutine = StartCoroutine(CameraShakeDecay());
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
        Debug.Log("Log" + _bossStaggerShake.Frequency + " " + _bossStaggerShake.Intensity + " " + _bossStaggerShake.Intensity);
        StartCameraShake(_bossStaggerShake);
    }

    private void CameraShakeOnBossDeath()
    {
        StartCameraShake(_bossDeathShake);
    }
    #endregion

    #region Camera Rotation
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
    #endregion
    
    #region Camera Zoom

    /*private void StartCameraZoomProcess()
    {
        if (!_cameraZoomCoroutine.IsUnityNull())
        {
            StopCoroutine(_cameraZoomCoroutine);
        }
        StartCoroutine(CameraZoomProcess());
    }

    private IEnumerator CameraZoomProcess()
    {
        yield return null;
    }
    
    /// <summary>
    /// Calculates the zoom of the camera
    /// </summary>
    private void CalculateCameraZoom()
    {
        _virtualCamera.m_Lens.OrthographicSize = 
            _baseCameraZoom * Mathf.Lerp(1,0,BossManager.Instance.GetBossBase().GetBossStats().GetBossHealthPercentage());
    }*/
    #endregion

    /// <summary>
    /// Sets the starting values for this script
    /// </summary>
    private void StartingValues()
    {
        _screenShakeMultiplier = SaveManager.Instance.GetScreenShakeIntensity();

        _multiChannelPerlin.m_AmplitudeGain = _minimumIntensity * _screenShakeMultiplier;
        _multiChannelPerlin.m_FrequencyGain = _minimumFrequency * _screenShakeMultiplier;
        
        _baseCameraZoom = _virtualCamera.m_Lens.OrthographicSize;
    }

    #region Base Manager
    /// <summary>
    /// Establishes the instance for the CameraGameManager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    
    /// <summary>
    /// Performs the needed set up on the CameraGameManager
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        
        _multiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        StartingValues();
    }

    protected override void SubscribeToEvents()
    {
        BossBase.Instance.GetBossStaggeredEvent().AddListener(CameraShakeOnBossStagger);
        
        GameStateManager.Instance.GetBattleWonEvent().AddListener(CameraShakeOnBossDeath);
    }
    #endregion

    #region Getters
    public Camera GetGameplayCamera() => _gameplayCamera;
    #endregion
}

[System.Serializable]
public class CinemachineCameraShakeData
{
    public float Intensity;
    public float Frequency;
    public float Duration;
}
