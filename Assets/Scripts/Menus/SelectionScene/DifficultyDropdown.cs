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

    private SelectionManager _selectionManager;


    // Start is called before the first frame update
    void Start()
    {
        _selectionManager = UniversalManagers.Instance.GetSelectionManager();

        SetStartingDropdownValue();
        SetStartingDropdownVisuals();
    }

    private void SetStartingDropdownVisuals()
    {
        List<string> diffNames = _selectionManager.GetDifficultyNames();
        List<Sprite> diffIcons = _selectionManager.GetDifficultyIcons();

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
        _dropdown.value = (int)_selectionManager.GetSelectedDifficulty() - 1;
    }

    public void UpdateDifficulty()
    {
        _selectionManager.SetSelectedDifficulty((GameDifficulty)_dropdown.value+1);
    }
}
