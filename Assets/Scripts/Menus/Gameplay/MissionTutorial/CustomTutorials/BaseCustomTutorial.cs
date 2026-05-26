using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BaseCustomTutorial : MonoBehaviour
{
    public static BaseCustomTutorial Instance;
    
    [SerializeField] protected CustomTutorialStep[] _customTutorialSteps;
    protected int _currentTutorialStepIndex = -1;
    protected CustomTutorialStep _currentTutorialStep;
    
    [Space]
    [SerializeField] protected GameObject _individualStepSection;
    protected GameObject[] _individualStepSections;
    
    [Space] 
    [SerializeField] protected GeneralCustomTutorialStep _generalCustomTutorialStep;
    [SerializeField] protected GameObject _stepsHolder;
    [SerializeField] protected CurveProgression _customTutorialAlphaProgression;

    [SerializeField] protected UnityEvent _onSetUpComplete;

    public virtual void SetUpBaseCustomTutorial()
    {
        SetUpInstance();

        SetUpGeneralCustomTutorialStep();
        CreateIndividualCustomStepUI();
        
        InvokeOnSetUpComplete();
    }

    protected virtual void SetUpInstance()
    {
        Instance = this;
    }

    protected virtual void SetUpGeneralCustomTutorialStep()
    {
        _generalCustomTutorialStep.SetUp();
    }

    protected virtual void CreateIndividualCustomStepUI()
    {
        _individualStepSections = new GameObject[_customTutorialSteps.Length];
        
        for (int i = 0; i < _customTutorialSteps.Length; i++)
        {
            _individualStepSections[i] = Instantiate(_individualStepSection, _stepsHolder.transform);
            
            SetUpCustomTutorialStepUI(i);
            
            _individualStepSections[i].SetActive(false);
        }
        
    }

    protected virtual void SetUpCustomTutorialStepUI(int stepIndex)
    {
        _individualStepSections[stepIndex].name = _customTutorialSteps[stepIndex].StepName;
        
        if (!_customTutorialSteps[stepIndex].CustomStepUI.IsUnityNull())
        {
            Instantiate(_customTutorialSteps[stepIndex].CustomStepUI, _individualStepSections[stepIndex].transform);
        }
    }

    public virtual void ProgressToNextTutorialStep()
    {
        Debug.Log("ProgressToNextTutorialStep" + _currentTutorialStepIndex);
        if (_currentTutorialStepIndex >= _customTutorialSteps.Length)
        {
            return;
        }
        
        _currentTutorialStepIndex++;
        _currentTutorialStep = _customTutorialSteps[_currentTutorialStepIndex];
        
        OpenCurrentTutorialStepSection();
    }

    public virtual void OpenCurrentTutorialStepSection()
    {
        if (_currentTutorialStep.IsUnityNull())
        {
            return;
        }
        
        _individualStepSections[_currentTutorialStepIndex].SetActive(true);
        ShowGeneralStepUI();
        DisplayCustomTutorial();
        
        if (_currentTutorialStep.DoesStopBossAttacksOnProgression)
        {
            
        }
    }

    public virtual void CloseCurrentTutorialStepSection()
    {
        if (_currentTutorialStep.IsUnityNull())
        {
            return;
        }

        if (_currentTutorialStep.DoesAutomaticallyProgressToNextStepOnContinue)
        {
            ProgressToNextTutorialStep();
        }
        else
        {
            _individualStepSections[_currentTutorialStepIndex].SetActive(false);
            HideGeneralStepUI();
            HideCustomTutorial();
        }
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
    
    #region General Transparency and Appearance
    public void DisplayCustomTutorial()
    {
        _customTutorialAlphaProgression.StartMovingUpOnCurve();
    }

    public void HideCustomTutorial()
    {
        _customTutorialAlphaProgression.StartMovingDownOnCurve();
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
    
    #endregion
    
    #region Events

    public void InvokeOnSetUpComplete()
    {
        _onSetUpComplete?.Invoke();
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
    
    [Space]
    public bool DoesStopBossAttacksOnProgression;

    [Space]
    [Header("General Step UI")]
    public bool DoesStepHaveGeneralStepUI;
    public CustomGeneralTutorialUI GeneralStepUI;
    
    [Space]
    [Header("Custom Step UI")]
    public GameObject CustomStepUI;
    
    [Space]
    public UnityEvent OnCustomTutorialOpen;
    public UnityEvent OnCustomTutorialClose;
}

[System.Serializable]
public class CustomGeneralTutorialUI
{
    [TextArea(2, 10)]public string CustomTutorialUIText;

    public Vector2 CustomTutorialPosition;
    public Vector2 CustomTutorialDimensions;
}