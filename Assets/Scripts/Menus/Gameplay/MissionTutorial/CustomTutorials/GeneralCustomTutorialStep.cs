using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GeneralCustomTutorialStep : MonoBehaviour
{
    [Header("General Step")]
    [SerializeField] private RectTransform _generalStepTransform;
    [SerializeField] private RectTransform _backgroundTransform;

    [Space]
    [SerializeField] private TextWithBackground _tutorialText;
    [SerializeField] private RectTransform _textTransform;

    [Space] 
    [SerializeField] private Image _tutorialImage;
    [SerializeField] private RectTransform _tutorialImageTransform;

    [Space] 
    [SerializeField] private float _buttonInteractabilityDelay;
    [SerializeField] private float _continueButtonOffset;
    [SerializeField] private RectTransform _continueButtonTransform;
    [SerializeField] private Button _continueButton;
    private WaitForSeconds _buttonInteractabilityWait;
    
    [Space]
    [SerializeField] private CurveProgression _generalStepScaleCurve;
    
    private CustomTutorialStep _currentTutorialStep;

    private bool _isBufferingNewDisplay = false;

    public void SetUp()
    {
        _buttonInteractabilityWait = new WaitForSeconds(_buttonInteractabilityDelay);
    }

    public void AttemptShowGeneralStep(CustomTutorialStep customTutorialStep)
    {
        _currentTutorialStep = customTutorialStep;
        
        if (_generalStepScaleCurve.CurveStatus == ECurveStatus.AtMinValue)
        {
            ShowGeneralStep();
        }
        else
        {
            _isBufferingNewDisplay = true;

            HideGeneralStep();
        }
    }
    
    public void ShowGeneralStep()
    {
        _generalStepScaleCurve.StartMovingUpOnCurve();
    }

    public void HideGeneralStep()
    {
        _generalStepScaleCurve.StartMovingDownOnCurve();
    }

    public void GeneralStepReachedMinimumSize()
    {
        if (_isBufferingNewDisplay)
        {
            _isBufferingNewDisplay = false;
            ShowGeneralStep();
        }
    }

    public void UpdateGeneralStepUIContents()
    {
        _generalStepTransform.anchoredPosition = _currentTutorialStep.GeneralStepUI.CustomTutorialPosition;
        _backgroundTransform.sizeDelta = _currentTutorialStep.GeneralStepUI.CustomTutorialDimensions;
        
        Vector2 continueButtonTransform = _backgroundTransform.sizeDelta;
        continueButtonTransform.Set(0, (-continueButtonTransform.y / 2)-_continueButtonOffset);
        _continueButtonTransform.anchoredPosition = continueButtonTransform;
        
        _tutorialText.UpdateText(_currentTutorialStep.GeneralStepUI.CustomTutorialUIText);
        
        _textTransform.anchoredPosition = _currentTutorialStep.GeneralStepUI.CustomTutorialTextOffsetPosition;

        if (!_currentTutorialStep.GeneralStepUI.CustomTutorialImage.IsUnityNull())
        {
            _tutorialImage.sprite = _currentTutorialStep.GeneralStepUI.CustomTutorialImage;
            _tutorialImage.color = new Color(1f, 1f, 1f, 1f);
            
            _tutorialImageTransform.anchoredPosition = _currentTutorialStep.GeneralStepUI.CustomTutorialImageOffsetPosition;
            _tutorialImageTransform.sizeDelta = _currentTutorialStep.GeneralStepUI.CustomTutorialImageDimensions;
            _tutorialImageTransform.localScale = _currentTutorialStep.GeneralStepUI.CustomTutorialImageScale;
        }
        else
        {
            _tutorialImage.color = new Color(1f, 1f, 1f, 0f);
        }

        StartButtonInteractabilityDelay();
    }

    private void StartButtonInteractabilityDelay()
    {
        StartCoroutine(ButtonInteractabilityDelay());
    }

    private IEnumerator ButtonInteractabilityDelay()
    {
        _continueButton.interactable = false;
        yield return _buttonInteractabilityWait;
        _continueButton.interactable = true;
    }
}
