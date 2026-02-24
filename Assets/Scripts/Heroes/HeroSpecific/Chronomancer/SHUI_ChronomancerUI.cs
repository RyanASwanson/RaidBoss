using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SHUI_ChronomancerUI : SpecificHeroUIFramework
{
    [SerializeField] private TextWithBackground _textWithBackground;

    [Space] 
    [SerializeField] private float _startingScale;
    [SerializeField] private float _maxScale;
    [SerializeField] private float _healingForMaxScale;
    [SerializeField] private AnimationCurve _scaleCurve;
    private float _scaleProgress;

    [Space] 
    [SerializeField] private float _activationTime;
    private WaitForSeconds _activationWait;

    [Space] 
    [SerializeField] private Image _upperSand;
    [SerializeField] private Image _lowerSand;
    
    [Space]
    [SerializeField] private Animator _uiAnimator;
    [SerializeField] private GameObject _numberHolder; 

    private const string CHRONOMANCER_UI_INCREASE_ANIM_TRIGGER = "Increase";
    private const string CHRONOMANCER_UI_DECREASE_ANIM_TRIGGER = "Decrease";
    
    private const string CHRONOMANCER_UI_ACTIVATION_ANIM_TRIGGER = "Activate";
    private const string CHRONOMANCER_UI_ACTIVATION_ANIM_BOOL = "Activated";
    
    
    private SH_Chronomancer _associatedChronomancer;

    private int _lastTextNumber = 0;

    private bool _isActivationProcessActive = false;

    public void AdditionalSetUp(SH_Chronomancer chronomancer)
    {
        _associatedChronomancer = chronomancer;
        _activationWait = new WaitForSeconds(_activationTime);
    }
    
    private void UpdateHealthStoredDisplayText(float value)
    {
        int newValue = Mathf.RoundToInt(value);
        if (_lastTextNumber > newValue)
        {
            PlayDecreaseAnimation();
        }
        // Else if as to not count as an increase when the value is the exact same
        else if (_lastTextNumber < newValue)
        {
            PlayIncreaseAnimation();
        }
        
        _lastTextNumber = Mathf.RoundToInt(value);
        SetTextNumber();

        ModifyTextScale();
        UpdateSandFill();
    }

    private void SetTextNumber()
    {
        if (_isActivationProcessActive)
        {
            return;
        }

        _textWithBackground.UpdateText(_lastTextNumber.ToString());
    }

    private void ResetSpecificHeroText()
    {
        UpdateHealthStoredDisplayText(0);
    }

    private void ModifyTextScale()
    {
        if (_isActivationProcessActive)
        {
            return;
        }
        _scaleProgress = Mathf.Clamp(_lastTextNumber / _healingForMaxScale, 0,1);
        float newScale = Mathf.Lerp(_startingScale,_maxScale,_scaleCurve.Evaluate(_scaleProgress));
        _numberHolder.transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    private void UpdateSandFill()
    {
        _upperSand.fillAmount = 1-_scaleProgress;
        _lowerSand.fillAmount = _scaleProgress;
    }

    private void AbilityActivation()
    {
        StartCoroutine(ActivationProcess());
    }

    private IEnumerator ActivationProcess()
    {
        _isActivationProcessActive = true;
        PlayActivationAnimation();
        yield return _activationWait;
        StopActivationAnimation();
        _isActivationProcessActive = false;
        
        ResetSpecificHeroText();
        ModifyTextScale();
    }

    private void PlayIncreaseAnimation()
    {
        _uiAnimator.SetTrigger(CHRONOMANCER_UI_INCREASE_ANIM_TRIGGER);
    }

    private void PlayDecreaseAnimation()
    {
        _uiAnimator.SetTrigger(CHRONOMANCER_UI_DECREASE_ANIM_TRIGGER);
    }

    private void PlayActivationAnimation()
    {
        _uiAnimator.SetTrigger(CHRONOMANCER_UI_ACTIVATION_ANIM_TRIGGER);
        _uiAnimator.SetBool(CHRONOMANCER_UI_ACTIVATION_ANIM_BOOL, true);
    }

    private void StopActivationAnimation()
    {
        _uiAnimator.ResetTrigger(CHRONOMANCER_UI_INCREASE_ANIM_TRIGGER);
        _uiAnimator.ResetTrigger(CHRONOMANCER_UI_DECREASE_ANIM_TRIGGER);
        
        _uiAnimator.SetBool(CHRONOMANCER_UI_ACTIVATION_ANIM_BOOL, false);
    }
    
    #region BaseHeroUI

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        _associatedChronomancer.GetOnStoredHealingUpdated().AddListener(UpdateHealthStoredDisplayText);
        _associatedChronomancer._myHeroBase.GetHeroManualAbilityAttemptEvent().AddListener(AbilityActivation);
        _associatedChronomancer._myHeroBase.GetHeroDiedEvent().AddListener(ResetSpecificHeroText);
    }

    protected override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        _associatedChronomancer.GetOnStoredHealingUpdated().RemoveListener(UpdateHealthStoredDisplayText);
        _associatedChronomancer._myHeroBase.GetHeroManualAbilityAttemptEvent().RemoveListener(AbilityActivation);
        _associatedChronomancer._myHeroBase.GetHeroDiedEvent().RemoveListener(ResetSpecificHeroText);
    }
    #endregion
}
