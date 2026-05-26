using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralCustomTutorialStep : MonoBehaviour
{
    [Header("General Step")]
    [SerializeField] private RectTransform _generalStepTransform;
    [SerializeField] private RectTransform _backgroundTransform;

    [SerializeField] private TextWithBackground _tutorialText;

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
