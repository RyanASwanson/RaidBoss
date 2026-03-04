using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SB_TerraLord : SpecificBossFramework
{
    [Space]

    [Header("Unstable Precipice")]
    [SerializeField] private float _passiveTickRate;
    [SerializeField] private float _minimumTickValue;
    [SerializeField] private float _maximumTickValue;

    [SerializeField] private float _passiveMaxValue;

    [SerializeField] private float _zRotationMultiplier;

    [Space] 
    [SerializeField] private float _baseWeightMultiplier;
    [SerializeField] private List<float> _difficultyWeightMultiplier;

    [SerializeField] private AnimationCurve _movingTowardsMaxMultiplierBasedOnProgress;
    [SerializeField] private AnimationCurve _movingAwayFromMaxMultiplierBasedOnProgress;

    [Space] 
    [SerializeField] private CinemachineCameraShakeData _passiveMaxShake;
    
    private WaitForSeconds _passiveTickWait;
    
    private float _passiveHeroWeightMultiplier;

    private float _passiveCounterValue = 0;
    private float _passiveCounterProgressTowardsMax = 0;

    private bool _isPassiveMovingTowardsMax = false;
    
    private Coroutine _passiveProcessCoroutine;
    
    //Invokes the passive counter value scaled from -1 to 1
    private UnityEvent<float> _onPassivePercentUpdated = new UnityEvent<float>();

    [Space] 
    [SerializeField] private float _fallingRubbleAppearThreshold;
    [SerializeField] private Vector3 _fallingRubblePositionOffset;
    [SerializeField] private GameObject _fallingRubbleVFX;

    private GeneralVFXFunctionality _leftFallingRubble;
    private GeneralVFXFunctionality _rightFallingRubble;
    private GeneralVFXFunctionality _activeFallingRubble;

    private float rubbleProgressPercent;
    private bool _isRubbleFalling = false;

    [Space] 
    [SerializeField] private float _minimumFallingRubbleAudioRate;
    [SerializeField] private float _maximumFallingRubbleAudioRate;
    [SerializeField] private AnimationCurve _fallingRubbleAudioRateCurve;

    [SerializeField] private float _rubbleAudioStopDelay;

    private float _currentFallingRubbleAudioRate;
    private float _timeSinceLastRubbleAudioPlayed;
    private Coroutine _fallingRubbleAudioCoroutine;

    public const int RUBBLE_FALL_AUDIO_ID = 0;
    
    #region Passive

    /// <summary>
    /// At the start of the fight or after a stagger ends the boss passive is activated
    /// </summary>
    private void StartPassiveProcess()
    {
        if (!_passiveProcessCoroutine.IsUnityNull())
        {
            return;
        }

        if (GameStateManager.Instance.GetIsFightOver())
        {
            return;
        }
        
        _passiveProcessCoroutine = StartCoroutine(PassiveProcess());
    }

    /// <summary>
    /// Sets up the initial values for the passive to function
    /// </summary>
    private void SetStartingPassiveWeightMultiplier()
    {
        //Gets the difficulty
        EGameDifficulty selectedDifficulty = SelectionManager.Instance.GetSelectedDifficulty();
        //Scales the speed of the passive based on the difficulty
        _passiveHeroWeightMultiplier = _baseWeightMultiplier * _difficultyWeightMultiplier[(int)selectedDifficulty-1];
    }

    /// <summary>
    /// The process at which the passive ticks every few set amount of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator PassiveProcess()
    {
        while(HeroesManager.Instance.GetCurrentLivingHeroes().Count > 0)
        {
            yield return _passiveTickWait;
            PassiveTick();
        }
    }

    /// <summary>
    /// Each individual tick of the passive
    /// </summary>
    private void PassiveTick()
    {
        ChangePassiveCounterValue(CalculatePassiveHeroWeight());
    }

    /// <summary>
    /// Calculates how much to increase or decrease the passive value by each tick
    /// </summary>
    /// <returns></returns>
    private float CalculatePassiveHeroWeight()
    {
        float weightCounter = 0;

        //Determines the center of mass based on how far each hero is from the center in the X
        foreach (HeroBase heroBase in HeroesManager.Instance.GetCurrentLivingHeroes())
        {
            weightCounter += heroBase.transform.position.x * _passiveHeroWeightMultiplier;
        }

        //Scales the speed of the passive with how many heroes are alive
        weightCounter /= HeroesManager.Instance.GetCurrentLivingHeroes().Count;
        
        if (weightCounter > 0 && weightCounter < _minimumTickValue)
        {
            weightCounter = _minimumTickValue;
        }
        else if (weightCounter < 0 && weightCounter > -_minimumTickValue)
        {
            weightCounter = -_minimumTickValue;
        }

        if (weightCounter > _maximumTickValue)
        {
            weightCounter = _maximumTickValue;
        }
        else if (weightCounter < -_maximumTickValue)
        {
            weightCounter = -_maximumTickValue;
        }
        
        return weightCounter;
    }

    /// <summary>
    /// Changes the passive value based on the input float and calls and needed functionality
    /// </summary>
    /// <param name="val"></param>
    private void ChangePassiveCounterValue(float val)
    {
        _isPassiveMovingTowardsMax = (val > 0 != _passiveCounterValue + val > 0);
        if (_isPassiveMovingTowardsMax)
        {
            // We are moving away from max
            val *= _movingAwayFromMaxMultiplierBasedOnProgress.Evaluate(_passiveCounterProgressTowardsMax);
        }
        else
        {
            // We are moving towards max
            // Switching sides also counts as moving towards max even though we are initially moving away from max
            //  as we switch sides part way through
            val *= _movingTowardsMaxMultiplierBasedOnProgress.Evaluate(_passiveCounterProgressTowardsMax);
        }
        
        _passiveCounterValue += val;
        _passiveCounterProgressTowardsMax = Mathf.Abs(_passiveCounterValue / _passiveMaxValue);
        
        //Debug.Log("End result" + val);

        //Rotates the camera to demonstrate the imbalance of the arena
        RotateCameraBasedOnPassive(val);
        DetermineActivationOfRubble();
        InvokePassivePercentUpdate();

        //Checks if the passive value is too far in either direction
        CheckPassiveMax();
    }

    /// <summary>
    /// Stops the passive from ticking
    /// </summary>
    private void StopPassiveProcess()
    {
        // Check if the passive is in process
        if (_passiveProcessCoroutine.IsUnityNull())
        {
            // Stop as there is no passive to stop
            return;
        }

        StopCoroutine(_passiveProcessCoroutine);
        _passiveProcessCoroutine = null;
    }

    /// <summary>
    /// Rotates the camera to demonstrate the imbalance of the arena
    /// </summary>
    /// <param name="passiveDifference"> The difference of weight. Positive is a right rotation.</param>
    private void RotateCameraBasedOnPassive(float passiveDifference)
    {
        CameraGameManager.Instance.StartRotateCinemachineCamera
            (passiveDifference * _zRotationMultiplier, _passiveTickRate);
    }

    /// <summary>
    /// Checks if the passive counter value is too far in either direction
    /// </summary>
    private void CheckPassiveMax()
    {
        if (Mathf.Abs(_passiveCounterValue) >= _passiveMaxValue)
        {
            PassiveMax();
        }
    }

    /// <summary>
    /// If the passive goes too far in either direction all heroes are killed
    /// </summary>
    private void PassiveMax()
    {
        HeroesManager.Instance.ForceKillAllHeroes();
        
        CameraGameManager.Instance.StartCameraShake(_passiveMaxShake);
    }
    #endregion

    #region Rubble

    private void CreateRubbleVFX()
    {
        _leftFallingRubble = Instantiate(_fallingRubbleVFX, 
            CameraGameManager.Instance.GetVirtualCamera().gameObject.transform).GetComponent<GeneralVFXFunctionality>();
        
        _leftFallingRubble.transform.localPosition -= _fallingRubblePositionOffset;
        
        _rightFallingRubble = Instantiate(_fallingRubbleVFX, 
            CameraGameManager.Instance.GetVirtualCamera().gameObject.transform).GetComponent<GeneralVFXFunctionality>();
        
        _rightFallingRubble.transform.localPosition += _fallingRubblePositionOffset;
    }

    private void DetermineActivationOfRubble()
    {
        if (_passiveCounterValue >= _fallingRubbleAppearThreshold || _passiveCounterValue <= -_fallingRubbleAppearThreshold)
        {
            _isRubbleFalling = true;
            
            _activeFallingRubble = _passiveCounterValue > 0 ? _rightFallingRubble : _leftFallingRubble;
            _activeFallingRubble.PlayAllParticleSystems();
            _activeFallingRubble.SetLoopOfParticleSystems(true);
            
            StartPlayRubbleAudioProcess();
        }

        if (!_activeFallingRubble.IsUnityNull())
        {
            if (Mathf.Abs(_passiveCounterValue) < _fallingRubbleAppearThreshold)
            {
                _activeFallingRubble.SetLoopOfParticleSystems(false);

                _isRubbleFalling = false;
            }
            else
            {
                rubbleProgressPercent = (Mathf.Abs(_passiveCounterValue)- _fallingRubbleAppearThreshold) / (
                    _passiveMaxValue - _fallingRubbleAppearThreshold);
                
                _activeFallingRubble.SetEmissionRateMultiplierWithCurve(rubbleProgressPercent);

                DetermineFallingRubbleAudioRate();
            }
        }
    }

    private void StartPlayRubbleAudioProcess()
    {
        StopPlayRubbleAudioProcess();

        DetermineFallingRubbleAudioRate();
        rubbleProgressPercent = 0;
        
        _fallingRubbleAudioCoroutine = StartCoroutine(PlayRubbleAudioProcess());
    }

    private void StopPlayRubbleAudioProcess()
    {
        if (!_fallingRubbleAudioCoroutine.IsUnityNull())
        {
            StopCoroutine(_fallingRubbleAudioCoroutine);
        }
    }

    private IEnumerator PlayRubbleAudioProcess()
    {
        float rubbleContinueTimer = 0;
        while (_isRubbleFalling || rubbleContinueTimer <= _rubbleAudioStopDelay)
        {
            if (!_isRubbleFalling)
            {
                rubbleContinueTimer += Time.deltaTime;
            }
            
            _timeSinceLastRubbleAudioPlayed += Time.deltaTime;

            if (_timeSinceLastRubbleAudioPlayed >= _currentFallingRubbleAudioRate)
            {
                _timeSinceLastRubbleAudioPlayed = 0;
                PlayRubbleAudio();
            }

            yield return null;
        }
    }
    
    private void PlayRubbleAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].MiscellaneousBossAudio[RUBBLE_FALL_AUDIO_ID]);
    }

    private void DetermineFallingRubbleAudioRate()
    {
        _currentFallingRubbleAudioRate = Mathf.Lerp(_minimumFallingRubbleAudioRate, _maximumFallingRubbleAudioRate, 
            _fallingRubbleAudioRateCurve.Evaluate(rubbleProgressPercent));
    }
    #endregion
    
    #region BaseBoss
    /// <summary>
    /// Called when the fight begins.
    /// </summary>
    protected override void StartFight()
    {
        base.StartFight();
        
        _passiveTickWait = new WaitForSeconds(_passiveTickRate);
        
        SetStartingPassiveWeightMultiplier();
        StartPassiveProcess();

        CreateRubbleVFX();

    }

    /// <summary>
    /// Stops the passive when the boss is staggered
    /// </summary>
    protected override void BossStaggerOccured()
    {
        base.BossStaggerOccured();

        StopPassiveProcess();
    }

    /// <summary>
    /// Resumes the passive when the boss is no longer staggered
    /// </summary>
    protected override void BossNoLongerStaggeredOccured()
    {
        base.BossNoLongerStaggeredOccured();

        StartPassiveProcess();
    }

    protected override void BossDied()
    {
        base.BossDied();

        StopPassiveProcess();
    }

    #endregion

    #region Events
    private void InvokePassivePercentUpdate()
    {
        _onPassivePercentUpdated?.Invoke(GetPassiveCounterPercent());
    }
    #endregion

    #region Getters
    /// <summary>
    /// Gets a value from -1 to 1 of how far the balance is in either direction
    /// </summary>
    /// <returns></returns>
    private float GetPassiveCounterPercent()
    {
        return Mathf.Clamp(_passiveCounterValue / _passiveMaxValue, -_passiveMaxValue, _passiveMaxValue);
    }

    private float GetPassiveCounterPercentageAbsolute()
    {
        return Mathf.Abs(GetPassiveCounterPercent());
    }
    
    public UnityEvent<float> GetPassivePercentUpdatedEvent() => _onPassivePercentUpdated;
    #endregion
}
