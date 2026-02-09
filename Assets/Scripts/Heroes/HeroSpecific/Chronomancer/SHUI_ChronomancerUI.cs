using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SHUI_ChronomancerUI : SpecificHeroUIFramework
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Text _textBackground;

    [Space] 
    [SerializeField] private float _startingScale;
    [SerializeField] private float _maxScale;
    [SerializeField] private float _healingForMaxScale;
    [SerializeField] private AnimationCurve _scaleCurve;

    [Space] 
    [SerializeField] private float _activationTime;
    private WaitForSeconds _activationWait;
    
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
    
    private void UpdateSpecificHeroText(float value)
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
    }

    private void SetTextNumber()
    {
        if (_isActivationProcessActive)
        {
            return;
        }
        
        _text.text = _lastTextNumber.ToString();
        _textBackground.text = _lastTextNumber.ToString();
    }

    private void ModifyTextScale()
    {
        if (_isActivationProcessActive)
        {
            return;
        }
        float scaleProgress = Mathf.Clamp(_lastTextNumber / _healingForMaxScale, 0,1);
        float newScale = Mathf.Lerp(_startingScale,_maxScale,_scaleCurve.Evaluate(scaleProgress));
        _numberHolder.transform.localScale = new Vector3(newScale, newScale, newScale);
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

        SetTextNumber();
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
        _associatedChronomancer.GetOnStoredHealingUpdated().AddListener(UpdateSpecificHeroText);
        _associatedChronomancer._myHeroBase.GetHeroManualAbilityAttemptEvent().AddListener(AbilityActivation);
    }

    protected override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        _associatedChronomancer.GetOnStoredHealingUpdated().RemoveListener(UpdateSpecificHeroText);
        _associatedChronomancer._myHeroBase.GetHeroManualAbilityAttemptEvent().RemoveListener(AbilityActivation);
    }
    #endregion
}
