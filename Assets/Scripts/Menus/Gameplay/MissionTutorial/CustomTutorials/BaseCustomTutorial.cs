using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseCustomTutorial : MonoBehaviour
{
    public static BaseCustomTutorial Instance;
    
    [SerializeField] protected CustomTutorialStep[] _customTutorialSteps;
    protected int _currentTutorialStepIndex = -1;
    protected CustomTutorialStep _currentTutorialStep;
    protected CustomTutorialIndividualStepUISection _currentIndividualStepUISection;

    [SerializeField] protected bool _doesStopBossEnrageProgressOnBattleStart;
    
    [SerializeField] protected bool _doesStopChargingHeroAbilitiesOnBattleStart;
    [SerializeField] protected bool _doesStopHeroAbilitiesOnBattleStart;
    
    [Space]
    [SerializeField] protected GameObject _individualStepSection;
    protected CustomTutorialIndividualStepUISection[] _individualStepSections;
    
    [Space] 
    [SerializeField] protected GeneralCustomTutorialStep _generalCustomTutorialStep;
    [SerializeField] protected GameObject _stepsHolder;
    [SerializeField] protected CurveProgression _customTutorialAlphaProgression;

    [SerializeField] protected UnityEvent _onSetUpComplete;
    [SerializeField] protected UnityEvent _onBattleStart;

    protected virtual void SetUpGeneralCustomTutorialStep()
    {
        _generalCustomTutorialStep.SetUp();
    }

    protected virtual void CreateIndividualCustomStepUI()
    {
        _individualStepSections = new CustomTutorialIndividualStepUISection[_customTutorialSteps.Length];
        
        for (int i = 0; i < _customTutorialSteps.Length; i++)
        {
            _individualStepSections[i] = 
                Instantiate(_individualStepSection, _stepsHolder.transform).GetComponent<CustomTutorialIndividualStepUISection>();
            
            _individualStepSections[i].SetUpCustomTutorialIndividualStepUISection(_customTutorialSteps[i].CustomIndividualStepUI);
            
            SetUpCustomIndividualTutorialStepUI(i);
        }
        
    }

    protected virtual void SetUpCustomIndividualTutorialStepUI(int stepIndex)
    {
        _individualStepSections[stepIndex].gameObject.name = _customTutorialSteps[stepIndex].StepName;
        
        if (!_customTutorialSteps[stepIndex].IndividualStepUI.IsUnityNull())
        {
            GameObject newestIndividualStep = Instantiate(_customTutorialSteps[stepIndex].IndividualStepUI, Vector3.zero, Quaternion.identity);

            _individualStepSections[stepIndex].SetUpChildIndividualTutorialUI(newestIndividualStep);
        }
    }

    protected virtual void SetUpCustomTutorialObject(int stepIndex)
    {
        if (!_customTutorialSteps[stepIndex].CustomTutorialStepObjects.IsUnityNull())
        {
            Instantiate(_customTutorialSteps[stepIndex].CustomTutorialStepObjects, _customTutorialSteps[stepIndex].CustomStepObjectPosition, Quaternion.identity);
        }
    }

    public virtual void ProgressToNextTutorialStep()
    {
        Debug.Log("ProgressToNextTutorialStep" + (_currentTutorialStepIndex+1));
        if (_currentTutorialStepIndex >= _customTutorialSteps.Length)
        {
            return;
        }
        
        _currentTutorialStepIndex++;
        _currentTutorialStep = _customTutorialSteps[_currentTutorialStepIndex];
        _currentIndividualStepUISection = _individualStepSections[_currentTutorialStepIndex];
        
        OpenCurrentTutorialStepSection();
    }

    protected virtual void ProgressToNextTutorialStepFromCurrent()
    {
        HideCurrentTutorialStepSection();
        
        if (_currentTutorialStep.AutomaticStepProgressDelay > 0)
        {
            StartDelayProgressToNextTutorialStep(_currentTutorialStep.AutomaticStepProgressDelay);
        }
        else
        {
            ProgressToNextTutorialStep();
        }
    }

    protected virtual void StartDelayProgressToNextTutorialStep(float waitTime)
    {
        StartCoroutine(DelayProgressToNextTutorialStep(waitTime));
    }

    protected virtual IEnumerator DelayProgressToNextTutorialStep(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ProgressToNextTutorialStep();
    }

    public virtual void OpenCurrentTutorialStepSection()
    {
        if (_currentTutorialStep.IsUnityNull())
        {
            return;
        }
        
        ShowGeneralAndIndividualStepUI();
        
        SetUpCustomTutorialObject(_currentTutorialStepIndex);
        
        DisplayCustomTutorialBack();
        
        ToggleDirectHeroMovement(false);
        
        if (_currentTutorialStep.DoesStopBossAttacksOnOpen)
        {
            BossBase.Instance.GetSpecificBossScript().StopCurrentAttack();
        }

        if (_currentTutorialStep.DoesToggleHeroAbilityUse)
        {
            if (_currentTutorialStep.DoesDisableChargingHeroAbilitiesOnOpen)
            {
                ToggleHeroAbilityCharging(false);
            }
            if (_currentTutorialStep.DoesDisableHeroAbilitiesOnOpen)
            {
                ToggleHeroAbilityUse(false);
            }
        }
        

        InvokeCustomTutorialStepTutorialOpen(_currentTutorialStep);
    }

    public virtual void CloseCurrentTutorialStepSection()
    {
        if (_currentTutorialStep.IsUnityNull())
        {
            return;
        }

        if (_currentTutorialStep.DoesToggleHeroAbilityUse)
        {
            if (_currentTutorialStep.DoesEnableHeroAbilitiesOnClose)
            {
                ToggleHeroAbilityCharging(true);
            }
            if (_currentTutorialStep.DoesEnableHeroAbilitiesOnClose)
            {
                ToggleHeroAbilityUse(true);
            }
        }
        
        
        InvokeCustomTutorialStepTutorialClose(_currentTutorialStep);

        if (_currentTutorialStep.DoesAutomaticallyProgressToNextStepOnContinue)
        {
            ProgressToNextTutorialStepFromCurrent();
        }
        else
        {
            HideCurrentTutorialStepSection();
        }
    }

    protected virtual void HideCurrentTutorialStepSection()
    {
        HideGeneralAndIndividualStepUI();
        HideCustomTutorialBack();
        ToggleDirectHeroMovement(true);
    }


    protected virtual void ShowGeneralAndIndividualStepUI()
    {
        ShowGeneralStepUI();
        ShowIndividualStepUI();
    }

    protected virtual void HideGeneralAndIndividualStepUI()
    {
        HideGeneralStepUI();
        HideIndividualStepUI();
        
    }

    #region General Step
    public void ShowGeneralStepUI()
    {
        if (!_currentTutorialStep.DoesStepHaveGeneralStepUI)
        {
            return;
        }
        
        _generalCustomTutorialStep.AttemptShowGeneralStep(_currentTutorialStep);
    }

    public void HideGeneralStepUI()
    {
        _generalCustomTutorialStep.HideGeneralStep();
    }
    
    #endregion
    
    #region Individual Step

    protected virtual void ShowIndividualStepUI()
    {
        _currentIndividualStepUISection.ShowIndividualStepUI();
    }

    protected virtual void HideIndividualStepUI()
    {
        _currentIndividualStepUISection.HideIndividualStepUI();
    }
    #endregion
    
    #region General Transparency and Appearance
    public void DisplayCustomTutorialBack()
    {
        _customTutorialAlphaProgression.StartMovingUpOnCurve();
    }

    public void HideCustomTutorialBack()
    {
        _customTutorialAlphaProgression.StartMovingDownOnCurve();
    }
    
    #endregion
    
    #region GeneralFunctionality
    
    public void ToggleBossAutomaticAbilityUse(bool canBossAutomaticallyUseAbilities)
    {
        BossBase.Instance.GetSpecificBossScript().SetCanAutomaticallyUseAbilities(canBossAutomaticallyUseAbilities);
    }

    public void ActivateBossAbility(int abilityID)
    {
        BossBase.Instance.GetSpecificBossScript().StartAbility(abilityID, true);
    }

    public void ToggleCanBossProgressStagger(bool canBossProgressStagger)
    {
        BossStats.Instance.SetCanProgressBossEnrage(canBossProgressStagger);
    }

    public void StaggerBoss()
    {
        BossStats.Instance.DealStaggerRequiredToStaggerBoss();
    }

    public void ToggleDirectHeroMovement(bool canDirectMovement)
    {
        PlayerInputGameplayManager.Instance.SetCanDirectHeroMovement(canDirectMovement);
    }

    public void ToggleHeroAbilityCharging(bool canChargeHeroAbilities)
    {
        HeroesManager.Instance.ToggleHeroesChargingAbilities(canChargeHeroAbilities);
    }
    
    public void ToggleHeroAbilityUse(bool canHeroUseAbilities)
    {
        HeroesManager.Instance.ToggleHeroesAbleToUseAbilities(canHeroUseAbilities);
    }

    public void FullyCooldownAllHeroManualAbilities()
    {
        HeroesManager.Instance.FullyCooldownAllHeroManualAbilities();
    }
    #endregion
    
    #region SetUp
    
    public virtual void SetUpBaseCustomTutorial()
    {
        SetUpInstance();

        SetUpGeneralCustomTutorialStep();
        CreateIndividualCustomStepUI();

        SubscribeToEvents();
        
        InvokeOnSetUpComplete();
    }

    protected virtual void BattleStarted()
    {
        if (_doesStopBossEnrageProgressOnBattleStart)
        {
            ToggleCanBossProgressStagger(false);
        }
        
        if (_doesStopChargingHeroAbilitiesOnBattleStart)
        {
            ToggleHeroAbilityCharging(false);
        }
        
        if (_doesStopHeroAbilitiesOnBattleStart)
        {
            ToggleHeroAbilityUse(false);
        }
        
        InvokeOnBattleStarted();
    }

    protected virtual void SetUpInstance()
    {
        Instance = this;
    }

    protected virtual void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    protected virtual void SubscribeToEvents()
    {
        GameStateManager.Instance.GetStartOfBattleEvent().AddListener(BattleStarted);
    }

    protected virtual void UnsubscribeFromEvents()
    {
        GameStateManager.Instance.GetStartOfBattleEvent().RemoveListener(BattleStarted);
    }

    #endregion
    
    #region Custom Progression Event Subscriptions

    public virtual void SubscribeHeroControlToStepProgression()
    {
        PlayerInputGameplayManager.Instance.GetOnHeroControlledEvent.AddListener(HeroControlledProgression);
    }

    protected virtual void HeroControlledProgression(HeroBase hero)
    {
        UnsubscribeHeroControlToStepProgression();
        ProgressToNextTutorialStep();
    }

    public virtual void UnsubscribeHeroControlToStepProgression()
    {
        PlayerInputGameplayManager.Instance.GetOnHeroControlledEvent.RemoveListener(HeroControlledProgression);
    }

    public virtual void SubscribeHeroManualAbilityActivationToStepProgression()
    {
        HeroesManager.Instance.GetOnHeroManualAbilityUsedEvent().AddListener(HeroManualAbilityActivationProgression);
    }
    
    public virtual void HeroManualAbilityActivationProgression(HeroBase hero)
    {
        UnsubscribeHeroManualAbilityActivationToStepProgression();
        ProgressToNextTutorialStep();
    }
    
    public virtual void UnsubscribeHeroManualAbilityActivationToStepProgression()
    {
        HeroesManager.Instance.GetOnHeroManualAbilityUsedEvent().AddListener(HeroManualAbilityActivationProgression);
    }
    #endregion
    
    #region Events

    public void InvokeOnSetUpComplete()
    {
        _onSetUpComplete?.Invoke();
    }

    public void InvokeOnBattleStarted()
    {
        _onBattleStart?.Invoke();
    }

    public void InvokeCustomTutorialStepTutorialOpen(CustomTutorialStep tutorialStep)
    {
        tutorialStep.OnCustomTutorialOpen?.Invoke();
    }
    
    public void InvokeCustomTutorialStepTutorialClose(CustomTutorialStep tutorialStep)
    {
        tutorialStep.OnCustomTutorialClose?.Invoke();
    }
    #endregion
}

[System.Serializable]
public class CustomTutorialStep
{
    public string StepName;

    [Space] 
    public bool DoesAutomaticallyProgressOnBossHealthThreshold;
    [Range(0,1)] public float AutomaticProgressBossHealthThreshold;

    [Space]
    public bool DoesAutomaticallyProgressToNextStepOnContinue;
    public float AutomaticStepProgressDelay;
    
    [Space]
    public bool DoesStopBossAttacksOnOpen;

    [Space]
    public bool DoesToggleHeroAbilityUse;

    public bool DoesDisableChargingHeroAbilitiesOnOpen;
    public bool DoesDisableHeroAbilitiesOnOpen;
    
    public bool DoesEnableChargingHeroAbilitiesOnClose;
    public bool DoesEnableHeroAbilitiesOnClose;

    [Space]
    [Header("General Step UI")]
    public bool DoesStepHaveGeneralStepUI;
    public CustomGeneralTutorialUI GeneralStepUI;
    
    [Space]
    [Header("Custom Step UI")]
    public CustomIndividualTutorialUI CustomIndividualStepUI;
    public GameObject IndividualStepUI;
    
    [Space]
    [Header("Custom Tutorial Step Objects")]
    public Vector3 CustomStepObjectPosition;
    public GameObject CustomTutorialStepObjects;
    
    [Space]
    public UnityEvent OnCustomTutorialOpen;
    public UnityEvent OnCustomTutorialClose;
}

[System.Serializable]
public class CustomGeneralTutorialUI
{
    [TextArea(2, 10)]public string CustomTutorialUIText;
    public Vector2 CustomTutorialTextOffsetPosition;

    [Space]
    public Sprite CustomTutorialImage;
    public Vector2 CustomTutorialImageOffsetPosition;
    public Vector2 CustomTutorialImageDimensions;
    public Vector3 CustomTutorialImageScale;

    [Space]
    public Vector2 CustomTutorialPosition;
    public Vector2 CustomTutorialDimensions;
}

[System.Serializable]
public class CustomIndividualTutorialUI
{
    public bool DoesUseCanvasGroupCurve;
    
    public bool DoesUseScaleCurve;

    public bool DoesUseCustomUIObjectCanvas;
}