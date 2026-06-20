using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MythicPlusScalerSelection : MonoBehaviour
{
    [SerializeField] private float _minDirectionalButtonSpeed;
    [SerializeField] private float _maxDirectionalButtonSpeed;
    [SerializeField] private float _timeToReachMaxDirectionalButtonSpeed;
    [SerializeField] private AnimationCurve _directionButtonSpeedCurve;
    
    [Space]
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [SerializeField] private CurveProgression _leftButtonPressScaleCurve;
    [SerializeField] private CurveProgression _rightButtonPressScaleCurve;
    private Coroutine _directionalButtonHeldCoroutine;
    
    [SerializeField] private Button _lowestLevelButton;
    [SerializeField] private Button _highestLevelButton;

    [Space] 
    [SerializeField] private TextWithBackground _levelNumberText;

    [Space] 
    [SerializeField] private CurveProgression _textDecreaseScaleCurve;
    [SerializeField] private CurveProgression _textIncreaseScaleCurve;
    
    [Space]
    [SerializeField] private ButtonPressAudio _buttonPressAudio;

    [Space] 
    [SerializeField] private Image _lockCover;
    private bool _isLocked;
    
    [Space]
    [SerializeField] private DifficultyDropdown _difficultyDropdown;
    
    // Start is called before the first frame update
    void Start()
    {
        DetermineLockState();
        
        if (!_isLocked)
        {
            SetMythicPlusLevel(SelectionManager.Instance.GetMythicPlusLevel(),true);
        }
        else
        {
            _leftButton.interactable = false;
            _rightButton.interactable = false;
        }
        
    }

    public void LeftButtonPressed()
    {
        if (!_leftButton.interactable)
        {
            return;
        }
        
        SetMythicPlusLevel(SelectionManager.Instance.GetMythicPlusLevel() - 1,false);

        StartDirectionButtonHeldProcess(-1,_leftButton);
    }

    public void RightButtonPressed()
    {
        if (!_rightButton.interactable)
        {
            return;
        }
        
        SetMythicPlusLevel(SelectionManager.Instance.GetMythicPlusLevel() + 1,false);

        StartDirectionButtonHeldProcess(1,_rightButton);
    }

    public void StartDirectionButtonHeldProcess(int direction,Button associatedButton)
    {
        if (!associatedButton.interactable)
        {
            return;
        }
        
        StopDirectionButtonHeldProcess();

        _directionalButtonHeldCoroutine = StartCoroutine(DirectionalButtonHeldProcess(direction,associatedButton));
    }

    public void StopDirectionButtonHeldProcess()
    {
        if (!_directionalButtonHeldCoroutine.IsUnityNull())
        {
            StopCoroutine(_directionalButtonHeldCoroutine);
        }
    }

    private IEnumerator DirectionalButtonHeldProcess(int direction, Button associatedButton)
    {
        float curveProgress = 0;
        float curveTime = 0;

        bool hasReachedEnd = false;

        while (!hasReachedEnd)
        {
            curveTime = _directionButtonSpeedCurve.Evaluate(curveProgress);
            
            curveTime = 1/Mathf.Lerp(_minDirectionalButtonSpeed,_maxDirectionalButtonSpeed,curveProgress);
            
            yield return new WaitForSeconds(curveTime);
            curveProgress += (curveTime) /_timeToReachMaxDirectionalButtonSpeed;
            
            SetMythicPlusLevel(SelectionManager.Instance.GetMythicPlusLevel() + direction,false);
            
            if (!associatedButton.interactable)
            {
                hasReachedEnd = true;
            }
        }
    }

    public void LowestLevelButtonPressed()
    {
        SetMythicPlusLevel(0,false);
    }

    public void HighestLevelButtonPressed()
    {
        SetMythicPlusLevel(SaveManager.Instance.GetHighestMythicPlusLevelUnlocked(),false);
    }

    public void DetermineButtonInteractability()
    {
        _leftButton.interactable = SelectionManager.Instance.GetMythicPlusLevel() > 0;
        
        _rightButton.interactable = SelectionManager.Instance.GetMythicPlusLevel() !=
                                    SaveManager.Instance.GetHighestMythicPlusLevelUnlocked();
    }

    private void SetMythicPlusLevel(int level, bool isCalledByStart)
    {
        if (_isLocked)
        {
            return;
        }
        
        if (level > SelectionManager.Instance.GetMythicPlusLevel())
        {
            _textIncreaseScaleCurve.StartMovingUpOnCurve();
            _rightButtonPressScaleCurve.StartMovingUpOnCurve();
        }
        else
        {
            _textDecreaseScaleCurve.StartMovingUpOnCurve();
            _leftButtonPressScaleCurve.StartMovingUpOnCurve();
        }
        
        SelectionManager.Instance.SetMythicPlusScalingLevel(level);
        DetermineButtonInteractability();
        UpdateLevelText();

        if (!isCalledByStart)
        {
            _difficultyDropdown.UpdateDifficultyHeaderText(true);
            PlayButtonPressedSound();
        }
    }

    private void UpdateLevelText()
    {
        _levelNumberText.UpdateText(SelectionManager.Instance.GetMythicPlusLevel().ToString());
    }
    
    private void PlayButtonPressedSound()
    {
        _buttonPressAudio.PlayButtonPressedSound();
    }
    
    private void DetermineLockState()
    {
        SetDifficultyLock(SaveManager.Instance.GetHighestDifficultyUnlocked() < EGameDifficulty.MythicPlus);
    }
    
    private void SetDifficultyLock(bool isLocked)
    {
        _isLocked = isLocked;
        
        _lockCover.enabled = isLocked;
    }
}
