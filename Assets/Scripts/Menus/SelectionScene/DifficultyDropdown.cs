using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultyDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;

    [Space]
    [SerializeField] private Image _currentDifficultyIcon;

    // Start is called before the first frame update
    void Start()
    {
        SetStartingDropdownVisuals();
        SetStartingDropdownValue();
    }

    private void SetStartingDropdownVisuals()
    {
        List<string> diffNames = SelectionManager.Instance.GetDifficultyNames();
        List<Sprite> diffIcons = SelectionManager.Instance.GetDifficultyIcons();

        _dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> difficultyIcons = new List<TMP_Dropdown.OptionData>();

        for(int i = 0; i< diffIcons.Count; i++)
        {
            /*GameDifficulty difficulty = (GameDifficulty)i+1;

            var iconOptions = new TMP_Dropdown.OptionData(difficulty.ToString().Replace("_", " "), diffIcons[i]);*/
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

    public void UpdateDifficulty()
    {
        SelectionManager.Instance.SetSelectedDifficulty((EGameDifficulty)_dropdown.value+1);
    }
}
