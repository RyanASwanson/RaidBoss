using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class DifficultyDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;

    [Space]
    [SerializeField] private Image _currentDifficultyIcon;

    [Space]
    [SerializeField] private string _mythicPlusLevelString;
    [SerializeField] private TMP_Text _headerText;

    [Space] 
    [SerializeField] private CurveProgression _scaleCurve;
    [SerializeField] private CurveProgression _textScaleCurve;

    [Space] 
    [SerializeField] private Color[] _dropdownColors;

    // Start is called before the first frame update
    void Start()
    {
        SetStartingDropdownVisuals();
        SetStartingDropdownValue();
        UpdateDifficultyHeaderText(false);
        
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SetStartingDropdownVisuals()
    {
        List<string> diffNames = SelectionManager.Instance.GetDifficultyNames();
        List<Sprite> diffIcons = SelectionManager.Instance.GetDifficultyIcons();

        _dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> difficultyIcons = new List<TMP_Dropdown.OptionData>();

        for(int i = 0; i< diffIcons.Count; i++)
        {
            var iconOptions = new TMP_Dropdown.OptionData(diffNames[i], diffIcons[i]);
            difficultyIcons.Add(iconOptions);
        }
        _dropdown.AddOptions(difficultyIcons);
    }

    private void SetStartingDropdownValue()
    {
        _dropdown.value = (int)SelectionManager.Instance.GetSelectedDifficulty() - 1;
        _dropdown.RefreshShownValue();
    }

    public void UpdateDifficulty(int difficulty)
    {
        SelectionManager.Instance.SetSelectedDifficulty((EGameDifficulty)_dropdown.value+1);
        
        _scaleCurve.StartMovingUpOnCurve();

        UpdateDifficultyHeaderText(true);
    }

    public void ForceSetDifficulty(int difficulty)
    {
        _dropdown.value = difficulty;

        UpdateDifficulty(difficulty);
    }

    public void UpdateDifficultyHeaderText(bool canMoveOnTextScale)
    {
        string previousHeaderText = _headerText.text;
        if (SelectionManager.Instance.IsPlayingMythicPlusLevelsAboveZero())
        {
            _headerText.text = _mythicPlusLevelString + SelectionManager.Instance.GetMythicPlusLevel().ToString();
        }
        else
        {
            _headerText.text = SelectionManager.Instance.GetDifficultyNames()
                [(int)SelectionManager.Instance.GetSelectedDifficulty() - 1];
        }

        if (canMoveOnTextScale && previousHeaderText != _headerText.text)
        {
            _textScaleCurve.StartMovingUpOnCurve();
        }
        
    }

    private void SubscribeToEvents()
    {
        _dropdown.onValueChanged.AddListener(UpdateDifficulty);
    }

    private void UnsubscribeFromEvents()
    {
        _dropdown.onValueChanged.RemoveListener(UpdateDifficulty);
    }

    #region Getters

    public Color[] GetDropdownColors() => _dropdownColors;

    #endregion
    
}
