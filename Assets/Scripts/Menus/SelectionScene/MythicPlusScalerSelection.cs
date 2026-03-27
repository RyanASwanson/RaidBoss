using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MythicPlusScalerSelection : MonoBehaviour
{
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    
    [SerializeField] private Button _highestLevelButton;

    [Space] 
    [SerializeField] private TextWithBackground _levelNumberText;

    [Space] 
    [SerializeField] private CurveProgression _scaleCurve;
    
    // Start is called before the first frame update
    void Start()
    {
        SetMythicPlusLevel(SelectionManager.Instance.GetMythicPlusLevel());
        
        CheckActivationOfMythicPlusLevelUI(SelectionManager.Instance.GetSelectedDifficulty());
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void CheckActivationOfMythicPlusLevelUI(EGameDifficulty gameDifficulty)
    {
        if (gameDifficulty == EGameDifficulty.MythicPlus)
        {
            _scaleCurve.StartMovingUpOnCurve();
        }
        else
        {
            _scaleCurve.StartMovingDownOnCurve();
        }
    }

    public void LeftButtonPressed()
    {
        SetMythicPlusLevel(SelectionManager.Instance.GetMythicPlusLevel() - 1);
    }

    public void RightButtonPressed()
    {
        SetMythicPlusLevel(SelectionManager.Instance.GetMythicPlusLevel() + 1);
    }

    public void HighestLevelButtonPressed()
    {
        SetMythicPlusLevel(SaveManager.Instance.GetHighestMythicPlusLevelUnlocked());
    }

    public void DetermineLeftRightButtonInteractability()
    {
        _leftButton.interactable = SelectionManager.Instance.GetMythicPlusLevel() > 0;
        
        _rightButton.interactable = SelectionManager.Instance.GetMythicPlusLevel() !=
                                    SaveManager.Instance.GetHighestMythicPlusLevelUnlocked();

        _highestLevelButton.interactable = _rightButton.interactable;
    }

    private void SetMythicPlusLevel(int level)
    {
        SelectionManager.Instance.SetMythicPlusScalingLevel(level);
        DetermineLeftRightButtonInteractability();
        UpdateLevelText();
    }

    private void UpdateLevelText()
    {
        _levelNumberText.UpdateText(SelectionManager.Instance.GetMythicPlusLevel().ToString());
    }

    private void SubscribeToEvents()
    {
        SelectionManager.Instance.GetDifficultySelectionEvent().AddListener(CheckActivationOfMythicPlusLevelUI);
    }

    private void UnsubscribeFromEvents()
    {
        SelectionManager.Instance.GetDifficultySelectionEvent().RemoveListener(CheckActivationOfMythicPlusLevelUI);
    }
}
